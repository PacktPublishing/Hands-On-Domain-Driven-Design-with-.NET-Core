using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd
{
    public class PictureSize : Value<PictureSize>
    {
        public int Width { get; internal set; }
        public int Height { get; internal set; }

        internal PictureSize() { }

        public PictureSize(int width, int height)
        {
            if (Width <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    "Picture width must be a positive number");

            if (Height <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(height),
                    "Picture height must be a positive number");

            Width = width;
            Height = height;
        }
    }
}