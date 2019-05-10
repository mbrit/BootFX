// BootFX - Application framework for .NET applications
// 
// File: PropertiesDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.UI.Desktop.DataBinding;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for PropertiesDialog.
	/// </summary>
	public class PropertiesDialog : DataDialog
	{
		/// <summary>
		/// Defines the base caption for the dialog.
		/// </summary>
		private const string BaseCaption = "Properties";

		/// <summary>
		/// Private field to support <c>Pages</c> property.
		/// </summary>
		private PropertyTabPageCollection _pages = new PropertyTabPageCollection();
		
		private BootFX.Common.UI.Desktop.DialogBar button1;
		private System.Windows.Forms.TabControl tabs;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertiesDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new BootFX.Common.UI.Desktop.DialogBar();
			this.tabs = new System.Windows.Forms.TabControl();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.button1.Location = new System.Drawing.Point(0, 382);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(372, 32);
			this.button1.TabIndex = 9;
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Location = new System.Drawing.Point(8, 8);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(360, 368);
			this.tabs.TabIndex = 10;
			// 
			// PropertiesDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(372, 414);
			this.Controls.Add(this.tabs);
			this.Controls.Add(this.button1);
			this.Name = "PropertiesDialog";
			this.Text = "Properties";
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnDataSourceChanged(EventArgs e)
		{
			base.OnDataSourceChanged (e);

			// update the view...
			RefreshView();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void RefreshView()
		{
			this.tabs.SuspendLayout();
			try
			{ 
				// clear...
				this.tabs.TabPages.Clear();

				// create...
				if(this.DataSource != null)
				{
					// set...
					this.Text = this.DataSource.ToString() + " " + BaseCaption;

					// get the controls...
					Control[] controls = this.GetControls(this.DataSource.GetType());
					if(controls == null)
						throw new InvalidOperationException("controls is null.");
				
					// do we have any pages?
					if(controls.Length > 0)
					{
						// create the pages...
						PropertyTabPageCollection tabPages = new PropertyTabPageCollection();

						// walk and create...
						foreach (Control control in controls)
						{
							// add the page...
							PropertyTabPage tabPage = new PropertyTabPage(control);
							this.tabs.TabPages.Add(tabPage);

							// do the binding...
							if(control is IDataControl)
								((IDataControl)control).DataSource = this.DataSource;
						}
					}
				}
				else
					this.Text = BaseCaption;
			}
			finally
			{
				this.tabs.ResumeLayout();
			}
		}

		/// <summary>
		/// Gets the pages.
		/// </summary>
		/// <returns></returns>
		private Control[] GetControls(Type entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// get the types...
			Type[] types = GetControlTypesForEntityType(entityType);
			if(types == null)
				throw new InvalidOperationException("types is null.");
			
			// create...
			IDictionary controls = new HybridDictionary();
			ArrayList names = new ArrayList();
			int generalAt = -1;
			for(int index = 0; index < types.Length; index++)
			{
				// create...
				Control control = (Control)Activator.CreateInstance(types[index]);
				if(control == null)
					throw new InvalidOperationException("control is null.");

				// name...
				string name = PropertyTabPage.GetPropertyPageName(control);
				if(string.Compare(name, "general", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					generalAt = names.Count;

				// get...
				controls[name] = control;
				names.Add(name);
			}

			// sort...
			names.Sort();

			// move...
			if(generalAt != -1)
			{
				string name = (string)names[generalAt];
				names.RemoveAt(generalAt);
				names.Insert(0, name);
			}

			// walk...
			Control[] results = new Control[names.Count];
			for(int index = 0; index < names.Count; index++)
				results[index] = (Control)controls[names[index]];

			// create then...
			return results;
		}

		/// <summary>
		/// Gets the control types for the given entity type.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		private static Type[] GetControlTypesForEntityType(Type entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// get them...
			TypeFinder finder = new TypeFinder(typeof(Control));
			finder.AddAttributeSpecification(typeof(PropertyPageAttribute), false, "EntityType", entityType);

			// return...
			return finder.GetTypes();
		}

		/// <summary>
		/// Gets a collection of PropertyPage objects.
		/// </summary>
		private PropertyTabPageCollection Pages
		{
			get
			{
				return _pages;
			}
		}

		/// <summary>
		/// Shows the properties for the given entity.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static DialogResult ShowDialog(Control owner, object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

			// do we have anything to do?
			Type[] types = GetControlTypesForEntityType(entity.GetType());
			if(types == null)
				throw new InvalidOperationException("types is null.");
			if(types.Length > 0)
			{
				// create...
				PropertiesDialog dialog = new PropertiesDialog();
				dialog.DataSource = entity;
				return dialog.ShowDialog(owner);
			}
			else
			{
				Alert.ShowWarning(owner, string.Format("'{0}' does not have any property pages.", entity));
				return DialogResult.OK;
			}
		}

		protected override bool DoApply(BootFX.Common.UI.Common.ApplyReason reason)
		{
			// walk our pages...
			foreach(PropertyTabPage tabPage in this.Pages)
			{
				if(tabPage.PropertyPageAsIPropertyPage == null)
					throw new InvalidOperationException("page.PropertyPageAsIPropertyPage is null.");
				if(tabPage.PropertyPageAsIPropertyPage.Apply(reason) == false)
					return false;
			}

			// base...
			return base.DoApply(reason);
		}
	}
}
