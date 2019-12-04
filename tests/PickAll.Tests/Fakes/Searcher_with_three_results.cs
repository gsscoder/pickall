using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;

namespace PickAll.Tests.Fakes
{
    public class Searcher_with_three_results : Searcher
    {
        public Searcher_with_three_results() : base()
        {
        }

        public override async Task<IEnumerable<ResultInfo>> Search(string query)
        {
            var results = new ResultInfo[] {
                CreateResult(0, "http://nonsense.org/", "Nothing useful"),
                CreateResult(1, "http://someurl.de/something", "Etwas im Web"),
                CreateResult(2, "http://someurl.com/", "Something on the web")
            };
            return await Task.Run(() => results);
        }

        public override string Name
        {
            get { return "fake_one"; }
        }
    }
}