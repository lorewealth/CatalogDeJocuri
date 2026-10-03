using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboutGame
{
    public interface IIdentifiable
    {
        public int Id { get; }
        public string Name { get; }
    }
}
