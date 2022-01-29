using BdfFontReader;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FontInfo
    {
        public static FontInfo CreateByFamilyName(string fontFamilyName, int size)
        {
            var instance = new FontInfo()
            {
                Name = fontFamilyName,
                FontFamilyName = fontFamilyName,
                ExtraHeight = 0,
                Height = size,
                Width = 0,  // does not matter at this time
            };

            return instance;
        }

        public static FontInfo CreateByFileName(string filename, IReadOnlyDictionary<string, int> extraHeights)
        {
            var shortName = Path.GetFileNameWithoutExtension(filename);
            var (width, height) = GetSize(filename);
            extraHeights.TryGetValue(shortName, out var extraHeight);

            var instance = new FontInfo()
            {
                Name = shortName,
                FontFileName = filename,
                ExtraHeight = extraHeight,
                Width = width,
                Height = height,
            };

            return instance;
        }

        private static (int width, int height) GetSize(string filename)
        {
            BdfReader reader = new(filename);
            if (reader.BdfFont == null || !reader.BdfFont.Properties.TryGetValue("FONTBOUNDINGBOX", out string? box) || box == null)
                return default;

            var parts = box.Split(' ');
            if (parts.Length != 4) return default;
            int.TryParse(parts[0], out int width);
            int.TryParse(parts[1], out int height);
            return(width, height);
        }

        public string Name { get; set; }
        public string? FontFamilyName { get; set; }
        public string? FontFileName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ExtraHeight { get; set; }



    }

}
