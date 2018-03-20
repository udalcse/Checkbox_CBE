using System.Web.UI.WebControls;

namespace Checkbox.Web.UI.Controls.Styles
{
    /// <summary>
    /// Drop down list bound do an XML data source.
    /// </summary>
    public class XmlBoundDropDownList : DropDownList
    {
        private XmlDataSource _dataSource;

        /// <summary>
        /// Get the data source
        /// </summary>
        protected XmlDataSource XmlSource
        {
            get 
            {
                if (_dataSource == null)
                {
                    _dataSource = new XmlDataSource();
                }

                return _dataSource;
            }
        }

        /// <summary>
        /// Get/set the selected value
        /// </summary>
        public override string SelectedValue
        {
            get
            {
                return base.SelectedValue;
            }
            set
            {
                if (Items.FindByValue(value) != null)
                {
                    base.SelectedValue = value;
                }
            }
        }


        /// <summary>
        /// Get/set the data source path
        /// </summary>
        public string DataFile
        {
            get { return XmlSource.DataFile; }
            set { XmlSource.DataFile = value; }
        }

        /// <summary>
        /// Get/set xpath
        /// </summary>
        public string XPath
        {
            get { return XmlSource.XPath; }
            set { XmlSource.XPath = value; }
        }

        /// <summary>
        /// Bind to data source
        /// </summary>
        public override void DataBind()
        {
            if (_dataSource != null)
            {
                DataSource = _dataSource;
            }

            base.DataBind();
        }
    }
}
