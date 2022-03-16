using System;
using System.Threading;
using System.Threading.Tasks;

namespace EMBRS
{
    class Program
    {
        private static MarketMaker marketMaker;
        private static Spinner spinner;

        static async Task Main(string[] args)
        {
            bool showMenu = true;

            Settings.Initialize();
            spinner = new Spinner(0, 23);
            marketMaker = new MarketMaker(spinner);

            while (showMenu)
            {
                showMenu = await MainMenuAsync();
            }
        }

        private static async Task<bool> MainMenuAsync()
        {
            Console.Clear();
            Console.WriteLine("1.Show Order Book");
            Console.WriteLine("2.Simulate Market Maker");
            Console.WriteLine("3.Start Market Maker");
            Console.WriteLine("4.Exit");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    await XRPL.ShowBookOffers();
                    return true;
                case "2":
                    await XRPL.ShowBookOffers(true);
                    return true;
                case "3":
                    Console.WriteLine("*** Are you sure you want to do this? This will begin the market maker bot based the config settings *** Y or N");
                    switch (Console.ReadLine())
                    {
                        case "Y":
                        case "y":
                            await MarketMaker.StartMarketMaker();
                            return true;
                        case "N":
                        case "n":
                            return true;
                    }
                    return true;
                case "4":
                    return false;
                default:
                    return true;
            }
        }
    }
}
