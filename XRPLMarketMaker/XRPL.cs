using Newtonsoft.Json.Linq;
using Ripple.TxSigning;
using RippleDotNet;
using RippleDotNet.Model;
using RippleDotNet.Model.Account;
using RippleDotNet.Model.Transaction;
using RippleDotNet.Model.Transaction.Interfaces;
using RippleDotNet.Model.Transaction.TransactionTypes;
using RippleDotNet.Requests.Transaction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EMBRS
{
    public static class XRPL
    {
        public static async Task ShowBookOffers(bool simulate = false)
        {
            IRippleClient client = new RippleClient(Settings.WebSocketUrl);
            try
            {
                client.Connect();

                BookOffersRequest request1 = new()
                {
                    TakerGets = new Currency
                    {
                        CurrencyCode = Settings.CurrencyCode,
                        Issuer = Settings.IssuerAddress
                    },
                    TakerPays = new Currency()
                };

                BookOffersRequest request2 = new()
                {
                    TakerGets = new Currency(),
                    TakerPays = new Currency
                    {
                        CurrencyCode = Settings.CurrencyCode,
                        Issuer = Settings.IssuerAddress
                    }
                };

                var offers = await client.BookOffers(request1);
                Thread.Sleep(Settings.TxnThrottle * 1000);
                var offers2 = await client.BookOffers(request2);
                Console.SetCursorPosition(0, 7);

                Console.WriteLine("Asks");
                decimal? lowestAsk = 100000;
                for (int i = offers.Offers.Count - 1; i > 0; i--)
                {
                    var value = offers.Offers[i].TakerPays.ValueAsXrp / offers.Offers[i].TakerGets.ValueAsNumber;
                    Console.WriteLine(offers.Offers[i].TakerPays.ValueAsXrp + " / " + offers.Offers[i].TakerGets.ValueAsNumber + " = " + value);
                    if (value < lowestAsk) lowestAsk = value;
                }

                Console.WriteLine();

                Console.WriteLine("Bids");
                decimal? highestBid = 0;
                for (int i = 0; i < offers2.Offers.Count; i++)
                {
                    var value = offers2.Offers[i].TakerGets.ValueAsXrp / offers2.Offers[i].TakerPays.ValueAsNumber;
                    Console.WriteLine(offers2.Offers[i].TakerGets.ValueAsXrp + " / " + offers2.Offers[i].TakerPays.ValueAsNumber + " = " + value);
                    if (value > highestBid) highestBid = value;
                }

                Console.WriteLine();

                var midPrice = (lowestAsk + highestBid) / 2;
                var spread = lowestAsk - highestBid;
                Console.WriteLine("Mid Price: " + midPrice.ToString());
                Console.WriteLine("Spread: " + spread.ToString());

                if (simulate)
                {
                    Console.WriteLine();
                    Console.WriteLine("Simulating Orders");
                    Console.WriteLine();
                    Console.WriteLine("Asks");
                    for (int i = 1; i < Settings.Steps + 1; i++)
                    {
                        var askPrice = midPrice * (1 + Settings.Interval * i);
                        var askXRPAmount = Settings.BotTokenAmt * askPrice;
                        Console.WriteLine("Ask Order Placed: (EMBRS) " + Settings.BotTokenAmt + " - (XRP) " + string.Format("{0:0.000000}", askXRPAmount) + " - (Price) " + askPrice);
                    }

                    Console.WriteLine();
                    Console.WriteLine("Bids");
                    for (int i = 1; i < Settings.Steps + 1; i++)
                    {
                        var bidPrice = midPrice * (1 - Settings.Interval * i);
                        var bidXRPAmount = Settings.BotTokenAmt * bidPrice;
                        Console.WriteLine("Bid Order Placed: (EMBRS) " + Settings.BotTokenAmt + " - (XRP) " + string.Format("{0:0.000000}", bidXRPAmount) + " - (Price) " + bidPrice);
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to return to menu...");
                while (!Console.KeyAvailable);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ShowBookOffers(): " + ex.Message);
            }
            finally
            {
                client.Disconnect();
            }
        }

        public static async Task<BookOfferReturnObj> GetBookOffers(IRippleClient client)
        {
            BookOfferReturnObj returnObj = new();

            try
            {
                BookOffersRequest request1 = new()
                {
                    TakerGets = new Currency
                    {
                        CurrencyCode = Settings.CurrencyCode,
                        Issuer = Settings.IssuerAddress
                    },
                    TakerPays = new Currency()
                };

                BookOffersRequest request2 = new()
                {
                    TakerGets = new Currency(),
                    TakerPays = new Currency
                    {
                        CurrencyCode = Settings.CurrencyCode,
                        Issuer = Settings.IssuerAddress
                    }
                };

                var offers = await client.BookOffers(request1);
                Thread.Sleep(Settings.TxnThrottle * 1000);
                var offers2 = await client.BookOffers(request2);

                decimal? lowestBid = 100000;
                for (int i = offers.Offers.Count - 1; i > 0; i--)
                {
                    var value = offers.Offers[i].TakerPays.ValueAsXrp / offers.Offers[i].TakerGets.ValueAsNumber;
                    if (value < lowestBid) lowestBid = value;
                }

                decimal? highestAsk = 0;
                for (int i = 0; i < offers2.Offers.Count; i++)
                {
                    var value = offers2.Offers[i].TakerGets.ValueAsXrp / offers2.Offers[i].TakerPays.ValueAsNumber;
                    if (value > highestAsk) highestAsk = value;
                }

                var midPrice = ((lowestBid) + (highestAsk)) / 2;
                var spread = lowestBid - highestAsk;
                returnObj.Midprice = midPrice;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetBookOffers(): " + ex.Message);
            }

            return returnObj;
        }

        public static async Task<Submit> FillOrder(IRippleClient client, decimal? xrpAmount)
        {
            try
            {
                AccountInfo accountInfo = await client.AccountInfo(Settings.BotAddress);

                IOfferCreateTransaction offerCreate = new OfferCreateTransaction
                {
                    Sequence = accountInfo.AccountData.Sequence,
                    TakerGets = new Currency { CurrencyCode = Settings.CurrencyCode, Issuer = Settings.IssuerAddress, Value = Settings.BotTokenAmt.ToString() },
                    TakerPays = new Currency { ValueAsXrp = xrpAmount },
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Account = Settings.BotAddress
                };

                TxSigner signer = TxSigner.FromSecret(Settings.BotSecret); //secret is not sent to server, offline signing only
                SignedTx signedTx = signer.SignJson(JObject.Parse(offerCreate.ToJson()));

                SubmitBlobRequest request = new()
                {
                    TransactionBlob = signedTx.TxBlob
                };

                Submit result = await client.SubmitTransactionBlob(request);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Submit> SetOffer(IRippleClient client, decimal? xrpAmount)
        {
            try
            {
                AccountInfo accountInfo = await client.AccountInfo(Settings.BotAddress);

                IOfferCreateTransaction offerCreate = new OfferCreateTransaction
                {
                    Sequence = accountInfo.AccountData.Sequence,
                    TakerGets = new Currency { ValueAsXrp = xrpAmount },
                    TakerPays = new Currency { CurrencyCode = Settings.CurrencyCode, Issuer = Settings.IssuerAddress, Value = Settings.BotTokenAmt.ToString() },
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Account = Settings.BotAddress
                };

                var json = offerCreate.ToJson();
                TxSigner signer = TxSigner.FromSecret(Settings.BotSecret); //secret is not sent to server, offline signing only
                SignedTx signedTx = signer.SignJson(JObject.Parse(offerCreate.ToJson()));

                SubmitBlobRequest request = new()
                {
                    TransactionBlob = signedTx.TxBlob
                };

                Submit result = await client.SubmitTransactionBlob(request);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public struct BookOfferReturnObj
        {
            public decimal? Midprice { get; set; }
        }
    }
}
