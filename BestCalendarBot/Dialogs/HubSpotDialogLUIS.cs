using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis.Models;
using BestCalendarBot.HubSpot;

namespace BestCalendarBot.Dialogs
{
    [LuisModel("908127d7-aade-405e-a748-1849d4d6ad24", "62752691d4ca4f76a78e97f85b6c7274")]
    [Serializable]
    public class HubSpotDialogLUIS : LuisDialog<object>
    {
        protected object HubKey;
        //InsertEventClass insertluis = new InsertEventClass();
        //HubSpotMethodClass method = new HubSpotMethodClass();
        protected string Name { get; set; }
        protected string Description { get; set; }
        protected string Date { get; set; }
        //"432b1757-a2f4-47e9-a164-345ddb78e197"

        [LuisIntent("HelloIntent")]
        private async Task Hello(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi, Im can take your task from HubSpot Calendar or add new what you want?(Firstly enter please your HubSpot api key to continue conversation)");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Key")]
        public async Task GetKey(IDialogContext context, LuisResult result)
        {
            string t = result.Query;
            HubKey = (object)t;
            await context.PostAsync("Im put your key to my memory.Write me what you want to do?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetEvent")]
        private async Task GetTask(IDialogContext context, LuisResult result)
        {
            HubSpotMethodClass method = new HubSpotMethodClass();
            foreach (var e in method.GetEvent(HubKey))
                await context.PostAsync(e);

            context.Wait(MessageReceived);
        }

        [LuisIntent("InsertEvent")]
        private async Task InsertTask(IDialogContext context, LuisResult result)
        {
            PromptDialog.Text(context: context, resume: NameHandler, prompt: "Enter name please!",retry: "Sorry try again");

            //await context.PostAsync("Write plaese name of task!");
            //context.Wait(MessageReceived);
        }

        private async Task NameHandler(IDialogContext context,IAwaitable<string> argument)
        {
            var task_name = await argument;
            Name = task_name;
            PromptDialog.Text(context: context, resume: DescriptionHendler, prompt: $"Ok write description of task! name task is {Name}",retry: "Sorry try again!");
        }

        private async Task DescriptionHendler(IDialogContext context,IAwaitable<string> argument)
        {
            var description = await argument;
            Description = description;

            PromptDialog.Text(context: context, resume: DateHandler, prompt: "Ok write a date of task!(yyyy-MM-dd HH:mm:ss)", retry: "Try again!");
        }

        private async Task DateHandler(IDialogContext context, IAwaitable<string> result)
        {
            var d = await result;
            Date = d;

            HubSpotMethodClass method = new HubSpotMethodClass();
            DateTime date = DateTime.ParseExact(Date, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            long task_date = (long)method.GetTime(date.AddHours(4));
            InsertEventClass insertluis = new InsertEventClass();
            insertluis.name = Name;
            insertluis.description = Description;
            insertluis.eventDate = task_date;

            method.InsertEvent(HubKey, insertluis);

            await context.PostAsync($@"Your task succesful added thank you)
                  {Environment.NewLine} Your task:
                   {Environment.NewLine} Name: {Name}
                   {Environment.NewLine}Description: {Description}
                   {Environment.NewLine}Date: {Date}");
        }

        [LuisIntent("Goodbye")]
        private async Task Goodbye(IDialogContext context,LuisResult result)
        {
            await context.PostAsync("Goodbye.");
        }

        [LuisIntent("")]
        private async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I dont understund you try again(");
            context.Wait(MessageReceived);
        }
    }
}