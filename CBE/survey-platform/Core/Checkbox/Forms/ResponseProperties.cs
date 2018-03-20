using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms
{
    /// <summary>
    /// Container for response-related properties
    /// </summary>
    [Serializable]
    public class ResponseProperties
    {
        private readonly Dictionary<string, object> _properties;

        /// <summary>
        /// Constructor
        /// </summary>
        public ResponseProperties()
        {
            _properties = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        public const string PageScorePropertyName = "PageScore_";

        /// <summary>
        /// 
        /// </summary>
        public const string PagePossibleScorePropertyName = "PagePossibleScore_";

        /// <summary>
        /// Static accessor to get a list of possible property names
        /// </summary>
        public static ReadOnlyCollection<string> PropertyNames
        {
            get
            {
                List<string> names = new List<string> 
                {
                    "CurrentDateUS", 
                    "CurrentDateROTW", 
                    "CurrentScore",
                    "TotalPossibleScore",
                    "ResponseID", 
                    "ResponseGuid", 
                    "LastPageViewed", 
                    "IP", 
                    "UniqueIdentifier", 
                    "NetworkUser", 
                    "Language", 
                    "Started", 
                    "Ended", 
                    "LastEdit", 
                    "IsComplete", 
                    "RespondentGuid",
                    "Invitee"
                };

                names.Sort();
                return new ReadOnlyCollection<string>(names);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxPagePosition"></param>
        /// <param name="includePossibleScore"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPageScoreProperties(int maxPagePosition, bool includePossibleScore)
        {
            var res = new Dictionary<string, string>();
            for (int i = 1; i < maxPagePosition; i++)
            {
                var text = Utilities.StripHtml(TextManager.GetText("/responseProperty/PageScore/text"), 64);
                text = text.Replace("{0}", i.ToString());
                res.Add(PageScorePropertyName + i, text);

                if (includePossibleScore)
                {
                    var textP = Utilities.StripHtml(TextManager.GetText("/responseProperty/PagePossibleScore/text"), 64);
                    textP = textP.Replace("{0}", i.ToString());
                    res.Add(PagePossibleScorePropertyName + i, textP);
                }
            }
            return res;
        }

        /// <summary>
        /// Intialize the properties collection with the response object
        /// </summary>
        /// <param name="response"></param>
        public void Initialize(Response response)
        {
            _properties["CurrentDateUS"] = DateTime.Now.ToString(Utilities.GetUsCulture());
            _properties["CurrentDateROTW"] = DateTime.Now.ToString(Utilities.GetRotwCulture());
            _properties["CurrentScore"] = GetCurrentScore(response);
            _properties["TotalPossibleScore"] = GetTotalPossibleScore(response);
            _properties["ResponseID"] = response.ID;
            _properties["ResponseGuid"] = response.GUID;
            _properties["LastPageViewed"] = response.LastPageViewed;
            _properties["IP"] = response.IPAddress;
            _properties["UniqueIdentifier"] = response.UniqueIdentifier;
            _properties["NetworkUser"] = response.NetworkUser;
            _properties["Language"] = response.LanguageCode;
            _properties["Started"] = response.DateCreated;
            _properties["Ended"] = response.DateCompleted;
            _properties["LastEdit"] = response.LastModified;
            _properties["IsComplete"] = response.Completed;
            _properties["RespondentGuid"] = response.RespondentGuid;
            _properties["ResponseTemplateID"] = response.ResponseTemplateID;
            _properties["Invitee"] = response.Invitee;

            //fill response pages score
            //response.PageCount returns count of visible pages, +1 here is for the completion page
            for (int i=1; i< response.PageCount + 1; i++)
            {
                _properties[PageScorePropertyName + i] = GetScoreForPageAtPosition(response, i);
                _properties[PagePossibleScorePropertyName + i] = GetPossibleScoreForPageAtPosition(response, i);
            }
        }

        /// <summary>
        /// Get the current score.
        /// </summary>
        /// <param name="response"></param>
        private static double GetCurrentScore(Response response)
        {
            double score = 0;

            foreach (ResponsePage p in response.GetResponsePages())
            {
                score += p.GetItems().OfType<IScored>().Sum(item => (item).GetScore());
            }

            return score;
        }

        /// <summary>
        /// Get the current score.
        /// </summary>
        /// <param name="response"></param>
        private static double GetTotalPossibleScore(Response response)
        {
            double score = 0;

            foreach (ResponsePage p in response.GetResponsePages())
            {
                score += p.GetItems().Where(i => !i.Excluded).OfType<IScored>().Sum(item => (item).GetPossibleMaxScore());
            }

            return score;
        }

        /// <summary>
        /// Get the current score.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="pageIndex"></param>
        private static double GetScoreForPageAtPosition(Response response, int pageIndex)
        {
            double score = 0;

            var page = response.GetPageByIndex(pageIndex);

            if (page != null)
                score += page.GetItems().OfType<IScored>().Sum(scored => scored.GetScore());

            return score;
        }

        /// <summary>
        /// Get the current score.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="pageIndex"></param>
        private static double GetPossibleScoreForPageAtPosition(Response response, int pageIndex)
        {
            double score = 0;

            var page = response.GetPageByIndex(pageIndex);

            if (page != null)
                score += page.GetItems().Where(i => !i.Excluded).OfType<IScored>().Sum(scored => scored.GetPossibleMaxScore());

            return score;
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return GetObjectValue(key); }
            set { SetValue(key, value); }
        }

        /// <summary>
        /// Get a property value as an object
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetObjectValue(string key)
        {
            if (_properties.ContainsKey(key))
            {
                return _properties[key];
            }
            
            return null;
        }

        /// <summary>
        /// Get the property value as an object
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetStringValue(string key)
        {
            object val = GetObjectValue(key);

            return val != null ? val.ToString() : string.Empty;
        }

        /// <summary>
        /// Set the value of a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetValue(string propertyName, object value)
        {
            _properties[propertyName] = value;
        }
    }
}