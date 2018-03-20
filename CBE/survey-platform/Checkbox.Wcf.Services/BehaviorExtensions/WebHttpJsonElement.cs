using System;
using System.ServiceModel.Configuration;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.BehaviorExtensions
{
    public class WebHttpJsonElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get
            {
                return typeof(WebHttpJsonBehavior);
            }
        }

        protected override object CreateBehavior()
        {
            var behavior = new WebHttpJsonBehavior
                               {
                                   DefaultBodyStyle = WebMessageBodyStyle.WrappedRequest,
                                   DefaultOutgoingResponseFormat = WebMessageFormat.Json,
                                   AutomaticFormatSelectionEnabled = false
                               };
            return behavior;
        }
    }
}
