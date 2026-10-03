using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboutGame
{
    public class Developer(int id, string name) : IIdentifiable
    {
        public int Id { get; } = id;
        public string Name { get; } = name;
    }
}
