using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Web.Page;
using Prezza.Framework.Caching;

namespace CheckboxWeb.Settings
{
        /// <summary>
    /// 
    /// </summary>
    public partial class MemcachedCache : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.OkVisible = false;
            Master.CancelVisible = false;
            _flushButton.Click += new EventHandler(_flushButton_Click);
            base.OnPageInit();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _flushButton_Click(object sender, EventArgs e)
        {
            try
            {
                var cms = CacheFactory.GetCacheManagers();

                foreach (var cm in cms)
                {
                    var cacheManager = CacheFactory.GetCacheManager(cm);

                    if (cacheManager != null)
                    {
                        cacheManager.Flush();
                    }
                }
            }
            catch
            {

            }
            finally
            {
            }
        }
    }
}