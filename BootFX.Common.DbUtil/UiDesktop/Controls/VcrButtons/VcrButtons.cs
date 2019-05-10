// BootFX - Application framework for .NET applications
// 
// File: VcrButtons.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using BootFX.Common.UI.Desktop.DataBinding;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Describes a control that displays VCR buttons.
	/// </summary>
	public class VcrButtons : UserControl
	{
		/// <summary>
		/// Private field to support <c>MarkForDeletionOnRemove</c> property.
		/// </summary>
		private bool _markForDeletionOnRemove = true;
		
		/// <summary>
		/// Raised when the <c>BindingManager</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the BindingManager property has changed.")]
		public event EventHandler BindingManagerChanged;
		
		/// <summary>
		/// Private field to support <c>BindingManager</c> property.
		/// </summary>
		private BindingManagerBase _bindingManager;
		
		private System.Windows.Forms.Button buttonFirst;
		private System.Windows.Forms.Button buttonPrevious;
		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown updownPosition;
		private System.Windows.Forms.Label labelTotal;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textFind;
		private System.Windows.Forms.Button buttonFind;
		private System.Windows.Forms.Button buttonLast;
	
		public VcrButtons()
		{
			this.InitializeComponent();
			this.UpdateButtonStates();
		}

		private void InitializeComponent()
		{
			this.buttonFirst = new System.Windows.Forms.Button();
			this.buttonPrevious = new System.Windows.Forms.Button();
			this.buttonNext = new System.Windows.Forms.Button();
			this.buttonLast = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.updownPosition = new System.Windows.Forms.NumericUpDown();
			this.labelTotal = new System.Windows.Forms.Label();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textFind = new System.Windows.Forms.TextBox();
			this.buttonFind = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.updownPosition)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonFirst
			// 
			this.buttonFirst.Location = new System.Drawing.Point(4, 4);
			this.buttonFirst.Name = "buttonFirst";
			this.buttonFirst.Size = new System.Drawing.Size(32, 23);
			this.buttonFirst.TabIndex = 0;
			this.buttonFirst.Text = "<<";
			this.buttonFirst.Click += new System.EventHandler(this.buttonFirst_Click);
			// 
			// buttonPrevious
			// 
			this.buttonPrevious.Location = new System.Drawing.Point(40, 4);
			this.buttonPrevious.Name = "buttonPrevious";
			this.buttonPrevious.Size = new System.Drawing.Size(32, 23);
			this.buttonPrevious.TabIndex = 1;
			this.buttonPrevious.Text = "<";
			this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
			// 
			// buttonNext
			// 
			this.buttonNext.Location = new System.Drawing.Point(76, 4);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(32, 23);
			this.buttonNext.TabIndex = 2;
			this.buttonNext.Text = ">";
			this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
			// 
			// buttonLast
			// 
			this.buttonLast.Location = new System.Drawing.Point(112, 4);
			this.buttonLast.Name = "buttonLast";
			this.buttonLast.Size = new System.Drawing.Size(32, 23);
			this.buttonLast.TabIndex = 3;
			this.buttonLast.Text = ">>";
			this.buttonLast.Click += new System.EventHandler(this.buttonLast_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(292, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 20);
			this.label1.TabIndex = 4;
			this.label1.Text = "&Item:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// updownPosition
			// 
			this.updownPosition.Location = new System.Drawing.Point(324, 4);
			this.updownPosition.Name = "updownPosition";
			this.updownPosition.Size = new System.Drawing.Size(68, 20);
			this.updownPosition.TabIndex = 5;
			this.updownPosition.ValueChanged += new System.EventHandler(this.updownPosition_ValueChanged);
			// 
			// labelTotal
			// 
			this.labelTotal.Location = new System.Drawing.Point(396, 4);
			this.labelTotal.Name = "labelTotal";
			this.labelTotal.Size = new System.Drawing.Size(92, 20);
			this.labelTotal.TabIndex = 6;
			this.labelTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(160, 4);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(32, 23);
			this.buttonAdd.TabIndex = 7;
			this.buttonAdd.Text = "*";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(244, 4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(32, 23);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "C";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Location = new System.Drawing.Point(196, 4);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(32, 23);
			this.buttonRemove.TabIndex = 9;
			this.buttonRemove.Text = "x";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(496, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 20);
			this.label2.TabIndex = 10;
			this.label2.Text = "&Find:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textFind
			// 
			this.textFind.AcceptsReturn = true;
			this.textFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textFind.Location = new System.Drawing.Point(528, 4);
			this.textFind.Name = "textFind";
			this.textFind.Size = new System.Drawing.Size(136, 20);
			this.textFind.TabIndex = 11;
			this.textFind.Text = "";
			this.textFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textFind_KeyDown);
			// 
			// buttonFind
			// 
			this.buttonFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonFind.Location = new System.Drawing.Point(668, 4);
			this.buttonFind.Name = "buttonFind";
			this.buttonFind.Size = new System.Drawing.Size(28, 23);
			this.buttonFind.TabIndex = 12;
			this.buttonFind.Text = "Go";
			this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
			// 
			// VcrButtons
			// 
			this.Controls.Add(this.buttonFind);
			this.Controls.Add(this.textFind);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.labelTotal);
			this.Controls.Add(this.updownPosition);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonLast);
			this.Controls.Add(this.buttonNext);
			this.Controls.Add(this.buttonPrevious);
			this.Controls.Add(this.buttonFirst);
			this.Name = "VcrButtons";
			this.Size = new System.Drawing.Size(700, 32);
			((System.ComponentModel.ISupportInitialize)(this.updownPosition)).EndInit();
			this.ResumeLayout(false);

		}

		private void buttonFirst_Click(object sender, System.EventArgs e)
		{
			this.MoveFirst();
		}

		public void MoveFirst()
		{
			this.Position = 0;
		}

		private void buttonPrevious_Click(object sender, System.EventArgs e)
		{
			this.MovePrevious();
		}

		public void MovePrevious()
		{
			this.Position--;
		}

		private void buttonNext_Click(object sender, System.EventArgs e)
		{
			this.MoveNext();
		}

		public void MoveNext()
		{
			this.Position++;
		}

		private void buttonLast_Click(object sender, System.EventArgs e)
		{
			this.MoveLast();
		}

		public void MoveLast()
		{
			this.Position = this.Count - 1;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
			this.BindingManager = null;
		}

		/// <summary>
		/// Gets the binding manager as a currency manager.
		/// </summary>
		private CurrencyManager CurrencyManager
		{
			get
			{
				return this.BindingManager as CurrencyManager;
			}
		}

		/// <summary>
		/// Gets or sets the bindingmanager
		/// </summary>
		[Browsable(false)]
		public BindingManagerBase BindingManager
		{
			get
			{
				return _bindingManager;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _bindingManager)
				{
					// unsubcribe...
					this.UnsubscribeBindingManager();

					// set the value...
					_bindingManager = value;

					// subscribe...
					SubscribeBindingManager();

					// set...
					UpdateButtonStates();

					// event...
					this.OnBindingManagerChanged();
				}
			}
		}

		/// <summary>
		/// Subscribes the binding manager.
		/// </summary>
		private void UnsubscribeBindingManager()
		{
			if(this.BindingManager == null)
				return;

			// set...
			this.BindingManager.PositionChanged -= new EventHandler(BindingManager_PositionChanged);
		}

		/// <summary>
		/// Subscribes the binding manager.
		/// </summary>
		private void SubscribeBindingManager()
		{
			if(this.BindingManager == null)
				return;

			// set...
			this.BindingManager.PositionChanged += new EventHandler(BindingManager_PositionChanged);
		}

		/// <summary>
		/// Updates the states on the buttons.
		/// </summary>
		private void UpdateButtonStates()
		{
			bool allowBack = true;
			bool allowForward = true;

			// check...
			if(this.BindingManager != null)
			{
				int position = this.BindingManager.Position;
				if(position == 0)
					allowBack = false;
				if(position == this.BindingManager.Count - 1)
					allowForward = false;

				// set...
				this.updownPosition.Minimum = 1;
				this.updownPosition.Maximum = this.Count;
				this.updownPosition.Value = this.BindingManager.Position + 1;
				this.updownPosition.Enabled = true;
				this.labelTotal.Text = " of " + this.BindingManager.Count;
			}
			else
			{
				allowBack = false;
				allowForward = false;
				this.updownPosition.Enabled = false;
				this.labelTotal.Text = string.Empty;
			}

			// set...
			this.buttonFirst.Enabled = allowBack;
			this.buttonPrevious.Enabled = allowBack;
			this.buttonNext.Enabled = allowForward;
			this.buttonLast.Enabled = allowForward;
			this.buttonAdd.Enabled = this.AllowAdd;
			this.buttonRemove.Enabled = this.AllowDelete;
		}

		private void BindingManager_PositionChanged(object sender, EventArgs e)
		{
			this.UpdateButtonStates();
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			this.BindingManager.AddNew();
			this.UpdateButtonStates();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			if(this.BindingManager != null)
				this.BindingManager.CancelCurrentEdit();
		}

		/// <summary>
		/// Returns true if we are allowed to add to the list.
		/// </summary>
		private bool AllowDelete
		{
			get
			{
				// list...
				if(this.InnerListAsIBindingList != null)
					return this.InnerListAsIBindingList.AllowRemove;
				else
					return false;
			}
		}

		/// <summary>
		/// Returns true if we are allowed to add to the list.
		/// </summary>
		private bool AllowAdd
		{
			get
			{
				// list...
				if(this.InnerListAsIBindingList != null)
					return this.InnerListAsIBindingList.AllowNew;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the inner list, if there is one.
		/// </summary>
		private IList InnerList
		{
			get
			{
				if(this.CurrencyManager != null)
					return this.CurrencyManager.List;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the inner list, if there is one.
		/// </summary>
		private IBindingList InnerListAsIBindingList
		{
			get
			{
				return this.InnerList as IBindingList;
			}
		}

		/// <summary>
		/// Returns true if the item is being edited.
		/// </summary>
		public bool IsInEdit
		{
			get
			{
				if(this.BindingManager != null)
				{
					if(this.BindingManager.Current is IInEdit)
						return ((IInEdit)this.BindingManager.Current).InEdit;
					else
						return true;
				}
				else
					return false;
			}
		}

		/// <summary>
		/// Raises the <c>BindingManagerChanged</c> event.
		/// </summary>
		private void OnBindingManagerChanged()
		{
			OnBindingManagerChanged(EventArgs.Empty);
		}

		private void buttonRemove_Click(object sender, System.EventArgs e)
		{
			if(BindingManager == null)
				throw new InvalidOperationException("BindingManager is null.");

			// do we delete when we remove?
			if(this.MarkForDeletionOnRemove)
			{
				// mark the entity...
				if(this.Current != null && this.Current is EntityView)
					((EntityView)this.Current).MarkForDeletion();
			}

			// end...
			this.BindingManager.EndCurrentEdit();

			// remove the item...
			int position = this.Position;
			this.BindingManager.RemoveAt(position);

			// update the chosen item...
			this.Position = position + 1;
			this.Position = position;

			// move the selection...
			this.UpdateButtonStates();
		}

		private void updownPosition_ValueChanged(object sender, System.EventArgs e)
		{
			this.Position = Convert.ToInt32(this.updownPosition.Value) - 1;
		}
		
		/// <summary>
		/// Raises the <c>BindingManagerChanged</c> event.
		/// </summary>
		protected virtual void OnBindingManagerChanged(EventArgs e)
		{
			if(BindingManagerChanged != null)
				BindingManagerChanged(this, e);
		}

		/// <summary>
		/// Gets or sets the position
		/// </summary>
		public int Position
		{
			get
			{
				if(this.BindingManager != null)
					return this.BindingManager.Position;
				else
					return 0;
			}
			set
			{
				if(this.BindingManager != null)
					this.BindingManager.Position = value;
			}
		}

		/// <summary>
		/// Gets the number of items in the binding list.
		/// </summary>
		public int Count
		{
			get
			{
				if(this.BindingManager != null)
					return this.BindingManager.Count;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public object Current
		{
			get
			{
				if(this.BindingManager != null)
					return this.BindingManager.Current;
				else
					return null;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			e.Graphics.DrawLine(SystemPens.ControlDark, this.ClientRectangle.Left, this.ClientRectangle.Bottom - 1, this.ClientRectangle.Right, this.ClientRectangle.Bottom - 1);
		}

		private void buttonGo_Click(object sender, System.EventArgs e)
		{
			Find();
		}

		/// <summary>
		/// Finds the given record.
		/// </summary>
		private bool Find()
		{
			string text = this.textFind.Text;
			if(text == null || text.Length == 0)
			{
				Alert.ShowWarning(this, "You must enter something to search for.");
				return false;
			}
			else
				return this.Find(text);
		}

		/// <summary>
		/// Finds the given record.
		/// </summary>
		/// <param name="text"></param>
		private bool Find(string text)
		{
			if(text == null)
				throw new ArgumentNullException("text");
			
			// walk...
			int from = this.Position + 1;
			if(from == this.Count)
				from = 0;
			int to = this.Count;
			bool found = false;
			if(this.Find(text, from, to) == false && from > 0)
			{
				// beginning?
				if(Alert.AskYesNoQuestion(this, "An item was not found.  Do you want to start from the beginning?") == DialogResult.Yes)
				{
					if(this.Find(text, 0, from))
						found = true;
				}
			}
			else
				found = true;

			// found?
			if(found == false)
				Alert.ShowInformation(this, "An item was not found.");

			// return...
			return found;
		}

		/// <summary>
		/// Finds the given text.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		private bool Find(string text, int from, int to)
		{
			if(text == null)
				throw new ArgumentNullException("text");
			if(text.Length == 0)
				throw new ArgumentOutOfRangeException("'text' is zero-length.");

			if(InnerList == null)
				throw new InvalidOperationException("InnerList is null.");

			// lower...
			text = text.ToLower();
			
			// examine in turn...
			for(int index = from; index < to; index++)
			{
				// get it...
				object entity = this.InnerList[index];
				if(entity == null)
					throw new InvalidOperationException("entity is null.");
				entity = EntityView.Unwrap(entity);
				if(entity == null)
					throw new InvalidOperationException("entity is null (2).");

				// entity type...
				EntityType entityType = EntityType.GetEntityType(entity, OnNotFound.ThrowException);
				if(entityType == null)
					throw new InvalidOperationException("entityType is null.");

				// walk fields...
				foreach(EntityField field in entityType.Fields)
				{
					// text?
					if(this.IsFindCandiate(field))
					{
						// get the value...
						string value = ConversionHelper.ToString(entityType.Storage.GetValue(entity, field, ConversionFlags.Safe), Cultures.User);
						if(value != null)
						{
							// find...
							value = value.ToLower();
							int textIndex = value.IndexOf(text);
							if(textIndex != -1)
							{
								this.Position = index;
								return true;
							}
						}
					}
				}
			}

			// nope...
			return false;
		}

		/// <summary>
		/// Returns true if the given field is good for text searching.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private bool IsFindCandiate(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			switch(field.DBType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
					return true;

				default:
					return false;
			}
		}

		private void buttonFind_Click(object sender, System.EventArgs e)
		{
			this.Find();
		}

		private void textFind_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Return)
				this.Find();
		}

		/// <summary>
		/// Gets or sets whether to mark the entity under the item as "for deletion" when it the delete button is pressed.
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(true), 
			Description("Gets or sets whether to mark the entity under the item as \"for deletion\" when it the delete button is pressed.")]
		public bool MarkForDeletionOnRemove
		{
			get
			{
				return _markForDeletionOnRemove;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _markForDeletionOnRemove)
				{
					// set the value...
					_markForDeletionOnRemove = value;
				}
			}
		}
	}
}
