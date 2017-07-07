using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetBot
{
    class BetList
    {
        public string arbId { get ; set; }
        public string eventName { get; set; }
        public string league { get; set; }
        public string countryId { get; set; }
        public string betId { get; set; }
        public string bookmakerId { get; set; }
        public string parentDiv { get; set; }
        public string childDiv { get; set; }
        public string sportId { get; set; }
        public string sportName { get; set; }
        public string home { get; set; }
        public string away { get; set; }
        public string koef { get; set; }
        public string betType { get; set; }

       
        public BetList() { }

        public BetList(string carbId, string ceventName, string cleague, string ccountryId, string cbetId, string cbookmakerId, string cparentDiv, string cchildDiv, string csportId, string csportName, string chome, string caway, string ckoef, string cbetType)
        {

            arbId = carbId;
            eventName = ceventName;
            league = cleague;
            countryId = ccountryId;
            betId = cbetId;
            bookmakerId = cbookmakerId;
            parentDiv = cparentDiv;
            childDiv = cchildDiv;
            sportId = csportId;
            sportName = csportName;
            home = chome;
            away = caway;
            koef = ckoef;
            betType = cbetType;
        }
    }
}
