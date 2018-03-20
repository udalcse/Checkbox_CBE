using System;
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;

namespace Checkbox.Wcf.Services.BehaviorExtensions
{
    internal class MessageFormatter : IDispatchMessageFormatter
    {
        private readonly JsonSerializer serializer = null;

        internal MessageFormatter()
        {
            serializer = new JsonSerializer();
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            var jtw = new JsonTextWriter(streamWriter);
            
            serializer.Serialize(jtw, result);
            jtw.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            
            return WebOperationContext.Current.CreateStreamResponse(stream, "application/json");
        }
    }
}
