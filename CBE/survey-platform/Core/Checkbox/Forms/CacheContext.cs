using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Security.Principal;

namespace Checkbox.Forms
{
    /// <summary>
    /// Cache context for the one request to the server
    /// </summary>
    public class CacheContext
    {
        public CacheContext()
        {
            _storage = new Dictionary<string, object>();
        }

        public ResponseTemplate ResponseTemplate
        {
            get;
            set;
        }

        Dictionary<string, object> _storage;
        public Dictionary<string, object> Storage
        {
            get
            {
                return _storage;
            }
        }
    }
}
