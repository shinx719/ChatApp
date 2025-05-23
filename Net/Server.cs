﻿using ChatClient.Net.IO;
using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
		public event Action msgReceivedEvent;
		public event Action userDissconnectEvent;
		public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected) 
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
					var connectPacket = new PacketBuilder();
					connectPacket.WriteOpCode(0);
					connectPacket.WriteMessage(username);
					_client.Client.Send(connectPacket.GetPacketBytes());
				}
                ReadPackets();
				
            }
        }

		private void ReadPackets()
		{
			Task.Run(() =>
			{
				try
				{
					while (_client.Connected) // Проверяем, что соединение открыто
					{
						var opcode = PacketReader.ReadByte();
						switch (opcode)
						{
							case 1:
								connectedEvent?.Invoke();
								break;
							case 5:
								msgReceivedEvent?.Invoke();
								break;
							case 10:
								userDissconnectEvent?.Invoke();
								break;
							default:
								Console.WriteLine("Неизвестный OpCode.");
								break;
						}
					}
				}
				catch (IOException ex)
				{
					Console.WriteLine($"Ошибка ввода-вывода: {ex.Message}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка: {ex.Message}");
				}
			});
		}


		public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
			messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
		}

		public void Disconnect()
		{
			if (_client.Connected)
			{
				var disconnectPacket = new PacketBuilder();
				disconnectPacket.WriteOpCode(10); // Предположим, 10 — код отключения
				_client.Client.Send(disconnectPacket.GetPacketBytes());

				_client.Close(); // Закрываем соединение
			}
		}

	}
}