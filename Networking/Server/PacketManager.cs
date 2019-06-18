using Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Server
{
    public static class PacketManager
    {
        public static void ManagePacket(Packet packet, ChatClient client)
        {
            switch(packet.Type)
            {
                //This is the base Type (Packet)
                case 5:

                    break;
            }
        }
    }
}