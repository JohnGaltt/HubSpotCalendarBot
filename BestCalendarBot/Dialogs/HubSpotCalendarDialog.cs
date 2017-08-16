using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BestCalendarBot.HubSpot;

namespace BestCalendarBot.Dialogs
{
    [Serializable]
    public class HubSpotCalendarDialog : IDialog
    {
        protected object Key { get; set; }
        private string eventName { get; set; }
        private string eventDescription { get; set; }
        private long eventDate { get; set; }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessegeRecivedAsync);

            return Task.CompletedTask;
        }

        private Task MessegeRecivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            PromptDialog.Text(context, KeyRecivedAsync, @"Hi enter your api key)");

            return Task.CompletedTask;
        }

        private async Task KeyRecivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            Key = await result;
            await context.PostAsync($@"Hi { Key } its your key! You can get or add your event from your hybspot calendar.");

            context.Wait(OperationRecivedAsync);
        }

        private async Task OperationRecivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var messege = await result as Activity;

            if (messege.Text.Contains("get"))
            {
                HubSpotMethodClass method = new HubSpotMethodClass();
                foreach (var e in method.GetEvent(Key))
                    await context.PostAsync(e);

                context.Wait(MessegeRecivedAsync);
            }
            else if(messege.Text.Contains("add"))
            {
                await context.PostAsync("Enter please name of event!");
                context.Wait(InsertNameRecivedAsync);
            }
            else
            {
                await context.PostAsync("I dont understand you sorry try again)");
                context.Wait(MessegeRecivedAsync);
            }
        }

        private async Task InsertNameRecivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            eventName  = activity.Text;

            await context.PostAsync("Enter please description of event!");
            context.Wait(InsertDescriptionRecivedAsync);
        }


        private async Task InsertDescriptionRecivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            eventDescription = activity.Text;

            await context.PostAsync("Enter please date!(yyyy-MM-dd HH:mm:ss AM)");
            context.Wait(InsertDateRecivedAsync);
        }

        private async Task InsertDateRecivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            HubSpotMethodClass method = new HubSpotMethodClass();

            string messege = "Goodbye";

            try
            {
                var activity = await result as Activity;
                string dates = activity.Text;
                DateTime date = DateTime.ParseExact(dates, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                eventDate = (long)method.GetTime(date.AddHours(4));

                InsertEventClass insertedevent = new InsertEventClass();
                insertedevent.name = eventName;
                insertedevent.description = eventDescription;
                insertedevent.eventDate = eventDate;

                method.InsertEvent(Key, insertedevent);
                await context.PostAsync(messege);
                context.Wait(MessegeRecivedAsync);
            }
            catch
            {
                messege = "Enter please corect date.";
                await context.PostAsync(messege);
                context.Wait(InsertDateRecivedAsync);
            }
            //await context.PostAsync(messege);
            //context.Wait(MessegeRecivedAsync);
        }
    }
}