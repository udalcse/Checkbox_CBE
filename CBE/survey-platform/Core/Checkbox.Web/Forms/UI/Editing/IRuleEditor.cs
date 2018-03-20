namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Interface definition for rule editor control
    /// </summary>
    public interface IRuleEditor
    {
        /// <summary>
        /// Initialize editor with information about item & response template
        /// </summary>
        /// <param name="isPagePostback"></param>
        /// <param name="itemId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="languageCode"></param>
        void Initialize(bool isPagePostback, int itemId, int pagePosition, int responseTemplateId, string languageCode);

        /// <summary>
        /// Persist rule changes
        /// </summary>
        void SaveChanges();
    }
}
