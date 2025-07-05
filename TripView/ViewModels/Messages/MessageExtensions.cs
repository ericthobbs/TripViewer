using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripView.ViewModels.Messages
{
    public static class MessageExtensions
    {
        public static string ToFileExtension(this SaveAsFormat format)
        {
            switch (format)
            {
                case SaveAsFormat.PNG:
                    return "*.png";
                default:
                    return "*.*";
            }
        }

        public static SkiaSharp.SKEncodedImageFormat ToSkiaImageFormat(this SaveAsFormat format)
        {
            switch (format)
            {
                case SaveAsFormat.PNG:
                default:
                    return SkiaSharp.SKEncodedImageFormat.Png;
            }
        }
    }
}
