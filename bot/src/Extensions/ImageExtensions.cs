using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace Shinobu.Extensions
{
    public static class ImageExtensions
    {
        public static IImageProcessingContext DrawTextCentered(
            this IImageProcessingContext context,
            string text,
            Font font,
            Color color,
            int height) =>
            context.DrawText(
                new TextGraphicsOptions(new GraphicsOptions(), new TextOptions() {HorizontalAlignment = HorizontalAlignment.Center}),
                text,
                font,
                Brushes.Solid(color),
                null,
                new PointF(context.GetCurrentSize().Width / 2, height)
            );
    }
}