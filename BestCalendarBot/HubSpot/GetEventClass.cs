using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BestCalendarBot.HubSpot
{
    public class GetEventClass
    {
        public string id { get; set; }
        public string eventType { get; set; }
        public long eventDate { get; set; }
        public string category { get; set; }
        public int categoryId { get; set; }
        public object contentId { get; set; }
        public string state { get; set; }
        public string campaignGuid { get; set; }
        public int portalId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public object url { get; set; }
        public int ownerId { get; set; }
        public int createdBy { get; set; }
        public bool createContent { get; set; }
        public object previewKey { get; set; }
        public object templatePath { get; set; }
        public object socialUsername { get; set; }
        public object socialDisplayName { get; set; }
        public object avatarUrl { get; set; }
        public bool isRecurring { get; set; }
        public object topicIds { get; set; }
        public object contentGroupId { get; set; }
        public int userId { get; set; }
        public object userIds { get; set; }
        public bool recurring { get; set; }
    }
}