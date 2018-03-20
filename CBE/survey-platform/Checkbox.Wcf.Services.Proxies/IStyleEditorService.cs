using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    [ServiceContract]
    public interface IStyleEditorService
    {
        /// <summary>
        /// Get data associated with a style.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<StyleMetaData> GetStyleData(string authTicket, int styleId);

        /// <summary>
        /// Get value of css property of a given css class
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="elementName"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> GetStyleElementProperty(string authTicket, int styleId, string elementName, string property);

        /// <summary>
        /// Get key/value pairs of style properties by css class name
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SimpleNameValueCollection> GetStyleElementProperties(string authTicket, int styleId, string elementName);

        /// <summary>
        /// Save style template by assembling key/value pairs into css
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="languageCode"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> SaveFormStyle(string authTicket, int styleId, string languageCode, StyleData style);

        /// <summary>
        /// Update one single setting of style Template
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="styleId"></param>
        /// <param name="settingName"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string> UpdateStyleTemplateSetting(string authToken, int styleId, string settingName,
                                                                  string newValue);
    }
}
