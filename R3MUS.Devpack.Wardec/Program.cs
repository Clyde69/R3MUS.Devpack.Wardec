using eZet.EveLib.EveCrestModule;
using eZet.EveLib.EveXmlModule;
using eZet.EveLib.EveXmlModule.Models.Corporation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R3MUS.Devpack.Wardec
{
    class Program
    {
        static void Main(string[] args)
        {
            var cKey = EveXml.CreateCharacterKey(4433864, "rwZ1Eg8mGQrYG0AivsU11U8oklD83RnQ22O0YMzG6i4iqLVuReNRDCGGeF9fFoQo");
            var notes = cKey.Characters.LastOrDefault().GetNotifications().Result.Notifications;

            var all = EveXml.Eve.GetAllianceList().Result.Alliances.ToList();
            

            notes.Where(note => (note.TypeId == 5)).ToList().ForEach(note =>
            {
                Console.WriteLine(string.Concat("Wardec from alliance", all.Where(a => a.AllianceId == note.SenderId).FirstOrDefault().AllianceTag));
            });
            notes.Where(note => (note.TypeId == 27)).ToList().ForEach(note =>
            {
                var corp = EveXml.Eve.GetCorporationSheet(note.SenderId).Result;
                Console.WriteLine(string.Concat("Wardec from corp", corp.Ticker));
            });
            notes.Where(note => (note.TypeId == 8)).ToList().ForEach(note =>
            {
                Console.WriteLine(string.Concat("War ending with ", all.Where(a => a.AllianceId == note.SenderId)));
            });
            notes.Where(note => (note.TypeId == 31)).ToList().ForEach(note =>
            {
                var corp = EveXml.Eve.GetCorporationSheet(note.SenderId).Result;
                Console.WriteLine(string.Concat("War ending with ", corp.Ticker));
            });
            Console.WriteLine("That's all folks...");
            Console.ReadLine();
        }
    }
}
