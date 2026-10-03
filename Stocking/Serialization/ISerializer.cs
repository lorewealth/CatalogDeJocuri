using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AboutGame;

namespace Stocking.Serialization
{
    public interface ISerializer<T> where T : class
    {
        string FileExtension { get; }
        public byte[] Serialize(T package);
        public T Deserialize(byte[] data);
    }
}
