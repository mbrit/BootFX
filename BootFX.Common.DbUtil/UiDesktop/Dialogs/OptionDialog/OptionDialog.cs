// BootFX - Application framework for .NET applications
// 
// File: OptionDialog.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for OptionDialog.
	/// </summary>
	public class OptionDialog : BaseForm
	{
		private const int Spacing = 5;

        /// <summary>
        /// Private value to support the <see cref="DoNotShowAgainCheckBox">DoNotShowAgainCheckBox</see> property.
        /// </summary>
        private CheckBox _doNotShowAgainCheckBox;

        /// <summary>
        /// Private value to support the <see cref="MoreInformationLink">MoreInformationLink</see> property.
        /// </summary>
        private LinkLabel _moreInformationLink;

        /// <summary>
        /// Private value to support the <see cref="DoNotShowAgainKey">DoNotShowAgainKey</see> property.
        /// </summary>
        private string _doNotShowAgainKey;

        private bool _doNotShowAgainDefault = true;

        /// <summary>
        /// Private value to support the <see cref="MoreInformationUrl">MoreInformationUrl</see> property.
        /// </summary>
        private string _moreInformationUrl;

        /// <summary>
		/// Private field to support <c>ButtonWidth</c> property.
		/// </summary>
		private int _buttonWidth = 0;
		
		/// <summary>
		/// Private field to support <c>ButtonHeight</c> property.
		/// </summary>
		private int _buttonHeight = 0;
		
		/// <summary>
		/// Private field to support <c>Icon</c> property.
		/// </summary>
		private MessageBoxIcon _icon = MessageBoxIcon.Information;
		
		private System.Windows.Forms.PictureBox pictureIcon;
		private System.Windows.Forms.Label labelPrompt;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public OptionDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public OptionDialog(string prompt) 
			: this()
		{
			this.Prompt = prompt;
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
            this.pictureIcon = new System.Windows.Forms.PictureBox();
            this.labelPrompt = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureIcon
            // 
            this.pictureIcon.Location = new System.Drawing.Point(10, 11);
            this.pictureIcon.Name = "pictureIcon";
            this.pictureIcon.Size = new System.Drawing.Size(32, 32);
            this.pictureIcon.TabIndex = 0;
            this.pictureIcon.TabStop = false;
            this.pictureIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureIcon_Paint);
            // 
            // labelPrompt
            // 
            this.labelPrompt.Location = new System.Drawing.Point(53, 12);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(236, 32);
            this.labelPrompt.TabIndex = 1;
            this.labelPrompt.Text = "label1";
            // 
            // OptionDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(301, 51);
            this.Controls.Add(this.labelPrompt);
            this.Controls.Add(this.pictureIcon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Query";
            ((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void pictureIcon_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Icon icon = this.GetIcon();
			if(icon == null)
				throw new InvalidOperationException("icon is null.");

			// paint...
			e.Graphics.DrawIcon(icon, 0,0);
		}

		private Icon GetIcon()
		{
			switch(this.PromptIcon)
			{
				case MessageBoxIcon.Error:
					return SystemIcons.Error;

				case MessageBoxIcon.Question:
					return SystemIcons.Question;

				case MessageBoxIcon.Information:
				case MessageBoxIcon.None:
					return SystemIcons.Information;

				case MessageBoxIcon.Warning:
					return SystemIcons.Warning;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", Icon, Icon.GetType()));
			}
		}

		/// <summary>
		/// Gets or sets the icon
		/// </summary>
		public MessageBoxIcon PromptIcon
		{
			get
			{
				return _icon;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _icon)
				{
					// set the value...
					_icon = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the prompt text.
		/// </summary>
		public string Prompt
		{
			get
			{
				return this.labelPrompt.Text;
			}
			set
			{
				this.labelPrompt.Text = value;
                this.LayoutView();
			}
		}

		/// <summary>
		/// Adds a button.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="reuslt"></param>
		public void AddButton(string text, DialogResult result)
		{
			// create...
			OptionDialogButton button = new OptionDialogButton(text, result);
			this.Controls.Add(button);

            // layout...
            this.LayoutView();
		}

		private OptionDialogButton[] GetButtons()
		{
			ArrayList results = new ArrayList();
			foreach(Control control in this.Controls)
			{
				if(control is OptionDialogButton)
					results.Add(control);
			}

			// return...
			return (OptionDialogButton[])results.ToArray(typeof(OptionDialogButton));
		}

		/// <summary>
		/// Gets or sets the buttonwidth
		/// </summary>
		private int ButtonWidth
		{
			get
			{
				return _buttonWidth;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _buttonWidth)
				{
					// set the value...
					_buttonWidth = value;
				}
			}
		}

		/// <summary>
		/// Gets the buttonheight.
		/// </summary>
		private int ButtonHeight
		{
			get
			{
				// returns the value...
				if(_buttonHeight == 0)
				{
					SizeF size = DesktopRuntime.Current.MeasureString("Gj", this.Font);
					_buttonHeight = (int)(size.Height + (2 * Spacing));
				}
				return _buttonHeight;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

            // runtime...
            this.Text = Alert.Caption;

			// get...
			OptionDialogButton[] buttons = this.GetButtons();
			if(buttons == null)
				throw new InvalidOperationException("buttons is null.");

			// setup the default buttons...
			foreach(OptionDialogButton button in buttons)
			{
				if(button.DialogResult == DialogResult.Cancel)
					this.CancelButton = button;
				else if(button.DialogResult == DialogResult.OK || button.DialogResult == DialogResult.Yes)
					this.AcceptButton = button;
			}
		}

        /// <summary>
        /// Gets the MoreInformationUrl value.
        /// </summary>
        public string MoreInformationUrl
        {
            get
            {
                return _moreInformationUrl;
            }
            set
            {
                _moreInformationUrl = value;
                this.LayoutView();
            }
        }

        /// <summary>
        /// Gets the DoNotShowAgainKey value.
        /// </summary>
        public string DoNotShowAgainKey
        {
            get
            {
                return _doNotShowAgainKey;
            }
            set
            {
                _doNotShowAgainKey = value;
                this.LayoutView();
            }
        }

        /// <summary>
        /// Gets the DoNotShowAgainKey value.
        /// </summary>
        public bool DoNotShowAgainDefault
        {
            get
            {
                return _doNotShowAgainDefault;
            }
            set
            {
                _doNotShowAgainDefault = value;

                // select it if we need to...
                if (_doNotShowAgainCheckBox != null)
                    _doNotShowAgainCheckBox.Checked = value;
            }
        }

        /// <summary>
        /// Gets the MoreInformationLink value.
        /// </summary>
        private LinkLabel MoreInformationLink
        {
            get
            {
                if (_moreInformationLink == null)
                {
                    _moreInformationLink = new LinkLabel();
                    _moreInformationLink.Text = "More information";
                    _moreInformationLink.Click += new EventHandler(_moreInformationLink_Click);
                    _moreInformationLink.Width = this.labelPrompt.Width;
                    this.Controls.Add(_moreInformationLink);
                }
                return _moreInformationLink;
            }
        }

        void _moreInformationLink_Click(object sender, EventArgs e)
        {
            try
            {
                if (MoreInformationUrl == null)
                    throw new InvalidOperationException("'MoreInformationUrl' is null.");
                if (MoreInformationUrl.Length == 0)
                    throw new InvalidOperationException("'MoreInformationUrl' is zero-length.");

                // run...
                System.Diagnostics.Process.Start(this.MoreInformationUrl);
            }
            catch (Exception ex)
            {
                Alert.ShowWarning(this, "The 'more information' operation failed.", ex);
            }
        }

        /// <summary>
        /// Gets the DoNotShowAgainCheckBox value.
        /// </summary>
        private CheckBox DoNotShowAgainCheckBox
        {
            get
            {
                if (_doNotShowAgainCheckBox == null)
                {
                    _doNotShowAgainCheckBox = new CheckBox();
                    _doNotShowAgainCheckBox.Text = "Do not show this message again";
                    _doNotShowAgainCheckBox.Width = this.labelPrompt.Width;
                    _doNotShowAgainCheckBox.Checked = this.DoNotShowAgainDefault;
                    this.Controls.Add(_doNotShowAgainCheckBox);
                }
                return _doNotShowAgainCheckBox;
            }
        }

        private bool HasDoNotShowAgain
        {
            get
            {
                return !(string.IsNullOrEmpty(this.DoNotShowAgainKey));
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (this.DesignMode)
                return;

            try
            {
                if (string.IsNullOrEmpty(this.DoNotShowAgainKey) || !(Runtime.IsStarted))
                    return;

                // write...
                if (DoNotShowAgainCheckBox == null)
                    throw new InvalidOperationException("'DoNotShowAgainCheckBox' is null.");
                Runtime.Current.UserSettings[this.DoNotShowAgainSettingsKey] = this.DoNotShowAgainCheckBox.Checked;
            }
            catch (Exception ex)
            {
                if (this.Log.IsWarnEnabled)
                    this.Log.Warn(string.Format("Failed to disable message '{0}'.", this.DoNotShowAgainKey), ex);
            }
        }

        private string DoNotShowAgainSettingsKey
        {
            get
            {
                return GetDoNotShowAgainSettingsKey(this.DoNotShowAgainKey);
            }
        }

        private static string GetDoNotShowAgainSettingsKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length == 0)
                throw new ArgumentException("'key' is zero-length.");

            // return...
            return "DoNotShow_" + key;
        }

        public static bool IsSetToNotShowAgain(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (key.Length == 0)
                throw new ArgumentException("'key' is zero-length.");

            // runtime...
            if (!(Runtime.IsStarted))
                return false;

            // check...
            string fullKey = GetDoNotShowAgainSettingsKey(key);
            if (fullKey == null)
                throw new InvalidOperationException("'fullKey' is null.");
            if (fullKey.Length == 0)
                throw new InvalidOperationException("'fullKey' is zero-length.");

            // get...
            bool doNotShow = Runtime.Current.UserSettings.GetBooleanValue(fullKey, false, Cultures.System, OnNotFound.ReturnNull);
            return doNotShow;
        }

        private void LayoutView()
        {
            this.SuspendLayout();
            try
            {
                // buttons...
                using(Graphics g = Graphics.FromHwnd(this.Handle))
                {
                    // max width of buttons...
                    float buttonWidth = 0;
                    int buttonY = this.labelPrompt.Top - Spacing;
                    OptionDialogButton[] buttons = this.GetButtons();
                    foreach(OptionDialogButton button in buttons)
                    {
                        SizeF size = g.MeasureString(button.Text, button.Font);
                        int useWidth = (int)size.Width + (4 * Spacing);
                        if(useWidth > buttonWidth)
                            buttonWidth = useWidth;

                        // move down...
                        button.Top = buttonY;
                        button.Left = labelPrompt.Right + Spacing;

                        // next...
                        buttonY += button.Height + Spacing;
                    }

                    // adjust...
                    if (buttonWidth < 75)
                        buttonWidth = 75;

                    // make the widths the same...
                    foreach(OptionDialogButton button in buttons)
                        button.Width = (int)buttonWidth;

                    // get the height of the label...
                    SizeF labelSize = g.MeasureString(labelPrompt.Text, labelPrompt.Font, labelPrompt.Width);
                    labelPrompt.Height = (int)labelSize.Height;

                    // so the new width is easy...
                    int newWidth = labelPrompt.Right + (int)buttonWidth + (2 * Spacing);

                    // so is the new height...
                    int newHeight = (int)labelSize.Height;

                    // walk the controls...
                    int y = labelPrompt.Bottom + Spacing;
                    foreach(Control extra in this.GetExtraControls())
                    {
                        extra.Left = this.labelPrompt.Left;   
                        extra.Top = y;
                        extra.Height = (int)g.MeasureString("Gj", extra.Font).Height + Spacing;

                        // next...
                        y += extra.Height + Spacing;
                    }

                    // set...
                    newHeight = y + (2 * Spacing);
                    if(newHeight <= buttonY)
                        newHeight = buttonY + Spacing;

                    // set...
                    this.Width = newWidth + (this.Width - this.ClientRectangle.Width);
                    this.Height = newHeight + (this.Height - this.ClientRectangle.Height);
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        private List<Control> GetExtraControls()
        {
            List<Control> results = new List<Control>();
            if(this.HasMoreInformation)
                results.Add(this.MoreInformationLink);
            if(this.HasDoNotShowAgain)
                results.Add(this.DoNotShowAgainCheckBox);

            // return...
            return results;
        }

        public bool HasMoreInformation
        {
            get
            {
                return !(string.IsNullOrEmpty(this.MoreInformationUrl));
            }
        }
	}
}
