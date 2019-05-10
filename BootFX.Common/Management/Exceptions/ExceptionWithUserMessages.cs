// BootFX - Application framework for .NET applications
// 
// File: ExceptionWithUserMessages.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Security;

namespace BootFX.Common
{
	/// <summary>
	/// Raised when an exception with "UI friendly" messages is raised.
	/// </summary>
	[Serializable()]
	public class ExceptionWithUserMessages : ApplicationException, IUserMessages
	{
		/// <summary>
		/// Private field to support <see cref="UserMessages"/> property.
		/// </summary>
		private StringCollection _userMessages = new StringCollection();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		// TODO: Add a default message here.
		internal ExceptionWithUserMessages() 
			: base()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExceptionWithUserMessages(string message)
			: this(message, message)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public ExceptionWithUserMessages(string message, string userMessage)
			: base(MangleMessage(message, userMessage))
		{
			InitializeMessages(userMessage);
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public ExceptionWithUserMessages(string message, string[] userMessages)
			: base(MangleMessage(message, userMessages))
		{
			InitializeMessages(userMessages);
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public ExceptionWithUserMessages(string message, Exception innerException) 
			: this(message, message, innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExceptionWithUserMessages(string message, string userMessage, Exception innerException) 
			: base(MangleMessage(message, userMessage), innerException)
		{
			InitializeMessages(userMessage);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExceptionWithUserMessages(string message, string[] userMessages, Exception innerException) 
			: base(MangleMessage(message, userMessages), innerException)
		{
			InitializeMessages(userMessages);
		}

		private static string MangleMessage(string message, string userMessage)
		{
			return MangleMessage(message, new string[] { MangleSingleMessage(userMessage) });
		}

		private static string MangleMessage(string message, string[] userMessages)
		{
			// if we didn't get any, make something up.
			if(userMessages == null || userMessages.Length == 0)
				userMessages = new string[] { MangleSingleMessage(null) };

			// builder...
			StringBuilder builder = new StringBuilder();
			builder.Append(message);
			builder.Append("\r\nUser messages: ");
			for(int index = 0; index < userMessages.Length; index++)
			{
				builder.Append("\r\n\t");
				builder.Append(index + 1);
				builder.Append(": ");
				builder.Append(userMessages[index]);
			}

			// return...
			return builder.ToString();
		}

		private static string MangleSingleMessage(string userMessage)
		{
			if(userMessage == null || userMessage.Length == 0)
				return "???";
			else
				return userMessage;
		}

		private void InitializeMessages(string userMessage)
		{
			this.InitializeMessages(new string[] { MangleSingleMessage(userMessage) });
		}

		private void InitializeMessages(string[] userMessages)
		{
			if(userMessages == null || userMessages.Length == 0)
				userMessages = new string[] { MangleSingleMessage(null) };

			// set...
			_userMessages = new StringCollection();
			_userMessages.AddRange(userMessages);
		}

		/// <summary>
		/// Gets the usermessages.
		/// </summary>
		private StringCollection UserMessages
		{
			get
			{
				return _userMessages;
			}
		}

		/// <summary> 
		/// Deserialization constructor.
		/// </summary> 
		protected ExceptionWithUserMessages(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			_userMessages = (StringCollection)info.GetValue("_userMessages", typeof(StringCollection));
		}

		/// <summary> 
		/// Provides data to serialization.
		/// </summary> 
        [SecurityCritical]
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// base...
			base.GetObjectData(info, context);

			// add code here to stream extra state into 'info'.
			// remember to update the deserializtion constructor.
			info.AddValue("_userMessages", _userMessages);
		}

		/// <summary>
		/// Gets the user messages out.
		/// </summary>
		/// <returns></returns>
		public string[] GetUserMessages()
		{
			if(UserMessages == null)
				throw new InvalidOperationException("UserMessages is null.");

			// create...
			string[] results = new string[this.UserMessages.Count];
			this.UserMessages.CopyTo(results, 0);

			// return...
			return results;
		}

		/// <summary>
		/// Gets the user messages from the given exception.
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string[] GetUserMessages(Exception ex)
		{
			if(ex == null)
				return new string[] { "No error messages were returned." };

			// walk...
			StringCollection messages = new StringCollection();
			Exception scan = ex;
			while(scan != null)
			{
				// are we?
				if(scan is IUserMessages)
					messages.AddRange(((IUserMessages)scan).GetUserMessages());

				// next...
				scan = scan.InnerException;
			}

			// do we have any? if not build one string that expresses the stack...
			if(messages.Count == 0)
			{
				scan = ex;
				while(scan != null)
				{
					StringBuilder builder = new StringBuilder();
					for(int index = 0; index < messages.Count; index++)
						builder.Append("    ");

					// next...
					builder.Append(scan.Message);

					// add...
					messages.Add(builder.ToString());

					// next...
					scan = scan.InnerException;
				}
			}

			// return...
			string[] results = new string[messages.Count];
			for(int index = 0; index < messages.Count; index++)
				results[index] = messages[index];

			// return...
			return results;
		}
	}
}
