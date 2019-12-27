using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace PickAll.Tests.Fakes
{
    static class UrlParts
    {
        public static string[] schemes =
        {
            "http", "https", "ftp", "ssh", "git"
        };

        public static string[] domainsSub =
        {
            "www", "mail", "remote", "blog", "webmail", "server", "ns1", "ns2", "smtp", "secure", "vpn",
            "m", "shop", "ftp", "mail2", "test", "portal", "ns", "ww1", "host", "support", "dev", "web",
            "bbs"
        };

        public static string[] domains =
        {
            "2Dimensions", "500px", "7cups", "9gag", "about", "academia", "alik", "anobii", "archive",
            "audiojungle", "avizo", "blip", "badoo", "bandcamp", "basecamp", "bazar", "behance", "bitbucket",
            "bitcoinforum", "blogger", "bodybuilding", "bookcrossing", "brew", "buymeacoffee", "buzzfeed",
            "cnet", "canva", "capfriendly", "carbonmade", "cash", "cent", "championat", "chatujme", "chessru",
            "cloob", "codecademy", "codechef", "codementor", "coderwall", "codewars", "colourlovers", "contently",
            "coroflot", "cracked", "creativemarket", "crevado", "crunchyroll", "dev", "dailymotion",
            "designspiration", "deviantart", "discogs", "disqus", "dribbble", "ebay", "ello", "etsy", "eyeem",
            "facebook", "facenama", "fandom", "filmo", "fiverr", "flickr", "flightradar24", "flipboard",
            "fortnitetrackerchallenges", "gdprofiles", "gpsies", "gamespot", "giphy", "github", "gitlab",
            "gitee", "goodreads", "gumroad", "gunsandammo", "gurushots", "hackerone", "hackerrank", "house-mixes",
            "houzz", "hubpages", "hubski", "ifttt", "imageshack", "imgup", "insanejournal", "instagram",
            "instructables", "investing", "issuu", "itch", "jimdosite", "kaggle", "keybase", "kik", "kiwifarms",
            "kongregate", "linux", "launchpad", "leetcode", "letterboxd", "livejournal", "liveleak", "lobste",
            "mstdn", "medium", "meetme", "mixcloud", "myanimelist", "myspace", "native-instrumentsforum",
            "npmjs", "npmjs", "namemc", "nationstates", "nationstates", "newgrounds", "ok", "opencollective",
            "openstreetmap", "otzovik", "ourdjtalk", "pcpartpicker", "psnprofiles", "packagist", "pastebin",
            "patreon", "periscope", "pexels", "photobucket", "pinkbike", "pinterest", "pixabay", "googlestore",
            "pling", "plug", "pokemonshowdown", "polygon", "producthunt", "promodj", "quora", "rateyourmusic",
            "redbubble", "reddit", "redsun", "repl", "researchgate", "reverbnation", "roblox", "sbazar", "mit",
            "scribd", "shitpostbot", "slack", "slideshare", "smashcast", "smule", "soundcloud", "sourceforge",
            "speedrun", "splits", "sporcle", "sports", "sports-tracker", "spotify", "robertsspaceindustries",
            "steamcommunity", "steamcommunity", "sublimetext", "t-mobile", "tamtamt", "taringa", "tellonym",
            "tiktok", "tinder", "tm-ladderindex", "tradingview", "trakt", "trashbox", "trello", "skyscanner",
            "tripadvisor", "twitch", "twitter", "ultimate-guitar", "unsplash", "vk", "vsco", "venmo", "viadeoen",
            "vimeo", "virgool", "virustotal", "wattpad", "weheartit", "webnode", "wikidot", "wikipedia", "wix",
            "wordpress", "wordpress", "yandex", "younow", "youpic", "youtube", "zhihu", "zomato", "akniga",
            "authorstream", "baby", "babyblog", "boingboing", "couchsurfing", "d3", "dailykos", "dating",
            "devrant", "drive2", "egpu", "easyen", "fanpop", "fixya", "fl", "geocaching", "gfycat", "habr",
            "hackster", "imgsrc", "interpals", "irecommend", "kwork", "pentestit", "last", "leasehackr",
            "livelib", "metacritic", "mixer", "moikrug", "opennet", "pedsovet", "pikabu", "echo", "segmentfault",
            "sparkpeople", "spletnik", "toster", "travellerspoint", "warriorforum"
        };

        public static string[] domainsTop =
        {
            "com", "org", "net", "int", "edu", "gov", "mil", "arpa"
        };

        public static string[] domainsCountry =
        {
            "ac", "ad", "ae", "af", "ag", "ai", "al", "am", "ao", "aq", "ar", "as", "at", "au", "aw", "ax",
            "az", "ba", "bb", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bm", "bn", "bo", "br", "bt", "bw",
            "by", "bz", "ca", "cc", "cd", "cf", "cg", "ch", "ci", "ck", "cl", "cm", "cn", "co", "cr", "cu",
            "cv", "cw", "cx", "cy", "cz", "de", "dj", "dk", "dm", "do", "dz", "ec", "ee", "eg", "er", "es",
            "et", "eu", "fi", "fj", "fk", "fm", "fo", "fr", "ga", "gd", "ge", "gf", "gg", "gh", "gi", "gl",
            "gm", "gn", "gp", "gq", "gs", "gt", "gu", "gw", "gy", "hk", "hm", "hn", "hr", "ht", "hu", "id",
            "ie", "il", "im", "in", "io", "iq", "ir", "it", "je", "jm", "jo", "jp", "ke", "kg", "kh", "ki",
            "km", "kn", "kp", "kr", "kw", "ky", "kz", "la", "lb", "lc", "li", "lk", "lk", "lr", "ls", "lt",
            "lu", "lv", "ly", "ma", "mc", "md", "me", "mg", "mk", "ml", "mm", "mn", "mo", "mp", "mq", "mr",
            "ms", "mt", "mu", "mv", "mx", "my", "mz", "na", "nc", "ne", "nf", "ng", "ni", "nl", "no", "np",
            "nr", "nu", "nz", "om", "pa", "pe", "pf", "pg", "ph", "pk", "pl", "pm", "pn", "pr", "ps", "pt",
            "pw", "py", "qa", "re", "ro", "rs", "ru", "rw", "sa", "sb", "sc", "sd", "se", "sg", "sh", "si",
            "sk", "sl", "sm", "so", "sr", "ss", "st", "su", "sv", "sx", "sy", "sz", "tc", "td", "tf", "tg",
            "th", "tj", "tk", "tl", "tm", "tn", "to", "tr", "tt", "tv", "tw", "tz", "ua", "ug", "uk", "us",
            "uy", "uz", "va", "vc", "ve", "vg", "vi", "vn", "vu", "wf", "ws", "ye", "yt", "za", "zm", "zw"
        };

        public static string[] paths = {
            "index", "home", "search", "contacts", "about", "info", "credits"
        };

        public static string[] extensions = {
            "htm", "html", "php", "cgi", "asp", "aspx"
        };
    }

    class UrlEngine
    {
        private Random _random = new Random();
        private List<string> _chosen = new List<string>();

        public string Build(bool? countryTopLevel, int pathLength)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("{0}://", Choice(UrlParts.schemes));
            if (countryTopLevel != null && !countryTopLevel.Value) {
                builder.AppendFormat("{0}.", Choice(UrlParts.domainsCountry));
            }
            else {
                builder.AppendFormat("{0}.", Choice(UrlParts.domainsSub));
            }
            builder.AppendFormat("{0}.", Choice(UrlParts.domains));
            builder.AppendFormat("{0}", Choice(UrlParts.domainsTop));
            if (countryTopLevel != null && countryTopLevel.Value) {
                builder.AppendFormat(".{0}", Choice(UrlParts.domainsCountry));
            }
            if (pathLength == 0) {
                builder.Append("/");
            }
            else {
                for (var i = 0; i < pathLength; i++) {
                    builder.AppendFormat("/{0}", Choice(UrlParts.paths, false));
                }
            }
            if (pathLength > 0) {
                builder.AppendFormat(".{0}", Choice(UrlParts.extensions));
            }
            return builder.ToString();
        }

        string Choice(IEnumerable<string> source, bool unique = true)
        {
            var item = source.ElementAt(_random.Next(0, source.Count() - 1));
            var exists = _chosen.Contains(item);
            if (exists) {
                if (unique) {
                    Choice(source);
                }
                return item;
            }
            _chosen.Add(item);
            return item;
        }
    }
}