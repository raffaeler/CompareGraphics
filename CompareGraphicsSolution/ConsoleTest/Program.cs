using Common;

using ConsoleTest;

var fontFiles = FontsHelper.GetBdfFontFiles();
//foreach (var fontFile in fontFiles)
//{
//    BdfFontReader.BdfReader reader = new(fontFile.FullName);
//    var font = reader.BdfFont;
//}

var extraHeights = new Dictionary<string, int>()
{
    { "4x6", 2 },
    { "5x7", 2 },
    { "spleen-5x8", 3 },
};

IReadOnlyCollection<FontInfo> fonts = fontFiles
    .Select(f => FontInfo.CreateByFileName(f.FullName, extraHeights))
    .ToList();

var demoText = "abcdefgABCDEFG 0123456789 - . , :";
var sizeX = 320;
var sizeY = 200;
Func<FontInfo, bool> fontFilter = f => f.Name.StartsWith("Cascadia");

using var skia = new DemoSkia.FontsDemo();
skia.Initialize(sizeX, sizeY);
skia.AddFont(skia.GetFonts(8).First(fontFilter));
skia.AddFonts(fonts);
skia.DrawTo(demoText, "results/skia.png");

using var imageSharp = new DemoImageSharp.FontsDemo();
imageSharp.Initialize(sizeX, sizeY);
imageSharp.AddFont(imageSharp.GetFonts(8).First(fontFilter));
imageSharp.AddFonts(fonts);
imageSharp.DrawTo(demoText, "results/imageSharp.png");

// This will only work on Windows
using var sysDrawing = new DemoImageSharp.FontsDemo();
sysDrawing.Initialize(sizeX, sizeY);
sysDrawing.AddFont(sysDrawing.GetFonts(8).First(fontFilter));
sysDrawing.AddFonts(fonts);
sysDrawing.DrawTo(demoText, "results/sysDrawing.png");

DemoSkia.OldDemo d = new();
d.Run("results/skia-original.png");

