using System.Configuration;
using System.Data;
using System.Windows;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			MainWindow mainWindow1 = new MainWindow();
			mainWindow1.Show();

			MainWindow mainWindow2 = new MainWindow();
			mainWindow2.Show();
		}

	}


}
