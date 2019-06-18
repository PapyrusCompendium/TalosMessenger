using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Packets
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class PacketType : Attribute
    {
        public string Name { get; }
        public byte Type { get; }
        public PacketType(string name, byte type)
        {
            Name = name;
            Type = type;
        }

        public static byte GetType(Type type)
        {
            return (type.GetCustomAttributes(typeof(PacketType), false).FirstOrDefault() as PacketType).Type;
        }
    }
}
