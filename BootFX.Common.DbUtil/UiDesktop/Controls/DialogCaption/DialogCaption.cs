// BootFX - Application framework for .NET applications
// 
// File: DialogCaption.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using BootFX.Common;
using BootFX.Common.Management;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for DialogCaption.
	/// </summary>
	public class DialogCaption : Control
	{
		private string _caption = null;

		/// <summary>
		/// Private field to support <see cref="ImageRectangle"/> property.
		/// </summary>
		private Rectangle _imageRectangle;
		
		/// <summary>
		/// Private field to support <c>TextRectangle</c> property.
		/// </summary>
		private RectangleF _textRectangle;
		
		/// <summary>
		/// Private field to support <c>SharedImage</c> property.
		/// </summary>
		private static Image _sharedImage;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DialogCaption()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this.Dock = DockStyle.Top;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// DialogCaption
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Name = "DialogCaption";
			this.Size = new System.Drawing.Size(420, 52);

		}
		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			if(this.DesignMode)
				return;

			// image...
			Image image = this.ResolvedImage;
			if(image == null)
				e.Graphics.DrawIconUnstretched(SystemIcons.Information, this.ImageRectangle);
			else
				e.Graphics.DrawImageUnscaled(image, this.ImageRectangle);

			// text...
			StringFormat format = new StringFormat();
			format.LineAlignment = StringAlignment.Center;
			using(Font font = new Font("Tahoma", 10, FontStyle.Bold, GraphicsUnit.Point))
				e.Graphics.DrawString(this.Caption, font, Brushes.Black, this.TextRectangle, format);

//			e.Graphics.DrawRectangle(Pens.Red, this.TextRectangle.Left, this.TextRectangle.Top, 
//				this.TextRectangle.Width, this.TextRectangle.Height);
//			e.Graphics.DrawRectangle(Pens.Green, this.ImageRectangle.Left, this.ImageRectangle.Top, 
//				this.ImageRectangle.Width, this.ImageRectangle.Height);

			// line...
			e.Graphics.DrawLine(Pens.Black, this.ClientRectangle.Left, this.ClientRectangle.Bottom - 1,
				this.ClientRectangle.Right, this.ClientRectangle.Bottom - 1);
		}

		/// <summary>
		/// Property Text (string)
		/// </summary>
		[Browsable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				_caption = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the sharedimage
		/// </summary>
		public static Image SharedImage
		{
			get
			{
				return _sharedImage;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _sharedImage)
				{
					// set the value...
					_sharedImage = value;
				}
			}
		}

		private Image ResolvedImage
		{
			get
			{
				if(SharedImage != null)
					return SharedImage;
				else
					return null;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			if(this.DesignMode)
				return;

			try
			{
				// get...
				Image image = this.ResolvedImage;
				Size size = Size.Empty;
				if(image == null)
					size = new Size(32, 32);
				else
					size = image.Size;

				// get...
				Rectangle all = new Rectangle(this.ClientRectangle.Location, this.ClientRectangle.Size);
				const int padding = 5;
				all.Inflate(0 - padding, 0 - padding);

				// image...
				_imageRectangle = new Rectangle(all.Right - size.Width - padding, all.Top + (all.Height / 2) - (size.Height / 2), 
					size.Width, size.Height);
				_textRectangle = new Rectangle(all.Left, all.Top, all.Right - _imageRectangle.Width - (2 * padding),
					all.Height);

				// invalidate...
				this.Invalidate();
			}
			catch(Exception ex)
			{
				ILog log = LogSet.GetLog(this.GetType());
				if(log.IsWarnEnabled)
					log.Warn("Failed when resizing.", ex);
			}
		}

		/// <summary>
		/// Gets the textrectangle.
		/// </summary>
		private RectangleF TextRectangle
		{
			get
			{
				// returns the value...
				return _textRectangle;
			}
		}

		/// <summary>
		/// Gets the imagerectangle.
		/// </summary>
		private Rectangle ImageRectangle
		{
			get
			{
				return _imageRectangle;
			}
		}
	}
}
