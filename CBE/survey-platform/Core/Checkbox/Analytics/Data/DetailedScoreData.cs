using System;
using System.Collections.Generic;

namespace Checkbox.Analytics.Data
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DetailedScoreData
    {
        public List<PageScoreData> PageScores { set; get; }
        public double PossibleSurveyMaxScore { set; get; }
        public double CurrentSurveyScore { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PageScoreData
    {
        public bool IsExcluded { set; get; }
        public int Position { set; get; }
        public double MaxPossibleScore { set; get; }
        public double CurrentScore { set; get; }
    }
}
