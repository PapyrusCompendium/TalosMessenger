using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Packets
{
    [Serializable]
    [PacketType("User", 5)]
    public class User : Packet
    {
        public string Username { get; }

        public User()
        {
            Type = 5;
        }
    }
}
