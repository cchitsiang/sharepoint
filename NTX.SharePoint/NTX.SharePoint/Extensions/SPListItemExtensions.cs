using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTX.SharePoint.Extensions
{
    public static class SPListItemExtensions
    {
        public static void SetValue(this SPListItem item, string internalName, object value)
        {
            SPField field = item.Fields.GetFieldByInternalName(internalName);
            item[field.Id] = value;
        }

        public static object GetValue(this SPListItem item, string internalName)
        {
            SPField field = item.Fields.GetFieldByInternalName(internalName);
            return item[field.Id];
        }

        public static SPFieldLookupValue GetLookup(this SPListItem item)
        {
            return item != null ? new SPFieldLookupValue(item.ID, String.Empty) : null;
        }

        public static void UpdateUnsafe(this SPListItem item)
        {
            if (item != null)
            {
                bool allow = item.Web.AllowUnsafeUpdates;
                item.Web.AllowUnsafeUpdates = true;
                item.Update();
                item.Web.AllowUnsafeUpdates = allow;
            }
        }

        public static void SystemUpdateUnsafe(this SPListItem item)
        {
            if (item != null)
            {
                bool allow = item.Web.AllowUnsafeUpdates;
                item.Web.AllowUnsafeUpdates = true;
                item.SystemUpdate();
                item.Web.AllowUnsafeUpdates = allow;
            }
        }

        public static void DeleteUnsafe(this SPListItem item)
        {
            if (item != null)
            {
                bool allow = item.Web.AllowUnsafeUpdates;
                item.Web.AllowUnsafeUpdates = true;
                item.Delete();
                item.Web.AllowUnsafeUpdates = allow;
            }
        }
    }
}
