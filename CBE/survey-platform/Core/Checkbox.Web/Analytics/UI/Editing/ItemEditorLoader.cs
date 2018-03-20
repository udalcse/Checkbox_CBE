using System;
using System.Data;
using System.Reflection;

using Checkbox.Analytics;
using Checkbox.Analytics.Items;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Loads an AnalysisItemEditor
	/// </summary>
	public class ItemEditorLoader
	{
		private ItemEditorLoader()
		{
			
		}

		/// <summary>
		/// Loads an editor for an existing AnalysisItem
		/// </summary>
		/// <param name="item">The AnalysisItem to load the editor for</param>
		/// <returns>A base AnalysisItemEditor</returns>
		public static AnalysisItemEditor Load(AnalysisItem item)
		{
			//Get the class name and assembly name of the editor for the passed in analysisitem
			DataTable metaData = Checkbox.Analytics.AnalysisManager.GetAnalysisItemRegistration(item.ID);
			string className = metaData.Rows[0]["EditorClassName"].ToString();
			string assemblyName = metaData.Rows[0]["EditorAssemblyName"].ToString();

			AnalysisItemEditor editor = Load(className, assemblyName);
			editor.Initialize(item);
			return editor;
		}

		/// <summary>
		/// Loads an editor for a specified AnalysisItem type
		/// </summary>
		/// <param name="itemTypeID">The ID of the item type</param>
		/// <param name="analysisID">the id of the parent analysis</param>
		/// <returns>A base AnalysisItemEditor</returns>
		public static AnalysisItemEditor Load(int itemTypeID, int analysisID)
		{
			//Look up the class and assembly names for the editor of this item type
			DataTable metaData = Checkbox.Analytics.AnalysisManager.GetAnalysisItemTypeRegistration(itemTypeID);
			string className = metaData.Rows[0]["EditorClassName"].ToString();
			string assemblyName = metaData.Rows[0]["EditorAssemblyName"].ToString();

			AnalysisItemEditor editor = Load(className, assemblyName);
			editor.Initialize(analysisID);
			return editor;
		}

		/// <summary>
		/// Loads an editor specified by the class and assembly name
		/// </summary>
		/// <param name="className">The class name of the editor to load</param>
		/// <param name="assemblyName">The assembly that contains the editor class</param>
		/// <returns>A base AnalysisItemEditor</returns>
		public static AnalysisItemEditor Load(string className, string assemblyName)
		{
			//Use reflection to create the editor
			Assembly assembly = Assembly.Load(assemblyName);
			AnalysisItemEditor editor = (AnalysisItemEditor)Activator.CreateInstance(assembly.GetType(className));
			return editor;
		}
	}
}
