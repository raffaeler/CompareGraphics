using System.Diagnostics;

namespace BdfFontReader;

public class BdfReader
{
    public BdfReader(string bdfFontFilename)
    {
        if (!File.Exists(bdfFontFilename)) throw new FileNotFoundException($"The file {bdfFontFilename} does not exists");

        var content = File.ReadAllText(bdfFontFilename);
        BdfFont = Read(content);
    }

    public BdfFont? BdfFont { get; }

    private BdfFont? Read(string content)
    {
        using StringReader sr = new StringReader(content);

        IBdfSectionReader? current = null;
        Stack<IBdfSectionReader> sections = new();
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            string key = line;
            string value = string.Empty;
            var index = line.IndexOf(' ');
            if(index != -1)
            {
                key = line.Substring(0, index);
                value = line.Substring(index + 1);
            }

            if (line.StartsWith("START", StringComparison.InvariantCultureIgnoreCase))
            {
                //Debug.WriteLine($"Start Detected: {line}");
                var startCommand = line.Split(' ').First();
                current = IBdfSectionReader.Create(key);
                current.Read(key, value);
                sections.Push(current);
                continue;
            }

            if (line.StartsWith("END", StringComparison.InvariantCultureIgnoreCase))
            {
                //Debug.WriteLine($"END Detected: {line}");
                current = sections.Pop();
                if (sections.Count > 0)
                {
                    var parent = sections.Peek();
                    parent.Add(current);
                }
                continue;
            }

            current = sections.Peek();
            current.Read(key, value);
        }

        return current as BdfFont;
    }


}
