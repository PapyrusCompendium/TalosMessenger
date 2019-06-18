using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Packets
{
    [Serializable]
    [PacketType("Unicast", 6)]
    public class Unicast : Packet
    {
        string Message { get; }
        ulong Recipient { get; }
        public Unicast()
        {
            Type = 6;
        }
    }

    [Serializable]
    [PacketType("Multicast", 7)]
    public class Multicast : Packet
    {
        string Message { get; }
        ulong[] Recipients { get; }
        public Multicast()
        {
            Type = 7;
        }
    }

    [Serializable]
    [PacketType("Broadcast", 8)]
    public class Broadcast : Packet
    {
        string Message { get; }
        public Broadcast()
        {
            Type = 8;
        }
    }
}