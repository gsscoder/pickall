using CSharpx;

namespace PickAll
{
    public abstract class Service
    {
        private SetOnce<SearchContext> _context = new SetOnce<SearchContext>();
        private SetOnce<object> _settings = new SetOnce<object>();

        public SearchContext Context
        {
            get
            {
                return _context.Value;
            }
            set
            {
                _context.Value = value;
            }
        }

        protected object Settings
        {
            get
            {
                return _settings.Value;
            }
            set
            {
                _settings.Value = value;
            }
        }
    }
}