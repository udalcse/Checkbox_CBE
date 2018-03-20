using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Checkbox.Wcf.Services.BehaviorExtensions
{
    public class WebHttpJsonBehavior : WebHttpBehavior
    {
        protected override IDispatchMessageFormatter GetReplyDispatchFormatter(
            OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            return new MessageFormatter();
        }
    }
}
