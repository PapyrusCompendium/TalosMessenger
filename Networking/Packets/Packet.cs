using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Packets
{
    [Serializable]
    [PacketType("Packet", 0)]
    public class Packet
    {
        public ulong ID { get; internal set; }
        public byte Type { get; internal set; }
        public DateTime Sent { get; internal set; }

        public void Send(Socket destination)
        {
            if (!destination.Connected)
            {
                Log.Error("Can not send packet, not connected!");
                return;
            }

            destination.Send(Serialize(this));
        }

        public static byte[] Serialize(Packet packet)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream outputStream = new MemoryStream())
            {
                binaryFormatter.Serialize(outputStream, packet);
                return outputStream.GetBuffer();
            }
        }

        public static Packet Deserialize(byte[] packet)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream inputStream = new MemoryStream(packet))
                return (Packet)binaryFormatter.Deserialize(inputStream);
        }
    }
}
