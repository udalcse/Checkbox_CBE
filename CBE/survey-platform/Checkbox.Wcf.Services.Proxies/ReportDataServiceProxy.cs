﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Checkbox.Wcf.Services.Proxies
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class ReportDataServiceProxy : System.ServiceModel.ClientBase<IReportDataService>, IReportDataService
    {

        public ReportDataServiceProxy()
        {
        }

        public ReportDataServiceProxy(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public ReportDataServiceProxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ReportDataServiceProxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ReportDataServiceProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public ServiceOperationResult<IItemProxyObject> GetItem(string authTicket, int reportId, int itemId, bool includeIncompleteResponses, string languageCode)
        {
            return Channel.GetItem(authTicket, reportId, itemId, includeIncompleteResponses, languageCode);
        }

        public ServiceOperationResult<IItemProxyObject> GetItemNoPreload(string authTicket, int reportId, int itemId, bool includeIncompleteResponses, string languageCode)
        {
            return Channel.GetItemNoPreload(authTicket, reportId, itemId, includeIncompleteResponses, languageCode);
        }

        public ServiceOperationResult<IItemProxyObject> GetResultsForSurveyItem(string authTicket, int surveyId, int surveyItemId, bool includeIncompleteResponses, string languageCode)
        {
            return Channel.GetResultsForSurveyItem(authTicket, surveyId, surveyItemId, includeIncompleteResponses, languageCode);
        }
    }
}
