using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class MatrixRowsNormalEntry : Checkbox.Web.Common.UserControlBase
    {
        [QueryParameter("isNew")]
        public bool IsNew { get; set; }

        [QueryParameter("row")]
        public int? RowNumber { get; set; }

        [QueryParameter("i")]
        public int? ItemId { get; set; }

        [QueryParameter("lid")]
        public int? LibraryTemplateId { get; set; }

        public string Html { get; set; }

        /// <summary>
        /// Matrix id
        /// </summary>
        public MatrixItemTextDecorator MatrixItemTextDecorator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SurveyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PagePosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EditMode EditMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Fires before redirect to html options editor
        /// </summary>
        public event EventHandler OnHtmlEditorRedirect;

        /// <summary>
        /// Bind events
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _addRowLink.Click += _addRowLink_Click;
            _htmlEditorLink.Click += _htmlEditorLink_Click;
            _updateRowButton.Click += _updateRowButton_Click;
            _deleteRowButton.Click += _deleteRowButton_Click;
            _postRows.Click += _postRows_Click;
            _updateRowsOrder.Click += UpdateRowsOrderOnClick;
        }

        private void UpdateRowsOrderOnClick(object sender, EventArgs eventArgs)
        {
            if (OnHtmlEditorRedirect != null)
                OnHtmlEditorRedirect(this, new EventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _postRows_Click(object sender, EventArgs e)
        {
            if (OnHtmlEditorRedirect != null)
                OnHtmlEditorRedirect(this, new EventArgs());

            var row = _currentrow.Value;
            var html = _currenthtml.Value;

            var uri = ResolveUrl("~/Forms/Surveys/HtmlEditor.aspx") +
                      "?s=" + SurveyId.ToString() +
                      "&p=" + PagePosition +
                      "&i=" + ItemId +
                      "&l=" + LanguageCode +
                      "&row=" + row +
                      "&isNew=" + IsNew +
                      "&isMatrix=false" + 
                      "&lid=" + (LibraryTemplateId.HasValue ? LibraryTemplateId.ToString() : string.Empty) +
                        ((Request == null || string.IsNullOrEmpty(Request["w"])) ? "" : ("&w=" + Request["w"]));

            uri += "&html=" + html;

            Response.Redirect(uri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _pipeSelector.Initialize(EditMode == EditMode.Survey ? (int?)SurveyId : null, PagePosition, LanguageCode, _newMatrixRowText.ClientID);
        }

        /// <summary>
        /// Matrix item to edit
        /// </summary>
        /// <param name="matrixItemTextDecorator"></param>
        /// <param name="surveyId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        /// <param name="languageCode"></param>
        public void Initialize(int surveyId, int pagePosition, MatrixItemTextDecorator matrixItemTextDecorator, EditMode editMode, string languageCode)
        {
            //get from query, doesn't work as an attribute for some reason
            IsNew = Convert.ToBoolean(HttpContext.Current.Request.Params["isNew"]);
            ItemId = Convert.ToInt32(HttpContext.Current.Request.Params["i"]);
            RowNumber = Convert.ToInt32(HttpContext.Current.Request.Params["row"]);
            bool fromHtmlRedactor = Convert.ToBoolean(HttpContext.Current.Request.Params["fromHtmlRedactor"]);
            bool cancel = Convert.ToBoolean(HttpContext.Current.Request.Params["cancel"]);

            int lid;
            LibraryTemplateId = null;
            if (int.TryParse(HttpContext.Current.Request.Params["lid"], out lid))
                LibraryTemplateId = lid;

            MatrixItemTextDecorator = matrixItemTextDecorator;
            SurveyId = surveyId;
            PagePosition = pagePosition;
            LanguageCode = languageCode;
            
            if (fromHtmlRedactor && !cancel && RowNumber > 0)
            {
                Html = HttpContext.Current.Session["temporary_html_" + ItemId + "_c="] as string;
                MatrixItemTextDecorator.SetRowText(RowNumber.Value, Html);
                RemoveHtmlEditorParameter();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        protected string GetRowAlias(int rowNumber)
        {
            var text = MatrixItemTextDecorator.Data.GetRowAlias(rowNumber);

            //if (!Utilities.IsHtmlFormattedText(text))
            //    text = Utilities.AdvancedHtmlEncode(text);

            return text;
        }

        protected string GetRowText(int rowNumber)
        {
            var text = MatrixItemTextDecorator.GetRowText(rowNumber);

            if (Utilities.IsTextEncoded(text))
                text = Utilities.AdvancedHtmlDecode(text);

            //if (!Utilities.IsHtmlFormattedText(text))
            //    text = Utilities.AdvancedHtmlEncode(text);

            return text;
        }

        private void RemoveHtmlEditorParameter()
        {
            var nameValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            nameValues.Set("fromHtmlRedactor", "false");
            string url = Request.Url.AbsolutePath;
            string updatedQueryString = "?" + nameValues;
            Response.Redirect(url + updatedQueryString);
        }

        private void AddRow(bool withHtmlRedactor)
        {
            var rowType = "normal".Equals(_newRowType.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                              ? RowType.Normal
                              : "subheading".Equals(_newRowType.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                                    ? RowType.Subheading
                                    : RowType.Other;

            //Add row
            MatrixItemTextDecorator.Data.AddRow(rowType.ToString(), _newMatrixRowAlias.Text.Trim());
            
            string rowText = _newMatrixRowText.Text.Trim();
            if (Utilities.IsHtmlFormattedText(rowText))
                rowText = Utilities.AdvancedHtmlDecode(rowText);
            
            MatrixItemTextDecorator.SetRowText(MatrixItemTextDecorator.Data.RowCount, string.IsNullOrEmpty(rowText) ? "Row " + MatrixItemTextDecorator.Data.RowCount.ToString() : rowText);

            //Store in session
            HttpContext.Current.Session[Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] = MatrixItemTextDecorator;

            //Rebind
            DataBind();

            //
            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "onRowAdded",
                "$(function(){onRowAdded(" + withHtmlRedactor.ToString().ToLower() + ");});",
                true);
        }

        private void _htmlEditorLink_Click(object sender, EventArgs e)
        {
            AddRow(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _addRowLink_Click(object sender, EventArgs e)
        {
            AddRow(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _updateRowButton_Click(object sender, EventArgs e)
        {
            var editRow = _currentEditRow.Text.Trim();
            
            var updatedAlias = _updatedRowAlias.Text.Trim();
            var updatedType = _updatedRowType.Text.Trim();
            var updatedText = _updatedRowText.Text.Trim();

            int? rowNumber = Utilities.AsInt(editRow);

            if (!rowNumber.HasValue)
            {
                return;
            }

            if (updatedType == "other")
                updatedText = string.Empty;

            //Update text
            if (!"[Empty]".Equals(updatedText, StringComparison.InvariantCultureIgnoreCase))
            {
                MatrixItemTextDecorator.SetRowText(rowNumber.Value, updatedText);
            }

            //Update alias
            if (!"[Empty]".Equals(updatedAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                MatrixItemTextDecorator.Data.SetRowAlias(rowNumber.Value, updatedAlias);
            }

            //Update row type
            if (!"[Empty]".Equals(updatedType, StringComparison.InvariantCultureIgnoreCase))
            {
                MatrixItemTextDecorator.Data.UpdateRow(
                    rowNumber.Value, 
                    MatrixItemTextDecorator.Data.GetRowAlias(rowNumber.Value), 
                    updatedType);
            }

            //Store in session
            HttpContext.Current.Session[Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] = MatrixItemTextDecorator;

            DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void _deleteRowButton_Click(object sender, EventArgs e)
        {
            var editRow = _currentEditRow.Text.Trim();

            int? rowNumber = Utilities.AsInt(editRow);

            if (!rowNumber.HasValue)
            {
                return;
            }

            MatrixItemTextDecorator.RemoveRow(rowNumber.Value);

            //Store in session
            HttpContext.Current.Session[Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] = MatrixItemTextDecorator;

            DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RowOrderChanged
        {
            get { return Utilities.IsNotNullOrEmpty(_newRowOrder.Text); }
            set
            {
                if (value)
                    _newRowOrder.Text = String.IsNullOrEmpty(_newRowOrder.Text) ? "1" : _newRowOrder.Text;
                else                
                    _newRowOrder.Text = "";                
            }
        }

        /// <summary>
        /// Get new row order as list of previous row numbers
        /// </summary>
        /// <returns></returns>
        public List<int> GetNewRowOrder()
        {
            if (!RowOrderChanged)
            {
                return new List<int>();
            }

            return _newRowOrder.Text
                .Split(',')
                .Where(rowNumber => Utilities.AsInt(rowNumber).HasValue)
                .Select(rowNumber => Utilities.AsInt(rowNumber).Value)
                .ToList();
        }

        /// <summary>
        /// Update matrix item data with new row order
        /// </summary>
        /// <param name="matrixTextDecorator"></param>
        public void UpdateData(MatrixItemTextDecorator matrixTextDecorator)
        {
            //Get Ids of rows at current positions
            if (!RowOrderChanged)
            {
                return;
            }

            matrixTextDecorator.SetRowOrder(GetNewRowOrder());
            
            //Make order clear
            RowOrderChanged = false;
        }
    }
}