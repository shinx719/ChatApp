using System;
using System.IO;
using System.Text;

namespace ChatClient.Net.IO
{
	class PacketBuilder
	{
		private MemoryStream _ms;
		private BinaryWriter _writer;

		public PacketBuilder()
		{
			_ms = new MemoryStream();
			_writer = new BinaryWriter(_ms);
		}

		public void WriteOpCode(byte opcode)
		{
			_writer.Write(opcode);
		}

		public void WriteMessage(string msg)
		{
			byte[] msgBytes = Encoding.UTF8.GetBytes(msg); // Используем UTF-8
			_writer.Write(msgBytes.Length); // Записываем длину строки
			_writer.Write(msgBytes); // Записываем саму строку
		}

		public byte[] GetPacketBytes()
		{
			return _ms.ToArray();
		}
	}
}
