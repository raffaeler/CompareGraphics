namespace ConsoleTest;

internal class FontsHelper
{
    public static FileInfo[] GetBdfFontFiles()
    {
        DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
        var files = di.GetFiles("*.bdf", SearchOption.AllDirectories);
        return files;
    }



}
