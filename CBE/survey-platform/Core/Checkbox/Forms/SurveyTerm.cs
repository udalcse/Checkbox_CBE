using System;
using Checkbox.Forms.Items;

namespace Checkbox.Forms
{
    [Serializable]
    public class SurveyTerm
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Term { get; set; }

        public string Definition { get; set; }

        public string Link { get; set; }

        public int ResponseTemplateId { get; set; }
    }
}
