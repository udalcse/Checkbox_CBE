namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public partial class ValueListSlider : SliderBase
    {
        /// <summary>
        /// Get selected option ID
        /// </summary>
        public int SelectedOptionID
        {
            get
            {
                string value = Request[_options.UniqueID];

                int id;
                if (int.TryParse(value, out id))
                    return id;
                
                return string.IsNullOrEmpty(_options.SelectedValue) ? -1 : int.Parse(_options.SelectedValue);
            }
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude("selectToUISlider", ResolveUrl("~/Resources/selectToUISlider.jQuery.js"));
        }
    }
}