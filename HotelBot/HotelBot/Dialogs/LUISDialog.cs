using HotelBot.Models;
using HotelBot.Models.FacebookModels;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HotelBot.Dialogs
{
    [LuisModel("4c319ffe-1bfd-4cf7-b8de-a6d08d8371f1", "bcac5e3e55b4456c933432d89938e388")]
    [Serializable]
    public class LUISDialog : LuisDialog<RoomReservation>
    {
        private readonly BuildFormDelegate<RoomReservation> ReserveRoom;

        [field: NonSerialized()]
        protected Activity _message;

        public LUISDialog(BuildFormDelegate<RoomReservation> reserveRoom)
        {
            this.ReserveRoom = reserveRoom;
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I don't understand what you are meaning.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            context.Call(new GreetingDialog(), Callback);
        }

        [LuisIntent("ReserveRoom")]
        public async Task RoomReservation(IDialogContext context, LuisResult result)
        {
            var enrollmentForm = new FormDialog<RoomReservation>(new RoomReservation(), this.ReserveRoom, FormOptions.PromptInStart);
            context.Call(enrollmentForm, Callback);
        }

        [LuisIntent("QueryAmenities")]
        public async Task QueryAmenities(IDialogContext context, LuisResult result)
        {
            foreach(var entity in result.Entities.Where(e => e.Type == "Amenity"))
            {
                var entityValue = entity.Entity.ToLower();

                if(entityValue == "kitchen" || entityValue == "towels" || entityValue == "wifi" || entityValue == "gym")
                {
                    //await context.PostAsync("We have that.");

                    Activity replyMessage = _message.CreateReply();

                    var facebookMessage = new FacebookMessage();
                    facebookMessage.attachment = new FacebookAttachment();
                    facebookMessage.attachment.type = "template";
                    facebookMessage.attachment.payload = new FacebookPayload();
                    facebookMessage.attachment.payload.template_type = "generic";

                    var amenity = new FacebookElement();
                    amenity.subtitle = "Yes, we have that";
                    amenity.title = entityValue;

                    switch (entityValue)
                    {
                        case "kitchen":
                            amenity.image_url = "http://luxurylaunches.com/wp-content/uploads/2015/10/Mark-hotel-suites-3.jpg";
                            break;
                        case "gym":
                            amenity.image_url = "https://s-media-cache-ak0.pinimg.com/originals/cb/c9/4a/cbc94af79da9e334a8555e850da136f4.jpg";
                            break;
                        case "wifi":
                            amenity.image_url = "http://media.idownloadblog.com/wp-content/uploads/2016/02/wifi-icon.png";
                            break;
                        case "towels":
                            amenity.image_url = "http://www.prabhutextile.com/images/bath_towel_1.jpg";
                            break;
                        default:
                            break;
                    }

                    facebookMessage.attachment.payload.elements = new FacebookElement[] { amenity };

                    replyMessage.ChannelData = facebookMessage;
                    await context.PostAsync(replyMessage);
                    context.Wait(MessageReceived);
                    return;
                }
                else
                {
                    await context.PostAsync("I'm sorry. We don't have that.");
                    context.Wait(MessageReceived);
                    return;
                }
            }

            await context.PostAsync("I'm sorry. We don't have that.");
            context.Wait(MessageReceived);
            return;
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            _message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }
    }
}