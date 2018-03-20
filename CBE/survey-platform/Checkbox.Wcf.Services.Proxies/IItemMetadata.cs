namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface for meta data associated with an item.  An item's IItemMetadata object is typically a container
    /// for transferring data over a service interface.  This data should contain full details of an item's
    /// configuration, unlike the simple metadata in IItemDataTransferObject which is a simplified version required
    /// for rendering an item.
    /// </summary>
    public interface IItemMetadata
    {
        /// <summary>
        /// The id of item.
        /// </summary>
        int ItemId { get; }

        /// <summary>
        /// Type name for item.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Position of page that contains the item.
        /// </summary>
        int PagePosition { get; }

        /// <summary>
        /// Position of item within page containing it.
        /// </summary>
        int ItemPosition { get; }

        /// <summary>
        /// Collection of configuration properties associated with an item.
        /// </summary>
        SimpleNameValueCollection Properties { get; }

        /// <summary>
        /// Additional configuration data associated with an item, such as data that is too complex.
        /// </summary>
        object AdditionalData { get; }
    }
}
