using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APISearch
{
    public record class GameSearchResult
    {
        public int IGDBId { get; set; }
        public required string Name { get; set; }
        public DateOnly? ReleaseDate { get; set; } 
    }
}
