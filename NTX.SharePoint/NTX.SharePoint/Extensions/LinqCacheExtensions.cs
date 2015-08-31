using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace NTX.SharePoint.Extensions
{
    public static class LinqCacheExtensions
    {
        public static IEnumerable<T> Cache<T>(this IEnumerable<T> source,
        string key,
        DateTime absoluteExpiration)
        {
            var items = HttpRuntime.Cache.Get(key) as List<T>;
            if (items == null)
            {
                items = source.ToList();
                HttpRuntime.Cache.Add(key, items, null, absoluteExpiration,
                System.Web.Caching.Cache.NoSlidingExpiration,
                CacheItemPriority.Normal, null);
            }
            foreach (var item in items)
            {
                yield return item;
            }
        }
        public static IEnumerable<T> Cache<T>(this IEnumerable<T> source, string key,
        TimeSpan slidingExpiration)
        {
            var items = HttpRuntime.Cache.Get(key) as List<T>;
            if (items == null)
            {
                items = source.ToList();
                HttpRuntime.Cache.Add(key, items, null,
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                slidingExpiration,
                CacheItemPriority.Normal, null);
            }
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}
