// BootFX - Application framework for .NET applications
// 
// File: ListViewHelper.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>ListViewHelper</c>.
	/// </summary>
	public sealed class ListViewHelper
	{
		private ListViewHelper()
		{
		}

		/// <summary>
		/// Autosizes all columns.
		/// </summary>
		public static void AutoSizeColumns(ListView listView)
		{
			if(listView == null)
				throw new ArgumentNullException("listView");
			
			listView.BeginUpdate();
			try
			{
				foreach(ColumnHeader header in listView.Columns)
					AutoSizeColumn(listView, header);
			}
			finally
			{
				listView.EndUpdate();
			}
		}

		/// <summary>
		/// Autosizes the column.
		/// </summary>
		/// <param name="header"></param>
		public static void AutoSizeColumn(ListView listView, ColumnHeader header)
		{
			if(listView == null)
				throw new ArgumentNullException("listView");
			if(header == null)
				throw new ArgumentNullException("header");			
			
			// create...
			using(Bitmap bitmap = new Bitmap(1,1, PixelFormat.Format32bppPArgb))
			{
				using(Graphics g = Graphics.FromImage(bitmap))
				{
					// measure the column header text...
					float required = g.MeasureString(header.Text, listView.Font).Width;

					// now loop the items...
					int index = 0;
					foreach(ListViewItem item in listView.Items)
					{
						// get it...
						string text = item.SubItems[header.Index].Text;
						if(text != null && text.Length > 0)
						{
							float textWidth = g.MeasureString(text, listView.Font).Width;
							if(textWidth > required)
								required = textWidth;
						}

						// next...
						index++;
						if(index == 10000)
							break;
					}

					// set...
					required += 15;

					// set...
					header.Width = (int)required;
				}
			}
		}

		/// <summary>
		/// Handled the keystroke.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="e"></param>
		public static void HandleKeyDown(ListView view, KeyEventArgs e)
		{
			if(view == null)
				throw new ArgumentNullException("view");
			if(e == null)
				throw new ArgumentNullException("e");
			
			// key...
			if(e.KeyCode == Keys.A && (e.Modifiers & Keys.Control) != 0)
			{
				SelectAll(view, true);
				e.Handled = true;
			}
			else if(e.KeyCode == Keys.Apps)
			{
				if(view is ICommandProvider)
				{
					// show...
					CommandUIObjectBuilder builder = new CommandUIObjectBuilder();
					builder.CreateAndShowContextMenu(view, view.PointToClient(Control.MousePosition), view);

					// return...
					e.Handled = true;	
				}
			}
		}

		/// <summary>
		/// Sets all items in the list to be Selected.
		/// </summary>
		public static void CheckAll(ListView list, bool isChecked)
		{
			if(list == null)
				throw new ArgumentNullException("list");
			
			for(int index = 0; index < list.Items.Count; index++)
				list.Items[index].Checked = isChecked;
		}	

		/// <summary>
		/// Sets all items in the list to be Selected.
		/// </summary>
		public static void SelectAll(ListView list, bool select)
		{
			if(list == null)
				throw new ArgumentNullException("list");
			
			for(int index = 0; index < list.Items.Count; index++)
				list.Items[index].Selected = select;
		}	

		/// <summary>
		/// Gets the grid settings for this control.
		/// </summary>
		/// <returns></returns>
		public static GridSettings GetGridSettings(ListView list)
		{
			if(list == null)
				throw new ArgumentNullException("list");
			
			// get...
			GridSettings settings = new GridSettings();

			// return...
			return settings;
		}
	}
}
