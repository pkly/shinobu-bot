using System;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus;
using Disqord.Rest;

namespace Shinobu.Interaction.View
{
    public class ConfirmView : ViewBase
    {
        private readonly Func<Task<DiscordResponseCommandResult>> _onAccept;

        public ConfirmView(string message, Func<Task<DiscordResponseCommandResult>> onAccept) : base(new LocalMessage().WithEmbeds(Program.GetEmbed(message)))
        {
            _onAccept = onAccept;
        }

        [Button(Label = "Yes", Style = LocalButtonComponentStyle.Success)]
        public async ValueTask OnAccept(ButtonEventArgs e)
        {
            var response = await _onAccept();

            var message = new LocalInteractionResponse()
                .WithEmbeds(response.Message.Embeds);
            
            await e.Interaction.Response().SendMessageAsync(message);
        }

        [Button(Label = "No", Style = LocalButtonComponentStyle.Danger)]
        public async ValueTask OnCancel(ButtonEventArgs e)
        {
            Menu.Stop();
            await Menu.Client.DeleteMessageAsync(Menu.ChannelId, Menu.MessageId);
        }
    }
}