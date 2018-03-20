
namespace Checkbox.Web.Forms.UI.Rendering
{
    /// <summary>
    /// Interface for binding rule data to control to display rule data
    /// </summary>
    public interface IItemRuleDisplay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="languageCode"></param>
        void InitializeAndBind(int itemId, int responseTemplateId, string languageCode);
    }
}