using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface for data transfer objects consumed by Checkbox renderers
    /// </summary>
    public interface IItemDataTransferObject
    {
        /// <summary>
        /// ID of item
        /// </summary>
        int ItemId { get; }

        /// <summary>
        /// Type name for item
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Item metadata (i.e. configuration data that is not specific to an instance
        /// of an item.
        /// </summary>
        SimpleNameValueCollection Metadata { get; }

        /// <summary>
        /// Instance data specfic to the instance of an item.
        /// </summary>
        SimpleNameValueCollection InstanceData { get; }

        /// <summary>
        /// Additional data associated with an item. Depending on use of WCF/Web services and such,
        /// this data should be serializable.
        /// </summary>
        object AdditionalData { get; }
    }
}
