using System;

namespace Checkbox.Styles
{
    /// <summary>
    /// Mobile style
    /// </summary>
    [Serializable]
    public class MobileStyle
    {
        /// <summary>
        /// 
        /// </summary>
        public int StyleId { set; get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Name { set; get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string CssUrl { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDefault { set; get; }
    }
}
