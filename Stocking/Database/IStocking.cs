using System;
using AboutGame;

namespace Stocking.Database
{
    public interface IStocking<T>
    {
        Task<bool> AddGame(T game);
        Task<bool> RemoveGame(int id);
        Task<IEnumerable<T>> GetGames();
        Task<T?> GetGame(int id);
        Task<bool> UpdateGame(T game);
    }
}