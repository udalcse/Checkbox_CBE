using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Logic;
using Checkbox.Security.Principal;
using Checkbox.Forms.Piping.Tokens;
using Checkbox.Forms.Piping;

namespace Checkbox.Forms
{
    [Serializable]
    public class CachableResponse
    {
        /// <summary>
        /// The state
        /// </summary>
        public ResponseState ResponseState
        {
            get;
            set;
        }
        
        /// <summary>
        /// The rules
        /// </summary>
        public RulesEngine RulesEngine
        {
            get;
            set;
        }

        /// <summary>
        /// Respondent
        /// </summary>
        public CheckboxPrincipal Respondent
        {
            get;
            set;
        }

        /// <summary>
        /// Visited Pages
        /// </summary>
        public int[] VisitedPages
        {
            get;
            set;
        }

        /// <summary>
        /// Response Pipes
        /// </summary>
        public List<ItemToken> ResponsePipes
        {
            get;
            set;
        }

        /// <summary>
        /// Pipe Mediator
        /// </summary>
        public PipeMediator PipeMediator
        {
            get;
            set;
        }

        /// <summary>
        /// Next page number
        /// </summary>
        public int? NextPagePump
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentPageIndex
        {
            get;
            set;
        }
    }
}
