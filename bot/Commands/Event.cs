using System.Threading.Tasks;
using Disqord.Bot;
using Qmmands;
using Shinobu.Services.Event;

namespace Shinobu.Commands
{
    public class Event : ShinobuModuleBase
    {
        private readonly WinterService _winterService;

        public Event(WinterService winterService)
        {
            _winterService = winterService;
        }
        
        [RequireBotOwner]
        [Command("winter-event")]
        public async Task ForceWinterEvent()
        {
            await _winterService.RandomizeTask();
        }
    }
}