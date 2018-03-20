using System;
using System.Collections.Generic;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Page branch action that moves response to a new page.
    /// </summary>
    [Serializable]
    public class GoToPageAction : Action
    {
        private readonly int _targetPageId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="targetPageId"></param>
        public GoToPageAction(int pageId, int targetPageId)
            : base(ActionReceiverType.Page, pageId)
        {
            _targetPageId = targetPageId;
        }

        /// <summary>
        /// Execute action.
        /// </summary>
        /// <param name="directive"></param>
        /// <param name="response"></param>
        public override void Execute(bool directive, Response response)
        {
            if (directive)
            {
                ResponsePage rp = response.GetPage(_targetPageId);

                if (rp != null)
                {
                    int pageIndex = response.GetResponsePages().IndexOf(rp);

                    response.PrimeNextPage(pageIndex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TargetPageId
        {
            get { return _targetPageId; }
        }

        /// <summary>
        /// Get action as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Go To Page " + _targetPageId;
        }
    }
}
