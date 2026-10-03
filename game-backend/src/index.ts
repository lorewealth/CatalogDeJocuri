import { searchGames, getGameData } from "./GameDataService";

export default {
    async fetch(request, env, ctx): Promise<Response>
    {
        const url = new URL(request.url);
		
		if (url.pathname === "/")
			return new Response("\"It seems the surface I am scratching is the bed that I have made.\"", {status: 200});

        // Search for games for a dropdown menu in wpf
        if (url.pathname === "/api/search")
        {
            const name = url.searchParams.get("name");

            if (!name)
            {
                return Response.json(
                    { error: "Missing name" },
                    { status: 400 }
                );
            }

            const games = await searchGames(name, env);

            return Response.json(games);
        }

        // Get exact selected game
        if (url.pathname === "/api/game")
        {
            const idParam = url.searchParams.get("id");
            const country = url.searchParams.get("country");

            if (!idParam || !country)
            {
                return Response.json(
                    { error: "Missing id or country" },
                    { status: 400 }
                );
            }

            const id = Number(idParam);

            if (!Number.isInteger(id))
            {
                return Response.json(
                    { error: "Invalid game id" },
                    { status: 400 }
                );
            }

            const game = await getGameData(id, country, env);

            if (!game)
            {
                return Response.json(
                    { error: "Game not found" },
                    { status: 404 }
                );
            }

            return Response.json(game);
        }

        return new Response(
            "Not found",
            { status: 404 }
        );
    },
} satisfies ExportedHandler<Env>;