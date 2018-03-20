using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Styles;

namespace CheckboxWeb.Styles.Forms
{ 
    /// <summary>
    /// 
    /// </summary>
    public class EditStylePropertiesPage : SecuredPage
    {
        private StyleTemplate _currentStyleTemplate;

        [QueryParameter("s")]
        public int? TemplateId { get; set; }

        protected const String InMemoryCurrentStyleTemplateEditorSessionKey = "CurrentStyleTemplate";
        
        /// <summary>
        /// Get current style template
        /// </summary>
        public StyleTemplate CurrentStyleTemplate
        {
            get
            {
                if (_currentStyleTemplate == null || _currentStyleTemplate.TemplateID != TemplateId)
                {
                    if (Session[InMemoryCurrentStyleTemplateEditorSessionKey] == null ||
                        (Session[InMemoryCurrentStyleTemplateEditorSessionKey] as StyleTemplate).TemplateID != TemplateId 
                        )
                    {
                        if (!TemplateId.HasValue)
                            throw new Exception("Cannot load style template");

                        Session[InMemoryCurrentStyleTemplateEditorSessionKey] =
                            StyleTemplateManager.GetStyleTemplate(TemplateId.Value);
                    }

                    _currentStyleTemplate = Session[InMemoryCurrentStyleTemplateEditorSessionKey] as StyleTemplate;
                }

                return _currentStyleTemplate;
            }
        }

    }
}