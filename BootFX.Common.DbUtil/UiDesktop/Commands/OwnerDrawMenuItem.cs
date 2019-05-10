// BootFX - Application framework for .NET applications
// 
// File: OwnerDrawMenuItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Collections;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>OwnerDrawMenuItem</c>.
	/// </summary>
	public class ImageMenuItem : MenuItem
	{
		private const int ImageWidth = 16;
		private const int ImageHeight = 16;
		private const int Spacing = 3;

		/// <summary>
		/// Private field to support <c>ImageResourceAssembly</c> property.
		/// </summary>
		private Assembly _imageResourceAssembly;

		/// <summary>
		/// Private field to support <c>ImageResourceName</c> property.
		/// </summary>
		private string _imageResourceName;
		
		/// <summary>
		/// Private field to support <c>Image</c> property.
		/// </summary>
		private Image _image;
		
		public ImageMenuItem(string text)
		{
			// set...
			this.Text = text;

			// mbr - 28-03-2006 - added.			
			this.OwnerDraw = true;
		}

        ~ImageMenuItem()
        {
            this.Dispose();
        }
		
		// mbr - 28-03-2006 - added.		
		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			base.OnMeasureItem (e);

			// get...
			using(Font font = this.GetFont())
			{
				// measure the actual string.
				SizeF actual = e.Graphics.MeasureString(this.Text, font, 1000, this.GetStringFormat());

				// set...
				e.ItemWidth = (int)actual.Width + ImageWidth + (3 * Spacing);
				e.ItemHeight = ImageHeight + (2 * Spacing);
			}
		}

		private StringFormat GetStringFormat()
		{
			StringFormat format = new StringFormat();
			format.HotkeyPrefix = HotkeyPrefix.Show;

			// return...
			return format;
		}

		// mbr - 28-03-2006 - added.		
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem (e);

			// ok...
			using(Brush backgroundBrush = this.GetBackgroundBrush(e.State))	
			using(Brush textBrush = this.GetTextBrush(e.State))
			using(Font font = this.GetFont())
			{
				// fill...
				e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

				// rects...
				Rectangle imageRect = new Rectangle(e.Bounds.Left + Spacing, e.Bounds.Top + Spacing, ImageWidth, ImageHeight);
				Rectangle textRect = new Rectangle(imageRect.Right + Spacing, e.Bounds.Top + Spacing, e.Bounds.Width - (3 * Spacing) - ImageWidth, e.Bounds.Height - (2 * Spacing));

				// draw...
				if(this.Image != null)
					e.Graphics.DrawImage(this.Image, imageRect);

				// text...
				e.Graphics.DrawString(this.Text, font, textBrush, textRect, this.GetStringFormat());
			}
		}

		/// <summary>
		/// Gets the font.
		/// </summary>
		/// <returns></returns>
		private Font GetFont()
		{
			return new Font("Tahoma", 8, GraphicsUnit.Point);
		}

		/// <summary>
		/// Gets the background brush.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		private Brush GetBackgroundBrush(DrawItemState state)
		{
			if((int)(state & DrawItemState.Selected) != 0)
				return new SolidBrush(SystemColors.Highlight);
			else
				return new SolidBrush(SystemColors.Window);
		}

		/// <summary>
		/// Gets the background brush.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		private Brush GetTextBrush(DrawItemState state)
		{
			if((int)(state & DrawItemState.Selected) != 0)
				return new SolidBrush(SystemColors.HighlightText);
			else
				return new SolidBrush(SystemColors.WindowText);
		}

		/// <summary>
		/// Gets or sets the image
		/// </summary>
		public virtual Image Image
		{
			get
			{
				if(_image == null && this.HasImage)
				{
					_image = DesktopResourceHelper.GetImage(this.ImageResourceAssembly, this.ImageResourceName);
					if(_image == null)
						throw new InvalidOperationException("_image is null.");
				}
				return _image;
			}
		}

		private bool HasImage
		{
			get
			{
				if(this.ImageResourceName != null && this.ImageResourceName.Length > 0)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets or sets the imageresourcename
		/// </summary>
		public string ImageResourceName
		{
			get
			{
				return _imageResourceName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _imageResourceName)
				{
					// set the value...
					_imageResourceName = value;
					_image = null;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the imageresourceassembly
		/// </summary>
		public Assembly ImageResourceAssembly
		{
			get
			{
				return _imageResourceAssembly;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _imageResourceAssembly)
				{
					// set the value...
					_imageResourceAssembly = value;
					_image = null;
				}
			}
		}

        protected override void Dispose(bool disposing)
        {
            // clear the image...
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }

            // mbr - 2009-07-09 - added - if called from the finalizer on application
            // teardown, this can blow-up...
            try
            {
                base.Dispose(disposing);
            }
            catch
            {
                // no-op...
            }
        }
    }
}
