using System.Web.UI;
using Checkbox.Forms.Items.UI;

namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Based class for user control-based appearnance editors
    /// </summary>
    public abstract class UserControlAppearanceEditorBase : Checkbox.Web.Common.UserControlBase, IAppearanceEditor
    {
        /// <summary>
        /// Get appearance being edited.
        /// </summary>
        protected AppearanceData AppearanceData { get; private set; }


        #region IDataEditor<AppearanceData> Members


        /// <summary>
        /// Initialize the appearance editor
        /// </summary>
        /// <param name="data"></param>
        public virtual void Initialize(AppearanceData data)
        {
            AppearanceData = data;
        }

        /// <summary>
        /// Validate inputs
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            return true;
        }

        /// <summary>
        /// Update data
        /// </summary>
        public virtual void UpdateData()
        {
        
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SaveData()
        {
            if (AppearanceData != null)
            {
                AppearanceData.Save();
            }
        }

        #endregion
    }
}
