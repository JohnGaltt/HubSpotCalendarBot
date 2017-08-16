using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BestCalendarBot.HubSpot
{
    public class InsertEventClass
    {
        public long eventDate { get; set; }
        public string category { get; set; }
        public string state { get; set; }
        public string campaignGuid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int ownerId { get; set; }
        public List<int> topicIds { get; set; }
        public int contentGroupId { get; set; }
    }
}