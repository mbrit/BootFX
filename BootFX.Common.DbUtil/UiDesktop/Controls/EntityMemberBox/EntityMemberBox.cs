// BootFX - Application framework for .NET applications
// 
// File: EntityMemberBox.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.UI.Desktop.DataBinding;
using BootFX.Common.UI.DataBinding;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EntityFieldBox.
	/// </summary>
	public class EntityMemberBox : UserControl, IDesktopBindableControl
	{
		/// <summary>
		/// Raised when the editor needs initializing.
		/// </summary>
		public event MemberEditorEventHandler InitializeEditor;
		
		/// <summary>
		/// Private field to support <c>ReadOnly</c> property.
		/// </summary>
		private bool _readOnly;
		
		/// <summary>
		/// Raised when the <c>Tag</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Tag property has changed.")]
		public event EventHandler TagChanged;
		
		/// <summary>
		/// Private field to support <c>Binding</c> property.
		/// </summary>
		private Binding _binding;
		
		/// <summary>
		/// Private field to support <see cref="Member"/> property.
		/// </summary>
		private EntityMember _member = null;
		
		/// <summary>
		/// Private field to support <c>Caption</c> property.
		/// </summary>
		private string _caption = null;
		
		/// <summary>
		/// Private field to support <c>LabelPosition</c> property.
		/// </summary>
		private LabelPosition _labelPosition;
		
		/// <summary>
		/// Private field to support <c>LabelVisible</c> property.
		/// </summary>
		private bool _labelVisible = true;
		
		/// <summary>
		/// Private field to support <c>LabelWidth</c> property.
		/// </summary>
		private int _labelWidth = 100;
		
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Panel panel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntityMemberBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
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
			this.label = new System.Windows.Forms.Label();
			this.panel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Dock = System.Windows.Forms.DockStyle.Left;
			this.label.Location = new System.Drawing.Point(0, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(100, 98);
			this.label.TabIndex = 0;
			this.label.Text = "label1";
			// 
			// panel
			// 
			this.panel.BackColor = System.Drawing.SystemColors.Control;
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(100, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(232, 98);
			this.panel.TabIndex = 1;
			this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			// 
			// EntityMemberBox
			// 
			this.Controls.Add(this.panel);
			this.Controls.Add(this.label);
			this.Name = "EntityMemberBox";
			this.Size = new System.Drawing.Size(332, 98);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets the labelwidth
		/// </summary>
		[Browsable(true), Description("Gets or sets the width of the label."), DefaultValue(150), Category("Appearance")]
		public int LabelWidth
		{
			get
			{
				return _labelWidth;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _labelWidth)
				{
					// set the value...
					_labelWidth = value;
					RefreshLayout();
				}
			}
		}

		/// <summary>
		/// Gets or sets the labelvisible
		/// </summary>
		[Browsable(true), Description("Gets or sets whether the label is visible."), DefaultValue(true), Category("Appearance")]
		public bool LabelVisible
		{
			get
			{
				return _labelVisible;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _labelVisible)
				{
					// set the value...
					_labelVisible = value;
					RefreshLayout();
				}
			}
		}

		/// <summary>
		/// Gets or sets the labelposition
		/// </summary>
		[Browsable(true), Description("Gets or sets the position of the label."), DefaultValue(LabelPosition.Left), Category("Appearance")]
		public LabelPosition LabelPosition
		{
			get
			{
				return _labelPosition;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _labelPosition)
				{
					// set the value...
					_labelPosition = value;
					RefreshLayout();
				}
			}
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		private void RefreshLayout()
		{
			this.SuspendLayout();
			try
			{
				// visible?
				if(this.LabelVisible)
				{
					// show...
					this.label.Visible = true;

					// where?
					switch(LabelPosition)
					{
						case LabelPosition.Left:
							label.Dock = DockStyle.Left;
							label.Width = this.LabelWidth;
							label.TextAlign = ContentAlignment.MiddleLeft;
							break;

						case LabelPosition.Top:
							label.Dock = DockStyle.Top;
							label.Height = 20;
							label.TextAlign = ContentAlignment.TopLeft;
							break;

						default:
							throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", LabelPosition, LabelPosition.GetType()));
					}
				}
				else
					this.label.Visible = false;
			}
			finally
			{
				this.ResumeLayout();
			}
		}

		/// <summary>
		/// Gets or sets the caption
		/// </summary>
		[Browsable(true), Description("Gets or sets the caption."), DefaultValue(null), Category("Appearance")]
		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _caption)
				{
					// set the value...
					_caption = value;
				}
			}
		}

		/// <summary>
		/// Gets the member.
		/// </summary>
		public EntityMember Member
		{
			get
			{
				return _member;
			}
		}

		/// <summary>
		/// Gets the binding property.
		/// </summary>
		/// <returns></returns>
		string IBindableControl.GetDefaultBindProperty()
		{
			return "Value";
		}

		/// <summary>
		/// Gets or sets the value
		/// </summary>
		[Browsable(false)]
		public object Value
		{
			get
			{
				if(this.Editor != null)
					return this.Editor.Value;
				else
					return null;
			}
			set
			{
				if(this.Editor != null)
					this.Editor.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the binding
		/// </summary>
		Binding IDesktopBindableControl.Binding
		{
			get
			{
				return this.Binding;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _binding)
				{
					// set the value...
					_binding = value;
					RefreshView();
				}
			}
		}

		/// <summary>
		/// Gets the binding.
		/// </summary>
		private Binding Binding
		{
			get
			{
				return _binding;
			}
		}

		/// <summary>
		/// Gets the parent data control.
		/// </summary>
		private IEntityDataControl ParentDataControl
		{
			get
			{
				Control parent = this.Parent;
				while(parent != null)
				{
					if(parent is IEntityDataControl)
						return (IEntityDataControl)parent;
					parent = parent.Parent;
				}

				// nope...
				return null;
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void RefreshView()
		{
			if(this.DesignMode)
				return;

			try
			{
				// parent...
				if(ParentDataControl == null)
					throw new InvalidOperationException(string.Format("Control must be parented by an IEntityDataControl-implementing control."));

				// binding?
				Type type = null;
				if(this.Binding != null)
				{
					// get the type...
					type = GetEditorType(this.Binding);

					// get the name...
					this.label.Text = GetEditorCaption(this.Binding) + ":";
				}
				else
					this.label.Text = string.Empty;

				// reset...
				// TODO: make this more elegant so that we can reuse a control is appropriate...
				this.panel.Controls.Clear();

				// show...
				if(type != null)
				{
					// create one...
					IMemberEditor editor = (IMemberEditor)Activator.CreateInstance(type);
					if(editor == null)
						throw new InvalidOperationException("editor is null.");

					// get the control...
					Control editorControl = (Control)editor;

					// get the bound member...
					EntityMember boundMember = null;
					if(this.Binding is EntityMemberBinding)
						boundMember = ((EntityMemberBinding)this.Binding).Member;

					// call out to the editor...
					editor.Initializing(this, boundMember);
					this.OnInitializeEditor(new MemberEditorEventArgs(editorControl, boundMember));
					editor.Initialized();

					// show...
					editorControl.Dock = DockStyle.Fill;
					this.panel.Controls.Add(editorControl);

					// check...
					if(editorControl.Height > this.Height)
						this.Height = editorControl.Height;

					// bind the control...
					this.ParentDataControl.EntityBindingManager.CreateBinding(editorControl, EntityBindingManager.GetPropertyToBind(editorControl), (string)this.Tag);
				}
				else
					throw new InvalidOperationException(string.Format("Type not found for '{0}'.", this.Binding));
			}
			catch(Exception ex)
			{
				ShowException(ex);
			}
		}

		/// <summary>
		/// Shows an exception.
		/// </summary>
		/// <param name="ex"></param>
		private void ShowException(Exception ex)
		{
			this.panel.Controls.Clear();
			this.panel.Controls.Add(new ExceptionView(ex));
		}

		/// <summary>
		/// Defines a view that displays an exception.
		/// </summary>
		private class ExceptionView : TextBox
		{
			public Exception Exception;

			public ExceptionView(Exception exception)
			{
				this.Exception = exception;
				this.ReadOnly = true;
				this.Dock = DockStyle.Fill;
				if(this.Exception != null)
					this.Text = this.Exception.Message;
				else
					this.Text = "(No exception)";
			}

			protected override void OnClick(EventArgs e)
			{
				// show...
				if(this.Exception != null)
					Alert.ShowWarning(this, "This field experienced an excepiton when initializing.", this.Exception);
				else
					Alert.ShowWarning(this, "This field experienced an exception when initializing, but the exception was not found.");
			}
		}

		/// <summary>
		/// Gets the caption from the given binding.
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		private static string GetEditorCaption(Binding binding)
		{
			if(binding != null)
			{
				if(binding is EntityFieldBinding)
				{
					if(((EntityFieldBinding)binding).Field == null)
						throw new InvalidOperationException("'((EntityFieldBinding)binding).Field' is null.");
					return ((EntityFieldBinding)binding).Field.Name;
				}
				if(binding is EntityLinkBinding)
				{
					if(((EntityLinkBinding)binding).Link == null)
						throw new InvalidOperationException("((EntityLinkBinding)binding).Link is null.");
					return ((EntityLinkBinding)binding).Link.Name;
				}
				else
					return "???";
			}
			else
				return string.Empty;
		}

		/// <summary>
		/// Gets the editor type for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private Type GetEditorType(Binding binding)
		{
			if(binding != null)
			{
				// bound?
				if(binding is EntityFieldBinding)
					return GetEditorType(((EntityFieldBinding)binding).Field);
				if(binding is EntityLinkBinding)
					return GetEditorType(((EntityLinkBinding)binding).Link);
				else
					return null;
			}
			else
				return null;
		}

		/// <summary>
		/// Gets the editor type for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private Type GetEditorType(EntityLink link)
		{
			if(link == null)
				throw new ArgumentNullException("link");
			
			if(link is ChildToParentEntityLink)
			{
				if(this.ReadOnly)
					return typeof(EntityBoxMemberEditor);
				else
					return typeof(EntityComboBoxMemberEditor);
			}
			else
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", link));
		}

		/// <summary>
		/// Gets the editor type for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private Type GetEditorType(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// enum?
			if(field.HasEnumerationType)
				return typeof(EnumComboBoxMemberEditor);
			else
			{
				// return..
				switch(field.DBType)
				{
					case DbType.String:
					case DbType.Int16:
					case DbType.Int32:
					case DbType.Int64:
						return typeof(TextBoxMemberEditor);

					case DbType.DateTime:
						return typeof(DateTimePickerMemberEditor);

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", field.DBType, field.DBType.GetType()));
				}
			}
		}

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		public new string Tag
		{
			get
			{
				return (string)base.Tag;
			}
			set
			{
				if(this.Tag != value)
				{
					base.Tag = value;
					this.OnTagChanged();
				}
			}
		}

		/// <summary>
		/// Raises the <c>TagChanged</c> event.
		/// </summary>
		private void OnTagChanged()
		{
			OnTagChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>TagChanged</c> event.
		/// </summary>
		protected virtual void OnTagChanged(EventArgs e)
		{
			if(this.DesignMode)
				this.label.Text = this.Tag;

			if(TagChanged != null)
				TagChanged(this, e);
		}

		/// <summary>
		/// Gets the current editor.
		/// </summary>
		private IMemberEditor Editor
		{
			get
			{
				if(this.panel.Controls.Count > 0)
					return this.panel.Controls[0] as IMemberEditor;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the current editor as a control.
		/// </summary>
		private Control EditorAsControl
		{
			get
			{
				return this.Editor as Control;
			}
		}

		/// <summary>
		/// Gets or sets the readonly
		/// </summary>
		[Browsable(true), Description("Gets or sets whether the control is read-only."), DefaultValue(false), Category("Behavior")]
		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _readOnly)
				{
					// set the value...
					_readOnly = value;
					this.RefreshView();
				}
			}
		}

		/// <summary>
		/// Raises the <c>InitializeEditor</c> event.
		/// </summary>
		protected virtual void OnInitializeEditor(MemberEditorEventArgs e)
		{
			// raise...
			if(InitializeEditor != null)
				InitializeEditor(this, e);
		}

		private void panel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(this.DesignMode)
			{
				// fill..
				e.Graphics.FillRectangle(SystemBrushes.Window, this.panel.ClientRectangle);
				e.Graphics.DrawString(Convert.ToString(this.Tag), this.Font, Brushes.Black, 2, 2);
			}
		}
	}
}
