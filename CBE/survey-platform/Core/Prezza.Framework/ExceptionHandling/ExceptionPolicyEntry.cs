//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling.Configuration;

namespace Prezza.Framework.ExceptionHandling
{
	internal sealed class ExceptionPolicyEntry
	{
		private ExceptionHandlerFactory factory;
		private PostHandlingAction postHandlingAction;
		private string policyName;
		private string typeDataName;

		internal ExceptionPolicyEntry(string policyName, ExceptionTypeData typeData, ConfigurationBase config)
		{
			this.policyName = policyName;
			this.postHandlingAction = typeData.PostHandlingAction;
			this.typeDataName = typeData.Name;
			this.factory = new ExceptionHandlerFactory(config);
		}

		internal bool Handle(ref Exception ex)
		{
			Guid handlingInstanceId;

			if(ex.InnerException != null && ex.InnerException is BaseException)
				handlingInstanceId = ((BaseException)ex.InnerException).ExceptionId;
			else if(ex is BaseException)
				handlingInstanceId = ((BaseException)ex).ExceptionId;
			else
				handlingInstanceId = Guid.NewGuid();

			Exception chainException = ExecuteHandlerChain(ex, handlingInstanceId);

			return RethrowRecommended(ref chainException, ref ex);
		}


		private void IntentionalRethrow(Exception chainException, Exception originalException)
		{
			if(chainException != null)
			{
				throw chainException;
			}
			else
			{
				Exception wrappedException = new ExceptionHandlingException("Exception to handle was null.");
				ExceptionUtility.LogHandlingException(policyName, wrappedException, chainException, originalException);
				throw wrappedException;
			}
		}

		private bool RethrowRecommended(ref Exception chainException, ref Exception originalException)
		{
			switch(postHandlingAction)
			{
				case PostHandlingAction.None:
					return false;
				case PostHandlingAction.ThrowNewException:
					IntentionalRethrow(chainException, originalException);
					return true;
				case PostHandlingAction.NotifyRethrow:
					originalException = chainException;
					return true;
				default:
					return true;
			}
		}

		private Exception ExecuteHandlerChain(Exception ex, Guid handlingInstanceId)
		{
			string lastHandlerName = string.Empty;
			Exception originalException = ex;

			try
			{
				IExceptionHandler[] handlers = factory.CreateExceptionHandlers(policyName, typeDataName);
				for(int i = 0; i  < handlers.Length; i++)
				{
					lastHandlerName = handlers[i].GetType().Name;
					ex = handlers[i].HandleException(ex, policyName, handlingInstanceId);
				}
			}
			catch(Exception)
			{
				ExceptionUtility.LogHandlingException(policyName, 
													  new ExceptionHandlingException("Unable to handle exception: " + lastHandlerName),
													  ex,
													  originalException);
				throw new ExceptionHandlingException("Unable to handle exception: " + lastHandlerName);
			}

			return ex;
		}
	}
}
