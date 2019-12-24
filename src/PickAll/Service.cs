namespace PickAll
{
    public abstract class Service
    {
        private SearchContext _context;
        private object _settings;

        public SearchContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        protected object Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }
    }
}