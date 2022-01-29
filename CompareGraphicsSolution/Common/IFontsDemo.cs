namespace Common
{
    public interface IFontsDemo
    {
        void Initialize(int width, int height);
        IList<FontInfo> GetFonts(int size);
        void AddFont(FontInfo fontInfo);
        void DrawTo(DrawStrategy drawStrategy, string extraText, string targetFilename);
    }

}