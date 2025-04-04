using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream _ns;

        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMessage()
        {
            int length = ReadInt32(); // Читаем длину сообщения
            byte[] msgBuffer = ReadBytes(length); // Читаем байты

            return Encoding.UTF8.GetString(msgBuffer); // Декодируем в строку
        }
    }
}
