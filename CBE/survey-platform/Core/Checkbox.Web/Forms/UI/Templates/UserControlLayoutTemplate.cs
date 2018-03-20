using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Checkbox.Common;
using Checkbox.Forms.PageLayout;
using Checkbox.Forms.PageLayout.Configuration;
using Checkbox.Web.Common;

namespace Checkbox.Web.Forms.UI.Templates
{
    /// <summary>
    /// Form template, implemented as a user control
    /// </summary>
    public class UserControlLayoutTemplate : UserControlBase, IWebLayoutTemplate
    {
        private Dictionary<string, IWebLayoutZone> _zonesDictionary;
        private Dictionary<int, string> _itemZoneMappings;

        /// <summary>
        /// Get the type name for the control
        /// </summary>
        public virtual string TypeName { get { return "UserControl"; } }

        /// <summary>
        /// Determine if the template is read-only and can't be modified by the template
        /// page editor
        /// </summary>
        public virtual bool ReadOnly { get { return false; } }

        /// <summary>
        /// Get the name of the default zone
        /// </summary>
        protected virtual string DefaultZoneName { get{return "Default";}}

        /// <summary>
        /// Get the name of the next zone.
        /// </summary>
        protected virtual string NextZoneName { get{return "Next/Finish";}}

        /// <summary>
        /// Get the name of the previous zone
        /// </summary>
        protected virtual string PreviousZoneName { get{return "Back";}}

        /// <summary>
        /// Get the name of the form reset zone
        /// </summary>
        protected virtual string FormResetZoneName { get { return "Form Reset"; } }

        /// <summary>
        /// Get the name of the finish zone
        /// </summary>
        protected virtual string FinishZoneName { get { return NextZoneName; } }

        /// <summary>
        /// Get the name of the save and quit zone
        /// </summary>
        protected virtual string SaveAndQuitZoneName { get { return "Save and Quit"; } }

        /// <summary>
        /// Get the name of the progress bar zone
        /// </summary>
        protected virtual string ProgressBarTopZoneName { get{return "Progress Bar Top";}}

        /// <summary>
        /// Get the name of the progress bar zone
        /// </summary>
        protected virtual string ProgressBarBottomZoneName { get { return "Progress Bar Bottom"; } }

        /// <summary>
        /// Get the name of the title zone
        /// </summary>
        protected virtual string TitleZoneName { get{return "Title";}}

        /// <summary>
        /// Get the name of the page number zone
        /// </summary>
        protected virtual string PageNumberZoneName { get{return "Page Numbers";}}

        /// <summary>
        /// Get the name of the header zone
        /// </summary>
        protected virtual string HeaderZoneName { get { return "Header"; } }

        /// <summary>
        /// Get the name of the footer zone
        /// </summary>
        protected virtual string FooterZoneName { get { return "Footer"; } }

        /// <summary>
        /// Get/set whether this is design mode
        /// </summary>
        public bool LayoutDesignMode { get; set; }

        /// <summary>
        /// Get/set the current language code
        /// </summary>
        public string CurrentLanguageCode { get; set; }

        /// <summary>
        /// Initialize with page layout data
        /// </summary>
        /// <param name="layoutData"></param>
        /// <param name="languageCode"></param>
        public virtual void Initialize(PageLayoutTemplateData layoutData, string languageCode)
        {
            _itemZoneMappings = layoutData.GetItemZoneMappings();
            CurrentLanguageCode = languageCode;
        }

        /// <summary>
        /// Get the item zone mappings
        /// </summary>
        protected Dictionary<int, string> ItemZoneMappings
        {
            get 
            {
                if (_itemZoneMappings == null)
                {
                    _itemZoneMappings = new Dictionary<int, string>();
                }

                return _itemZoneMappings;
            }
        }

        /// <summary>
        /// Get the default zone for items not in any other zone
        /// </summary>
        public ILayoutZone DefaultZone
        {
            get { return GetZone(DefaultZoneName); }
        }

        /// <summary>
        /// Get the zone for the finish button
        /// </summary>
        public ILayoutZone FinishButtonZone
        {
            get { return GetZone(FinishZoneName); }
        }

        /// <summary>
        /// Get the zone for the next button
        /// </summary>
        public ILayoutZone NextButtonZone
        {
            get { return GetZone(NextZoneName); }
        }

        /// <summary>
        /// Get the zone for the next button
        /// </summary>
        public ILayoutZone FormResetZone
        {
            get { return GetZone(FormResetZoneName); }
        }

        /// <summary>
        /// Get the zone for the previous button
        /// </summary>
        public ILayoutZone PreviousButtonZone
        {
            get { return GetZone(PreviousZoneName); }
        }

        /// <summary>
        /// Get the zone for the save and quit button
        /// </summary>
        public ILayoutZone SaveAndQuitButtonZone
        {
            get { return GetZone(SaveAndQuitZoneName); }
        }

        /// <summary>
        /// Get the zone to contain the progress bar
        /// </summary>
        public ILayoutZone ProgressBarTopZone
        {
            get { return GetZone(ProgressBarTopZoneName); }
        }

        /// <summary>
        /// Get the zone to contain the progress bar
        /// </summary>
        public ILayoutZone ProgressBarBottomZone
        {
            get { return GetZone(ProgressBarBottomZoneName); }
        }

        /// <summary>
        /// Get the zone to contain the title
        /// </summary>
        public ILayoutZone TitleZone
        {
            get { return GetZone(TitleZoneName); }
        }

        /// <summary>
        /// Get the zone to contain the page number
        /// </summary>
        public ILayoutZone PageNumberZone
        {
            get{return GetZone(PageNumberZoneName);}
        }

        /// <summary>
        /// Get the header zone
        /// </summary>
        public ILayoutZone HeaderZone
        {
            get { return GetZone(HeaderZoneName); }
        }

        /// <summary>
        /// Get the footer zone
        /// </summary>
        public ILayoutZone FooterZone
        {
            get { return GetZone(FooterZoneName); }
        }

        /// <summary>
        /// Get the zone for an item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ILayoutZone GetItemZone(int itemID)
        {
            ILayoutZone zone = null;

            if (ItemZoneMappings.ContainsKey(itemID))
            {
                zone = GetZone(ItemZoneMappings[itemID]);
            }

            if (zone == null)
            {
                zone = DefaultZone;
            }

            return zone;
        }

        /// <summary>
        /// Get the zone with the specified name
        /// </summary>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        private ILayoutZone GetZone(string zoneName)
        {
            if (Utilities.IsNotNullOrEmpty(zoneName) && ZonesDictionary.ContainsKey(zoneName))
            {
                return ZonesDictionary[zoneName];
            }
            
            return null;
        }


        /// <summary>
        /// Get the item zones for this control
        /// </summary>
        protected Dictionary<string, IWebLayoutZone> ZonesDictionary
        {
            get
            {
                if (_zonesDictionary == null)
                {
                    _zonesDictionary = GetLayoutZonesDictionary();
                }

                return _zonesDictionary;
            }
        }

        /// <summary>
        /// Get a list of item zones for the template
        /// </summary>
        public ReadOnlyCollection<ILayoutZone> Zones
        {
            get
            {
                List<ILayoutZone> zones = new List<ILayoutZone>();

                foreach (IWebLayoutZone zone in ZonesDictionary.Values)
                {
                    zones.Add(zone);
                }

                return new ReadOnlyCollection<ILayoutZone>(zones);
            }
        }

        /// <summary>
        /// Remove all items from template zones
        /// </summary>
        public virtual void ClearZones()
        {
            foreach(IWebLayoutZone  layoutZone in ZonesDictionary.Values)
            {
                layoutZone.Clear();
            }
        }

        /// <summary>
        /// Add a control to a zone
        /// </summary>
        /// <param name="zoneName"></param>
        /// <param name="c"></param>
        public virtual void AddControlToZone(string zoneName, Control c)
        {
            if (ZonesDictionary.ContainsKey(zoneName))
            {
                ZonesDictionary[zoneName].AddControl(c);
            }
        }

        /// <summary>
        /// Remove a control from a zone
        /// </summary>
        /// <param name="zoneName"></param>
        /// <param name="c"></param>
        public virtual void RemoveControlFromZone(string zoneName, Control c)
        {
            if (ZonesDictionary.ContainsKey(zoneName))
            {
                ZonesDictionary[zoneName].RemoveControl(c);
            }
        }

        /// <summary>
        /// Get the item zones for the template
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, IWebLayoutZone> GetLayoutZonesDictionary()
        {
            Dictionary<string, IWebLayoutZone> itemZones = new Dictionary<string, IWebLayoutZone>();

            BuildLayoutZoneDictionary(itemZones, this);

            return itemZones;
        }



        /// <summary>
        /// Recursively build the list of item zones
        /// </summary>
        /// <param name="itemZones"></param>
        /// <param name="parent"></param>
        private static void BuildLayoutZoneDictionary(IDictionary<string, IWebLayoutZone> itemZones, Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is IWebLayoutZone)
                {
                    itemZones[((IWebLayoutZone)child).ZoneName] = (IWebLayoutZone)child;
                }

                BuildLayoutZoneDictionary(itemZones, child);
            }
        }

        /// <summary>
        /// Get a list of zones reserved for non-question purposes
        /// </summary>
        public ReadOnlyCollection<string> ReservedZones
        {
            get
            {
                return new ReadOnlyCollection<string>(new[] { "Next/Finish", "Title", "Progress Bar", "Back", "Save and Quit", "Page Numbers", "Header", "Footer", "Form Reset" });
            }
        }

        /// <summary>
        /// Set design mode for zones
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            foreach (IWebLayoutZone zone in ZonesDictionary.Values)
            {
                zone.LayoutDesignMode = LayoutDesignMode;
            }

            base.OnPreRender(e);
        }
    }
}
