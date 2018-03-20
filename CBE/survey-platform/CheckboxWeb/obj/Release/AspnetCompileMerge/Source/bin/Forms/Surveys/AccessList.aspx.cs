using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Page;
using Checkbox.Web.UI.Controls.Security;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class AccessList : ResponseTemplateSecurityEditorPage
    {
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));

           
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            
            _grantAccess.Initialize(SecuredResourceType.Survey, ResponseTemplate.ID.Value, "Form.Fill");
            

            Master.OkClick += new EventHandler(OkButton_OnClick);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OkButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl(string.Format("~/Forms/Surveys/Launch.aspx?s={0}&step=1&pt=2", ResponseTemplateId)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Prezza.Framework.Security.IAccessControllable GetControllableEntity()
        {
            return ResponseTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission 
        { 
            get 
            { 
               return "Form.Administer"; 
            } 
        }
    }
}