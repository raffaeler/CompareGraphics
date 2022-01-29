namespace Common
{
    public interface IFontsDemo
    {
        void Initialize(int width, int height);
        IList<FontInfo> GetFonts(int size);
        void AddFont(FontInfo fontInfo);
        void DrawTo(string extraText, string targetFilename);
    }

}