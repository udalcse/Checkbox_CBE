/****************************************************************************
 * IMultiLanguageControl.cs													*
 * Contains the definition of the IMultiLanguageControl interface.			*
 ****************************************************************************/
using System;

namespace Checkbox.Web.UI.Controls
{
	/// <summary>
	/// Interface for multi language controls
	/// </summary>
	public interface IMultiLanguageControl
	{
        /// <summary>
        /// Get/Set the TextID for the control
        /// </summary>
		string TextId {get; set;}

        /// <summary>
        /// Get/Set the textID for teh tooltip
        /// </summary>
		string ToolTipTextId {get; set;}

        /// <summary>
        /// Get/set the language code
        /// </summary>
        string LanguageCode { get; set; }
	}
}
