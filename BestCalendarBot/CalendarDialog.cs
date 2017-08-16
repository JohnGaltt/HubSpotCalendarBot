using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using BotCalendar;

namespace BestCalendarBot
{
    [Serializable]
    public class CalendarDialog : IDialog
    {
        protected DateTime date { get; set; }
        protected string description { get; set; }
        //protected string UserName { get; set; }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessegeRecivedAsync);

            return Task.CompletedTask;
        }

        private Task MessegeRecivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            PromptDialog.Text(context, NameEntered, @"Hi whats your name?");

            return Task.CompletedTask;
        }

        private async Task NameEntered(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync($@"Hi { await result}! How can i help you?)You can select your event or add or delete!What you want?");

            context.Wait(MessageReceivedOperationChoice);
        }

        private async Task MessageReceivedOperationChoice(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Contains("add"))
            {
                await context.PostAsync("Ok enter please date of your new shedule!)");

                context.Wait(MessageReceivedAddShedule);
            }
            else if (activity.Text.Contains("select"))
            {
                await context.PostAsync("enter please show my calendar to see your shedule");

                context.Wait(ActivityRecivedAysnc);
            }
            else if (activity.Text.Contains("delete"))
            {
                await context.PostAsync("enter please date of event you want delete!");

                context.Wait(MessageReceivedDeleteShedule);
            }
            else
            {
                context.Wait(MessegeRecivedAsync);
            }
        }

        private async Task MessageReceivedDeleteShedule(IDialogContext context, IAwaitable<object> result)
        {
            Calendar calendar = new Calendar();

            var activity = await result as Activity;
            this.date = DateTime.Parse(activity.Text);

            calendar.DeleteEvent(date);

            await context.PostAsync("Your event succesful delete)");

            context.Wait(MessegeRecivedAsync);
        }

        private async Task MessageReceivedAddShedule(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            this.date = DateTime.Parse(activity.Text);

            await context.PostAsync("Enter description of event)");

            context.Wait(MessageReceivedAddDescription);
        }

        private async Task MessageReceivedAddDescription(IDialogContext context, IAwaitable<object> result)
        {
            Calendar calendar = new Calendar();

            var activity = await result as Activity;
            this.description = activity.Text;

            calendar.AddShedule(date, description);

            await context.PostAsync("Your shedule succesful added");

            context.Wait(MessegeRecivedAsync);
        }

        private async Task ActivityRecivedAysnc(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Contains("show calendar") || activity.Text.Contains("show my event"))
            {
                Calendar calendar = new Calendar();

                Shedule[] shedule = calendar.GetEvent();

                foreach (var s in shedule)
                {
                    await context.PostAsync($"your shedule: { s.Date.ToShortDateString() } is { s.Decription.ToString() } ");
                }
            }
            context.Wait(MessegeRecivedAsync);
        }
    }
}