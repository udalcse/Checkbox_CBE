using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.UI.Controls;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Web.Forms.UI.Rendering
{
    /// <summary>
    /// Item renderer that hosts a user control that extends the UserControlItemRendererBase class.
    /// </summary>
    /// <remarks>Item model must be set before any work can be done.</remarks>
    public class UserControlHostItemRenderer : Checkbox.Web.Common.WebControlBase, IItemRenderer
    {
        private UserControlItemRendererBase _userControl;

        /// <summary>
        /// Wrap control in div.
        /// </summary>
        protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }

        /// <summary>
        /// Get/set override for user control to use for rendering.  Path is relative to 
        /// Forms/Surveys/Controls/ItemRenderers folder.
        /// </summary>
        public string ControlNameOverride { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override Unit Width
        {
            get { return InternalItemRenderer != null && InternalItemRenderer.Width.HasValue ? InternalItemRenderer.Width.Value : Unit.Empty; }
            set
            {
                if (InternalItemRenderer != null)
                {
                    InternalItemRenderer.Width = value;
                }
            }
        }

        /// <summary>
        /// Get internal ItemRenderer
        /// </summary>
        public UserControlItemRendererBase InternalItemRenderer
        {
            get { return _userControl; }
        }

        /// <summary>
        /// Get control base path as absolute path from application root. e.g. /Forms/Surveys/Controls/ItemEditors
        /// </summary>
        protected virtual string ControlBasePath { get { return "/Forms/Surveys/Controls/ItemRenderers"; } }

        /// <summary>
        /// Get the path to the user control to use for rendering an item.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="renderMode"></param>
        /// <returns></returns>
        protected virtual string GetControlPath(IItemProxyObject model, RenderMode renderMode)
        {
            if (renderMode == RenderMode.SurveyMobilePreview)
                renderMode = RenderMode.SurveyMobile;

            string controlRoot = string.Format("{0}{1}", ApplicationManager.ApplicationRoot, ControlBasePath);
            string modeSpecificFolder = string.Format(controlRoot + "/" + renderMode);

            //First, see if there is a specific control for the current "mode".  i.e. edit survey, take survey, etc.
            try
            {
                string controlPath = Utilities.IsNullOrEmpty(ControlNameOverride)
                    ? string.Format("{0}/{1}.ascx", modeSpecificFolder, model.TypeName)
                    : string.Format("{0}/{1}.ascx", modeSpecificFolder, ControlNameOverride);

                if (HttpContext.Current != null
                    && HttpContext.Current.Server != null
                    && File.Exists(HttpContext.Current.Server.MapPath(controlPath)))
                {
                    return controlPath;
                }

            }
            catch
            {
                //Do nothing so we fall through and load normal renderer
            }
        
            //Otherwise, return path to default renderer
            return Utilities.IsNullOrEmpty(ControlNameOverride)
              ? string.Format("{0}/{1}.ascx", controlRoot, model.TypeName)
              : string.Format("{0}/{1}.ascx", controlRoot, ControlNameOverride);
        }

        #region IItemRenderer Members

        /// <summary>
        /// Initialize the renderer
        /// </summary>
        /// <param name="model"></param>
        /// <param name="itemPosition"></param>
        /// <param name="renderMode"></param>
        /// <param name="exportMode"></param>
        public void Initialize(IItemProxyObject model, int? itemPosition, RenderMode renderMode, ExportMode exportMode)
        {
            if (model == null)
            {
                throw new Exception("Attempt to set NULL model for UserControlHostItemRenderer.");
            }

            
            //Get path to user control
            string controlPath = GetControlPath(model, renderMode);

            //If no path, return
            if (Utilities.IsNullOrEmpty(controlPath))
            {
                return;
            }
			
			string path = HttpContext.Current.Server.MapPath(controlPath);
			string cp = controlPath.Remove(0,1).Replace("/", "\\");
			string path2 = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, cp);

            //If control does not exist, return
			if (!File.Exists(path) && !File.Exists(path2))
            {
                _userControl = new UserControlLoadErrorRenderer();
                ((UserControlLoadErrorRenderer)_userControl).SetError("Renderer control not found", controlPath);
                ExceptionPolicy.HandleException(new Exception(string.Format("Renderer control not found at [{0}].", controlPath)), "UIProcess");
            }

            try
            {
                //Attempt to load the user control
                if (_userControl == null)
                {
                    UserControl tempControl = new UserControl();
                    _userControl = tempControl.LoadControl(controlPath) as UserControlItemRendererBase;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
                throw;
            }

            //Ensure control was created
            if (_userControl == null)
            {
                throw new Exception(string.Format("Control located at [{0}] could not be loaded or was not a UserControlItemRendererBase object.", controlPath));
            }

            //Set model
            _userControl.Initialize(model, itemPosition, exportMode);

            //Add user control to controls collection
            Controls.Add(_userControl);

            //Set visibility based on model's visibility
            if (model is SurveyResponseItem)
            {
                Visible = ((SurveyResponseItem)model).Visible;
            }
        }

        void IItemRenderer.Initialize(IItemProxyObject model, int? itemPosition, RenderMode renderMode)
        {
            Initialize(model, itemPosition, renderMode, ExportMode.None);
        }

        /// <summary>
        /// User control item model
        /// </summary>
        public IItemProxyObject DataTransferObject
        {
            get
            {
                return _userControl != null 
                    ? _userControl.DataTransferObject 
                    : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SimpleNameValueCollection Appearance
        {
            get
            {
                return _userControl != null
                    ? _userControl.Appearance
                    : null;
            }
        }

        /// <summary>
        /// Bind
        /// </summary>
        public void BindModel()
        {
            if (_userControl != null)
            {
                _userControl.BindModel();
            }
        }

        /// <summary>
        /// Update model
        /// </summary>
        public void UpdateModel()
        {
            if (_userControl != null)
            {
                _userControl.UpdateModel();
            }
        }

        #endregion

        /// <summary>
        /// Attempt to handle error during data binding
        /// </summary>
        public override void DataBind()
        {
            try
            {
                base.DataBind();
            }
            catch (Exception ex)
            {
                Controls.Clear();
                Controls.Add(new ErrorMessage {Exception = ex, Visible=true});
            }
        }
    }
}
