using Common;

using ConsoleTest;

var resultsDir = new DirectoryInfo("results");
if (!resultsDir.Exists) resultsDir.Create();

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
    .OrderBy(f => f.Height)
    .ToList();

var demoText = "abcdefgABCDEFG 0123456789 - . , :";
var sizeX = 320;
var sizeY = 200;
string fontName;
if (OperatingSystem.IsWindows())
{
    fontName = "Cascadia Code";
}
else if (OperatingSystem.IsLinux())
{
    fontName = "Liberation Mono";
}
else
{
    throw new Exception("This test runs only on Windows or Linux");
}

Func<FontInfo, bool> fontFilter = f => f.Name.StartsWith(fontName);

using var skia = new DemoSkia.FontsDemo();
skia.Initialize(sizeX, sizeY);
skia.AddFont(skia.GetFonts(8).First(fontFilter));
skia.AddFonts(fonts);
skia.DrawTo(DrawStrategy.IoTBdfFont, demoText, "results/skia-IoTBdf.png");
skia.Clear();
skia.DrawTo(DrawStrategy.Native, demoText, "results/skia-Native.png");

using var imageSharp = new DemoImageSharp.FontsDemo();
imageSharp.Initialize(sizeX, sizeY);
imageSharp.AddFont(imageSharp.GetFonts(8).First(fontFilter));
imageSharp.AddFonts(fonts);
imageSharp.DrawTo(DrawStrategy.IoTBdfFont, demoText, "results/imageSharp.png");

// This will only work on Windows
using var sysDrawing = new DemoSystemDrawingCommon.FontsDemo();
sysDrawing.Initialize(sizeX, sizeY);
sysDrawing.AddFont(new FontInfo()
{
    Name = fontName,
    Height= 8,
    ExtraHeight = 0,
    FontFamilyName = fontName,
});
sysDrawing.AddFonts(fonts);
sysDrawing.DrawTo(DrawStrategy.IoTBdfFont, demoText, "results/sysDrawing.png");

DemoSkia.OldDemo d = new();
d.Run("results/skia-original.png");


// Compare the results with this tool:
// https://www.guiffy.com/Image-Diff-Tool.html