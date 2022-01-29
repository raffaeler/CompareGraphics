using Common;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

// SixLabors.Fonts is a pre-release package
// SixLabors.ImageSharp.Drawing is a pre-release package

namespace DemoImageSharp
{
    public class FontsDemo : IFontsDemo, IDisposable
    {
        private Image<Rgba32>? _bitmap;
        private Dictionary<string, (IoTBdfFont? bdffont, Font? systemFont, FontInfo fontInfo)> _fonts = new();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public void Dispose()
        {
            _bitmap?.Dispose();
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;

            if (SystemFonts.Families.Count() == 0)
                throw new Exception($"Invalid native assets, possibly missing the SkiaSharp.NativeAssets.Linux assembly");

            _bitmap = new Image<Rgba32>(width, height, Color.White);
        }

        /// <summary>
        /// Returns the system fonts 
        /// initialized at the given size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public IList<FontInfo> GetFonts(int size)
            => SystemFonts.Families
                .Select(f => FontInfo.CreateByFamilyName(f.Name, size))
                .ToList();

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
                var fontFamily = SystemFonts.Families.First(f => f.Name == fontInfo.FontFamilyName);
                var font = fontFamily.CreateFont(fontInfo.Height, FontStyle.Regular);
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

            int x = 10;
            int top = 10;
            int count = 0;
            foreach (var f in _fonts.Values)
            {
                var height = f.fontInfo.Height;// + f.fontInfo.ExtraHeight;
                var y = top + count * (height + 10);

                var text = $"{f.fontInfo.Name} - {extraText}";
                if (f.bdffont != null)
                    DrawText(x, y, text, f.bdffont, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF);
                else
                {
                    var point = new PointF(x, y);
                    _bitmap.Mutate(x =>
                        x.DrawText(text, f.systemFont, Color.Black, point));
                }
                count++;
            }

            _bitmap.SaveAsPng(targetFilename);
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

            // reversing rows and heights to optimize ImageSharp code
            for (int i = 0; i < hightToDraw; i++)
            {
                var row = _bitmap.GetPixelRowSpan(y + i);
                for (int j = firstColumnToDraw; j < lastColumnToDraw; j++)
                {
                    int value = charData[i] << (b + j - firstColumnToDraw);

                    if ((value & 0x8000) != 0)
                    {
                        var color = Color.FromRgba(textR, textG, textB, 255);
                        row[x + j] = color;
                        //_bitmap?.SetPixel(x + j, y + i, color);
                        //SetPixel(x + j, y + i, textR, textG, textB, buffer);
                    }
                    else
                    {
                        var color = Color.FromRgba(bkR, bkG, bkB, 255);
                        row[x + j] = color;
                        //_bitmap?.SetPixel(x + j, y + i, color);
                        //SetPixel(x + j, y + i, bkR, bkG, bkB, buffer);
                    }
                }
            }
        }

    }

}