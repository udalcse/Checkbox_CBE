using System.Drawing;
using System.Web.UI.WebControls;
using Checkbox.Web.UI.Controls;

namespace Checkbox.Web.Forms.UI.Rendering
{
    ///<summary>
    ///</summary>
    public class UserControlLoadErrorRenderer : UserControlItemRendererBase
    {
        private Panel _errorPanel;
        private ErrorMessage _errorCtrl;

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _errorPanel = new Panel
            {
                BorderColor = Color.Red,
                BorderStyle = BorderStyle.Double,
                BorderWidth = Unit.Pixel(3),
                BackColor = Color.LightYellow
            };

            _errorCtrl = new ErrorMessage {Visible = true};

            _errorPanel.Controls.Add(_errorCtrl);
            
            Controls.Add(_errorPanel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="additionalData"></param>
        public void SetError(string msg, string additionalData)
        {
            EnsureChildControls();

            _errorCtrl.Message = msg;
            _errorCtrl.SubMessage = additionalData;


        }
    }
}
