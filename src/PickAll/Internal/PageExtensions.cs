using PuppeteerSharp;
using SharpX.Extensions;

static class PageExtensions
{
    static string[] _desktopUserAgents = new[]
    {
        // Chrome User Agents
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.5615.138 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.5615.138 Safari/537.36",
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.5563.146 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.5414.120 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 11_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.133 Safari/537.36",
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.133 Safari/537.36",

        // Firefox User Agents
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:114.0) Gecko/20100101 Firefox/114.0",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 11.0; rv:110.0) Gecko/20100101 Firefox/110.0",
        "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:116.0) Gecko/20100101 Firefox/116.0",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:116.0) Gecko/20100101 Firefox/116.0",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:109.0) Gecko/20100101 Firefox/109.0",
        "Mozilla/5.0 (X11; Fedora; Linux x86_64; rv:113.0) Gecko/20100101 Firefox/113.0",

        // Edge User Agents
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.5615.138 Safari/537.36 Edg/112.0.1722.48",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.5481.77 Safari/537.36 Edg/110.0.1587.69",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.133 Safari/537.36 Edg/114.0.1823.79",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.133 Safari/537.36 Edg/114.0.1823.79",

        // Safari User Agents
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0.3 Safari/605.1.15",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 11_1_0) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0.2 Safari/605.1.15",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_2) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.2 Safari/605.1.15",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1.2 Safari/605.1.15",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_3) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.3 Safari/605.1.15",

        // Additional Desktop User Agents
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.81 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.5672.126 Safari/537.36"
    };

    public static async Task UseStealthMode(this IPage page)
    {
        await page.SetRequestInterceptionAsync(true);
        page.Request += async (sender, e) =>
        {
            if (e.Request.IsNavigationRequest) {
                var randomUserAgent = _desktopUserAgents.Shuffle().Choice();

                var headers = new Dictionary<string, string>(e.Request.Headers)
                {
                    ["User-Agent"] = randomUserAgent
                };

                await e.Request.ContinueAsync(new Payload {
                    Headers = headers
                });
            }
            else {
                await e.Request.ContinueAsync();
            }
        };

        await page.EvaluateExpressionOnNewDocumentAsync(@"
            // Remove navigator.webdriver
            Object.defineProperty(navigator, 'webdriver', { get: () => false });

            // Mock navigator.languages
            Object.defineProperty(navigator, 'languages', { get: () => ['en-US', 'en'] });

            // Mock navigator.plugins
            Object.defineProperty(navigator, 'plugins', { get: () => [1, 2, 3] });

            // Mock window.chrome
            Object.defineProperty(window, 'chrome', { get: () => undefined });

            // Mock permissions query
            const originalQuery = window.navigator.permissions.query;
            window.navigator.permissions.query = (parameters) => 
                parameters.name === 'notifications' 
                ? Promise.resolve({ state: 'denied' }) 
                : originalQuery(parameters);

            // Mock screen dimensions
            Object.defineProperty(window.screen, 'availWidth', { get: () => 1920 });
            Object.defineProperty(window.screen, 'availHeight', { get: () => 1080 });

            // Add random human-like mouse movements
            const simulateMouseMovement = () => {
                const event = new MouseEvent('mousemove', {
                    clientX: Math.random() * window.innerWidth,
                    clientY: Math.random() * window.innerHeight
                });
                document.dispatchEvent(event);
            };
            setInterval(simulateMouseMovement, 1000);
        ");

    }
}

