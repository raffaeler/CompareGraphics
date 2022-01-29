namespace BdfFontReader;

public class BdfPropertySection : BdfSectionReaderBase, IBdfSectionReader
{
    public const string Start = "STARTPROPERTIES";
    public const string End = "ENDPROPERTIES";

    public void Add(IBdfSectionReader child)
    {
    }

    public override void Read(string key, string value)
    {
        base.Read(key, value);
    }

}
