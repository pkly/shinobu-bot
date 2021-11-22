using SixLabors.Fonts;

namespace Shinobu.Services
{
    public class FontService
    {
        private readonly FontCollection _fontCollection = new();
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        public FontFamily FontFamily { get; private set; }
        public Font Bold { get; private set; }

        public FontService()
        {
            FontFamily = _fontCollection.Install(Program.AssetsPath + "/fonts/Roboto.ttf");
            Bold = FontFamily.CreateFont(34, FontStyle.Bold);
        }
    }
}