using Common;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.CompilerServices;

namespace DemoSystemDrawingCommon
{
    public class FontsDemo : IFontsDemo, IDisposable
    {
        private Bitmap? _bitmap;
        private Dictionary<string, (IoTBdfFont? bdffont, Font? nativefont, FontInfo fontInfo)> _fonts = new();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public FontsDemo()
        {
            //_systemFonts = new();
        }

        public void Dispose()
        {
            _bitmap?.Dispose();
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;

            _bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        }

        public void Clear() => _bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

        /// <summary>
        /// Returns the system fonts 
        /// initialized at the given size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public IList<FontInfo> GetFonts(int size)
            => new List<FontInfo>();

        /// <summary>
        /// Add fonts to the demo
        /// Can be a mix of font and system fonts
        /// </summary>
        public void AddFonts(IEnumerable<FontInfo> fontInfos)
        {
            foreach (var fontInfo in fontInfos)
            {
                AddFont(fontInfo);
            }
        }

        /// <summary>
        /// Add fonts to the demo
        /// Can be a mix of font and system fonts
        /// </summary>
        public void AddFont(FontInfo fontInfo)
        {
            if (fontInfo.FontFamilyName != null)
            {
                Font font = new Font(fontInfo.FontFamilyName, fontInfo.Height);
                _fonts[fontInfo.FontFamilyName] = (default, font, fontInfo);
                return;
            }
            else if (fontInfo.FontFileName != null)
            {
                var bdf = IoTBdfFont.Load(fontInfo.FontFileName);

                _fonts[fontInfo.FontFileName] = (bdf, default, fontInfo);
                return;
            }

            throw new Exception($"Invalid font");
        }

        /// <summary>
        /// Draw to the canvas and save the result to a png file
        /// </summary>
        public void DrawTo(DrawStrategy drawStrategy, string extraText, string targetFilename)
        {
            if (_bitmap == null) return;

            using Graphics g = Graphics.FromImage(_bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.White);

            int x = 10;
            int y = 10;
            foreach (var f in _fonts.Values)
            {
                var height = f.fontInfo.Height;// + f.fontInfo.ExtraHeight;
                y = y + (height + 10);
                var text = $"{f.fontInfo.Name} - {extraText}";

                if(f.bdffont != null)
                    DrawText(x, y, text, f.bdffont, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF);
                if (f.nativefont != null)
                    g.DrawString(text, f.nativefont, Brushes.Black, x, y);
            }

            g.Flush();
            _bitmap.Save(targetFilename, ImageFormat.Png);
        }


        public void DrawText(int x, int y, ReadOnlySpan<char> text, IoTBdfFont font, byte textR, byte textG, byte textB,
            byte bkR, byte bkG, byte bkB, bool backBuffer = false)
        {
            int charWidth = font.Width;
            int totalTextWith = charWidth * text.Length;

            if (y <= -font.Height || y >= Height || x >= Width || x + totalTextWith <= 0)
            {
                return;
            }

            //byte[] buffer = backBuffer ? _colorsBackBuffer : _colorsBuffer;

            int index = 0;
            while (index < text.Length)
            {
                if (x + charWidth < 0)
                {
                    x += charWidth;
                    index++;
                    continue;
                }

                DrawChar(x, y, text[index], font, textR, textG, textB, bkR, bkG, bkB);

                x += charWidth;
                index++;
            }
        }

        private void DrawChar(int x, int y, char c, IoTBdfFont font, byte textR, byte textG, byte textB, byte bkR,
            byte bkG, byte bkB)
        {
            if (_bitmap == null) return;

            int hightToDraw = Math.Min(Height - y, font.Height);
            int firstColumnToDraw = x < 0 ? Math.Abs(x) : 0;
            int lastColumnToDraw = x + font.Width > Width ? Width - x : font.Width;

            font.GetCharData(c, out ReadOnlySpan<ushort> charData);

            int b = 8 * (sizeof(ushort) - (int)Math.Ceiling(((double)font.Width) / 8)) + firstColumnToDraw;

            for (int j = firstColumnToDraw; j < lastColumnToDraw; j++)
            {
                for (int i = 0; i < hightToDraw; i++)
                {
                    int value = charData[i] << (b + j - firstColumnToDraw);

                    if ((value & 0x8000) != 0)
                    {
                        var color = Color.FromArgb(255, textR, textG, textB);
                        _bitmap?.SetPixel(x + j, y + i, color);
                        //SetPixel(x + j, y + i, textR, textG, textB, buffer);
                    }
                    else
                    {
                        var color = Color.FromArgb(255, bkR, bkG, bkB);
                        _bitmap?.SetPixel(x + j, y + i, color);
                        //SetPixel(x + j, y + i, bkR, bkG, bkB, buffer);
                    }
                }
            }
        }

    }

}