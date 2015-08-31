using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTX.SharePoint.Extensions
{
    public static class SPFieldLookupValueExtensions
    {
        public static int Id(this SPFieldLookupValue lookup)
        {
            return lookup != null ? lookup.LookupId : 0;
        }

        public static string Value(this SPFieldLookupValue lookup)
        {
            return lookup != null ? lookup.LookupValue : String.Empty;
        }
    }
}
