namespace BdfFontReader;

public interface IBdfSectionReader
{
    void Read(string key, string value);
    void Add(IBdfSectionReader child);

    public static IBdfSectionReader Create(string sectionName) => sectionName switch
    {
        BdfFont.Start => new BdfFont(),
        BdfPropertySection.Start => new BdfPropertySection(),
        BdfCharacter.Start => new BdfCharacter(),
        _ => new BdfNotImplemented(),
    };
}
