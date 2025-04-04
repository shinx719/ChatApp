using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
		public ObservableCollection<UserModel> Users { get; set; }
		public ObservableCollection<string> Messages { get; set; }

		public  RelayCommand ConnectToServerCommand { get; set; }
		public RelayCommand DisconnectFromServerCommand { get; set; }
		public RelayCommand SendMessageCommand { get; set; }

		public string Username { get; set; }
		public string Message { get; set; }
		private Server _server;

		public MainViewModel() 
        {
			Users = new ObservableCollection<UserModel>();
			Messages = new ObservableCollection<string>();

			_server = new Server();
            _server.connectedEvent += UserConnected;
			_server.msgReceivedEvent += MessageReceived;
			_server.userDissconnectEvent += RemoveUser;
			ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
			SendMessageCommand = new RelayCommand(o =>
			{
				_server.SendMessageToServer(Message);
			}, o => !string.IsNullOrEmpty(Message));
			DisconnectFromServerCommand = new RelayCommand(o => DisconnectFromServer(), o => _server != null);

		}

		private string HashMessage(string message)
		{
			using var sha256 = SHA256.Create();
			byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
			return Convert.ToHexString(hashBytes).ToLower(); 
		}

		private void MessageReceived()
		{
			var msg = _server.PacketReader.ReadMessage();
			Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
			SaveMessageToFile(msg);
		}

		public void SaveMessageToFile(string message)
		{
			string filePath = "C:\\Users\\erlan\\source\\chat\\logs.txt";
			string hashFilePath = "C:\\Users\\erlan\\source\\chat\\hashedLogs.txt";

			using (StreamWriter writer = new StreamWriter(filePath, true))
			{
				writer.WriteLine(message);
			}

			string hashedMessage = HashMessage(message);
			using (StreamWriter writer = new StreamWriter(hashFilePath, true))
			{
				writer.WriteLine(hashedMessage);
			}
		}


		private void DisconnectFromServer()
		{
			_server.Disconnect();
			Application.Current.Dispatcher.Invoke(() =>
			{
				Users.Clear();
				Messages.Clear();
			});
		}

	private void RemoveUser()
		{
			var uid = _server.PacketReader.ReadMessage();
			var user = Users.Where(x => x.UID == uid).FirstOrDefault();
			Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
		}

		

		private void UserConnected()
		{
			var user = new UserModel
			{
				UserName = _server.PacketReader.ReadMessage(),
				UID = _server.PacketReader.ReadMessage(),
			};

			if (!Users.Any(x => x.UID == user.UID))
			{
				Application.Current.Dispatcher.Invoke(() => Users.Add(user));
			}
		}


	}
}
