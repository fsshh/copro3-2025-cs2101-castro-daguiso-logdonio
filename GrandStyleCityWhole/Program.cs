using System;

namespace GrandStyleCityWhole
{
    public class Program
    {
        static void Main(string[] args)
        {
            // instance nato pre para dun database na class
            DatabaseHelper.InitializeDatabase();
            DatabaseHelper.InitializeArrays();

           // see malinis yung flow malinis yung bato and clear yung main natin
            GrandStyleCityBaseClass game = new GrandStyleCityBaseClass();
            game.MainMenu();
            // 16/05/25 Dagz Officially Done!
        }
    }
}
