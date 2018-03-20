//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Queue message interface for internal framework us.
	/// </summary>
	internal interface IQueueMessage
	{
		/// <summary>
		/// Run the message.
		/// </summary>
		void Run();
	}
}
