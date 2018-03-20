using System.Collections.Specialized;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// 
    /// </summary>
    public class JavascriptItem : ResponseItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Script { set; get; }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            if (!ApplicationManager.AppSettings.EnableJavascriptItem || !ApplicationManager.AppSettings.AllowJavascriptItem)
                Excluded = true;

            base.Configure(configuration, languageCode, templateId);

            var data = (JavascriptItemData)configuration;
            Script = data.Script;
        }

        /// <summary>
        /// Get instance collection
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection collection = base.GetInstanceDataValuesForSerialization();

            collection["Script"] = Script;

            return collection;
        }
    }
}
