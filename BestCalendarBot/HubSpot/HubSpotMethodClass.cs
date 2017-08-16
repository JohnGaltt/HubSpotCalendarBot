using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace BestCalendarBot.HubSpot
{
    public class HubSpotMethodClass
    {
        //private string geturl = "https://api.hubapi.com/calendar/v1/events?startDate=1504161900000&endDate=1506840300000&hapikey=432b1757-a2f4-47e9-a164-345ddb78e197&limit=2";
        //private string inserturl = "https://api.hubapi.com/calendar/v1/events/task?hapikey=432b1757-a2f4-47e9-a164-345ddb78e197";
        //string key ="432b1757-a2f4-47e9-a164-345ddb78e197";
        private DateTime todaydate = DateTime.Now;
        private DateTime thismonth = DateTime.Now.AddMonths(1);

        public IEnumerable<string> GetEvent(object key)
        {
            double today = GetTime(todaydate);
            double month = GetTime(thismonth);
            string geturl = "https://api.hubapi.com/calendar/v1/events?startDate=" + (long)today + "&endDate=" + (long)month + "&hapikey=" + key + "&limit=10";
            string result = String.Empty;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(geturl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            List<GetEventClass> eventslist = JsonConvert.DeserializeObject<List<GetEventClass>>(result);

            var query = from e in eventslist
                        select "Name of event: " + e.name + ". Description: " + e.description + ". Time: " + DateTimeParse(e.eventDate).AddHours(-4).ToString();

            return query;
        }

        public void InsertEvent(object key,InsertEventClass even)
        {
            string inserturl = "https://api.hubapi.com/calendar/v1/events/task?hapikey=" + key;
            InsertEventClass insertevent = new InsertEventClass();
            insertevent.name = even.name;
            insertevent.description = even.description;
            insertevent.eventDate = even.eventDate;
            insertevent.category = "CUSTOM";
            insertevent.state = "TODO";

            string jsoneventinsert = JsonConvert.SerializeObject(insertevent);

            var request = (HttpWebRequest)WebRequest.Create(inserturl);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsoneventinsert);
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        public DateTime DateTimeParse(double milliseconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds).ToLocalTime();
        }

        public double GetTime(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}