using System.Collections.Generic;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Interface definition for item editors
    /// </summary>
    public interface IItemEditor
    {
        /// <summary>
        /// Initialize item editor
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="itemData"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        void Initialize(int responseTemplateId, int pagePosition, ItemData itemData, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview);

        /// <summary>
        /// Return a boolean indicating if editor input state is valid
        /// </summary>
        /// <returns></returns>
        bool Validate();

        /// <summary>
        /// Apply inputs to item data
        /// </summary>
        void UpdateData();

        /// <summary>
        /// Apply inputs to item data
        /// </summary>
        void UpdateData(bool updateTextDecoratorOptions);

        /// <summary>
        /// Determine if user entered enough information.
        /// </summary>
        /// <param name="message">Contains a warning message, which information should be added to the item</param>
        /// <returns></returns>
        bool DoesItemContainEnoughInformation(out string message);

        /// <summary>
        /// Persist data and return id of persisted data object
        /// </summary>
        int SaveData();

        /// <summary>
        /// Get a boolean indicating if the itme editor supports hosting
        /// the appearance editor.
        /// </summary>
        bool SupportsEmbeddedAppearanceEditor { get; }

        /// <summary>
        /// Embed the appearance editor in the item editor.
        /// </summary>
        /// <param name="appearanceEditor"></param>
        void EmbedAppearanceEditor(IAppearanceEditor appearanceEditor);
    }
}
