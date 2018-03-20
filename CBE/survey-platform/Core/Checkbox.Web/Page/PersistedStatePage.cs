using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Management;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// A base class designed to persist the ViewState to a database
    /// </summary>
    public class PersistedStatePage : ApplicationPage
    {
        private PageStatePersister _pageStatePersister;

        /// <summary>
        /// 
        /// </summary>
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                if (ApplicationManager.AppSettings.PersistViewStateToDb)
                {
                    if (_pageStatePersister == null)
                    {
                        string guid;

                        if (Request["__DATABASE_VIEWSTATE"] == null)
                        {
                            Guid newGuid = Guid.NewGuid();
                            guid = newGuid.ToString();
                        }
                        else
                        {
                            guid = Request["__DATABASE_VIEWSTATE"];
                        }

                        _pageStatePersister = new DatabasePageStatePersister(this, guid, Session.SessionID);
                    }

                    return _pageStatePersister;
                }
                
                return base.PageStatePersister;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (ApplicationManager.AppSettings.PersistViewStateToDb)
            {
                string guid = ((DatabasePageStatePersister)PageStatePersister).GUID;

                //Store a guid which identifies the ViewState data. This is to prevent potential errors
                //if a user where to open multiple copies of the same page.
                Literal databaseViewState = new Literal {Text = string.Format("<div><input type=\"hidden\" name=\"__DATABASE_VIEWSTATE\" value=\"{0}\" /></div>", guid)};
                Form.Controls.AddAt(0, databaseViewState);
            }
        }
    }
}