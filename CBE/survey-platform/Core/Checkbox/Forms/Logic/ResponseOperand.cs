using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Operand for comparing properties from the response.
    /// </summary>
    [Serializable]
    public class ResponseOperand : Operand
    {
        private readonly string _key;

        /// <summary>
        /// Response operand
        /// </summary>
        /// <param name="key"></param>
        public ResponseOperand(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Get value.
        /// </summary>
        protected override object GetValue(Response response)
        {
            ResponseProperties properties = new ResponseProperties();
            properties.Initialize(response);

            return properties.GetObjectValue(_key);
        }
    }
}
