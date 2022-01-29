namespace BdfFontReader;

public class BdfNotImplemented : BdfSectionReaderBase, IBdfSectionReader
{
    public void Add(IBdfSectionReader child)
    {
    }

    public override void Read(string key, string value)
    {
        base.Read(key, value);
    }
}
