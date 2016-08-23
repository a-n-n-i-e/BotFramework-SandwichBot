using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;

namespace SandwichBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        //質問項目と回答
        public enum SandwichOptions
        {
            RoastBeef, BLT, SubwayClub, RoastChicken,
            TeriyakiChicken, TurkeyBreast, Ham, Tuna, VeggieDelite
        }
        public enum LengthOptions
        {
            Regular, Footlong
        }
        public enum BreadOptions
        {
            Flatbread, White, Wheat, Sesame, HoneyOats
        }
        public enum ToppingsOptions
        {
            SliceCheese, CreamCheese, Bacon, Tuna, Avocado, Jalapeno, None
        }
        public enum VegetableLessOptions
        {
            All, Lettuce, Tomato, Pement, Onion, Pickles, Olives, None
        }
        public enum VegetableMoreOptions
        {
            All, Lettuce, Tomato, Pement, Onion, Pickles, Olives, None
        }
        public enum SauseOptions
        {
            Caesar, HoneyMustard, WasabiSoy, Basil, Balsamico, Mayonnaise, OilVinegar
        }

        [Serializable]
        public class SandwichOrder
        {
            //[Prompt("{&}をひとつお選びください{||}")]
            [Template(TemplateUsage.EnumSelectOne, "{&}をひとつお選びください{||}")]
            public SandwichOptions? サンドイッチの種類;
            [Template(TemplateUsage.EnumSelectOne, "{&}をひとつお選びください{||}")]
            public LengthOptions? サイズ;
            [Template(TemplateUsage.EnumSelectOne, "{&}をひとつお選びください{||}")]
            public BreadOptions? パンの種類;
            [Template(TemplateUsage.EnumSelectMany, "{&}(複数選択もOK)をお選びください{||}")]
            public List<ToppingsOptions> 追加するオプション;
            [Template(TemplateUsage.EnumSelectMany, "{&}(複数選択もOK)をお選びください{||}")]
            public List<VegetableLessOptions> 抜きたい野菜;
            [Template(TemplateUsage.EnumSelectMany, "{&}(複数選択もOK)をお選びください{||}")]
            public List<VegetableMoreOptions> 増やしたい野菜;
            [Template(TemplateUsage.EnumSelectMany, "{&}(複数選択もOK)をお選びください{||}")]
            public List<SauseOptions> ソース;

            public static IForm<SandwichOrder> BuildForm()
            {
                OnCompletionAsyncDelegate<SandwichOrder> processOrder = async (context, state) =>
                 {
                     await context.PostAsync("ご利用ありがとうございました！");
                 };

                return new FormBuilder<SandwichOrder>()
                    //.Message("こんにちは! 野菜の○ブウェイです。ご注文をどうぞ!")
                    //.Build();

                    .Message("こんにちは! 野菜の○ブウェイです。ご注文を承ります！")
                    .Field(nameof(サンドイッチの種類))
                    .Field(nameof(サイズ))
                    .Field(nameof(パンの種類))
                    .Field(nameof(追加するオプション))
                    .Field(nameof(抜きたい野菜))
                    .Field(nameof(増やしたい野菜))
                    .Field(nameof(ソース))
                    .Confirm("注文はこちらでよろしいでしょうか？ (宜しければ 1:はい, 変更する場合は 2:いいえ を送信してください) ----- {サンドイッチの種類}、{サイズ}サイズ＆{パンの種類} (追加オプション:{追加するオプション}、野菜抜き:{抜きたい野菜}、野菜増量:{増やしたい野菜}、{ソース}ソース)")
                    .Message("ご注文完了です。")
                    .OnCompletion(processOrder)
                    .Build();

            }
        }

        internal static IDialog<SandwichOrder> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(SandwichOrder.BuildForm));
        }


        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null)
            {
                if (activity.GetActivityType() == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, () => MakeRootDialog());
                }
                else
                {
                    HandleSystemMessage(activity);
                }
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        //public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        //{
        //    if (activity.Type == ActivityTypes.Message)
        //    {
        //        //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
        //        //// calculate something for us to return
        //        //int length = (activity.Text ?? string.Empty).Length;

        //        //// return our reply to the user
        //        //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
        //        //await connector.Conversations.ReplyToActivityAsync(reply);

        //        await Conversation.SendAsync(activity, MakeRootDialog);
        //    }
        //    else
        //    {
        //        HandleSystemMessage(activity);
        //    }
        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    return response;
        //}

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}