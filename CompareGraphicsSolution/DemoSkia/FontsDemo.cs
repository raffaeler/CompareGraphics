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
        private Dictionary<string, (SKFont font, FontInfo fontInfo)> _fonts = new();
        private SKPaint? _paint;

        public void Dispose()
        {
            _canvas?.Dispose();
            _bitmap?.Dispose();
        }

        public void Initialize(int width, int height)
        {
            if (SKFontManager.Default.FontFamilies.Count() == 0)
                throw new Exception($"Invalid native assets, possibly missing the SkiaSharp.NativeAssets.Linux assembly");

            _paint = new SKPaint()
            {
                Color = SKColors.Black
            };

            _bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            _canvas = new SKCanvas(_bitmap);
            _canvas.Clear(SKColors.White);
        }

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
                _fonts[fontInfo.FontFamilyName] = (new SKFont(typeface, fontInfo.Height), fontInfo);
                return;
            }
            else if(fontInfo.FontFileName != null)
            {
                var typeface = SKTypeface.FromFile(fontInfo.FontFileName);
                var font = new SKFont(typeface)
                {
                    Edging = SKFontEdging.Alias,
                    //Hinting = SKFontHinting.Full,
                    //Subpixel = true,
                    //BaselineSnap = true,
                    Size = fontInfo.Height + fontInfo.ExtraHeight,
                };

                _fonts[fontInfo.FontFileName] = (font, fontInfo);
                return;
            }

            throw new Exception($"Invalid font");
        }

        /// <summary>
        /// Draw to the canvas and save the result to a png file
        /// </summary>
        public void DrawTo(string extraText, string targetFilename)
        {
            if (_canvas == null) return;

            int x = 10;
            int top = 10;
            int count = 0;
            foreach(var f in _fonts.Values)
            {
                var height = f.fontInfo.Height + f.fontInfo.ExtraHeight;
                var y = top + count * (height + 10);
                var text = $"{f.fontInfo.Name} - {extraText}";
                _canvas.DrawText(text, x, y, f.font, _paint);
                count++;
            }

            using (var sKFileWStream = new SKFileWStream(targetFilename))
            {
                _bitmap?.Encode(sKFileWStream, SKEncodedImageFormat.Png, 50);
            }
        }


    }
}
