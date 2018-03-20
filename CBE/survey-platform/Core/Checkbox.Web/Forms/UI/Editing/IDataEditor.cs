namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Interface definition for a data editing control
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataEditor<T>
    {
        /// <summary>
        /// Initialize editor with data to edit
        /// </summary>
        /// <param name="data"></param>
        void Initialize(T data);

        /// <summary>
        /// Validate state of editor
        /// </summary>
        /// <returns></returns>
        bool Validate();

        /// <summary>
        /// Update data
        /// </summary>
        void UpdateData();

        /// <summary>
        /// Persist object data
        /// </summary>
        void SaveData();
    }
}
