// BootFX - Application framework for .NET applications
// 
// File: WhatFirst.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.UI.Desktop;

namespace BootFX.Common.DbUtil
{
    /// <summary>
    /// Summary description for WhatFirst.
    /// </summary>
    public class WhatFirst : BaseForm
    {
        /// <summary>
        /// Private field to support <see cref="PromptForProject"/> property.
        /// </summary>
        private bool _promptForProject;

        /// <summary>
        /// Private field to support <see cref="ProjectToLoad"/> property.
        /// </summary>
        private string _projectToLoad;

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.GroupBox groupBox1;
        private ProjectLinkLabel linkCreate;
        private ProjectLinkLabel linkExit;
        private System.Windows.Forms.Label label1;
        private ProjectLinkLabel link1;
        private BootFX.Common.DbUtil.ProjectLinkLabel linkLabel4;
        private ProjectLinkLabel link4;
        private ProjectLinkLabel link3;
        private ProjectLinkLabel link2;
        private ProjectLinkLabel link5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelVersion;
        private BootFX.Common.DbUtil.ProjectLinkLabel linkExisting;
        private System.Windows.Forms.ComboBox listStoreType;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public WhatFirst()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.Text = Generator.BaseCaption;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listStoreType = new System.Windows.Forms.ComboBox();
            this.linkExisting = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.link5 = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.link4 = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.link3 = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.link2 = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.link1 = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.linkExit = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.linkCreate = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.linkLabel4 = new BootFX.Common.DbUtil.ProjectLinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(350, 100);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            this.label.Location = new System.Drawing.Point(4, 116);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(356, 20);
            this.label.TabIndex = 1;
            this.label.Text = "DBUtil is part of BootFX";
            this.label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.listStoreType);
            this.groupBox1.Controls.Add(this.linkExisting);
            this.groupBox1.Controls.Add(this.link5);
            this.groupBox1.Controls.Add(this.link4);
            this.groupBox1.Controls.Add(this.link3);
            this.groupBox1.Controls.Add(this.link2);
            this.groupBox1.Controls.Add(this.link1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.linkExit);
            this.groupBox1.Controls.Add(this.linkCreate);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(8, 200);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 236);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What first?";
            // 
            // listStoreType
            // 
            this.listStoreType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.listStoreType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listStoreType.Location = new System.Drawing.Point(12, 20);
            this.listStoreType.Name = "listStoreType";
            this.listStoreType.Size = new System.Drawing.Size(328, 21);
            this.listStoreType.TabIndex = 9;
            this.listStoreType.SelectedIndexChanged += new System.EventHandler(this.listStoreType_SelectedIndexChanged);
            // 
            // linkExisting
            // 
            this.linkExisting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.linkExisting.Location = new System.Drawing.Point(12, 68);
            this.linkExisting.Name = "linkExisting";
            this.linkExisting.Size = new System.Drawing.Size(328, 20);
            this.linkExisting.TabIndex = 8;
            this.linkExisting.TabStop = true;
            this.linkExisting.Text = "Open an existing project...";
            this.linkExisting.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExisting_LinkClicked);
            // 
            // link5
            // 
            this.link5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.link5.Location = new System.Drawing.Point(28, 188);
            this.link5.Name = "link5";
            this.link5.Size = new System.Drawing.Size(308, 20);
            this.link5.TabIndex = 7;
            this.link5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // link4
            // 
            this.link4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.link4.Location = new System.Drawing.Point(28, 168);
            this.link4.Name = "link4";
            this.link4.Size = new System.Drawing.Size(308, 20);
            this.link4.TabIndex = 6;
            this.link4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // link3
            // 
            this.link3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.link3.Location = new System.Drawing.Point(28, 148);
            this.link3.Name = "link3";
            this.link3.Size = new System.Drawing.Size(308, 20);
            this.link3.TabIndex = 5;
            this.link3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // link2
            // 
            this.link2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.link2.Location = new System.Drawing.Point(28, 128);
            this.link2.Name = "link2";
            this.link2.Size = new System.Drawing.Size(308, 20);
            this.link2.TabIndex = 4;
            this.link2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // link1
            // 
            this.link1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.link1.Location = new System.Drawing.Point(28, 108);
            this.link1.Name = "link1";
            this.link1.Size = new System.Drawing.Size(308, 20);
            this.link1.TabIndex = 3;
            this.link1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Open a recently used project:";
            // 
            // linkExit
            // 
            this.linkExit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.linkExit.Location = new System.Drawing.Point(12, 214);
            this.linkExit.Name = "linkExit";
            this.linkExit.Size = new System.Drawing.Size(328, 18);
            this.linkExit.TabIndex = 1;
            this.linkExit.TabStop = true;
            this.linkExit.Text = "Exit";
            this.linkExit.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkExit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExit_LinkClicked);
            // 
            // linkCreate
            // 
            this.linkCreate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.linkCreate.Location = new System.Drawing.Point(12, 48);
            this.linkCreate.Name = "linkCreate";
            this.linkCreate.Size = new System.Drawing.Size(328, 20);
            this.linkCreate.TabIndex = 0;
            this.linkCreate.TabStop = true;
            this.linkCreate.Text = "Create a new project...";
            this.linkCreate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkCreate_LinkClicked);
            // 
            // linkLabel4
            // 
            this.linkLabel4.Location = new System.Drawing.Point(0, 0);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8F);
            this.label2.Location = new System.Drawing.Point(4, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(356, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "BootFX is an open source application framework for Microsoft .NET, distributed un" +
                "der the Mozilla Public License 1.1";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelVersion
            // 
            this.labelVersion.Font = new System.Drawing.Font("Verdana", 8F);
            this.labelVersion.Location = new System.Drawing.Point(4, 176);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(356, 16);
            this.labelVersion.TabIndex = 4;
            this.labelVersion.Text = "xxx";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // WhatFirst
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(364, 444);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WhatFirst";
            this.ShowInTaskbar = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BootFX DBUtil";
            this.Load += new System.EventHandler(this.WhatFirst_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void linkExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void linkCreate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Gets the projecttoload.
        /// </summary>
        internal string ProjectToLoad
        {
            get
            {
                return _projectToLoad;
            }
        }

        private void WhatFirst_Load(object sender, System.EventArgs e)
        {
            try
            {
                // version...
                this.labelVersion.Text = "Version " + typeof(Runtime).Assembly.GetName().Version.ToString();

                // subscribe...
                ProjectStore.CurrentStoreChanged += new EventHandler(ProjectStore_CurrentStoreChanged);

                // list...
                foreach (ProjectStore store in ProjectStore.Stores)
                    this.listStoreType.Items.Add(new ProjectStoreListItem(store));
                this.listStoreType.SelectedIndex = 0;

                // update...
                this.RefreshMruList();
            }
            catch (Exception ex)
            {
                Alert.ShowWarning(this, "Failed to handle most-recently-used list.", ex);
            }
        }

        private void RefreshMruList()
        {
            // mru...
            MruList list = ProjectStore.CurrentStore.GetMruItems();
            if (list == null)
                throw new InvalidOperationException("list is null.");

            // set...
            const int maxItems = 5;
            for (int index = 0; index < maxItems; index++)
            {
                ProjectLinkLabel link = null;
                switch (index)
                {
                    case 0:
                        link = this.link1;
                        break;
                    case 1:
                        link = this.link2;
                        break;
                    case 2:
                        link = this.link3;
                        break;
                    case 3:
                        link = this.link4;
                        break;
                    case 4:
                        link = this.link5;
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", index, index.GetType()));
                }

                // set...
                if (list.Count > index)
                {
                    link.FilePath = (string)((IList)list)[index];
                    link.Enabled = true;
                }
                else
                {
                    link.Text = "(None available)";
                    link.Enabled = false;
                }
            }
        }

        private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _projectToLoad = ((ProjectLinkLabel)sender).FilePath;
            this.DialogResult = DialogResult.OK;
        }

        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                Stream stream = this.GetType().Assembly.GetManifestResourceStream("BootFX.Common.DbUtil.Resources.Logo.png");
                if (stream != null)
                {
                    Image image = Image.FromStream(stream);
                    if (image == null)
                        throw new InvalidOperationException("image is null.");

                    // render...
                    e.Graphics.DrawImage(image, this.pictureBox1.ClientRectangle);
                }
            }
            catch
            {
                // no-op...
            }
        }

        private void linkExisting_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            _promptForProject = true;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Gets the promptforproject.
        /// </summary>
        internal bool PromptForProject
        {
            get
            {
                return _promptForProject;
            }
        }

        private void ProjectStore_CurrentStoreChanged(object sender, EventArgs e)
        {
            this.RefreshMruList();
        }

        private void listStoreType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ProjectStore.CurrentStore = ((ProjectStoreListItem)this.listStoreType.SelectedItem).Store;
        }
    }
}
