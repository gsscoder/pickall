using System.Collections.Generic;
using System.Linq;
using WaffleGenerator;
using CSharpx;

namespace PickAll.Tests.Fakes
{
    static class ResultInfoGenerator
    {
        public static IEnumerable<ResultInfo> Generate(string originator, ushort samples)
        {
            for (ushort index = 0; index <= samples - 1; index++) {
                yield return new ResultInfo(
                    originator, index, WaffleHelper.Link(), WaffleEngine.Title(), null);
            } 
        }

        public static IEnumerable<ResultInfo> GenerateUnique(string originator, ushort samples)
        {
            var generated = new List<ResultInfo>();
            for (ushort index = 0; index <= samples - 1; index++) {
                var url = WaffleHelper.Link();
                var description = WaffleEngine.Title();
                var searched = from @this in generated
                               where @this.Url == url || @this.Description == description
                               select @this;
                if (searched.Count() == 0) {
                    var result = new ResultInfo(
                        originator, index, url, description, null);
                    generated.Add(result);
                }
                else {
                    index--;
                }
            }
            return generated;
        }
    }
}