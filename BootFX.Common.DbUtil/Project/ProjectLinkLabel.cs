// BootFX - Application framework for .NET applications
// 
// File: ProjectLinkLabel.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Defines an instance of <c>ProjectLinkLabel</c>.
	/// </summary>
	internal class ProjectLinkLabel : LinkLabel
	{
		/// <summary>
		/// Private field to support <see cref="FilePath"/> property.
		/// </summary>
		private string _filePath;

		internal ProjectLinkLabel()
		{
		}

		/// <summary>
		/// Gets the filepath.
		/// </summary>
		internal string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;

				// update...
				this.RefreshText();
			}
		}

		private void RefreshText()
		{
			if(this.FilePath != null && this.FilePath.Length > 0 && this.Width > 0)
			{
				// get the parts...
				string usePath = this.FilePath;
				bool startsUnc = false;
				if(usePath.StartsWith("\\"))
				{
					startsUnc = true;
					usePath = usePath.Substring(2);
				}

				// split...
				string[] parts = usePath.Split('\\');

				// many?
				if(parts.Length > 2)
				{
					Console.WriteLine(startsUnc);

					// tries...
					int tries = parts.Length - 2;
					for(int index = tries; index >= 0; index--)
					{
						// build...
						StringBuilder builder = new StringBuilder();
						builder.Append(parts[0]);
						builder.Append("\\");
						for(int i = 1; i <= index; i++)
						{
							builder.Append(parts[i]);
							builder.Append("\\");
						}
						if(index < tries)
							builder.Append("...\\");
						builder.Append(parts[parts.Length - 1]);

						// test...
						string asString = builder.ToString();

						// work it...
						using(Image image = new Bitmap(1,1, PixelFormat.Format32bppPArgb))
						using(Graphics graphics = Graphics.FromImage(image))
						{
							// measure...
							SizeF size = graphics.MeasureString(asString, this.Font);
							if((int)size.Width <= this.Width)
							{
								this.Text = asString;
								return;
							}
						}
					}
				}
				else
					this.Text = this.FilePath;
			}
			else
				this.Text = string.Empty;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			this.RefreshText();
		}
	}
}
