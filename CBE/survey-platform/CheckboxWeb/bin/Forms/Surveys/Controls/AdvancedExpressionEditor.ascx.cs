using System;
using System.Web.UI.WebControls;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Editor for expressions
    /// </summary>
    public partial class AdvancedExpressionEditor : ExpressionEditorControl
    {
        /// <summary>
        /// 
        /// </summary>
        public bool AddExpressionMode
        {
            get;
            set;
        }

        /// <summary>
        ///             
        /// </summary>
        protected void InitializeTargetPageId()
        {
            DropDownList pageList;
            if (Parent.ID=="_newBranchPanel")
            {
                pageList = Parent.FindControl("_newBranchPageList") as DropDownList;
            }
            else
            {
                pageList = Parent.FindControl("_goToPageList") as DropDownList;
            }
            if (pageList.SelectedValue!="")
                TargetPageId = Convert.ToInt32(pageList.SelectedValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeTargetPageId();

            RegisterClientScriptInclude(
                "advancedExpressionEditor.js",
                ResolveUrl("~/Resources/advancedExpressionEditor.js"));
        }


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="editorParams"></param>
        public void Initialize(ExpressionEditorParams editorParams)
        {
            //Initialize control
            Params = editorParams;
        }

        /// <summary>
        /// Expressions to bind/edit
        /// </summary>
        /// <param name="compositeExpression"></param>
        public void Bind(CompositeExpressionData compositeExpression)
        {
            if (Params == null)
            {
                throw new Exception("Basic expression editor control must be Initialized before it is bound.");
            }

        }

    }
}