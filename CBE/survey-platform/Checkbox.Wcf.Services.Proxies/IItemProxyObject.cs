namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface for data transfer objects consumed by Checkbox renderers.  Contains data associated with an 
    /// instance of an item (in a survey or report) along with a simplified version of any item meta data 
    /// necessary for a renderer to display an item.  Unlike IItemMetaData objects which typically contain
    /// all meta data information associated with an item, the metadata contained in these objects will
    /// generally be minimal for the sake of efficiency.
    /// </summary>
    public interface IItemProxyObject
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
        /// ID of template containing item
        /// </summary>
        int ParentTemplateId { get; }

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
        /// Collection of appearance-releated properties for the item
        /// </summary>
        SimpleNameValueCollection AppearanceData { get; }

        /// <summary>
        /// Additional data associated with an item. Depending on use of WCF/Web services and such,
        /// this data should be serializable.
        /// </summary>
        object AdditionalData { get; set; }
    }
}
