using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;
using WaffleGenerator;

namespace PickAll.Tests
{
    static class ResultInfoExtensions
    {
        public static ResultInfo UsingIndex(this ResultInfo result, ushort index)
        {
            return new ResultInfo(
                result.Originator,
                index, result.Url,
                result.Description,
                result.Data);
        }
    }

    static class ResultInfoHelper
    {
        public static ResultInfo OnlyDescription(string text)
        {
            return new ResultInfo(
                "helper",
                0,
                "http://fake-url.com/",
                text,
                null);
        }
    }

    static class WaffleHelper
    {
        public static IEnumerable<string> Titles(int times, Func<string, string> modifier = null)
        {
            Func<string, string> _nullModifier = @string => @string;
            var _modifier = modifier ?? _nullModifier;

            for (var i = 0; i < times; i++) {
                var title = WaffleEngine.Title();
                yield return _modifier(title);
            }
        }
    }
}