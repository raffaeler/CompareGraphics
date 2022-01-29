namespace BdfFontReader;

public class BdfSectionReaderBase
{
    public Dictionary<string, string> Properties { get; } = new();

    public virtual void Read(string key, string value)
    {
        if (string.Compare(key, "COMMENT", StringComparison.InvariantCultureIgnoreCase) == 0)
            return;

        Properties[key] = value;
    }
}
