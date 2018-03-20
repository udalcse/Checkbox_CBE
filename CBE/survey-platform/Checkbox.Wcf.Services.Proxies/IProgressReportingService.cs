using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for Progress Reporting Service
    /// </summary>
    [ServiceContract]
    public interface IProgressReportingService
    {
        #region GetData

        /// <summary>
        /// Get status of progress.
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="provider"> </param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        GetProgressResult GetProgressStatus(string progressKey, string provider);
        
        #endregion


        #region PostData

        /// <summary>
        /// Set status for a tracked operation.
        /// </summary>
        /// <param name="progressData"></param>
        /// <param name="provider"> </param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void SetProgressStatus(ProgressData progressData, string provider);

        #endregion
    }
}
