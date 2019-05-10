// BootFX - Application framework for .NET applications
// 
// File: ScriptDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using BootFX.Common.UI.Desktop;
using System.IO;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for <see cref="ScriptDialog"/>.
	/// </summary>
	internal class ScriptDialog : BaseForm
	{
		private const string BaseText = "Script";

		private System.Windows.Forms.RichTextBox textScript;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Button buttonCopy;
        private Button buttonSaveAs;
        private System.Windows.Forms.GroupBox groupBox1;

		private void InitializeComponent()
		{
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textScript = new System.Windows.Forms.RichTextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonSaveAs = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textScript);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(736, 468);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Script:";
            // 
            // textScript
            // 
            this.textScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textScript.Location = new System.Drawing.Point(12, 20);
            this.textScript.Name = "textScript";
            this.textScript.ReadOnly = true;
            this.textScript.Size = new System.Drawing.Size(716, 440);
            this.textScript.TabIndex = 0;
            this.textScript.Text = "";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(668, 488);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            // 
            // buttonCopy
            // 
            this.buttonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopy.Location = new System.Drawing.Point(587, 488);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(75, 23);
            this.buttonCopy.TabIndex = 2;
            this.buttonCopy.Text = "&Copy";
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonSaveAs
            // 
            this.buttonSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveAs.Location = new System.Drawing.Point(506, 488);
            this.buttonSaveAs.Name = "buttonSaveAs";
            this.buttonSaveAs.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveAs.TabIndex = 3;
            this.buttonSaveAs.Text = "&Save As...";
            this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
            // 
            // ScriptDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(752, 518);
            this.Controls.Add(this.buttonSaveAs);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBox1);
            this.Name = "ScriptDialog";
            this.Text = "Database Script";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		/// <summary>
		/// Creates a new instance of <see cref="ScriptDialog"/>.
		/// </summary>
		internal ScriptDialog()
		{
			this.InitializeComponent();
		}

		private void buttonCopy_Click(object sender, System.EventArgs e)
		{
            try
            {
                Clipboard.SetDataObject(this.Script);
                Alert.ShowInformation(this, "Done.");
            }
            catch (Exception ex)
            {
                Alert.ShowWarning(this, "An error occurred.", ex);
            }
		}

		/// <summary>
		/// Gets or sets the script
		/// </summary>
		internal string Script
		{
			get
			{
				return this.textScript.Text;
			}
			set
			{
				// set the value...
				this.textScript.Text = value;

				// set...
				if(value != null)
					this.Text = string.Format("{0} - {1:n0} bytes", BaseText, value.Length);
				else
					this.Text = BaseText;
			}
		}

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new SaveFileDialog()
                {
                    Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
                })
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        File.WriteAllText(dialog.FileName, this.textScript.Text);
                        Alert.ShowInformation(this, "Done.");
                    }
                }
            }
            catch (Exception ex)
            {
                Alert.ShowWarning(this, "An error occurred.", ex);
            }
        }
    }
}
