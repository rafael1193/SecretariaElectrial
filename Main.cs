using System;
using Gtk;

namespace SecretariaDataBase
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine(ConfigKeys.LastComboboxActive);
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
        }
    }
}
