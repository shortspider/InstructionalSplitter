using System.Globalization;
using FFMpegCore;

var partsPath = Path.Combine(Directory.GetCurrentDirectory(), "parts.txt");

// Too lazy to think of common interface/class so go with obj.
var items = File
                .ReadAllLines(partsPath)
                .Select(l =>
                {
                    if (Header.TryParse(l, out var header))
                    {
                        return (object)header!;
                    }
                    return Section.Parse(l);
                })
                .ToArray();

InitialValidation(items);

var episodeNumber = 1;
Header? header = null;
foreach (var item in items)
{
    if (item is Header)
    {
        header = (Header?)item;
    }
    else
    {
        Section section = (Section)item;
        var outFileName = $"{header!.OutFileNamePrefix}E{episodeNumber++}-{section.Name}{header!.Extension}";
        Console.WriteLine($"Writing file {outFileName} from {section.StartTime} to {section.EndTime}");

        var success = FFMpegArguments
                        .FromFileInput(header.FilePath, true, options => options.Seek(section.StartTime).EndSeek(section.EndTime))
                        .OutputToFile(outFileName)
                        .ProcessSynchronously();
        if (!success)
        {
            throw new Exception($"Failed to create file {outFileName}");
        }
    }
}

static void InitialValidation(object[] items)
{
    for (int i = 0; i < items.Length - 1; i++)
    {
        var item = items[i];
        if (item is not Header)
        {
            var currentSection = (Section)items[i];
            var nextPart = items[i + 1];
            if (nextPart is Header)
            {
                continue;
            }
            var nextSection = (Section)nextPart;
            if (currentSection.EndTime != nextSection.StartTime)
            {
                throw new Exception($"End time of {currentSection.Name} not equal start time of {nextSection.Name}");
            }
            currentSection = nextSection;
        }
    }
}

class Header
{
    public string FilePath { get; init; } = "";

    public string OutFileNamePrefix { get; init; } = "";

    public string Extension { get; init; } = "";

    public static bool TryParse(string line, out Header? header)
    {
        if (!line.StartsWith("FP:"))
        {
            header = null;
            return false;
        }
        var filePath = line.Split('\t')[1];
        header = new Header
        {
            FilePath = filePath,
            OutFileNamePrefix = filePath[..filePath.IndexOf("E0")],
            Extension = Path.GetExtension(filePath)
        };
        return true;
    }
}

class Section
{
    public TimeSpan StartTime { get; init; }

    public TimeSpan EndTime { get; init; }

    public string Name { get; init; } = "";

    public static Section Parse(string line)
    {
        var currentPart = line.Split('\t');
        var timeParts = currentPart[1].Split('-');
        return new Section
        {
            Name = SanitizeName(currentPart[0].Trim()),
            StartTime = ParseTimeString(timeParts[0]),
            EndTime = ParseTimeString(timeParts[1])
        };
    }

    static TimeSpan ParseTimeString(string timeString)
    {
        timeString = timeString.Trim();
        if (timeString.Count(c => c == ':') < 2)
        {
            timeString = $"00:{timeString}";
        }
        return TimeSpan.Parse(timeString);
    }

    static string SanitizeName(string name)
    {
        var validChars = name
                            .Replace("&", "and")
                            .Replace("/", " or ")
                            .Replace("\\", "")
                            .Replace("  ", " ")
                            .Replace(":", "")
                            .Replace(", and", " and", StringComparison.OrdinalIgnoreCase)
                            .Replace(",", " and")
                            .ToLower();
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(validChars).Replace(' ', '.');
    }
}