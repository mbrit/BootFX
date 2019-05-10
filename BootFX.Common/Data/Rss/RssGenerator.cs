// BootFX - Application framework for .NET applications
// 
// File: RssGenerator.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Collections;

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Defines a class that can generate RSS.
	/// </summary>
	public abstract class RssGenerator
	{
		/// <summary>
		/// Defines the default generator name.
		/// </summary>
		public static string DefaultGeneratorName = "BootFX RSS Generator " + typeof(RssGenerator).Assembly.GetName().Version.ToString();

		/// <summary>
		/// Private field to support <c>Title</c> property.
		/// </summary>
		private string _title;

		/// <summary>
		/// Private field to support <c>Description</c> property.
		/// </summary>
		private string _description;

		/// <summary>
		/// Private field to support <c>Generator</c> property.
		/// </summary>
		private string _generator = DefaultGeneratorName;

		/// <summary>
		/// Private field to support <c>MainUrl</c> property.
		/// </summary>
		private string _mainUrl;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected RssGenerator(string title, string mainUrl)
		{
			_title = title;
			_mainUrl = mainUrl;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected RssGenerator()
		{
		}

		/// <summary>
		/// Gets or sets the mainurl
		/// </summary>
		[Obsolete("Set this property on a channel.")]
		public string MainUrl
		{
			get
			{
				return _mainUrl;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _mainUrl)
				{
					// set the value...
					_mainUrl = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the generator
		/// </summary>
		[Obsolete("Set this property on a channel.")]
		public string Generator
		{
			get
			{
				return _generator;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _generator)
				{
					// set the value...
					_generator = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the description
		/// </summary>
		[Obsolete("Set this property on a channel.")]
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _description)
				{
					// set the value...
					_description = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the title
		/// </summary>
		[Obsolete("Set this property on a channel.")]
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _title)
				{
					// set the value...
					_title = value;
				}
			}
		}

		[Obsolete("Use an overload the supplies a channel, not items.")]
		public XmlDocument Generate(RssItemCollection items)
		{
			return this.Generate(this.CreateChannelFromLegacySettings(items));
		}

		/// <summary>
		/// Generates a document for the given items.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public abstract XmlDocument Generate(RssChannel channel);

		/// <summary>
		/// Saves the items to the given file.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="filename"></param>
		[Obsolete("Use an overload the supplies a channel, not items.")]
		public void Save(RssItemCollection items, string filePath)
		{
			if(items == null)
				throw new ArgumentNullException("items");

			// mbr - 18-08-2006 - defer...
			this.Save(this.CreateChannelFromLegacySettings(items), filePath);
		}

		private RssChannel CreateChannelFromLegacySettings(RssItemCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			
			// create...
			RssChannel channel = new RssChannel();
			channel.Title = _title;
			channel.Items.AddRange(items);

			// return...
			return channel;
		}

		public void Save(RssChannel channel, string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			
			// create...
			using(FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				Save(channel, stream);
		}

		/// <summary>
		/// Saves the items to the given stream.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="filename"></param>
		[Obsolete("Use an overload the supplies a channel, not items.")]
		public void Save(RssItemCollection items, Stream stream)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			if(stream == null)
				throw new ArgumentNullException("stream");

			// defer...
			this.Save(this.CreateChannelFromLegacySettings(items), stream);
		}

		public void Save(RssChannel channel, Stream stream)
		{
			if(channel == null)
				throw new ArgumentNullException("channel");			

			// get...
			XmlDocument document = this.Generate(channel);
			document.Save(stream);
		}

		/// <summary>
		/// Saves the items to the given text writer.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="filename"></param>
		[Obsolete("Use an overload the supplies a channel, not items.")]
		public void Save(RssItemCollection items, TextWriter writer)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			if(writer == null)
				throw new ArgumentNullException("writer");

			// defer...
			this.Save(this.CreateChannelFromLegacySettings(items), writer);
		}

		public void Save(RssChannel channel, TextWriter writer)
		{
			if(channel == null)
				throw new ArgumentNullException("channel");

			// get...
			XmlDocument document = this.Generate(channel);
			document.Save(writer);
		}
	}
}
