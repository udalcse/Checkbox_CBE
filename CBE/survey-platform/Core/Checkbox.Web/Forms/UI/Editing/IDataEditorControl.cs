using System;
using System.Web.UI;

namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Interaface for control that binds to and updates data
    /// </summary>
    /// <typeparam name="T">Type of data edited by the control.</typeparam>
    public interface IDataEditorControl<T>
    {
        /// <summary>
        /// Bind to the specified data source
        /// </summary>
        /// <param name="datasource">Data source to bind to.</param>
        void BindToDataSource(T datasource);

        /// <summary>
        /// Update the specified data source. Useful when the item shouldn't maintain]
        /// a reference to the source.
        /// </summary>
        /// <param name="dataSource"></param>
        void UpdateDataSource(T dataSource);

        /// <summary>
        /// Validate user inputs
        /// </summary>
        /// <returns></returns>
        bool ValidateInputs();
    }
}
