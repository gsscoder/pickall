using System;

namespace PickAll
{
    /// <summary>Represents a service managed by <c>SearchContext</c>.</summary>
    public abstract class Service
    {
        SearchContext _context;
        internal event EventHandler Load;

        public SearchContext Context 
        {
            get { return _context; }
            set
            {
                _context = value;
                // Guard against raising load event before configuration happens.
                // A service is loaded when is bound to a search context.
                if (_context == null) return;
                EventHelper.RaiseEvent(this, Load, EventArgs.Empty, _context.Settings.EnableRaisingEvents);
            }
        }

        public RuntimeInfo Runtime { get; internal set; }

        protected object Settings { get; set; }
    }
}