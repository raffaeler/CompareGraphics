namespace DemoSkia;
/*
 * This code was originally written by @A-J-Bauer Github user and given directly
 * to myself (@raffaeler) in order to create this demo project
 */

/*
Create a console app that targets .NET 6.0

Add packages with NuGet:
    SkiaSharp
    SkiaSharp.NativeAssets.Linux

Add font directory and files (copy if newer)

Depending on the target platform, build a self contained app with either:

    dotnet publish -r linux-arm --self-contained

    dotnet publish -r linux-arm64 --self-contained
 
Run the app on the target system
    
Check if "fonts.png" was created
    ls -l | grep png
 */

using SkiaSharp;

public class OldDemo
{
    public void Run(string filename)
    {

        // output the font names returned by the font manager to console
        foreach (string fontFamilyName in SKFontManager.Default.FontFamilies)
        {
            Console.WriteLine(fontFamilyName);
        }

        if (SKFontManager.Default.FontFamilies.Count() == 0)
        {
            Console.WriteLine("No fonts available, did you accidently add SkiaSharp.NativeAssets.Linux.NoDependencies instead of SkiaSharp.NativeAssets.Linux ?");
            return;
        }

        // simply use the first font available on the system
        string fontFamily = SKFontManager.Default.FontFamilies.First();
        SKFont font = new SKFont(SKTypeface.FromFamilyName(fontFamily), 20);

        // create paint objects
        SKPaint paint = new SKPaint()
        {
            Color = SKColors.Black
        };

        // create a bitmap and a canvas to draw on
        SKBitmap bitmap = new SKBitmap(800, 200, SKColorType.Rgba8888, SKAlphaType.Premul);
        SKCanvas canvas = new SKCanvas(bitmap);

        // clear the canvas
        canvas.Clear(SKColors.White);

        canvas.DrawText(fontFamily, 40, 20, font, paint);

        // SKTypeface.FromData
        // SKTypeface.FromStream
        // SKTypeface.FromFamilyName


        // Glyph Bitmap Distribution Format BDF https://en.wikipedia.org/wiki/Glyph_Bitmap_Distribution_Format

        // font from the RGBLedMatrix samples directory added to the project
        SKTypeface sKTypefaceBdf4x6 = SKTypeface.FromFile("fonts/4x6.bdf");
        SKFont fontBdf4x6 = new SKFont(sKTypefaceBdf4x6)
        {
            Edging = SKFontEdging.Alias,
            //Hinting = SKFontHinting.Full,
            //Subpixel = true,
            //BaselineSnap = true,
            Size = 6 + 2,
        };
        canvas.DrawText("BDF font 4x6 text abcdefgABCDEFG 0123456789 - . , :", 40, 40, fontBdf4x6, paint);

        // font from the RGBLedMatrix samples directory added to the project
        SKTypeface sKTypefaceBdf5x7 = SKTypeface.FromFile("fonts/5x7.bdf");
        SKFont fontBdf5x7 = new SKFont(sKTypefaceBdf5x7)
        {
            Edging = SKFontEdging.Alias,
            //Hinting = SKFontHinting.Full,
            //Subpixel = true,
            //BaselineSnap = true,
            Size = 7 + 2
        };
        canvas.DrawText("BDF font 5x7 text abcdefgABCDEFG 0123456789 - . , :", 40, 50, fontBdf5x7, paint);


        // fron from https://github.com/fcambus/spleen added to the project (appears to be almost the same as the 5x7
        SKTypeface sKTypefaceSpleen5x8 = SKTypeface.FromFile("spleen-5x8.bdf");
        SKFont fontSpleen5x8 = new SKFont(sKTypefaceSpleen5x8)
        {
            Edging = SKFontEdging.Alias,
            //Hinting = SKFontHinting.Full,
            //Subpixel = true,
            //BaselineSnap = true,
            Size = 8 + 3
        };
        canvas.DrawText("BDF font 5x8 text abcdefgABCDEFG 0123456789 - . , :", 40, 64, fontSpleen5x8, paint);




        // save the underlying bitmap as a png file
        using (var sKFileWStream = new SKFileWStream(filename))
        {
            if (sKFileWStream != null)
            {
                bitmap.Encode(sKFileWStream, SKEncodedImageFormat.Png, 50);
            }
        }


        canvas.Dispose();
        bitmap.Dispose();
    }
}