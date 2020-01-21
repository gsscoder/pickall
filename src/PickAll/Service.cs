using System;

namespace PickAll
{
    public abstract class Service
    {
        public SearchContext Context { get; set; }
        protected object Settings { get; set; }
    }
}