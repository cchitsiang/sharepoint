using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTX.SharePoint.Extensions
{
    public static class SPListExtensions
    {
        /// <summary>
        /// Get list by its internal name by checking the RootFolder.Name property.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="internalname"></param>
        /// <returns></returns>
        public static SPList GetListByInternalName(this SPWeb web, string internalname)
        {
            return web.Lists.Cast<SPList>().FirstOrDefault(list => list.RootFolder.Name == internalname); 
        }

        /// <summary>
        /// Checks whether the current user has the appropriate permissions to add items to the list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CanCreateItems(this SPList list)
        {
            return list != null && (list.EffectiveBasePermissions & SPBasePermissions.AddListItems) != SPBasePermissions.EmptyMask;
        }

        /// <summary>
        /// Checks whether the current user has read permission to the list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CanReadItems(this SPList list)
        {
            return list != null && (list.EffectiveBasePermissions & SPBasePermissions.ViewListItems) != SPBasePermissions.EmptyMask;
        }

        /// <summary>
        /// Checks whether the current user has write permission to the list.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CanWriteItems(this SPList list)
        {
            return list != null && (list.EffectiveBasePermissions & SPBasePermissions.EditListItems) != SPBasePermissions.EmptyMask;
        }

        /// <summary>
        /// Checks whether the current user has delete permission to the list. 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <remarks>Use this method with caution, as the entire list is read into memory, even if a subsquent LINQ clause extracts a single item.</remarks>
        public static bool CanDeleteItems(this SPList list)
        {
            return list != null && (list.EffectiveBasePermissions &
            SPBasePermissions.DeleteListItems) != SPBasePermissions.EmptyMask;
        }

        /// <summary>
        /// Returns all items of the list without any filtering.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<SPListItem> GetItems(this SPList list)
        {
            return list.GetItemsByQuery(String.Empty);
        }

        /// <summary>
        /// Returns filtered list of items by checking the field whether equals to specific value.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static IEnumerable<SPListItem> GetItemsByFieldEquals(this SPList list, string value, string field)
        {
            var query = new XElement("Where", 
                    new XElement("Eq", 
                    new XElement("FieldRef", new XAttribute("Name", field)), 
                    new XElement("Value", value)));
            return list.GetItemsByQuery(query.ToString(SaveOptions.DisableFormatting));
        }

        /// <summary>
        /// Returns filtered list of items by checking the field whether contains specific value.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static IEnumerable<SPListItem> GetItemsByFieldContains(this SPList list, string value, string field)
        {
            var query = new XElement("Where",
                    new XElement("Contains",
                    new XElement("FieldRef", new XAttribute("Name", field)),
                    new XElement("Value", value)));
            return list.GetItemsByQuery(query.ToString(SaveOptions.DisableFormatting));
        }

        /// <summary>
        /// Returns filtered list of items by checking the lookup field whether contains specific lookup value.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="lookupId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static IEnumerable<SPListItem> GetItemsByFieldLookupId(this SPList list, int value, string field)
        {
            var query = new XElement("Where", 
                    new XElement("Eq", 
                    new XElement("FieldRef", new XAttribute("LookupId", "true"), new XAttribute("Name", field)),
                    new XElement("Value", value)));
            return list.GetItemsByQuery(query.ToString(SaveOptions.DisableFormatting));
        }

        /// <summary>
        /// Returns list of items by CAML query.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="caml"></param>
        /// <param name="rowLimit"></param>
        /// <param name="paginationThreshold"></param>
        /// <returns></returns>
        public static IEnumerable<SPListItem> GetItemsByQuery(this SPList list, string caml, int rowLimit = 1000, int paginationThreshold = 5000)
        {
            if (list != null)
            {
                if (list.ItemCount <= paginationThreshold)
                {
                    SPQuery query = new SPQuery();
                    query.ViewAttributes = "Scope=\"Recursive\"";
                    query.Query = caml;
                    query.RowLimit = 1;
                    SPListItemCollection items = list.GetItems(query);
                    foreach (SPListItem item in items)
                    {
                        yield return item;
                    }
                }
                else
                {
                    SPQuery query = new SPQuery();
                    query.ViewAttributes = "Scope=\"Recursive\"";
                    query.Query = caml;
                    query.RowLimit = Convert.ToUInt32(rowLimit);
                    do
                    {
                        SPListItemCollection items = list.GetItems(query);
                        foreach (SPListItem item in items)
                        {
                            yield return item;
                        }
                        query.ListItemCollectionPosition = items.ListItemCollectionPosition;
                    } while (query.ListItemCollectionPosition != null);
                }
            }
        }

        /// <summary>
        /// Returns first item that matched CAML query filtering.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="caml"></param>
        /// <returns></returns>
        public static SPListItem GetItemByQuery(this SPList list, string caml)
        {
            SPQuery query = new SPQuery();
            query.ViewAttributes = "Scope=\"Recursive\"";
            query.Query = caml;
            query.RowLimit = 1;
            SPListItemCollection items = list.GetItems(query);
            return items.Cast<SPListItem>().FirstOrDefault();
        }

        public static SPListItem CreateItem(this SPList list)
        {
            return list.Items.Add();
        }

        public static void AddEventReceivers(this SPList list, Type eventReceiverType,
            params SPEventReceiverType[] erTypes)
        {
            foreach (SPEventReceiverType erType in erTypes)
            {
                list.EventReceivers.Add(erType, eventReceiverType.Assembly.FullName,
                              eventReceiverType.FullName);
            }
        }

        public static void RemoveEventReceivers(this SPList list, Type eventReceiverType, params SPEventReceiverType[] eventReceiverTypes)
        {
            List<SPEventReceiverDefinition> receivers = new List<SPEventReceiverDefinition>();
            for (int i = 0; i < list.EventReceivers.Count; ++i)
            {
                SPEventReceiverDefinition r = list.EventReceivers[i];
                if (r.Class.Equals(eventReceiverType.FullName) && (eventReceiverTypes.Length == 0 || eventReceiverTypes.Contains(r.Type)))
                    receivers.Add(r);
            }

            foreach (SPEventReceiverDefinition r in receivers)
            {
                r.Delete();
            }

        }

    }
}
