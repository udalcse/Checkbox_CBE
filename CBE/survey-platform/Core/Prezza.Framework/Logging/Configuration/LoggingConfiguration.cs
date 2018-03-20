//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Xml;
using System.ComponentModel;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Configuration
{
	/// <summary>
	/// Container for all logging configuration information.
	/// </summary>
	public class LoggingConfiguration : ConfigurationBase, IXmlConfigurationBase
	{
		/// <summary>
		/// Collection of logging category filters.
		/// </summary>
		private CategoryFilterDataCollection categoryFilters;

		/// <summary>
		/// Collection of logging distribution strategies.
		/// </summary>
		private DistributionStrategyDataCollection distributionStrategies;

		/// <summary>
		/// Collection of log formatters.
		/// </summary>
		private FormatterDataCollection formatters;

		/// <summary>
		/// Collection of logging sinks.
		/// </summary>
		private SinkDataCollection sinks;

		/// <summary>
		/// Collection of logging categories.
		/// </summary>
		private CategoryDataCollection categories;

		/// <summary>
		/// Indicates whether trace-level information will be logged in addition to normal log messages.  Note that trace logging
		/// is currently not supported.
		/// </summary>
		private bool tracingEnabled;

		/// <summary>
		/// Indicates whether log messages will be distributed according to the specified distribution strategies.
		/// </summary>
		private bool loggingEnabled;

		/// <summary>
		/// The name of log distribution strategy.
		/// </summary>
		private string distributionStrategy;

		/// <summary>
		/// Indicates the minimum priority a log entry must have before it is distributed.
		/// </summary>
		private int minimumPriority;

		/// <summary>
		/// Active category filter mode.
		/// </summary>
		private CategoryFilterMode categoryFilterMode;

		/// <summary>
		/// Default formatter for log entries.
		/// </summary>
		private string defaultFormatter;

		/// <summary>
		/// Default category for log entries.
		/// </summary>
		private string defaultCategory;

		/// <summary>
		/// Constructor.
		/// </summary>
		public LoggingConfiguration() : this(string.Empty)
		{
		}
		

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the logging configuration.</param>
		public LoggingConfiguration(string name) : base(name)
		{
			this.categoryFilters = new CategoryFilterDataCollection();
			this.distributionStrategies = new DistributionStrategyDataCollection();
			this.categories = new CategoryDataCollection();
			this.formatters = new FormatterDataCollection();
			this.sinks = new SinkDataCollection();
		}

		/// <summary>
		/// Get the <see cref="CategoryData"/> object for the specified category.
		/// </summary>
		/// <param name="category">Name of the category.</param>
		/// <returns>Specified <see cref="CategoryData"/> object.</returns>
		public CategoryData GetCategoryData(string category)
		{
			ArgumentValidation.CheckForEmptyString(category, "category");

			return categories[category];
		}

		/// <summary>
		/// Get the default <see cref="CategoryData"/> object.
		/// </summary>
		/// <returns>Default <see cref="CategoryData"/> object.</returns>
		public CategoryData GetDefaultCategoryData()
		{
			if(defaultCategory == null)
			{
				throw new Exception("Attempt to get the default category data is impossible because defaultCategory is null.");
			}

			return GetCategoryData(defaultCategory);
		}

		/// <summary>
		/// Get the <see cref="FormatterData"/> object for the specified formatter.
		/// </summary>
		/// <param name="formatter">Name of the formatter.</param>
		/// <returns>Specified <see cref="CategoryData"/> object.</returns>
		public FormatterData GetFormatterData(string formatter)
		{
			ArgumentValidation.CheckForEmptyString(formatter, "formatter");

			return formatters[formatter];
		}

		/// <summary>
		/// Get the default <see cref="FormatterData"/> object.
		/// </summary>
		/// <returns>Default <see cref="FormatterData"/> object.</returns>
		public FormatterData GetDefaultFormatterData()
		{
			if(defaultFormatter == null)
			{
				throw new Exception("Attempt to get the default formatter data is impossible because defaultFormatter is null.");
			}

			return GetFormatterData(defaultFormatter);
		}

		/// <summary>
		/// Get the <see cref="DistributionStrategyData"/> object for the specified distribution strategy.
		/// </summary>
		/// <param name="distributionStrategy">Name of the distribution strategy.</param>
		/// <returns>Specified <see cref="DistributionStrategyData"/> object.</returns>
		public DistributionStrategyData GetDistributionStrategyData(string distributionStrategy)
		{
			ArgumentValidation.CheckForEmptyString(distributionStrategy, "distributionStrategy");

			return distributionStrategies[distributionStrategy];
		}

		/// <summary>
		/// Get the <see cref="SinkData"/> object for the specified log sink.
		/// </summary>
		/// <param name="sink">Name of the log sink.</param>
		/// <returns>Specified <see cref="SinkData"/> object.</returns>
		public SinkData GetSinkData(string sink)
		{
			ArgumentValidation.CheckForEmptyString(sink, "sink");

			return sinks[sink];
		}

		#region IXmlConfigurationBase Members

		/// <summary>
		/// Load the logging configuration from the specified Xml node.
		/// </summary>
		/// <param name="node"><see cref="XmlNode"/> containing the logging configuration.</param>
		public void LoadFromXml(System.Xml.XmlNode node)
		{
			ArgumentValidation.CheckForNullReference(node, "node");

			//Load basic logging settings
			XmlNode loggingSettings = node.SelectSingleNode("/loggingConfiguration/settings");
			loggingEnabled = XmlUtility.GetAttributeBool(loggingSettings.SelectSingleNode("setting[@name='loggingEnabled']"), "value", false);
			tracingEnabled = XmlUtility.GetAttributeBool(loggingSettings.SelectSingleNode("setting[@name='tracingEnabled']"), "value", false);
			//Only required if logging is enabled or tracing is enabled
			distributionStrategy = XmlUtility.GetAttributeText(loggingSettings.SelectSingleNode("setting[@name='distributionStrategy']"), "value", (loggingEnabled || tracingEnabled));
			minimumPriority = XmlUtility.GetAttributeInt(loggingSettings.SelectSingleNode("setting[@name='minimumPriority']"), "value", (loggingEnabled || tracingEnabled));
			
			string stringFilterMode = XmlUtility.GetAttributeText(loggingSettings.SelectSingleNode("setting[@name='categoryFilterMode']"), "value", (loggingEnabled || tracingEnabled));

			if(stringFilterMode != string.Empty)
			{
				categoryFilterMode = (CategoryFilterMode)CategoryFilterMode.Parse(typeof(CategoryFilterMode), stringFilterMode, true);
			}

			//Category Filter Data
			categoryFilters.Clear();
			XmlNodeList categoryFilterList = node.SelectNodes("/loggingConfiguration/categoryFilters/categoryFilter");

			foreach(XmlNode categoryFilterNode in categoryFilterList)
			{
				string name = XmlUtility.GetAttributeText(categoryFilterNode, "name", true);
				CategoryFilterData data = new CategoryFilterData(name);
				categoryFilters.Add(data);
			}

			//Distribution Strategies
			distributionStrategies.Clear();
			XmlNodeList distributionStrategiesList = node.SelectNodes("/loggingConfiguration/distributionStrategies/distributionStrategy");

			//NOTE:  Only InProcDistributionStrategy is supported at this time!
			foreach(XmlNode distributionStrategyNode in distributionStrategiesList)
			{
				string name = XmlUtility.GetAttributeText(distributionStrategyNode, "name", true);
				//DistributionStrategyData data = (DistributionStrategyData)ConfigurationSettings.GetConfig(name);
				InProcDistributionStrategyData data = new InProcDistributionStrategyData(name);
				data.LoggingConfiguration = this;
				distributionStrategies.Add(data);
			}

			//Categories
			categories.Clear();
			XmlNodeList categoriesList = node.SelectNodes("/loggingConfiguration/categories/category");

			foreach(XmlNode categoryNode in categoriesList)
			{
				string name = XmlUtility.GetAttributeText(categoryNode, "name", true);
				CategoryData categoryData = new CategoryData(name);

				if(XmlUtility.GetAttributeBool(categoryNode, "default", false))
				{
					defaultCategory = name;
				}

				//Destination Data
				XmlNodeList destinationList = categoryNode.SelectNodes("destinations/destination");

				foreach(XmlNode destinationNode in destinationList)
				{
					DestinationData destinationData = new DestinationData();
					destinationData.Name = XmlUtility.GetAttributeText(destinationNode, "name", true);
					destinationData.Sink = XmlUtility.GetAttributeText(destinationNode, "sink", true);
					destinationData.Format = XmlUtility.GetAttributeText(destinationNode, "format", true);

					categoryData.DestinationDataCollection.Add(destinationData);
				}

				categories.Add(categoryData);
			}

			//Formatters
			formatters.Clear();
			XmlNodeList formattersList = node.SelectNodes("/loggingConfiguration/formatters/formatter");

			foreach(XmlNode formatterNode in formattersList)
			{
				string name = XmlUtility.GetAttributeText(formatterNode, "name", true);
				string configDataType = XmlUtility.GetAttributeText(formatterNode, "configDataType", true);
				string filePath = XmlUtility.GetAttributeText(formatterNode, "filePath", true);

				object[] extraParams = {name, string.Empty};
				FormatterData data = (FormatterData)ConfigurationManager.GetConfiguration(filePath, configDataType, extraParams);
				formatters.Add(data);

				if(XmlUtility.GetAttributeBool(formatterNode, "default"))
				{
					defaultFormatter = name;
				}
			}

			//Sinks
			sinks.Clear();
			XmlNodeList sinksList = node.SelectNodes("/loggingConfiguration/sinks/sink");

			foreach(XmlNode sinkNode in sinksList)
			{
				string name = XmlUtility.GetAttributeText(sinkNode, "name", true);
				string configDataType = XmlUtility.GetAttributeText(sinkNode, "configDataType", true);
				string filePath = XmlUtility.GetAttributeText(sinkNode, "filePath", true);

				object[] extraParams = {name};
				SinkData data = (SinkData)ConfigurationManager.GetConfiguration(filePath, configDataType, extraParams);
				sinks.Add(data);
			}
		}

		#endregion

		/// <summary>
		/// Indicates whether log entries will be distributed or not.
		/// </summary>
		public bool LoggingEnabled
		{
			get{return loggingEnabled;}
			set{loggingEnabled = value;}
		}

		/// <summary>
		/// Indicates whether tracing entries will be distributed.  NOTE:  This feature is currently not supported
		/// by the framework.
		/// </summary>
		public bool TracingEnabled
		{
			get{return tracingEnabled;}
			set{tracingEnabled = value;}
		}

		/// <summary>
		/// Get/Set the <see cref="CategoryFilterMode"/>.
		/// </summary>
		public CategoryFilterMode CategoryFilterMode
		{
			get{return categoryFilterMode;}
			set{categoryFilterMode = value;}
		}

		/// <summary>
		/// Get/Set the name of the <see cref="Prezza.Framework.Logging.Distributor.ILogDistributionStrategy"/>.
		/// </summary>
		public string DistributionStrategy
		{
			get{return distributionStrategy;}
			set{distributionStrategy = value;}
		}

		/// <summary>
		/// Get/Set the minimum priority for items to log.
		/// </summary>
		public int MinimumPriority
		{
			get{return minimumPriority;}
			set{minimumPriority = value;}
		}

		/// <summary>
		/// Get/Set the <see cref="CategoryFilterDataCollection"/> of <see cref="CategoryFilterData"/> objects.
		/// </summary>
		public CategoryFilterDataCollection CategoryFilters
		{
			get{return categoryFilters;}
			set{categoryFilters = value;}
		}

		/// <summary>
		/// Get/Set the <see cref="DistributionStrategyDataCollection"/> of <see cref="DistributionStrategyData"/> objects.
		/// </summary>
		public DistributionStrategyDataCollection DistributionStrategies
		{
			get{return distributionStrategies;}
			set{distributionStrategies = value;}
		}

		/// <summary>
		/// Get/Set the name of the default log entry formatter.
		/// </summary>
		public string DefaultFormatter
		{
			get{return defaultFormatter;}
			set{defaultFormatter = value;}
		}

		/// <summary>
		/// Get/Set the name of the default log entry category.
		/// </summary>
		public string DefaultCategory
		{
			get{return defaultCategory;}
			set{defaultCategory = value;}
		}
	}
}
