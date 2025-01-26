using SharpX.Extensions;

static class NamePicker
{
    static readonly List<string> FamousTechNames =
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
        "Peter Thiel"
    ];

    public static string GetRandom() => FamousTechNames.Shuffle().Choice();
}
