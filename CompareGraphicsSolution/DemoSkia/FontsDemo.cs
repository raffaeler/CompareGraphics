using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

using SkiaSharp;

namespace DemoSkia
{
    public class FontsDemo : IFontsDemo, IDisposable
    {
        private SKBitmap? _bitmap;
        private SKCanvas? _canvas;
        private Dictionary<string, (SKFont? font, IoTBdfFont? bdfFont, FontInfo fontInfo)> _fonts = new();
        private SKPaint _paint;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public FontsDemo()
        {
            if (SKFontManager.Default.FontFamilies.Count() == 0)
                throw new Exception($"Invalid native assets, possibly missing the SkiaSharp.NativeAssets.Linux assembly");

            _paint = new SKPaint()
            {
                Color = SKColors.Black
            };
        }

        public void Dispose()
        {
            _canvas?.Dispose();
            _bitmap?.Dispose();
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;

            _bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            _canvas = new SKCanvas(_bitmap);
            _canvas.Clear(SKColors.White);
        }

        public void Clear() => _canvas?.Clear(SKColors.White);

        /// <summary>
        /// Returns the system fonts 
        /// initialized at the given size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public IList<FontInfo> GetFonts(int size)
            => SKFontManager.Default.FontFamilies
                .Select(f => FontInfo.CreateByFamilyName(f, size))
                .ToList();

        /// <summary>
        /// Add fonts to the demo
        /// Can be a mix of font and system fonts
        /// </summary>
        public void AddFonts(IEnumerable<FontInfo> fontInfos)
        {
            foreach(var fontInfo in fontInfos)
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
                var typeface = SKTypeface.FromFamilyName(fontInfo.FontFamilyName);
                _fonts[fontInfo.FontFamilyName] = (new SKFont(typeface, fontInfo.Height), default, fontInfo);
                return;
            }
            else if(fontInfo.FontFileName != null)
            {
                var bdf = IoTBdfFont.Load(fontInfo.FontFileName);
                var typeface = SKTypeface.FromFile(fontInfo.FontFileName);
                var font = new SKFont(typeface)
                {
                    Edging = SKFontEdging.Alias,
                    //Hinting = SKFontHinting.Full,
                    //Subpixel = true,
                    //BaselineSnap = true,
                    
                    Size = fontInfo.Height + fontInfo.ExtraHeight,
                };

                _fonts[fontInfo.FontFileName] = (font, bdf, fontInfo);
                return;
            }

            throw new Exception($"Invalid font");
        }

        /// <summary>
        /// Draw to the canvas and save the result to a png file
        /// </summary>
        public void DrawTo(DrawStrategy drawStrategy, string extraText, string targetFilename)
        {
            if (_canvas == null) return;

            int x = 10;
            int y = 10;
            foreach(var f in _fonts.Values)
            {
                var height = f.fontInfo.Height;
                if (drawStrategy == DrawStrategy.Native) height += f.fontInfo.ExtraHeight;

                y = y + (height + 10);
                var text = $"{f.fontInfo.Name} - {extraText}";

                var yOff = f.font == null ? 0 : ((int)-f.font.Metrics.Ascent + (int)f.font.Metrics.Descent + y);
                if (f.bdfFont == null)
                    _canvas.DrawText(text, x, yOff, f.font, _paint);
                else if (drawStrategy == DrawStrategy.Native)
                    _canvas.DrawText(text, x, y + f.fontInfo.ExtraHeight, f.font, _paint);
                else if (f.bdfFont != null)
                {
                    DrawText(x, y, text, f.bdfFont, 0, 0, 0, 255, 255, 255);
                }
            }

            using (var sKFileWStream = new SKFileWStream(targetFilename))
            {
                _bitmap?.Encode(sKFileWStream, SKEncodedImageFormat.Png, 50);
            }
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
                        var color = new SKColor(textR, textG, textB, 255);
                        _bitmap?.SetPixel(x + j, y + i, color);
                        //SetPixel(x + j, y + i, textR, textG, textB, buffer);
                    }
                    else
                    {
                        var color = new SKColor(bkR, bkG, bkB, 255);
                        _bitmap?.SetPixel(x + j, y + i, color);
                        //SetPixel(x + j, y + i, bkR, bkG, bkB, buffer);
                    }
                }
            }
        }



    }
}
