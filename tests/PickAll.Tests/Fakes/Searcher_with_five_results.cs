using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;

namespace PickAll.Tests.Fakes
{
    public class Searcher_with_five_results : Searcher
    {
        public Searcher_with_five_results() : base()  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> Search(string query)
        {
            var results = new ResultInfo[] {
                CreateResult(0, "http://someurl.com/", "Something on the web"),
                CreateResult(1, "http://someurl.co.uk/something", "Other thing on the web"),
                CreateResult(2, "http://it.someurl.com/", "Qualcosa sul web"),
                CreateResult(3, "http://nonsense.org/", "Nothing useful"),
                CreateResult(4, "http://hello-xunit.com/", "XUnit web stuff")
            };
            return await Task.Run(() => results);
        }
        
        // public override string Name
        // {
        //     get { return "fake_earcher_with_five_results"; }
        // }
    }
}