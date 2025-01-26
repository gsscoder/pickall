using System.Net;
using PuppeteerSharp;

static class ResponseExtensions
{
    public static void EnsureSuccessOrThrow<T>(this IResponse response, T exception) where T : Exception
    {
        var isSuccessStatus = response.Status >= HttpStatusCode.OK &&
                              response.Status <= (HttpStatusCode)299;

        if (!isSuccessStatus) throw exception;
    }
}
