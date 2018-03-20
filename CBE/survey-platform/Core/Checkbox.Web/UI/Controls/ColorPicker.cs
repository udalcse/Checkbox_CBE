using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;


namespace Checkbox.Web.UI.Controls
{
	/// <summary>
	/// Summary description for ColorPicker.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:ColorPicker runat=server></{0}:ColorPicker>")]
	public class ColorPicker : PlaceHolder
	{
	    private TextBox _fieldName;
		private string _url;


	    ///<summary>
	    ///</summary>
	    [Bindable(true),
	     Category("Appearance"),
	     DefaultValue("")]
	    public string Text { get; set; }

	    /// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
	        Image image = new Image {ImageUrl = (Management.ApplicationManager.ApplicationRoot + "/Images/button-colorPicker.gif")};


	        LiteralControl spacer = new LiteralControl {Text = "&nbsp;"};
	        Controls.Add(spacer);
			
			Controls.Add(image);

			LiteralControl color = new LiteralControl();

            if (_fieldName != null)
            {
                //color.Text = "&nbsp;<table id=\"Table" + fieldName + "\" name=\"Table" + fieldName + "\" border=\"1px\" style=\"display:inline\" height=\"15\" width=\"15\" bgcolor=\"" + tb.Text + "\"><tr><td> </td></tr></table>&nbsp;";
                color.Text = "<span style=\"display:inline\" id=\"Div" + _fieldName.ClientID + "\" name=\"Div" + _fieldName.ClientID + "\">" +
                    "&nbsp;<table style=\"display:inline;width:16px;height:16px\"  border=\"1px\" bgcolor=\"" +
                    _fieldName.Text + "\"><tr><td width=\"16\"></td></tr></table></span>";
                Controls.Add(color);

                _url = Management.ApplicationManager.ApplicationRoot + "/Styles/ColorPicker.aspx?fieldName=" + _fieldName.UniqueID;

                //else
                //{
                //URL = Checkbox.Management.ApplicationManager.ApplicationRoot + "/Styles/ColorPicker.aspx?fieldName=" + fieldName.ClientID + "&noSwatch=True";
                //}

                string script = "window.open('" + _url + "','popped',  'toolbar=0, scrollbars=1, location=0, statusbar=0, menubar=0,resizable=1, width=487, height=500,left=100, top=100');return false;";

                image.Attributes.Add("onClick", script);
                image.Attributes.Add("title", WebTextManager.GetText("/common/newWindow"));
            }
			
		

			base.Render(output);
			
		}

		///<summary>
		///</summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("")] 
		public TextBox ColorPickerField
		{
			set
			{
				_fieldName = value;
			}
		}


	}
}
