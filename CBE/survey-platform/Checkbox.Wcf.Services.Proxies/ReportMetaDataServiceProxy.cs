﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Checkbox.Wcf.Services.Proxies
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class ReportMetaDataServiceProxy : System.ServiceModel.ClientBase<IReportMetaDataService>, IReportMetaDataService
    {

        public ReportMetaDataServiceProxy()
        {
        }

        public ReportMetaDataServiceProxy(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public ReportMetaDataServiceProxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ReportMetaDataServiceProxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ReportMetaDataServiceProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public ServiceOperationResult<int> GetReportCountForSurvey(string authTicket, int responseTemplateId)
        {
            return Channel.GetReportCountForSurvey(authTicket, responseTemplateId);
        }

        public ServiceOperationResult<ReportMetaData> GetReportWithId(string authTicket, int reportId)
        {
            return Channel.GetReportWithId(authTicket, reportId);
        }

        public ServiceOperationResult<ReportMetaData> GetReportWithName(string authTicket, int responseTemplateId, string reportName)
        {
            return Channel.GetReportWithName(authTicket, responseTemplateId, reportName);
        }

        public ServiceOperationResult<ReportPageMetaData> GetReportPageWithId(string authTicket, int reportId, int templatePageId)
        {
            return Channel.GetReportPageWithId(authTicket, reportId, templatePageId);
        }

        public ServiceOperationResult<IItemMetadata> GetItemMetaData(string authTicket, int reportId, int itemId)
        {
            return Channel.GetItemMetaData(authTicket, reportId, itemId);
        }

        public ServiceOperationResult<IItemMetadata[]> ListPageItemsData(string authTicket, int reportId, int pageId)
        {
            return Channel.ListPageItemsData(authTicket, reportId, pageId);
        }

        public ServiceOperationResult<GroupedResult<ReportMetaData>[]> Search(string authTicket, string searchTerm)
        {
            return Channel.Search(authTicket, searchTerm);
        }

        public ServiceOperationResult<ReportMetaData> GetReportWithGuid(string authTicket, Guid reportGuid)
        {
            return Channel.GetReportWithGuid(authTicket, reportGuid);
        }

        public ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListReportsForSurvey(string authTicket, int responseTemplateId, int pageNumber, int resultsPerPage, string nameFilter, string sortField, bool sortAscending)
        {
            return Channel.ListReportsForSurvey(authTicket, responseTemplateId, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending);
        }

        public ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListReportsByPeriod(string authTicket, int period, string dateFieldName, int pageNumber, int resultsPerPage, string nameFilter, string sortField, bool sortAscending)
        {
            return Channel.ListReportsByPeriod(authTicket, period, dateFieldName, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending);
        }

        public ServiceOperationResult<PagedListResult<ReportFilterData[]>> ListReportFiltersForSurvey(string authTicket, int responseTemplateId, int pageNumber, int resultsPerPage, string nameFilter, string sortField, bool sortAscending)
        {
            return Channel.ListReportFiltersForSurvey(authTicket, responseTemplateId, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending);
        }

        public ServiceOperationResult<ReportMetaData[]> ListRecentReportsForSurvey(string authTicket, int responseTemplateId, int count)
        {
            return Channel.ListRecentReportsForSurvey(authTicket, responseTemplateId, count);
        }

        public ServiceOperationResult<bool> DeleteReport(string authTicket, int reportID)
        {
            return Channel.DeleteReport(authTicket, reportID);
        }

        public ServiceOperationResult<bool> DeleteFilters(string authTicket, int surveyId, int[] Ids)
        {
            return Channel.DeleteFilters(authTicket, surveyId, Ids);
        }

        public ServiceOperationResult<int> CopyReport(string authTicket, int reportID, string newName)
        {
            return Channel.CopyReport(authTicket, reportID, newName);
        }

        public ServiceOperationResult<object> DeleteItem(string authTicket, int reportId, int itemId)
        {
            return Channel.DeleteItem(authTicket, reportId, itemId);
        }

        public ServiceOperationResult<object> DeletePage(string authTicket, int reportId, int pageId)
        {
            return Channel.DeletePage(authTicket, reportId, pageId);
        }

        public ServiceOperationResult<object> MoveReportItem(string authTicket, int reportId, int itemId, int? newPageId, int position)
        {
            return Channel.MoveReportItem(authTicket, reportId, itemId, newPageId, position);
        }

        public ServiceOperationResult<object> MoveReportPage(string authTicket, int reportId, int pageId, int position)
        {
            return Channel.MoveReportPage(authTicket, reportId, pageId, position);
        }

        public ServiceOperationResult<object> AddReportPage(string authTicket, int reportId)
        {
            return Channel.AddReportPage(authTicket, reportId);
        }

        public ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListAvailableReports(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListAvailableReports(authTicket, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }
    }
}