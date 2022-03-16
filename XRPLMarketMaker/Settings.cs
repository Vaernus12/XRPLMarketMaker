using Newtonsoft.Json.Linq;
using System.IO;

namespace EMBRS
{
    public static class Settings
    {
        public static string WebSocketUrl { get; set; }
        public static string BotAddress { get; set; }
        public static string BotSecret { get; set; }
        public static string CurrencyCode { get; set; }
        public static string IssuerAddress { get; set; }
        public static int BotTokenAmt { get; set; }
        public static int AccountLinesThrottle { get; set; }
        public static int TxnThrottle { get; set; }
        public static decimal FeeMultiplier { get; set; }
        public static int MaximumFee { get; set; }
        public static decimal TransferFee { get; set; }
        public static int Steps { get; set; }
        public static decimal Interval { get; set; }

        public static void Initialize()
        {
            string jsonConfig = File.ReadAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config/settings.json"));
            dynamic d = JObject.Parse(jsonConfig);
            WebSocketUrl = d.WebSocketURL;
            BotAddress = d.BotAddress;
            BotSecret = d.BotSecret;
            string currencyCodeVal = d.CurrencyCode.Value;
            if(currencyCodeVal.Length == 3) CurrencyCode = d.CurrencyCode.Value;
            else CurrencyCode = Utils.AddZeros(Utils.ConvertHex(d.CurrencyCode.Value));
            IssuerAddress = d.IssuerAddress;
            TransferFee = d.TransferFee;
            BotTokenAmt = d.BotTokenAmt;
            AccountLinesThrottle = d.AccountLinesThrottle;
            TxnThrottle = d.TxnThrottle;
            FeeMultiplier = d.FeeMultiplier;
            MaximumFee = d.MaximumFee;
            Steps = d.Steps;
            Interval = d.Interval;
        }
    }
}
