using SharpX.Extensions;

static class TextPicker
{
    static readonly string[] _sentences =
    [
        "info about",
        "facts about",
        "insides about",
        "life of",
        "career of",
        "private life of",
        "public life of",
        "curiosities about",
        "fake news regarding",
    ];

    static readonly string[] _famousTechNames =
    [
        "Elon Musk",
        "Larry Ellison",
        "Bill Gates",
        "Jeff Bezos",
        "Mark Zuckerberg",
        "Tim Cook",
        "Sundar Pichai",
        "Satya Nadella",
        "Sheryl Sandberg",
        "Susan Wojcicki",
        "Jack Dorsey",
        "Reed Hastings",
        "Steve Wozniak",
        "Linus Torvalds",
        "Marc Andreessen",
        "Peter Thiel",
    ];

    public static string GetName() => _famousTechNames.Shuffle().Choice();

    public static string GetSentence() => _sentences.Shuffle().Choice();
}
