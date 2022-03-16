using RippleDotNet;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EMBRS
{
    public class MarketMaker
    {
        private static Spinner spinner;

        public MarketMaker(Spinner _spinner)
        {
            spinner = _spinner;
        }

        public static async Task StartMarketMaker()
        {
            ConsoleScreen.ClearConsoleLines();
            ConsoleScreen.InitScreen(ref spinner, " Starting Market Maker...");
            Console.SetCursorPosition(0, 9);

            try
            {
                IRippleClient client = new RippleClient(Settings.WebSocketUrl);
                client.Connect();

                var initialMidPrice = await XRPL.GetBookOffers(client);

                for (int i = 1; i < Settings.Steps + 1; i++)
                {
                    var askPrice = initialMidPrice.Midprice * (1 + Settings.Interval * i);
                    var askXRPAmount = decimal.Round(Settings.BotTokenAmt * askPrice.Value, 6);
                    var result1 = await XRPL.FillOrder(client, askXRPAmount);
                    if(result1.EngineResult == "tesSUCCESS") Console.WriteLine("Ask Order Placed: " + Settings.BotTokenAmt + " tokens at " + askPrice);

                    Thread.Sleep(Settings.TxnThrottle * 1000);

                    var bidPrice = initialMidPrice.Midprice * (1 - Settings.Interval * i);
                    var bidXRPAmount = decimal.Round(Settings.BotTokenAmt * bidPrice.Value, 6);
                    var result2 = await XRPL.SetOffer(client, bidXRPAmount);
                    if (result2.EngineResult == "tesSUCCESS") Console.WriteLine("Bid Order Placed: " + Settings.BotTokenAmt + " tokens at " + bidPrice);

                    Thread.Sleep(Settings.TxnThrottle * 1000);
                }

                client.Disconnect();

                var timeUntilNextUpdate = DateTime.UtcNow.AddHours(1);
                ConsoleScreen.ClearConsoleLines();
                ConsoleScreen.InitScreen(ref spinner, " Time to next bot update: " + (timeUntilNextUpdate - DateTime.UtcNow).Minutes + ":" + (timeUntilNextUpdate - DateTime.UtcNow).Seconds);

                while (!Console.KeyAvailable)
                {
                    if ((timeUntilNextUpdate - DateTime.UtcNow).Minutes <= 0)
                    {
                        client = new RippleClient(Settings.WebSocketUrl);
                        client.Connect();

                        initialMidPrice = await XRPL.GetBookOffers(client);

                        for (int i = 1; i < Settings.Steps + 1; i++)
                        {
                            var askPrice = initialMidPrice.Midprice * (1 + Settings.Interval * i);
                            var askXRPAmount = decimal.Round(Settings.BotTokenAmt * askPrice.Value, 6);
                            var result1 = await XRPL.FillOrder(client, askXRPAmount);
                            if (result1.EngineResult == "tesSUCCESS") Console.WriteLine("Ask Order Placed: " + Settings.BotTokenAmt + " tokens at " + askPrice);

                            Thread.Sleep(Settings.TxnThrottle * 1000);

                            var bidPrice = initialMidPrice.Midprice * (1 - Settings.Interval * i);
                            var bidXRPAmount = decimal.Round(Settings.BotTokenAmt * bidPrice.Value, 6);
                            var result2 = await XRPL.SetOffer(client, bidXRPAmount);
                            if (result2.EngineResult == "tesSUCCESS") Console.WriteLine("Bid Order Placed: " + Settings.BotTokenAmt + " tokens at " + bidPrice);

                            Thread.Sleep(Settings.TxnThrottle * 1000);
                        }

                        client.Disconnect();

                        timeUntilNextUpdate = DateTime.UtcNow.AddHours(1);
                        ConsoleScreen.ClearConsoleLines();
                        ConsoleScreen.InitScreen(ref spinner, " Time to next bot update: " + (timeUntilNextUpdate - DateTime.UtcNow).Minutes + ":" + (timeUntilNextUpdate - DateTime.UtcNow).Seconds);
                    }
                    else
                    {                     
                        ConsoleScreen.InitScreen(" Time to next bot update: " + (timeUntilNextUpdate - DateTime.UtcNow).Minutes + ":" + (timeUntilNextUpdate - DateTime.UtcNow).Seconds);
                        Thread.Sleep(1);
                    }
                }

                spinner.Stop();

                ConsoleScreen.ClearConsoleLines();
                ConsoleScreen.InitScreen(" Market maker cancelled - Press any key to go back to the menu...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                spinner.Stop();
                ConsoleScreen.ClearConsoleLines();
                Console.SetCursorPosition(0, 7);
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
