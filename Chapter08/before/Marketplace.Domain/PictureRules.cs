namespace Marketplace.Domain
{
    public static class PictureRules
    {
        public static bool HasCorrectSize(this Picture picture)
            => picture != null 
               && picture.Size.Width >= 800 
               && picture.Size.Height >= 600;
    }
}