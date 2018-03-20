using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Delegate for event fired when column order is changed
    /// </summary>
    /// <param name="columnsToPrototypeIds"></param>
    public delegate void ColumnOrderChangedDelegate(Dictionary<int, int> columnsToPrototypeIds);

    /// <summary>
    /// Delegate for event fired before redirecting to other page
    /// </summary>
    public delegate void BeforeRedirectDelegate();

    /// <summary>
    /// 
    /// </summary>
    public partial class MatrixColumnEditor : Checkbox.Web.Common.UserControlBase
    {
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

        public bool AreColumnsCategorized { get; set; }

        /// <summary>
        /// Determine if this dialog was shown just after item creation
        /// </summary>
        public bool IsNew
        {
            get
            {
                string isNew = Request.QueryString["isNew"];
                return !string.IsNullOrEmpty(isNew) && isNew.ToLower() == "true";
            }
        }

        /// <summary>
        /// This event raises when column order is changed
        /// </summary>
        public event ColumnOrderChangedDelegate OnColumnOrderChanged;

        /// <summary>
        /// This event raises before control redirects to another page
        /// </summary>
        public event BeforeRedirectDelegate OnBeforeRedirect;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            if (Request.Params.Get("__EVENTTARGET") == "_editColumnLink")
            {
                RedirectToEditColumnPage((int.Parse(Request.Params.Get("__EVENTARGUMENT"))));
            }

            _refreshColumns.Click += _refreshColumns_Click;
            _deleteColumn.Click += _deleteColumn_Click;

            //if (IsNew)
                addColumnBtn.Click += addColumnBtn_Click;
        }

        /// <summary>
        /// Redirect to edit column page
        /// </summary>
        /// <param name="columnNumber"></param>
        private void RedirectToEditColumnPage(int columnNumber)
        {
			string url = "EditMatrixColumn.aspx?";
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key != "c" && key != "fromHtmlRedactor" && key != "html" && key != "cancel")
                    url += key + "=" + Request.QueryString[key] + "&";
            }

            if (AreColumnsCategorized)
                url += "&categorized=true";

            url += "&c=" + columnNumber;
            
            if (OnBeforeRedirect != null)
                OnBeforeRedirect();
            
            Response.Redirect(url);
        }


        void addColumnBtn_Click(object sender, System.EventArgs e)
        {
            string url = "AddMatrixColumn.aspx?";
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key != "c" && key != "showColumnsTab" && key != "fromHtmlRedactor" && key != "html" && key != "cancel" && key != "isMatrix")
                    url += key + "=" + Request.QueryString[key] + "&";
            }
            url += "c=" + (MatrixItemTextDecorator.Data.ColumnCount + 1);

            if (AreColumnsCategorized)
                url += "&categorized=true";

			//ResolveUrl(url);

            if (OnBeforeRedirect != null)
                OnBeforeRedirect();

            Response.Redirect(url);
        }

        /// <summary>
        /// Matrix item to edit
        /// </summary>
        /// <param name="matrixItemTextDecorator"></param>
        /// <param name="surveyId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="areColumnsCategorized"></param>
        public void Initialize(int surveyId, int pagePosition, MatrixItemTextDecorator matrixItemTextDecorator, bool areColumnsCategorized)
        {
            MatrixItemTextDecorator = matrixItemTextDecorator;
            SurveyId = surveyId;
            PagePosition = pagePosition;
            AreColumnsCategorized = areColumnsCategorized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _refreshColumns_Click(object sender, System.EventArgs e)
        {
            DataBind();
        }


        /// <summary>
        /// Delete column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _deleteColumn_Click(object sender, System.EventArgs e)
        {
            int? columnToDelete = Utilities.AsInt(_columnToDelete.Text.Trim());

            if (columnToDelete.HasValue
                && columnToDelete.Value != MatrixItemTextDecorator.Data.PrimaryKeyColumnIndex)
            {
                MatrixItemTextDecorator.Data.RemoveColumn(columnToDelete.Value);
            }

            //Update in-memory item
            Session[Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] = MatrixItemTextDecorator;

            _columnToDelete.Text = string.Empty;

            DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ColumnOrderChanged
        {
            get { return Utilities.IsNotNullOrEmpty(_newColumnOrder.Text); }
        }

        /// <summary>
        /// Get new column order as list of previous column numbers
        /// </summary>
        /// <returns></returns>
        public List<int> GetNewColumnOrder()
        {
            if (!ColumnOrderChanged)
            {
                return new List<int>();
            }

            return _newColumnOrder.Text
                .Split(',')
                .Where(columnNumber => Utilities.AsInt(columnNumber).HasValue)
                .Select(columnNumber => Utilities.AsInt(columnNumber).Value)
                .ToList();
        }

        /// <summary>
        /// Update matrix item data with new column order
        /// </summary>
        /// <param name="matrixTextDecorator"></param>
        public void UpdateData(MatrixItemTextDecorator matrixTextDecorator)
        {
            //Get Ids of columns at current positions
            if (!ColumnOrderChanged)
            {
                return;
            }

            matrixTextDecorator.SetColumnOrder(GetNewColumnOrder());

            if (OnColumnOrderChanged != null)
            {
                //Fire the columnOrder change event
                Dictionary<int, int> columnsToIds = new Dictionary<int, int>();
                for (int i = 1; i <= matrixTextDecorator.Data.ColumnCount; i++)
                {
                    if (i != matrixTextDecorator.Data.PrimaryKeyColumnIndex)
                        columnsToIds.Add(i, matrixTextDecorator.Data.GetColumnPrototypeId(i));
                }

                OnColumnOrderChanged(columnsToIds);
            }

            //Reorder operation has been made. Set _newColumnOrder.Text to empty string
            _newColumnOrder.Text = string.Empty;
        }
    }
}