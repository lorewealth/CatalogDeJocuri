using AboutGame;
using AboutGame.Enums;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;

namespace Stocking.Database
{
    public class SQLStocking : IStocking<Game>
    {
        private readonly string connectionString;
        public string? LastError { get; private set; }
        public SQLStocking(string connection)
        {
            this.connectionString = "Data Source=" + connection;
            InitializeDatabase();
        }
        private SqliteConnection CreateConnection()
        {
            SqliteConnection conn = new(connectionString);
            conn.Open();
            conn.Execute("PRAGMA foreign_keys = ON;");
            return conn;
        }
        private void InitializeDatabase()
        {
            using var db = CreateConnection();

            string sqlGames = @"
                CREATE TABLE IF NOT EXISTS Games(
                    InternalId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ExternalId TEXT UNIQUE,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    Store INTEGER NOT NULL DEFAULT 0,
                    SupportedOS INTEGER NOT NULL DEFAULT 0,
                    ReleaseDate TEXT,
                    IsAvailable BOOLEAN DEFAULT 0,
                    ImgUrl TEXT
                );";

            string sqlGenres = @"
                CREATE TABLE IF NOT EXISTS Genres(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );";

            string sqlPublishers = @"
                CREATE TABLE IF NOT EXISTS Publishers(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );";

            string sqlDevelopers = @"
                CREATE TABLE IF NOT EXISTS Developers(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );";

            string sqlGameGenresRelations = @"
                CREATE TABLE IF NOT EXISTS GameGenresRelations(
                    GameId INTEGER NOT NULL,
                    GenreId INTEGER NOT NULL,
                    PRIMARY KEY (GameId, GenreId),
                    FOREIGN KEY (GameId) REFERENCES Games(InternalId) ON DELETE CASCADE,
                    FOREIGN KEY (GenreId) REFERENCES Genres(Id) ON DELETE CASCADE
                );";

            string sqlGamePublishersRelations = @"
                CREATE TABLE IF NOT EXISTS GamePublishersRelations(
                    GameId INTEGER NOT NULL,
                    PublisherId INTEGER NOT NULL,
                    PRIMARY KEY (GameId, PublisherId),
                    FOREIGN KEY (GameId) REFERENCES Games(InternalId) ON DELETE CASCADE,
                    FOREIGN KEY (PublisherId) REFERENCES Publishers(Id) ON DELETE CASCADE
                );";

            string sqlGameDevelopersRelations = @"
                CREATE TABLE IF NOT EXISTS GameDevelopersRelations(
                    GameId INTEGER NOT NULL,
                    DeveloperId INTEGER NOT NULL,
                    PRIMARY KEY (GameId, DeveloperId),
                    FOREIGN KEY (GameId) REFERENCES Games(InternalId) ON DELETE CASCADE,
                    FOREIGN KEY (DeveloperId) REFERENCES Developers(Id) ON DELETE CASCADE
                );";

            string sqlGameScoresRelations = @"
                CREATE TABLE IF NOT EXISTS GameScoresRelations(
                    GameId INTEGER NOT NULL,
                    Source INTEGER NOT NULL,
                    RawScore REAL NOT NULL,
                    PRIMARY KEY (GameId, Source),
                    FOREIGN KEY (GameId) REFERENCES Games(InternalId) ON DELETE CASCADE
                );";

            string sqlGameAgeRatingsRelations = @"
                CREATE TABLE IF NOT EXISTS GameAgeRatingsRelations(
                    GameId INTEGER NOT NULL,
                    Source INTEGER NOT NULL,
                    Age INTEGER NOT NULL,
                    PRIMARY KEY (GameId, Source),
                    FOREIGN KEY (GameId) REFERENCES Games(InternalId) ON DELETE CASCADE
                );";

            db.Execute(sqlGames);
            db.Execute(sqlGenres);
            db.Execute(sqlPublishers);
            db.Execute(sqlDevelopers);
            db.Execute(sqlGameGenresRelations);
            db.Execute(sqlGamePublishersRelations);
            db.Execute(sqlGameDevelopersRelations);
            db.Execute(sqlGameScoresRelations);
            db.Execute(sqlGameAgeRatingsRelations);
            MigrateScoreTable(db);
        }        

        private static void MigrateScoreTable(SqliteConnection db)
        {
            var columns = db.Query<TableColumn>("PRAGMA table_info(GameScoresRelations);").ToList();
            bool rawScoreIsPartOfKey = columns.Any(column => column.Name == "RawScore" && column.Pk > 0);
            if (rawScoreIsPartOfKey)
                return;

            using var transaction = db.BeginTransaction();
            db.Execute("ALTER TABLE GameScoresRelations RENAME TO GameScoresRelations_Old;", transaction: transaction);
            db.Execute(@"
                CREATE TABLE GameScoresRelations(
                    GameId INTEGER NOT NULL,
                    Source INTEGER NOT NULL,
                    RawScore REAL NOT NULL,
                    PRIMARY KEY (GameId, Source, RawScore),
                    FOREIGN KEY (GameId) REFERENCES Games(InternalId) ON DELETE CASCADE
                );
                INSERT INTO GameScoresRelations (GameId, Source, RawScore)
                SELECT GameId, Source, RawScore FROM GameScoresRelations_Old;
                DROP TABLE GameScoresRelations_Old;", transaction: transaction);
            transaction.Commit();
        }

        private sealed class TableColumn
        {
            public string Name { get; init; } = "";
            public int Pk { get; init; }
        }

        public async Task<bool> AddGame(Game game)
        {
            LastError = null;
            using var db = CreateConnection();
            await using var transaction = await db.BeginTransactionAsync();

            try
            {
                int gameId = await db.ExecuteScalarAsync<int>(@"
                    INSERT INTO Games (ExternalId, Name, Price, Store, SupportedOS, ReleaseDate, IsAvailable, ImgUrl)
                    VALUES (@ExternalId, @Name, @Price, @Stores, @SupportedOS, @ReleaseDate, @IsAvailable, @ImgUrl);
                    SELECT last_insert_rowid();", game, transaction);
                
                var genreResolver = await TableResolver<Genre>.CreateAsync(db, "Genres", (id, name) => new Genre(id, name));
                foreach (Genre gen in game.Genres)
                {
                    var resolved = await genreResolver.ResolveAsync(gen.Name, transaction);
                    await db.ExecuteAsync(
                        @"INSERT INTO GameGenresRelations (GameId, GenreId) VALUES (@GameId, @GenreId);",
                        new { GameId = gameId, GenreId = resolved.Id }, transaction);
                }

                var publisherResolver = await TableResolver<Publisher>.CreateAsync(db, "Publishers", (id, name) => new Publisher(id, name));
                foreach (Publisher pubs in game.Publishers)
                {
                    var resolved = await publisherResolver.ResolveAsync(pubs.Name, transaction);
                    await db.ExecuteAsync(
                        @"INSERT INTO GamePublishersRelations (GameId, PublisherId) VALUES (@GameId, @PublisherId);",
                        new {GameId = gameId, PublisherId = resolved.Id}, transaction);
                }

                var developerResolver = await TableResolver<Developer>.CreateAsync(db, "Developers", (id, name) => new Developer(id, name));
                foreach (Developer dev in game.Developers)
                {
                    var resolved = await developerResolver.ResolveAsync(dev.Name, transaction);
                    await db.ExecuteAsync(
                        @"INSERT INTO GameDevelopersRelations (GameId, DeveloperId) VALUES (@GameId, @DeveloperId);",
                        new { GameId = gameId, DeveloperId = resolved.Id }, transaction);
                }

                foreach (Rating rating in game.Scores)
                {
                    await db.ExecuteAsync(
                        "INSERT INTO GameScoresRelations (GameId, Source, RawScore) VALUES (@GameId, @Source, @RawScore);",
                        new { GameId = gameId, Source = rating.Source, RawScore = rating.RawScore }, transaction);
                }

                foreach (AgeRating ageRating in game.AgeRatings)
                {
                    await db.ExecuteAsync(
                        "INSERT INTO GameAgeRatingsRelations (GameId, Source, Age) VALUES (@GameId, @Source, @Age);",
                        new { GameId = gameId, Source = ageRating.Source, Age = ageRating.Age }, transaction);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync();
                LastError = exception.Message;
                return false;
            }
        }
        public async Task<bool> RemoveGame(int id)
        {
            using var db = CreateConnection();
            var rowsAffected = await db.ExecuteAsync("DELETE FROM Games WHERE InternalId=@Id;", new { Id = id });
            
            if(rowsAffected > 0) return true;
            return false;
        }
        public async Task<IEnumerable<Game>> GetGames()
        {
            using var db = CreateConnection();
                
            var games = (await db.QueryAsync<Game>("SELECT InternalId, ExternalId, Name, Price, Store AS Stores, SupportedOS, ReleaseDate, IsAvailable, ImgUrl FROM Games;")).ToDictionary(g => g.InternalId);

            var genreLinks = await db.QueryAsync<(int GameId, int Id, string Name)>(@"
                            SELECT r.GameId, g.Id, g.Name
                            FROM GameGenresRelations r
                            JOIN Genres g ON g.Id = r.GenreId;");
            foreach (var group in genreLinks.GroupBy(x => x.GameId))
                if (games.TryGetValue(group.Key, out var game))
                    game.Genres = group.Select(x => new Genre(x.Id, x.Name)).ToList();

            var developerLinks = await db.QueryAsync<(int GameId, int Id, string Name)>(@"
                                SELECT r.GameId, d.Id, d.Name
                                FROM GameDevelopersRelations r
                                JOIN Developers d ON d.Id = r.DeveloperId;");
            foreach (var group in developerLinks.GroupBy(x => x.GameId))
                if (games.TryGetValue(group.Key, out var game))
                    game.Developers = group.Select(x => new Developer(x.Id, x.Name)).ToList();

            var publisherLinks = await db.QueryAsync<(int GameId, int Id, string Name)>(@"
                                SELECT r.GameId, p.Id, p.Name
                                FROM GamePublishersRelations r
                                JOIN Publishers p ON p.Id = r.PublisherId;");
            foreach (var group in publisherLinks.GroupBy(x => x.GameId))
                if (games.TryGetValue(group.Key, out var game))
                    game.Publishers = group.Select(x => new Publisher(x.Id, x.Name)).ToList();

            var scoreRows = await db.QueryAsync<(int GameId, RatingSource Source, double RawScore)>(
                "SELECT GameId, Source, RawScore FROM GameScoresRelations;");
            foreach (var group in scoreRows.GroupBy(x => x.GameId))
                if (games.TryGetValue(group.Key, out var game))
                    game.Scores = group.Select(x => new Rating(x.Source, x.RawScore)).ToList();

            var ageRatings = await db.QueryAsync<(int GameId, AgeRatingsCategory AgeCategory, AgeRatingsValue AgeValue)>(
                            "SELECT GameId, Source, Age FROM GameAgeRatingsRelations;");
            foreach (var group in ageRatings.GroupBy(x => x.GameId))
                if (games.TryGetValue(group.Key, out var game))
                    game.AgeRatings = group.Select(x => new AgeRating(x.AgeCategory, x.AgeValue)).ToList();

            return games.Values.ToList();
        }
        public async Task<Game?> GetGame(int id)
        {
            using var db = CreateConnection();

            var game = await db.QueryFirstOrDefaultAsync<Game>(@"
                   SELECT InternalId, ExternalId, Name, Price, Store AS Stores, SupportedOS, ReleaseDate, IsAvailable, ImgUrl FROM Games WHERE InternalId=@Id;", new { Id = id });

            if (game is null) return null;

            var genres = await db.QueryAsync<(int Id, string Name)>(@"
                            SELECT g.Id, g.Name
                            FROM GameGenresRelations r
                            JOIN Genres g ON g.Id = r.GenreId
                            WHERE r.GameId = @Id;", new { Id = id });
            game.Genres = [.. genres.Select(x => new Genre(x.Id, x.Name))];

            var developers = await db.QueryAsync<(int Id, string Name)>(@"
                                SELECT d.Id, d.Name
                                FROM GameDevelopersRelations r
                                JOIN Developers d ON d.Id = r.DeveloperId
                                WHERE r.GameId = @Id;", new { Id = id });
            game.Developers = [.. developers.Select(x => new Developer(x.Id, x.Name))];

            var publishers = await db.QueryAsync<(int Id, string Name)>(@"
                                SELECT p.Id, p.Name
                                FROM GamePublishersRelations r
                                JOIN Publishers p ON p.Id = r.PublisherId
                                WHERE r.GameId = @Id;", new { Id = id });
            game.Publishers = [.. publishers.Select(x => new Publisher(x.Id, x.Name))];

            var scoreRows = await db.QueryAsync<(RatingSource Source, double RawScore)>(
                "SELECT Source, RawScore FROM GameScoresRelations WHERE GameId = @Id;", new { Id = id });
            game.Scores = [.. scoreRows.Select(x => new Rating(x.Source, x.RawScore))];

            var ageRatings = await db.QueryAsync<(AgeRatingsCategory AgeCategory, AgeRatingsValue AgeValue)>(
                            "SELECT Source, Age FROM GameAgeRatingsRelations WHERE GameId = @Id;", new { Id = id });
            game.AgeRatings = [.. ageRatings.Select(x => new AgeRating(x.AgeCategory, x.AgeValue))];
            
            return game;
        }
        public async Task<bool> UpdateGame(Game game)
        {
            LastError = null;
            using var db = CreateConnection();
            await using var transaction = await db.BeginTransactionAsync();
            try
            {
                int rowsAffected = await db.ExecuteAsync(@"
                    UPDATE Games
                    SET ExternalId=@ExternalId, Name=@Name, Price=@Price, Store=@Stores, SupportedOS=@SupportedOS, ReleaseDate=@ReleaseDate, IsAvailable=@IsAvailable, ImgUrl=@ImgUrl
                    WHERE InternalId=@InternalId;", game, transaction);
                if (rowsAffected == 0)
                {
                    await transaction.RollbackAsync();
                    LastError = "The game no longer exists in the database.";
                    return false;
                }

                await db.ExecuteAsync("DELETE FROM GameGenresRelations WHERE GameId = @Id; DELETE FROM GamePublishersRelations WHERE GameId = @Id; DELETE FROM GameDevelopersRelations WHERE GameId = @Id;", new { Id = game.InternalId }, transaction);

                var genreResolver = await TableResolver<Genre>.CreateAsync(db, "Genres", (id, name) => new Genre(id, name));
                foreach (Genre genre in game.Genres)
                {
                    Genre resolved = await genreResolver.ResolveAsync(genre.Name, transaction);
                    await db.ExecuteAsync("INSERT INTO GameGenresRelations (GameId, GenreId) VALUES (@GameId, @GenreId);", new { GameId = game.InternalId, GenreId = resolved.Id }, transaction);
                }

                var publisherResolver = await TableResolver<Publisher>.CreateAsync(db, "Publishers", (id, name) => new Publisher(id, name));
                foreach (Publisher publisher in game.Publishers)
                {
                    Publisher resolved = await publisherResolver.ResolveAsync(publisher.Name, transaction);
                    await db.ExecuteAsync("INSERT INTO GamePublishersRelations (GameId, PublisherId) VALUES (@GameId, @PublisherId);", new { GameId = game.InternalId, PublisherId = resolved.Id }, transaction);
                }

                var developerResolver = await TableResolver<Developer>.CreateAsync(db, "Developers", (id, name) => new Developer(id, name));
                foreach (Developer developer in game.Developers)
                {
                    Developer resolved = await developerResolver.ResolveAsync(developer.Name, transaction);
                    await db.ExecuteAsync("INSERT INTO GameDevelopersRelations (GameId, DeveloperId) VALUES (@GameId, @DeveloperId);", new { GameId = game.InternalId, DeveloperId = resolved.Id }, transaction);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync();
                LastError = exception.Message;
                return false;
            }
        }
    }
}
