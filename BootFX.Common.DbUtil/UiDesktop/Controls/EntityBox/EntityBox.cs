// BootFX - Application framework for .NET applications
// 
// File: EntityBox.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.Drawing;
using BootFX.Common.UI.Desktop.DataBinding;
using BootFX.Common.UI.DataBinding;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EntityBox.
	/// </summary>
	public class EntityBox : UserControl, IDesktopBindableControl
	{
		/// <summary>
		/// Private field to support <c>Binding</c> property.
		/// </summary>
		private Binding _binding;
		
		private System.Windows.Forms.Label labelEntity;
		/// <summary>
		/// Private field to support <c>Entity</c> property.
		/// </summary>
		private object _entity;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityBox()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the entity
		/// </summary>
		public object Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _entity)
				{
					// set the value...
					_entity = value;
					this.RefreshView();
				}
			}
		}

		/// <summary>
		/// Refreshes the view.
		/// </summary>
		private void RefreshView()
		{
			// set...
			if(_entity != null)
			{
				this.labelEntity.Text = _entity.ToString();
				this.labelEntity.Enabled = true;
			}
			else
			{
				this.labelEntity.Text = "(None)";
				this.labelEntity.Enabled = false;
			}
		}

		private void InitializeComponent()
		{
			this.labelEntity = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelEntity
			// 
			this.labelEntity.BackColor = System.Drawing.Color.Transparent;
			this.labelEntity.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelEntity.Location = new System.Drawing.Point(4, 1);
			this.labelEntity.Name = "labelEntity";
			this.labelEntity.Size = new System.Drawing.Size(260, 18);
			this.labelEntity.TabIndex = 0;
			this.labelEntity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelEntity.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelEntity_MouseUp);
			// 
			// EntityBox
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.labelEntity);
			this.DockPadding.Bottom = 1;
			this.DockPadding.Left = 4;
			this.DockPadding.Right = 4;
			this.DockPadding.Top = 1;
			this.Name = "EntityBox";
			this.Size = new System.Drawing.Size(268, 20);
			this.ResumeLayout(false);

		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

			// refresh...
			this.RefreshView();
		}

		/// <summary>
		/// Gets the property that should be used for data binding.
		/// </summary>
		/// <returns></returns>
		string IBindableControl.GetDefaultBindProperty()
		{
			return "Entity";
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			// draw...
			using(Pen pen = new Pen(ControlPaint.LightLight(SystemColors.ActiveCaption), 1))
			{
				e.Graphics.DrawLine(pen, this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Right, this.ClientRectangle.Top);
				e.Graphics.DrawLine(pen, this.ClientRectangle.Left, this.ClientRectangle.Bottom - 1, this.ClientRectangle.Right, this.ClientRectangle.Bottom - 1);
				e.Graphics.DrawLine(pen, this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Left, this.ClientRectangle.Bottom);
				e.Graphics.DrawLine(pen, this.ClientRectangle.Right - 1, this.ClientRectangle.Top, this.ClientRectangle.Right - 1, this.ClientRectangle.Bottom);
			}
		}

		private void labelEntity_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// check...
			if(e.Button != MouseButtons.Right)
				return;

			// do we have an entity?
			if(this.Entity is ICommandProvider)
				return;

			// show...
			CommandUIObjectBuilder builder = new CommandUIObjectBuilder();
			builder.CreateAndShowContextMenu(this.labelEntity, new Point(e.X, e.Y), (ICommandProvider)this.Entity);
		}

		/// <summary>
		/// Gets or sets the binding
		/// </summary>
		Binding IDesktopBindableControl.Binding
		{
			get
			{
				return _binding;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _binding)
				{
					// set the value...
					_binding = value;
				}
			}
		}
	}
}
