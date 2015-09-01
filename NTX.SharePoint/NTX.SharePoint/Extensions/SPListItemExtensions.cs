using Microsoft.SharePoint;
using Microsoft.SharePoint.BusinessData.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NTX.SharePoint.Extensions
{
    public static class SPListItemExtensions
    {
        public static void SetValue(this SPListItem item, string internalName, object value)
        {
            SPField field = item.Fields.GetFieldByInternalName(internalName);
            item[field.Id] = value;
        }

        /// <summary>
        /// Sets the value of an SPBusinessDataField to the specific value.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldInternalName"></param>
        /// <param name="newValue"></param>
        public static void SetExternalFieldValue(this SPListItem item, string fieldInternalName, string newValue)
        {
            if (item.Fields[fieldInternalName].TypeAsString == "BusinessData")
            {
                SPField myField = item.Fields[fieldInternalName];
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(myField.SchemaXml);
                //Get teh internal name of the SPBusinessDataField's identity column.
                String entityName = xmlData.FirstChild.Attributes["RelatedFieldWssStaticName"].Value;

                //Set the value of the identity column.
                item[entityName] = EntityInstanceIdEncoder.EncodeEntityInstanceId(new object[] { newValue });
                item[fieldInternalName] = newValue;
            }
            else
            {
                throw new InvalidOperationException(fieldInternalName + " is not of type BusinessData");
            }
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
