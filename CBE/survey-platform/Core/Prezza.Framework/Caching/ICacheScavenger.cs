//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Cache scavenger interface.
	/// </summary>
	internal interface ICacheScavenger
	{
		/// <summary>
		/// Start the cache scavenging task.
		/// </summary>
		void StartScavenging();
	}
}
