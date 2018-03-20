using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using Checkbox.Analytics;
using Checkbox.Pagination;
using Prezza.Framework.Data;
using System.Data;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.UI.Controls
{
    ///<summary>
    ///</summary>
    public static class ResponseLifecycleChartControlHelper
    {
        private static Dictionary<string, int> BuildLifecycleData(int surveyId, int numberOfRecentMonths)
        {
            Dictionary<string, int> periodsAndResponses = new Dictionary<string, int>();

            Database db = DatabaseFactory.CreateDatabase();
            IDataReader reader = db.ExecuteReader("ckbx_sp_Response_LifecycleMonthly", surveyId, DateTime.Today.AddMonths(-numberOfRecentMonths));
            try
            {

                while (reader.Read())
                {
                    DateTime dt = (DateTime)reader["MinDate"];
                    int count = (int)reader["Count"];

                    periodsAndResponses.Add(string.Format("{0}/{1}", dt.Month, dt.Year), count);
                }

                if (periodsAndResponses.Count == 0)
                {
                    for (int i = 0; i < numberOfRecentMonths; i++)
                    {
                        DateTime dt = DateTime.Today.AddMonths(-numberOfRecentMonths + i + 1);
                        periodsAndResponses.Add(string.Format("{0}/{1}", dt.Month, dt.Year), 0);
                    }
                }
            }
            finally
            {
                reader.Close();
            }

            return periodsAndResponses;
        }

        private static Dictionary<string, int> BuildLifecycleData(int surveyId, int periodLengthInDays, int numberOfPeriods, out int totalResponses)
        {
            Dictionary<string, int> periodsAndResponses = new Dictionary<string, int>();

            Database db = DatabaseFactory.CreateDatabase();
            IDataReader reader = db.ExecuteReader("ckbx_sp_Response_LifecycleDaily", surveyId, DateTime.Today.AddDays(-periodLengthInDays * numberOfPeriods));
            try
            {
                DateTime now = DateTime.Now.AddDays(-periodLengthInDays + 1);
                int i = 0;
                
                while (reader.Read())
                {
                    var firstDate = now.Date.AddDays(-periodLengthInDays * (numberOfPeriods - 1 - i));
                    var lastDate = firstDate.AddDays(periodLengthInDays).AddSeconds(-1);

                    DateTime dt = (DateTime)reader["MinDate"];

                    while (dt > lastDate)
                    {
                        if (!periodsAndResponses.ContainsKey(FormatDate(firstDate) + "-" + FormatDate(lastDate)))
                        {
                            periodsAndResponses.Add(FormatDate(firstDate) + "-" + FormatDate(lastDate), 0);
                        }
                        i++;
                        firstDate = now.Date.AddDays(-periodLengthInDays * (numberOfPeriods - 1 - i));
                        lastDate = firstDate.AddDays(periodLengthInDays).AddSeconds(-1);
                    }

                    int count = (int)reader["Count"];

                    if (firstDate <= dt && dt <= lastDate)
                    {
                        if (!periodsAndResponses.ContainsKey(FormatDate(firstDate) + "-" + FormatDate(lastDate)))
                        {
                            periodsAndResponses.Add(FormatDate(firstDate) + "-" + FormatDate(lastDate), count);
                        }
                        else
                        {
                            periodsAndResponses[FormatDate(firstDate) + "-" + FormatDate(lastDate)] += count;
                        }
                    }
                }

                //read total response count
                reader.NextResult();
                reader.Read();
                totalResponses = (int)reader[0];

                if (periodsAndResponses.Count == 0)
                {
                    for (i = 0; i < numberOfPeriods; i++)
                    {
                        var firstDate = now.Date.AddDays(-periodLengthInDays * (numberOfPeriods - 1 - i));
                        var lastDate = firstDate.AddDays(periodLengthInDays).AddSeconds(-1);
                        periodsAndResponses.Add(FormatDate(firstDate) + "-" + FormatDate(lastDate), 0);
                    }
                }
            }
            finally
            {
                reader.Close();
            }

            return periodsAndResponses;
        }

        private static string FormatDate(DateTime date)
        {
            return string.Format("{0}/{1}", date.Month, date.Day );
        }

        private static string BuildChartImage(IEnumerable<int> yVal, IEnumerable<string> xName)
        {
            ColorConverter cc = new ColorConverter();

            Chart chart = new Chart
                              {
                                  ImageType = ChartImageType.Jpeg,
                                  Width = 600,
                                  Height = 300,
                                  BackColor = Color.White,
                                  BackSecondaryColor = (Color) cc.ConvertFromString("#cecac5")
                              };

            var series = new Series
                             {
                                 ChartType = SeriesChartType.Column,
                                 Color = (Color) cc.ConvertFromString("#c5dbe5"),
                                 CustomProperties = "DrawingStyle=Cylinder, MaxPixelPointWidth=50",
                                 ShadowOffset = 2,
                             };

            series.Points.DataBindXY(xName, yVal);
            int pointsCount = series.Points.Count();
            for (int i = 0; i < pointsCount; i++)
            {
                series.Points[i].IsValueShownAsLabel = yVal.ElementAt(i) != 0;
            }

            var chartArea = new ChartArea
                                {
                                    BackColor = Color.White
                                };
            chartArea.AxisX.MajorGrid.LineColor = (Color) cc.ConvertFromString("#cecac5");
            chartArea.Area3DStyle.Rotation =  15;
            chartArea.Area3DStyle.Inclination =  15;
            chartArea.Area3DStyle.Enable3D = true;

            chart.ChartAreas.Add(chartArea);
            chart.Series.Add(series);
            chart.Legends.Add(new Legend { Enabled = false });

            //Save image to database
            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms);
                var imageId = DbUtility.SaveImage(ms.GetBuffer(), string.Empty, string.Empty, "TempImage",
                                                  Guid.NewGuid().ToString(), DateTime.Now, true);
                //return image url
                return WebUtilities.ResolveUrl("~/ViewContent.aspx?imageId=" + imageId);
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="surveyId"></param>
        ///<param name="periodLengthInDays"></param>
        ///<param name="numberOfPeriods"></param>
        ///<returns></returns>
        public static string GetResponseLifecycleChartImageUrl(int surveyId, int periodLengthInDays, int numberOfPeriods)
        {
            int completeResponseCount;

            var data = BuildLifecycleData(surveyId, periodLengthInDays, numberOfPeriods, out completeResponseCount);

            return BuildChartImage(data.Select(d => d.Value).ToList(), data.Select(d => d.Key).ToList());
        }


        ///<summary>
        ///</summary>
        ///<returns></returns>
        public static string GetResponseLifecycleChartImageUrl(int surveyId, int numberOfRecentMonths)
        {
            var data = BuildLifecycleData(surveyId, numberOfRecentMonths);

            return BuildChartImage(data.Select(d => d.Value).ToList(), data.Select(d => d.Key).ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="periodLengthInDays"></param>
        /// <param name="numberOfPeriods"></param>
        /// <returns></returns>
        public static ResponseAggregatedData GetResponseLifecycle(int surveyId, int periodLengthInDays, int numberOfPeriods)
        {
            ResponseAggregatedData result = new ResponseAggregatedData();
            int completedResponseCount;
            
            var data = BuildLifecycleData(surveyId, periodLengthInDays, numberOfPeriods, out completedResponseCount);
            
            result.AggregateResults = (from d in data.Keys select new AggregateResult { ResultText = d, AnswerCount = data[d] }).ToArray();
            result.CompletedResponseCount = completedResponseCount;

            return result;
        }
    }
}
