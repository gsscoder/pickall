using System;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Browser;
using AngleSharp.Browser.Dom;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

namespace PickAll.Tests.Fakes
{
    public class EmptyBrowsingContext  : IBrowsingContext
    {
        public IWindow Current { get { throw new NotImplementedException(); } }

        public IDocument Active { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IHistory SessionHistory { get { throw new NotImplementedException(); } }
        
        public Sandboxes Security { get { throw new NotImplementedException(); } }
        
        public IBrowsingContext Parent { get { throw new NotImplementedException(); } }
        
        public IDocument Creator { get { throw new NotImplementedException(); } }
        
        public IEnumerable<object> OriginalServices { get { throw new NotImplementedException(); } }

        public IBrowsingContext CreateChild(string name, Sandboxes security) { throw new NotImplementedException(); }

        public  IBrowsingContext FindChild(string name) { throw new NotImplementedException(); }
        
        public T GetService<T>() where T : class { throw new NotImplementedException(); }
        
        public IEnumerable<T> GetServices<T>() where T : class { throw new NotImplementedException(); }
    
        void IEventTarget.AddEventListener(string type, DomEventHandler callback, bool capture) { throw new NotImplementedException(); }
    
        void IEventTarget.RemoveEventListener(string type, DomEventHandler callback, bool capture) { throw new NotImplementedException(); }
   
        void IEventTarget.InvokeEventListener(Event ev) { throw new NotImplementedException(); }

        bool IEventTarget.Dispatch(Event ev) { throw new NotImplementedException(); }
    }    
}