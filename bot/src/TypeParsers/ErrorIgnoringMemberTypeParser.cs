using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Parsers;
using Qmmands;

namespace Shinobu.TypeParsers
{
    public class ErrorIgnoringMemberTypeParser : MemberTypeParser
    {
        public override ValueTask<TypeParserResult<IMember?>> ParseAsync(
            Parameter parameter,
            string value,
            DiscordGuildCommandContext context)
        {
            var parsed = base.ParseAsync(parameter, value, context);
            return !parsed.IsCompletedSuccessfully ? 
                new ValueTask<TypeParserResult<IMember?>>(TypeParserResult<IMember?>.Successful(null)) :
                parsed;
        }
    }
}