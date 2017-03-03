using eZet.EveLib.EveCrestModule;
using eZet.EveLib.EveXmlModule;
using eZet.EveLib.EveXmlModule.Models.Character;
using eZet.EveLib.EveXmlModule.Models.Corporation;
using JKON.EveWho.Corporation;
using JKON.EveWho.Wars;
using Legend.Shared.SportsCourseServices.Models.CourseDetailsSummary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R3MUS.Devpack.Wardec
{
    class Program
    {
        private static CharacterKey cKey = EveXml.CreateCharacterKey(4232966, "dvb8nHXuh44ZRYwicVGpSjhFY5gxKScPtFZHGDeQxcwgxtYZy0fbat9NVft7fWH5");

        private static List<Character> chars = cKey.Characters.ToList();
        
        static void Main(string[] args)
        {
            //PingNotifications();

            //Properties.Settings.Default.Reset();

            GetWars_ESI();
        }
        
        static void GetWars_ESI()
        {
            var ourWars = new List<long>();
            try
            {
                ourWars = Properties.Settings.Default.CurrentWars.Split(',').Select(s => Int64.Parse(s)).ToList();
            }
            catch(Exception ex)
            {

            }
            var counter = 0;

            var allWars = War.GetWars().OrderBy(w => w).ToList();

            var newWars = allWars.Where(w => w > Properties.Settings.Default.LastMessageId).ToList();

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
                if (!allWars.Contains(f))
                {
                    try
                    {
                        var x = PingWar(f, "Ended");
                    }
                    catch (Exception e) {
                        SendMessage("Well, you can't seem to get details of a war that's ended :(", e.Message, "#ff0000");
                    }

                    ourWars.Remove(f);
                }
            });

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
                (warDeets.Aggressor.Alliance_Id == chars.First().AllianceId)
                ||
                (warDeets.Aggressor.Corporation_Id == chars.First().CorporationId)
                ||
                (warDeets.Defender.Alliance_Id == chars.First().AllianceId)
                ||
                (warDeets.Defender.Corporation_Id == chars.First().CorporationId)
            )
            {
                var name = string.Empty;

                if (warDeets.Aggressor.Alliance_Id.HasValue)
                {
                    name = GetAlliance(warDeets.Aggressor.Alliance_Id.Value).Alliance_Name;
                }
                else
                {
                    name = GetCorporation(warDeets.Aggressor.Corporation_Id.Value).Corporation_Name;
                }
                if (warDeets.Defender.Alliance_Id.HasValue)
                {
                    name = string.Concat(name, " vs ", GetAlliance(warDeets.Defender.Alliance_Id.Value).Alliance_Name);
                }
                else
                {
                    name = string.Concat(name, " vs ", GetCorporation(warDeets.Defender.Corporation_Id.Value).Corporation_Name);
                }

                var dt = new DateTime();
                var colour = string.Empty;
                if(type == "Started")
                {
                    dt = warDeets.Declared;
                    colour = "#ff80bf";
                }
                else
                {
                    dt = DateTime.UtcNow;
                    colour = "#ffe6f2";
                }

                SendMessage(name, string.Format("War {0}: {1}", type, dt.ToString()), colour);

                return warId;
            }
            else
            {
                return null;
            }
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

        static Alliance GetAlliance(long id)
        {
            return new Alliance(id);
        }

        static JKON.EveWho.Corporation.Corporation GetCorporation(long id)
        {
            return new JKON.EveWho.Corporation.Corporation(id);
        }

        static void PingNotifications()
        {
            var notes1 = new List<NotificationList.Notification>();
            var texts = new List<NotificationTexts.Notification>();

            chars.ForEach(toon => 
            {
                var toonNotes = toon.GetNotifications().Result.Notifications;
                notes1.AddRange(toonNotes);
                if (toonNotes.Any())
                {
                    texts.AddRange(toon.GetNotificationTexts(toonNotes.Select(note => note.NotificationId).ToArray()).Result.Notifications.ToList());
                }
            });

            notes1 = notes1.GroupBy(note => note.NotificationId).Select(grp => grp.First()).OrderBy(note => note.NotificationId).ToList();
            texts = texts.GroupBy(grp => grp.NotificationId).Select(grp => grp.First()).ToList();

            var text = texts.Where(t => t.NotificationId == 616759473).First();
            
            var includeIds = new List<long>();
            includeIds.Add(5);
            includeIds.Add(6);
            includeIds.Add(7);
            includeIds.Add(8);
            includeIds.Add(27);
            includeIds.Add(28);
            includeIds.Add(29);
            includeIds.Add(30);
            includeIds.Add(31);

            var warNotes = notes1.Where(note => includeIds.Contains(note.TypeId)).ToList();

            var all = EveXml.Eve.GetAllianceList().Result.Alliances.ToList();

            //Console.WriteLine("All:");
            //WriteTexts(notes1, texts);
            //Console.WriteLine("");
            Console.WriteLine("War:");
            WriteTexts(warNotes, texts);
            
            //if (notes1.Count() > 0)
            //{
            //    Properties.Settings.Default.LastMessageId = notes1.Select(note => note.NotificationId).Max();
            //    Properties.Settings.Default.Save();
            //}
            Console.WriteLine("That's all folks...");
            Console.ReadLine();
        }

        static void POSTest()
        {
            var cKey = EveXml.CreateCorporationKey(4285630, "hmZDX0Ptrvb33GscSJVfeRy6rogh06XcymVI9GdueMahoRxZDvh0lWsOeGiBaKbS");
            var poses = cKey.Corporation.GetStarbaseList().Result.Starbases.ToList();

            poses.ForEach(pos =>
            {
                var posDetails = cKey.Corporation.GetStarbaseDetails(pos.ItemId).Result;

                Console.WriteLine(pos.ItemId);
                Console.WriteLine(pos.LocationId);
                Console.WriteLine(pos.MoonId);
                
                Console.WriteLine(posDetails.OnlineTimestampAsString);
                Console.WriteLine(posDetails.State);
                Console.WriteLine(posDetails.StateTimestampAsString);
                Console.WriteLine();

                var assets = cKey.Corporation.GetAssetList().Result.Items.Where(item => item.LocationId == pos.LocationId).ToList();

                assets.ForEach(asset => {
                    Console.WriteLine(asset.ItemId);
                    //var typeName = EveXml.Eve.GetTypeName(new long[] { asset.TypeId }).Result.Types;
                    var types = EveXml.Eve.GetTypeName(new long[] { asset.TypeId }).Result.Types.ToList();
                    types.ForEach(type => {
                        Console.WriteLine(type.TypeName);
                    });
                    //Console.WriteLine(EveXml.Eve.GetTypeName(new long[] { asset.TypeId }).Result);
                    Console.WriteLine(asset.Quantity);
                });
            });

            Console.ReadLine();
        }

        static void WriteTexts(List<NotificationList.Notification> notes,List<NotificationTexts.Notification> texts)
        {
            notes.ForEach(note =>
            {
                var text = texts.Where(txt => txt.NotificationId == note.NotificationId).First();

                var contentString = text.Content.Replace("\n", ", ");
                if (contentString.Contains("applicationText:") && !contentString.Contains("''"))
                {
                    var splta = contentString.Split(':');
                    var splta2 = splta[1].Split(new string[] { ", charId:" }, StringSplitOptions.RemoveEmptyEntries);
                    splta[1] = string.Format("'{0}', charId", splta2[0].Replace("'", ""));
                    contentString = string.Join(": ", splta);
                }
                var jsonString = string.Concat("{", contentString, "}");

                var content = JsonConvert.DeserializeObject<Content>(jsonString);

                Console.WriteLine(string.Format("Notification Id {0}\n Type Id {1}\n Content {2}",
                    text.NotificationId,
                    note.TypeId,
                    text.Content));
            });
        }
    }
}
