using System;

namespace Checkbox.Styles
{
    /// <summary>
    /// Lightweight style template that contains basic information about a style template and
    /// is suitable for databinding applications where it may be too expensive to load a list
    /// of full style templates.
    /// </summary>
    public class LightweightStyleTemplate
    {
        /// <summary>
        /// The database id of the style
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// The database id of the analysis appearance id
        /// </summary>
        /// <remarks>This is only used for chart styles, otherwise it is null</remarks>
        public int? AppearanceId { get; set; }

        /// <summary>
        /// Style name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Style type
        /// </summary>
        public StyleTemplateType Type { get; set; }

        /// <summary>
        /// Indicates whether this style is usable by the public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Indicates whether this style can be edited by other users
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Indicates whether this style can be edited by the current user
        /// </summary>
        public bool CanBeEdited { get; set; }

        /// <summary>
        /// Name of the style's creator
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Date the style was created
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
