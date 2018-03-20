using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Container for collection of frequency analysis information. Each frequency item data contained herein contains frequency
    /// information for one survey item.  This class is also capable of aggregating across all the frequency item data it contains.
    /// </summary>
    [Serializable]
    public class FrequencyAnalysisDataCollection
    {
        private Dictionary<Int32, FrequencyAnalysisData> _data;
       
        /// <summary>
        /// Default constructor
        /// </summary>
        public FrequencyAnalysisDataCollection()
        {
            _data = new Dictionary<int, FrequencyAnalysisData>();
        }

        /// <summary>
        /// Add a data set to the collection
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="analysisData"></param>
        public void AddAnalysisData(Int32 itemID, FrequencyAnalysisData analysisData)
        {
            _data[itemID] = analysisData;
        }

        /// <summary>
        /// Remove analysis data for an item from the collection
        /// </summary>
        /// <param name="itemID"></param>
        public void RemoveAnalysisData(Int32 itemID)
        {
            if (_data.ContainsKey(itemID))
            {
                _data.Remove(itemID);
            }
        }

        /// <summary>
        /// Get the frequency data for a particular item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public FrequencyAnalysisData GetAnalysisData(Int32 itemID)
        {
            if (_data.ContainsKey(itemID))
            {
                return _data[itemID];
            }
            else
            {
                return null;
            }
        }

        //TODO: Provide aggregation methods when support is added for multiple item data to be added
        //      to the same table/graph.
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AnalysisAnswerData GetAggregateData()
        {
            throw new Exception("Method not supported.");
        }
        }
}
