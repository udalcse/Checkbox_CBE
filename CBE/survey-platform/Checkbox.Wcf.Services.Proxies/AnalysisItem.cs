using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Generic data container for a survey item data.
    /// </summary>
    [DataContract]
    [Serializable]    
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.DetailsItem))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyMatrixItem))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponseItem))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.ItemProxy))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SimpleNameValueCollection))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SimpleNameValue[]))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SimpleNameValue))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponseItemAnswer[]))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponseItemAnswer))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponseItemOption[]))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponseItemOption))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyFileUploadItem))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.ResponseSessionState))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponseData[]))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponseData))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.ResponseSessionData))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponsePage))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponsePage[]))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.SurveyResponseItem[]))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.PagePostResult))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Checkbox.Wcf.Services.Proxies.ItemPostResult))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(int[]))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(string[]))]
    [KnownType(typeof(System.Data.DataSet))] //we have to announce that as object Data can be sent a DataSet
    public class AnalysisItem : ItemProxy
    {
        [DataMember]
        public object Data { get; set; }
    }
}
