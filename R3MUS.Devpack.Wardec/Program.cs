using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using R3MUS.Devpack.ESI.Models;

namespace R3MUS.Devpack.Wardec
{
    class Program
    {

        static long corpId = Properties.Settings.Default.HomeCorp;
        static long allId = Properties.Settings.Default.HomeAlliance;

        private static string WarMessage = "SANGUINE ALERT! WARDECS LIVE! Highsec is still a hive of scum and villainy.";
        private static string NoWarMessage = "VERDANT ALERT! NO WARDECS! Highsec is still a hive of scum and villainy.";
        
        static void Main(string[] args)
        {
            //PingNotifications();

            //Properties.Settings.Default.Reset();

            GetWars_ESI();
        }
        
        static void GetWars_ESI()
        {
            var ourWars = new List<long>();
            var removeWars = new List<long>();
            try
            {
                ourWars = Properties.Settings.Default.CurrentWars.Split(',').Select(s => Int64.Parse(s)).ToList();
            }
            catch(Exception ex)
            {

            }
            var atWar = ourWars.Any();
            var counter = 0;

            var allWars = War.GetWarIds().OrderBy(w => w).ToList();

            var newWars = allWars.Where(w => w > Properties.Settings.Default.LastMessageId).ToList();

            Console.WriteLine(string.Format("Starting at war ID {0}", (Properties.Settings.Default.LastMessageId + 1).ToString()));

            newWars.ForEach(w =>
            {
                if (!ourWars.Contains(w))
                {
                    if (counter == 20)
                    {
                        System.Threading.Thread.Sleep(6000);
                        counter = 0;
                    }
                    try
                    {
                        var x = PingWar(w, "Started");
                        if(x.HasValue)
                        {
                            ourWars.Add(x.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally { counter++; }
                }
            });

            ourWars.ForEach(f =>
            {
                var warDeets = new War(f);
                if(warDeets.EndTime.HasValue && warDeets.EndTime.Value < DateTime.UtcNow)
                {
                    PingWar(f, "Ended");
                    removeWars.Add(f);
                }
            });
            ourWars = ourWars.Except(removeWars).ToList();

            if(ourWars.Any() && atWar != ourWars.Any())
            {
                Slack.Plugin.SetChannelTopic(Properties.Settings.Default.Group, Properties.Settings.Default.Room,
                    Properties.Settings.Default.Token, WarMessage);
            }
            else if (!ourWars.Any() && atWar != ourWars.Any())
            {
                Slack.Plugin.SetChannelTopic(Properties.Settings.Default.Group, Properties.Settings.Default.Room,
                    Properties.Settings.Default.Token, NoWarMessage);
            }

            Properties.Settings.Default.CurrentWars = String.Join(",", ourWars.Select(i => i.ToString()).ToArray());

            if (allWars.Count() > 0)
            {
                Properties.Settings.Default.LastMessageId = allWars.Max();
            }
            Properties.Settings.Default.Save();
        }

        static long? PingWar(long warId, string type)
        {
            var warDeets = new War(warId);
            if (
                (warDeets.Aggressor.Alliance_Id == allId)
                ||
                (warDeets.Aggressor.Corporation_Id == corpId)
                ||
                (warDeets.Defender.Alliance_Id == allId)
                ||
                (warDeets.Defender.Corporation_Id == corpId)
            )
            {
                var name = string.Empty;

                if (warDeets.Aggressor.Alliance_Id.HasValue)
                {
                    name = GetAlliance(warDeets.Aggressor.Alliance_Id.Value).Name;
                }
                else
                {
                    name = GetCorporation(warDeets.Aggressor.Corporation_Id.Value).Corporation_Name;
                }
                if (warDeets.Defender.Alliance_Id.HasValue)
                {
                    name = string.Concat(name, " vs ", GetAlliance(warDeets.Defender.Alliance_Id.Value).Name);
                }
                else
                {
                    name = string.Concat(name, " vs ", GetCorporation(warDeets.Defender.Corporation_Id.Value).Corporation_Name);
                }

                var dt = new DateTime();
                var colour = string.Empty;
                if(type == "Started")
                {
                    dt = warDeets.StartTime != null? warDeets.StartTime: warDeets.Declared;
                    colour = "#ff80bf";
                    AddPost(string.Format("War {0} {1}", type, dt.ToString("yyyy-MM-dd HHmm")), name);
                }
                else
                {
                    dt = warDeets.EndTime.Value;
                    colour = "#ffe6f2";
                    ScrapPost(name);
                }

                SendMessage(name, string.Format("War {0}: {1}", type, dt.ToString()), colour);

                return warId;
            }
            else
            {
                return null;
            }
        }

        static string AddPost(string name, string warName)
        {
            return BaseRequest(string.Format(Properties.Settings.Default.AddURI, name, warName, WarMessage));
        }

        static string ScrapPost(string warName)
        {
            return BaseRequest(string.Format(Properties.Settings.Default.ScrapURI, warName));
        }

        static string BaseRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "R3MUS.Devpack.EveWho-Clyde-en-Marland";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        static void SendMessage(string title, string text, string colour)
        {
            var att = new Slack.MessagePayloadAttachment() { Title = title, Text = text, Colour = colour };
            var payload = new Slack.MessagePayload() { Channel = Properties.Settings.Default.Room, Username = Properties.Settings.Default.BotName,
                Text = "Status Report", Attachments = new List<Slack.MessagePayloadAttachment>() };
            payload.Attachments.Add(att);
            Slack.Plugin.SendToRoom(payload,
                Properties.Settings.Default.Room,
                Properties.Settings.Default.SlackWebhook,
                Properties.Settings.Default.BotName);
        }

        static ESI.Models.Alliance.Detail GetAlliance(long id)
        {
            return new ESI.Models.Alliance.Detail(id);
        }

        static Corporation.Detail GetCorporation(long id)
        {
            return new Corporation.Detail(id);
        }
        
        //static void POSTest()
        //{
        //    var cKey = EveXml.CreateCorporationKey(, "");
        //    var poses = cKey.Corporation.GetStarbaseList().Result.Starbases.ToList();

        //    poses.ForEach(pos =>
        //    {
        //        var posDetails = cKey.Corporation.GetStarbaseDetails(pos.ItemId).Result;

        //        Console.WriteLine(pos.ItemId);
        //        Console.WriteLine(pos.LocationId);
        //        Console.WriteLine(pos.MoonId);
                
        //        Console.WriteLine(posDetails.OnlineTimestampAsString);
        //        Console.WriteLine(posDetails.State);
        //        Console.WriteLine(posDetails.StateTimestampAsString);
        //        Console.WriteLine();

        //        var assets = cKey.Corporation.GetAssetList().Result.Items.Where(item => item.LocationId == pos.LocationId).ToList();

        //        assets.ForEach(asset => {
        //            Console.WriteLine(asset.ItemId);
        //            //var typeName = EveXml.Eve.GetTypeName(new long[] { asset.TypeId }).Result.Types;
        //            var types = EveXml.Eve.GetTypeName(new long[] { asset.TypeId }).Result.Types.ToList();
        //            types.ForEach(type => {
        //                Console.WriteLine(type.TypeName);
        //            });
        //            //Console.WriteLine(EveXml.Eve.GetTypeName(new long[] { asset.TypeId }).Result);
        //            Console.WriteLine(asset.Quantity);
        //        });
        //    });

        //    Console.ReadLine();
        //}        
    }
}
