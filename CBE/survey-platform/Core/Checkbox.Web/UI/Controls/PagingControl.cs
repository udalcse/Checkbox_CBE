using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Checkbox.Management;


namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Summary description for QuestionEditor.
    /// </summary>
    [DefaultProperty("Text"),
    ToolboxData("<{0}:PagingControl runat=server></{0}:PagingControl>")]
    public class PagingControl : CompositeControl
    {

        private ImageButton _prevBtn;
        private ImageButton _firstBtn;
        private ImageButton _nextBtn;
        private ImageButton _lastBtn;
        private Image _prevDisabledImg;
        private Image _firstDisabledImg;
        private Image _nextDisabledImg;
        private Image _lastDisabledImg;
        private DropDownList _pageDropDown;
        private Label _pageNumberLbl;

        ///<summary>
        ///</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        public delegate void PagingChanged(object sender, CustomPagingEventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event PagingChanged OnPagingChanged;

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateChildControls()
        {
            _prevBtn = new ImageButton { ImageUrl = PreviousPageEnabledImageUrl };
            _prevBtn.Click += PrevBtn_Click;

            _prevDisabledImg = new Image { ImageUrl = PreviousPageDisabledImageUrl };


            _firstBtn = new ImageButton { ImageUrl = FirstPageEnabledImageUrl };
            _firstBtn.Click += FirstBtn_Click;

            _firstDisabledImg = new Image { ImageUrl = FirstPageDisabledImageUrl };

            _lastBtn = new ImageButton { ImageUrl = LastPageEnabledImageUrl };
            _lastBtn.Click += LastBtn_Click;

            _lastDisabledImg = new Image { ImageUrl = LastPageDisabledImageUrl };

            _nextBtn = new ImageButton { ImageUrl = NextPageEnabledImageUrl };
            _nextBtn.Click += NextBtn_Click;

            _nextDisabledImg = new Image { ImageUrl = NextPageDisabledImageUrl };

            _pageNumberLbl = new Label { Visible = false };

            _pageDropDown = new DropDownList { ID = "pagedropdown", AutoPostBack = true };
            _pageDropDown.SelectedIndexChanged += PageDropDown_SelectedIndexChanged;

            for (int i = 1; i <= PageCount; i++)
            {
                ListItem li = new ListItem { Text = i.ToString(), Value = i.ToString() };
                _pageDropDown.Items.Add(li);

            }

            //hide the paging if there are no more than 1 page
            string cssClass = "libraryPager";
          /*  if (PageCount < 2)
                cssClass += " hidden"; */

            Controls.Add(new LiteralControl("<table align=\"center\" class=\"" + cssClass + "\" ><tr>"));
            Controls.Add(new LiteralControl("<td valign=\"center\">"));
            Controls.Add(_firstBtn);
            Controls.Add(_firstDisabledImg);
            Controls.Add(_prevBtn);
            Controls.Add(_prevDisabledImg);
            Controls.Add(new LiteralControl("</td><td valign=\"center\"> "));
            Controls.Add(_pageDropDown);
            Controls.Add(new LiteralControl("</td><td valign=\"center\"  >"));
            Controls.Add(_nextBtn);
            Controls.Add(_nextDisabledImg);
            Controls.Add(_lastBtn);
            Controls.Add(_lastDisabledImg);
            Controls.Add(_pageNumberLbl);
            Controls.Add(new LiteralControl("</td></tr></table>"));

            UpdateControls();


        }

        ///<summary>
        ///</summary>
        public int PageNumber
        {
            get
            {
                if (ViewState["PageNumber"] != null)
                    return (int)ViewState["PageNumber"];

                return 1;
            }
            set
            {
                ViewState["PageNumber"] = value;
            }
        }

        protected string FirstPageEnabledImageUrl
        {
            get { return ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/page-first.gif"; }
        }

        protected string NextPageEnabledImageUrl
        {
            get { return ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/page-next.gif"; }
        }

        protected string PreviousPageEnabledImageUrl
        {
            get { return ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/page-prev.gif"; }
        }

        protected string LastPageEnabledImageUrl
        {
            get { return ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/page-last.gif"; }
        }

        protected string FirstPageDisabledImageUrl
        {
            get { return ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/page-first-off.gif"; }
        }

        protected string NextPageDisabledImageUrl
        {
            get { return ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/page-next-off.gif"; }
        }

        protected string PreviousPageDisabledImageUrl
        {
            get { return ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/page-prev-off.gif"; }
        }

        protected string LastPageDisabledImageUrl
        {
            get { return ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/page-last-off.gif"; }
        }

        ///<summary>
        ///</summary>
        public int PageCount
        {
            get
            {
                if (ViewState["PageCount"] != null)
                    return (int)ViewState["PageCount"];

                return 1;
            }
            set
            {
                ViewState["PageCount"] = value > 1 ? value : 1;
            }
        }

        ///<summary>
        ///</summary>
        public int ItemCount
        {
            get
            {
                if (ViewState["ItemCount"] != null)
                    return (int)ViewState["ItemCount"];

                return 0;
            }
            set
            {
                ViewState["ItemCount"] = value;
            }
        }

        private void PrevBtn_Click(object sender, ImageClickEventArgs e)
        {
            if (PageNumber > 1)
            {
                PageNumber--;
                UpdateControls();
                OnPagingChanged(this, new CustomPagingEventArgs(PageNumber));
            }
        }

        private void FirstBtn_Click(object sender, ImageClickEventArgs e)
        {
            PageNumber = 1;
            UpdateControls();
            OnPagingChanged(this, new CustomPagingEventArgs(PageNumber));
        }

        private void LastBtn_Click(object sender, ImageClickEventArgs e)
        {
            PageNumber = PageCount;
            UpdateControls();
            OnPagingChanged(this, new CustomPagingEventArgs(PageNumber));
        }

        private void NextBtn_Click(object sender, ImageClickEventArgs e)
        {
            if (PageNumber < PageCount)
            {
                PageNumber++;
                UpdateControls();
                OnPagingChanged(this, new CustomPagingEventArgs(PageNumber));
            }
        }

        private void PageDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageNumber = Convert.ToInt32(((DropDownList)sender).SelectedValue);
            UpdateControls();
            OnPagingChanged(this, new CustomPagingEventArgs(PageNumber));
        }

        ///<summary>
        ///</summary>
        public void Reload()
        {
            EnsureChildControls();
            _pageDropDown.Items.Clear();

            for (int i = 1; i <= PageCount; i++)
            {
                ListItem li = new ListItem { Text = i.ToString(), Value = i.ToString() };
                _pageDropDown.Items.Add(li);

            }
            UpdateControls();
        }

        private void UpdateControls()
        {
            // turn on the buttons that we need....
            if (PageNumber > PageCount)
            {
                PageNumber = PageCount;
            }

            if (PageNumber <= 1 || ItemCount == 0)
            {
                _prevDisabledImg.Visible = true;
                _firstDisabledImg.Visible = true;
                _prevBtn.Visible = false;
                _firstBtn.Visible = false;
            }
            else
            {
                _prevBtn.Visible = true;
                _firstBtn.Visible = true;
                _prevDisabledImg.Visible = false;
                _firstDisabledImg.Visible = false;
            }

            if (PageNumber == PageCount || ItemCount == 0)
            {
                _nextDisabledImg.Visible = true;
                _lastDisabledImg.Visible = true;
                _nextBtn.Visible = false;
                _lastBtn.Visible = false;
            }
            else
            {
                _nextDisabledImg.Visible = false;
                _lastDisabledImg.Visible = false;
                _nextBtn.Visible = true;
                _lastBtn.Visible = true;
            }

            _pageDropDown.SelectedValue = PageNumber.ToString();
        }
    }

    ///<summary>
    ///</summary>
    public class CustomPagingEventArgs : EventArgs
    {
        ///<summary>
        ///</summary>
        ///<param name="target"></param>
        public CustomPagingEventArgs(int target)
        {
            TargetPage = target;
        }

        ///<summary>
        ///</summary>
        public int TargetPage { get; set; }
    }
}
