namespace BdfFontReader;

public class BdfFont : BdfSectionReaderBase, IBdfSectionReader
{
    public const string Start = "STARTFONT";
    public const string End = "ENDFONT";

    private List<BdfCharacter> _characters = new();

    public BdfFont()
    {
    }

    public BdfPropertySection? PropertySection { get; private set; }
    public IReadOnlyCollection<BdfCharacter> Characters => _characters;

    public void Add(IBdfSectionReader child)
    {
        if (child is BdfCharacter character) _characters.Add(character);
        if (child is BdfPropertySection propertySection) PropertySection = propertySection;
    }

    public override void Read(string key, string value)
    {
        base.Read(key, value);
    }
}
