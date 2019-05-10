// BootFX - Application framework for .NET applications
// 
// File: FormPosition.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using BootFX.Common.Xml;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for FormPosition.
	/// </summary>
	[Serializable()]
	internal class FormPosition
	{
		/// <summary>
		/// Private field to support <see cref="WindowState"/> property.
		/// </summary>
		private FormWindowState _windowState;

		/// <summary>
		/// Private field to support <see cref="Location"/> property.
		/// </summary>
		private Point _location;

		/// <summary>
		/// Private field to support <see cref="Size"/> property.
		/// </summary>
		private Size _size;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public FormPosition(Form form) : this(form.WindowState, form.Location, form.Size)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public FormPosition(FormWindowState windowState, Point location, Size size)
		{
			_windowState = windowState;
			_location = location;
			_size = size;
		}

		/// <summary>
		/// Gets the size.
		/// </summary>
		public Size Size
		{
			get
			{
				return _size;
			}
		}
		
		/// <summary>
		/// Gets the location.
		/// </summary>
		public Point Location
		{
			get
			{
				return _location;
			}
		}
		
		/// <summary>
		/// Gets the windowstate.
		/// </summary>
		public FormWindowState WindowState
		{
			get
			{
				return _windowState;
			}
		}

		/// <summary>
		/// Restores the position of the form.
		/// </summary>
		/// <param name="form"></param>
		public void Restore(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");

			form.SuspendLayout();
			try
			{
				// set to manual...
				form.StartPosition = FormStartPosition.Manual;
			
				// state?
				switch(this.WindowState)
				{
						// maximize?  easy!
					case FormWindowState.Maximized:
						form.WindowState = this.WindowState;
						return;

						// minimize?  do nothing - just going to cause problems to restore a minimized form...
					case FormWindowState.Minimized:
						return;

						// the only other option... defer...
					case FormWindowState.Normal:
						this.RestoreNormal(form);
						return;

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", this.WindowState, this.WindowState.GetType()));
				}
			}
			finally
			{
				form.ResumeLayout();
			}
		}

		/// <summary>
		/// Restores the form.
		/// </summary>
		/// <param name="form"></param>
		/// <remarks>This method assumes the state is <see cref="FormWindowState.Normal"></see>.</remarks>
		private void RestoreNormal(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");
			
			// move it...
			form.Location = this.Location;
			form.Size = this.Size;

			// ensure...
			EnsureIsOnScreen(form);
		}

		/// <summary>
		/// Ensures that the given form is on the screen.
		/// </summary>
		/// <param name="form"></param>
		internal static void EnsureIsOnScreen(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");
			
			form.SuspendLayout();
			try
			{
				// make it fit...
				Rectangle screen = SystemInformation.WorkingArea;
				if(form.Bottom > screen.Bottom)
					form.Top = screen.Bottom - form.Height;
				if(form.Right > screen.Right)
					form.Left = screen.Right - form.Width;
				if(form.Left < screen.Left)
					form.Left = screen.Left;
				if(form.Top < screen.Top)
					form.Top = screen.Top;
			}
			finally
			{
				form.ResumeLayout();
			}
		}

		/// <summary>
		/// Gets the position as an XML string.
		/// </summary>
		/// <returns></returns>
		internal string ToXmlString()
		{
			XmlDocument doc = this.ToXml();
			return doc.OuterXml;
		}

		/// <summary>
		/// Gets the position as an XML string.
		/// </summary>
		/// <returns></returns>
		internal XmlDocument ToXml()
		{
			XmlDocument doc = new XmlDocument();
			XmlElement element = doc.CreateElement("Position");
			doc.AppendChild(element);

			// set...
			XmlHelper.AddElement(element, "WindowState", this.WindowState);
			XmlHelper.AddElement(element, "X", this.Location.X);
			XmlHelper.AddElement(element, "Y", this.Location.Y);
			XmlHelper.AddElement(element, "Width", this.Size.Width);
			XmlHelper.AddElement(element, "Height", this.Size.Height);

			// return...
			return doc;
		}

		internal static FormPosition FromXmlString(string asString)
		{
			if(asString == null || asString.Length == 0)
				return null;

			// load...
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(asString);

			// get...
			XmlElement element = (XmlElement)doc.SelectSingleNode("Position");
			if(element == null)
				throw new InvalidOperationException("'element' is null.");

			// set...
			FormWindowState state = (FormWindowState)Enum.Parse(typeof(FormWindowState), XmlHelper.GetElementString(element, "WindowState", OnNotFound.ThrowException));
			int x = XmlHelper.GetElementInt32(element, "X", OnNotFound.ThrowException);
			int y = XmlHelper.GetElementInt32(element, "Y", OnNotFound.ThrowException);
			int width = XmlHelper.GetElementInt32(element, "Width", OnNotFound.ThrowException);
			int height = XmlHelper.GetElementInt32(element, "Height", OnNotFound.ThrowException);

			// return...
			return new FormPosition(state, new Point(x, y), new Size(width, height));
		}
	}
}
