namespace BdfFontReader;

public class BdfCharacter : BdfSectionReaderBase, IBdfSectionReader
{
    public const string Start = "STARTCHAR";
    public const string End = "ENDCHAR";

    public void Add(IBdfSectionReader child)
    {
    }

    public override void Read(string key, string value)
    {
        base.Read(key, value);
    }
}
