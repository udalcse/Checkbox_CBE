using System;
using System.Collections.Generic;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms
{
    /// <summary>
    /// Interface for containers of answer data
    /// </summary>
    public interface IAnswerData
    {
        /// <summary>
        /// Get options for a specified item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        Dictionary<int, double?> GetOptionAnswersForItem(int itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="?"></param>
        /// <param name="optionId"> </param>
        /// <param name="points"> </param>
        /// <param name="otherText"> </param>
        /// <param name="doNotSave"> Will answer be inserted into database on save </param>
        void SetOptionAnswerForItem(int itemId, int optionId, double? points, string otherText, bool doNotSave = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="answer"></param>
        void SetTextAnswersForItem(int itemID, string answer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemID"></param>
        void SetEmptyAnswerForItem(int itemID);

        /// <summary>
        /// Get text answer for a specified item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        string GetTextAnswerForItem(int itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        List<long> GetAllAnswerIds(int itemId); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        void DeleteAllAnswersForItem(int itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId"> </param>
        void DeleteOptionAnswerForItem(int itemId, int optionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        bool IsAnswered(int itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        bool HasEmptyAnswer(int itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        List<SurveyResponseItemAnswer> BuildDataTransferObjectAnswerList(int itemId);

        /// <summary>
        /// Data saved event
        /// </summary>
        event EventHandler Saved;
    }
}
