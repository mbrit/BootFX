// BootFX - Application framework for .NET applications
// 
// File: Generator.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Security.Cryptography;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using BootFX.Common.UI;
using BootFX.Common.UI.Desktop;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.DbUtil;
using BootFX.Common.Entities;
using BootFX.Common.Entities.Attributes;
using BootFX.Common.Management;
using BootFX.Common.CodeGeneration;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Generator : BaseForm
	{
		/// <summary>
		/// Private field to support <c>PromptToLoad</c> property.
		/// </summary>
		private bool _promptToLoad;
		
		/// <summary>
		/// Private field to support <c>ProjectToLoadOnStart</c> property.
		/// </summary>
		private string _projectToLoadOnStart;
		
		/// <summary>
		/// Private field to support <c>Engine</c> property.
		/// </summary>
		private GenerateEngine _engine = new GenerateEngine();
		
		private const string XSplitterKey = "XSplitter";
		private const string YSplitterKey = "YSplitter";

		private delegate void GenerateOKDelegate(GenerateState state, bool cancelled);
		private delegate bool GenerateNeedsCheckoutDelegate(GenerateState state, string[][] files);
		private delegate void GenerateFailedDelegate(GenerateState state, Exception ex);

		/// <summary>
		/// Private field to support <c>GenerateThread</c> property.
		/// </summary>
		private Thread _generateThread = null;
		
		/// <summary>
		/// Private field to support <c>AppDomainOrdinal</c> property.
		/// </summary>
		private int _revisionOrdinal = 0;
		
		/// <summary>
		/// Defines the base caption.
		/// </summary>
		internal const string BaseCaption = "BootFX DBUtil";

		/// <summary>
		/// Private field to support <c>Project</c> property.
		/// </summary>
		private Project _project;

		/// <summary>
		/// Defines the project file filter.
		/// </summary>
		private string SchemaFileFilter = "DBUtil Schema XML Files (*.xml)|*.xml|All Files (*.*)|*.*||";
		private System.Windows.Forms.RichTextBox textCode;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuHelpAbout;
		private System.Windows.Forms.MenuItem menuDatabaseConnect;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuDatabaseMerge;
		private System.Windows.Forms.MenuItem menuFileSaveSchema;
		private System.Windows.Forms.MenuItem menuProjectGenerate;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuProjectSettings;
		private System.Windows.Forms.RadioButton radioEntityBase;
		private System.Windows.Forms.RadioButton radioEntity;
		private System.Windows.Forms.LinkLabel linkRefresh;
		private System.Windows.Forms.RadioButton radioCollectionBase;
		private System.Windows.Forms.RadioButton radioCollection;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuViewEntityBase;
		private System.Windows.Forms.MenuItem menuViewEntity;
		private System.Windows.Forms.MenuItem menuViewCollectionBase;
		private System.Windows.Forms.MenuItem menuViewCollection;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuViewRefreshPreview;
		private System.Windows.Forms.RadioButton radioSchemaXml;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem menuFileSaveProject;
		private System.Windows.Forms.MenuItem menuNewProject;
		private System.Windows.Forms.MenuItem menuFileOpenProject;
		private System.Windows.Forms.MenuItem menuFileSaveProjectAs;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuProjectReloadSchema;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.LinkLabel linkViewInNotepad;
        private System.Windows.Forms.RadioButton radioDto;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuDatabaseGetFromScratchScript;
		private System.Windows.Forms.MenuItem menuDatabaseShowPopulateData;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.RadioButton radioDtoBase;
		private ObjectTreeView treeObjects;
		private System.Windows.Forms.MenuItem menuExit;
		private System.Windows.Forms.MenuItem menuItem20;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.PropertyGrid gridProperties;
		private System.Windows.Forms.Splitter splitterY;
		private System.Windows.Forms.Splitter splitterX;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuSetAllGenerated;
		private System.Windows.Forms.MenuItem menuSetAllNotGenerated;
		private System.Windows.Forms.MenuItem menuItem21;
        private System.Windows.Forms.MenuItem menuDatabaseSprocs;
        private IContainer components;

		public Generator()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.Project = new Project();

			// auto...
			this.AutoSavePosition = true;
		}

		public Generator(string projectToLoad, bool promptToLoad) : this()
		{
			// load...
			_projectToLoadOnStart = projectToLoad;
			_promptToLoad = promptToLoad;
		}

		/// <summary>
		/// Gets the prompttoload.
		/// </summary>
		private bool PromptToLoad
		{
			get
			{
				// returns the value...
				return _promptToLoad;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			this.DisposeGenerateThread();

			if( disposing )
			{
				if (components != null) 
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
            this.components = new System.ComponentModel.Container();
            this.textCode = new System.Windows.Forms.RichTextBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuFile = new System.Windows.Forms.MenuItem();
            this.menuNewProject = new System.Windows.Forms.MenuItem();
            this.menuFileOpenProject = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuFileSaveProject = new System.Windows.Forms.MenuItem();
            this.menuFileSaveProjectAs = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.menuDatabaseMerge = new System.Windows.Forms.MenuItem();
            this.menuFileSaveSchema = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem20 = new System.Windows.Forms.MenuItem();
            this.menuExit = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuViewEntityBase = new System.Windows.Forms.MenuItem();
            this.menuViewEntity = new System.Windows.Forms.MenuItem();
            this.menuViewCollectionBase = new System.Windows.Forms.MenuItem();
            this.menuViewCollection = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.menuItem17 = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.menuItem19 = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.menuViewRefreshPreview = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuDatabaseConnect = new System.Windows.Forms.MenuItem();
            this.menuProjectReloadSchema = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuProjectGenerate = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuSetAllGenerated = new System.Windows.Forms.MenuItem();
            this.menuSetAllNotGenerated = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuProjectSettings = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuDatabaseGetFromScratchScript = new System.Windows.Forms.MenuItem();
            this.menuDatabaseSprocs = new System.Windows.Forms.MenuItem();
            this.menuItem21 = new System.Windows.Forms.MenuItem();
            this.menuDatabaseShowPopulateData = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuHelpAbout = new System.Windows.Forms.MenuItem();
            this.radioEntityBase = new System.Windows.Forms.RadioButton();
            this.radioEntity = new System.Windows.Forms.RadioButton();
            this.radioCollectionBase = new System.Windows.Forms.RadioButton();
            this.radioCollection = new System.Windows.Forms.RadioButton();
            this.linkRefresh = new System.Windows.Forms.LinkLabel();
            this.radioSchemaXml = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeObjects = new BootFX.Common.UI.Desktop.ObjectTreeView();
            this.splitterY = new System.Windows.Forms.Splitter();
            this.gridProperties = new System.Windows.Forms.PropertyGrid();
            this.splitterX = new System.Windows.Forms.Splitter();
            this.panel4 = new System.Windows.Forms.Panel();
            this.radioDto = new System.Windows.Forms.RadioButton();
            this.radioDtoBase = new System.Windows.Forms.RadioButton();
            this.linkViewInNotepad = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // textCode
            // 
            this.textCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textCode.Location = new System.Drawing.Point(0, 48);
            this.textCode.Name = "textCode";
            this.textCode.ReadOnly = true;
            this.textCode.Size = new System.Drawing.Size(720, 580);
            this.textCode.TabIndex = 10;
            this.textCode.Text = "";
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFile,
            this.menuItem8,
            this.menuItem4,
            this.menuItem11,
            this.menuItem5});
            // 
            // menuFile
            // 
            this.menuFile.Index = 0;
            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuNewProject,
            this.menuFileOpenProject,
            this.menuItem12,
            this.menuFileSaveProject,
            this.menuFileSaveProjectAs,
            this.menuItem14,
            this.menuDatabaseMerge,
            this.menuFileSaveSchema,
            this.menuItem3,
            this.menuItem20,
            this.menuExit});
            this.menuFile.Text = "&File";
            this.menuFile.Popup += new System.EventHandler(this.menuFile_Popup);
            // 
            // menuNewProject
            // 
            this.menuNewProject.Index = 0;
            this.menuNewProject.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.menuNewProject.Text = "&New Project";
            this.menuNewProject.Click += new System.EventHandler(this.menuNewProject_Click);
            // 
            // menuFileOpenProject
            // 
            this.menuFileOpenProject.Index = 1;
            this.menuFileOpenProject.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.menuFileOpenProject.Text = "&Open Project...";
            this.menuFileOpenProject.Click += new System.EventHandler(this.menuFileOpenProject_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 2;
            this.menuItem12.Text = "-";
            // 
            // menuFileSaveProject
            // 
            this.menuFileSaveProject.Index = 3;
            this.menuFileSaveProject.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuFileSaveProject.Text = "&Save Project";
            this.menuFileSaveProject.Click += new System.EventHandler(this.menuFileSaveProject_Click);
            // 
            // menuFileSaveProjectAs
            // 
            this.menuFileSaveProjectAs.Index = 4;
            this.menuFileSaveProjectAs.Text = "Save Project &As...";
            this.menuFileSaveProjectAs.Click += new System.EventHandler(this.menuFileSaveProjectAs_Click);
            // 
            // menuItem14
            // 
            this.menuItem14.Index = 5;
            this.menuItem14.Text = "-";
            // 
            // menuDatabaseMerge
            // 
            this.menuDatabaseMerge.Index = 6;
            this.menuDatabaseMerge.Text = "&Import Schema...";
            this.menuDatabaseMerge.Click += new System.EventHandler(this.menuDatabaseMerge_Click);
            // 
            // menuFileSaveSchema
            // 
            this.menuFileSaveSchema.Index = 7;
            this.menuFileSaveSchema.Text = "&Export Schema...";
            this.menuFileSaveSchema.Click += new System.EventHandler(this.menuFileSaveSchema_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 8;
            this.menuItem3.Text = "-";
            // 
            // menuItem20
            // 
            this.menuItem20.Index = 9;
            this.menuItem20.Text = "-";
            // 
            // menuExit
            // 
            this.menuExit.Index = 10;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 1;
            this.menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem10,
            this.menuItem9,
            this.menuViewEntityBase,
            this.menuViewEntity,
            this.menuViewCollectionBase,
            this.menuViewCollection,
            this.menuItem15,
            this.menuItem17,
            this.menuItem18,
            this.menuItem19,
            this.menuItem13,
            this.menuViewRefreshPreview});
            this.menuItem8.Text = "&View";
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 0;
            this.menuItem10.Shortcut = System.Windows.Forms.Shortcut.F4;
            this.menuItem10.Text = "&Schema XML";
            this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 1;
            this.menuItem9.Text = "-";
            // 
            // menuViewEntityBase
            // 
            this.menuViewEntityBase.Index = 2;
            this.menuViewEntityBase.Shortcut = System.Windows.Forms.Shortcut.F6;
            this.menuViewEntityBase.Text = "&Entity Base";
            this.menuViewEntityBase.Click += new System.EventHandler(this.menuViewEntityBase_Click);
            // 
            // menuViewEntity
            // 
            this.menuViewEntity.Index = 3;
            this.menuViewEntity.Shortcut = System.Windows.Forms.Shortcut.F7;
            this.menuViewEntity.Text = "E&ntity";
            this.menuViewEntity.Click += new System.EventHandler(this.menuViewEntity_Click);
            // 
            // menuViewCollectionBase
            // 
            this.menuViewCollectionBase.Index = 4;
            this.menuViewCollectionBase.Shortcut = System.Windows.Forms.Shortcut.F8;
            this.menuViewCollectionBase.Text = "&Collection Base";
            this.menuViewCollectionBase.Click += new System.EventHandler(this.menuViewCollectionBase_Click);
            // 
            // menuViewCollection
            // 
            this.menuViewCollection.Index = 5;
            this.menuViewCollection.Shortcut = System.Windows.Forms.Shortcut.F9;
            this.menuViewCollection.Text = "Co&llection";
            this.menuViewCollection.Click += new System.EventHandler(this.menuViewCollection_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 6;
            this.menuItem15.Shortcut = System.Windows.Forms.Shortcut.CtrlF6;
            this.menuItem15.Text = "&WS Entity Base";
            this.menuItem15.Click += new System.EventHandler(this.menuItem15_Click);
            // 
            // menuItem17
            // 
            this.menuItem17.Index = 7;
            this.menuItem17.Shortcut = System.Windows.Forms.Shortcut.CtrlF7;
            this.menuItem17.Text = "W&S Entity";
            this.menuItem17.Click += new System.EventHandler(this.menuItem17_Click);
            // 
            // menuItem18
            // 
            this.menuItem18.Index = 8;
            this.menuItem18.Shortcut = System.Windows.Forms.Shortcut.CtrlF8;
            this.menuItem18.Text = "We&b Service Base";
            this.menuItem18.Click += new System.EventHandler(this.menuItem18_Click);
            // 
            // menuItem19
            // 
            this.menuItem19.Index = 9;
            this.menuItem19.Shortcut = System.Windows.Forms.Shortcut.CtrlF9;
            this.menuItem19.Text = "Web Ser&vice";
            this.menuItem19.Click += new System.EventHandler(this.menuItem19_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 10;
            this.menuItem13.Text = "-";
            // 
            // menuViewRefreshPreview
            // 
            this.menuViewRefreshPreview.Index = 11;
            this.menuViewRefreshPreview.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuViewRefreshPreview.Text = "&Refresh Preview";
            this.menuViewRefreshPreview.Click += new System.EventHandler(this.menuViewRefreshPreview_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDatabaseConnect,
            this.menuProjectReloadSchema,
            this.menuItem6,
            this.menuProjectGenerate,
            this.menuItem1,
            this.menuItem2,
            this.menuItem7,
            this.menuProjectSettings});
            this.menuItem4.Text = "&Project";
            // 
            // menuDatabaseConnect
            // 
            this.menuDatabaseConnect.Index = 0;
            this.menuDatabaseConnect.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
            this.menuDatabaseConnect.Text = "&Connect to Database...";
            this.menuDatabaseConnect.Click += new System.EventHandler(this.menuFileConnect_Click);
            // 
            // menuProjectReloadSchema
            // 
            this.menuProjectReloadSchema.Index = 1;
            this.menuProjectReloadSchema.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.menuProjectReloadSchema.Text = "&Reload Schema";
            this.menuProjectReloadSchema.Click += new System.EventHandler(this.menuProjectReloadSchema_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 2;
            this.menuItem6.Text = "-";
            // 
            // menuProjectGenerate
            // 
            this.menuProjectGenerate.Index = 3;
            this.menuProjectGenerate.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
            this.menuProjectGenerate.Text = "&Generate Code...";
            this.menuProjectGenerate.Click += new System.EventHandler(this.menuProjectGenerate_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 5;
            this.menuItem1.Text = "-";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 6;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuSetAllGenerated,
            this.menuSetAllNotGenerated});
            this.menuItem2.Text = "&Utilities";
            // 
            // menuSetAllGenerated
            // 
            this.menuSetAllGenerated.Index = 0;
            this.menuSetAllGenerated.Text = "&Set All To Be Generated...";
            this.menuSetAllGenerated.Click += new System.EventHandler(this.menuSetAllGenerated_Click);
            // 
            // menuSetAllNotGenerated
            // 
            this.menuSetAllNotGenerated.Index = 1;
            this.menuSetAllNotGenerated.Text = "Set All To Be &Not Generated...";
            this.menuSetAllNotGenerated.Click += new System.EventHandler(this.menuSetAllNotGenerated_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 7;
            this.menuItem7.Text = "-";
            // 
            // menuProjectSettings
            // 
            this.menuProjectSettings.Index = 8;
            this.menuProjectSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlT;
            this.menuProjectSettings.Text = "Settings...";
            this.menuProjectSettings.Click += new System.EventHandler(this.menuProjectSettings_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 3;
            this.menuItem11.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDatabaseGetFromScratchScript,
            this.menuDatabaseSprocs,
            this.menuItem21,
            this.menuDatabaseShowPopulateData});
            this.menuItem11.Text = "&Database";
            // 
            // menuDatabaseGetFromScratchScript
            // 
            this.menuDatabaseGetFromScratchScript.Index = 0;
            this.menuDatabaseGetFromScratchScript.Text = "&Show Create Database Script...";
            this.menuDatabaseGetFromScratchScript.Click += new System.EventHandler(this.menuDatabaseGetFromScratchScript_Click);
            // 
            // menuDatabaseSprocs
            // 
            this.menuDatabaseSprocs.Index = 1;
            this.menuDatabaseSprocs.Text = "Show Recreate &Stored Procedures Script...";
            this.menuDatabaseSprocs.Click += new System.EventHandler(this.menuDatabaseSprocs_Click);
            // 
            // menuItem21
            // 
            this.menuItem21.Index = 2;
            this.menuItem21.Text = "-";
            // 
            // menuDatabaseShowPopulateData
            // 
            this.menuDatabaseShowPopulateData.Index = 3;
            this.menuDatabaseShowPopulateData.Text = "Show &Populate Data Script...";
            this.menuDatabaseShowPopulateData.Click += new System.EventHandler(this.menuDatabaseShowPopulateData_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuHelpAbout});
            this.menuItem5.Text = "&Help";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Index = 0;
            this.menuHelpAbout.Text = "&About...";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // radioEntityBase
            // 
            this.radioEntityBase.Checked = true;
            this.radioEntityBase.Location = new System.Drawing.Point(104, 4);
            this.radioEntityBase.Name = "radioEntityBase";
            this.radioEntityBase.Size = new System.Drawing.Size(80, 16);
            this.radioEntityBase.TabIndex = 11;
            this.radioEntityBase.TabStop = true;
            this.radioEntityBase.Text = "&Entity base";
            this.radioEntityBase.CheckedChanged += new System.EventHandler(this.radioEntityBase_CheckedChanged);
            // 
            // radioEntity
            // 
            this.radioEntity.Location = new System.Drawing.Point(204, 4);
            this.radioEntity.Name = "radioEntity";
            this.radioEntity.Size = new System.Drawing.Size(70, 16);
            this.radioEntity.TabIndex = 12;
            this.radioEntity.Text = "E&ntity";
            this.radioEntity.CheckedChanged += new System.EventHandler(this.radioEntity_CheckedChanged);
            // 
            // radioCollectionBase
            // 
            this.radioCollectionBase.Location = new System.Drawing.Point(280, 4);
            this.radioCollectionBase.Name = "radioCollectionBase";
            this.radioCollectionBase.Size = new System.Drawing.Size(104, 16);
            this.radioCollectionBase.TabIndex = 13;
            this.radioCollectionBase.Text = "&Collection base";
            this.radioCollectionBase.CheckedChanged += new System.EventHandler(this.radioCollectionBase_CheckedChanged);
            // 
            // radioCollection
            // 
            this.radioCollection.Location = new System.Drawing.Point(400, 4);
            this.radioCollection.Name = "radioCollection";
            this.radioCollection.Size = new System.Drawing.Size(76, 16);
            this.radioCollection.TabIndex = 14;
            this.radioCollection.Text = "Co&llection";
            this.radioCollection.CheckedChanged += new System.EventHandler(this.radioCollection_CheckedChanged);
            // 
            // linkRefresh
            // 
            this.linkRefresh.Location = new System.Drawing.Point(492, 4);
            this.linkRefresh.Name = "linkRefresh";
            this.linkRefresh.Size = new System.Drawing.Size(44, 16);
            this.linkRefresh.TabIndex = 15;
            this.linkRefresh.TabStop = true;
            this.linkRefresh.Text = "Refresh";
            this.linkRefresh.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkRefresh_LinkClicked);
            // 
            // radioSchemaXml
            // 
            this.radioSchemaXml.Location = new System.Drawing.Point(4, 4);
            this.radioSchemaXml.Name = "radioSchemaXml";
            this.radioSchemaXml.Size = new System.Drawing.Size(88, 16);
            this.radioSchemaXml.TabIndex = 16;
            this.radioSchemaXml.Text = "&Schema XML";
            this.radioSchemaXml.CheckedChanged += new System.EventHandler(this.radioSchemaXml_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.treeObjects);
            this.panel1.Controls.Add(this.splitterY);
            this.panel1.Controls.Add(this.gridProperties);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.panel1.Size = new System.Drawing.Size(256, 633);
            this.panel1.TabIndex = 17;
            // 
            // treeObjects
            // 
            this.treeObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeObjects.HideSelection = false;
            this.treeObjects.ImageIndex = 0;
            this.treeObjects.Location = new System.Drawing.Point(4, 4);
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.SelectedImageIndex = 0;
            this.treeObjects.Size = new System.Drawing.Size(252, 341);
            this.treeObjects.TabIndex = 5;
            this.treeObjects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeObjects_AfterSelect);
            // 
            // splitterY
            // 
            this.splitterY.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterY.Location = new System.Drawing.Point(4, 345);
            this.splitterY.Name = "splitterY";
            this.splitterY.Size = new System.Drawing.Size(252, 8);
            this.splitterY.TabIndex = 8;
            this.splitterY.TabStop = false;
            // 
            // gridProperties
            // 
            this.gridProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gridProperties.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.gridProperties.Location = new System.Drawing.Point(4, 353);
            this.gridProperties.Name = "gridProperties";
            this.gridProperties.Size = new System.Drawing.Size(252, 276);
            this.gridProperties.TabIndex = 9;
            this.gridProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.gridProperties_PropertyValueChanged);
            this.gridProperties.SelectedObjectsChanged += new System.EventHandler(this.gridProperties_SelectedViewItemsChanged);
            // 
            // splitterX
            // 
            this.splitterX.Location = new System.Drawing.Point(256, 0);
            this.splitterX.Name = "splitterX";
            this.splitterX.Size = new System.Drawing.Size(8, 633);
            this.splitterX.TabIndex = 18;
            this.splitterX.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.radioDto);
            this.panel4.Controls.Add(this.radioDtoBase);
            this.panel4.Controls.Add(this.linkViewInNotepad);
            this.panel4.Controls.Add(this.textCode);
            this.panel4.Controls.Add(this.radioEntityBase);
            this.panel4.Controls.Add(this.radioEntity);
            this.panel4.Controls.Add(this.radioCollectionBase);
            this.panel4.Controls.Add(this.radioCollection);
            this.panel4.Controls.Add(this.linkRefresh);
            this.panel4.Controls.Add(this.radioSchemaXml);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(264, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(724, 633);
            this.panel4.TabIndex = 19;
            // 
            // radioDto
            // 
            this.radioDto.Location = new System.Drawing.Point(204, 24);
            this.radioDto.Name = "radioDto";
            this.radioDto.Size = new System.Drawing.Size(80, 16);
            this.radioDto.TabIndex = 19;
            this.radioDto.Text = "DTO";
            this.radioDto.CheckedChanged += new System.EventHandler(this.radioInterface_CheckedChanged);
            // 
            // radioDtoBase
            // 
            this.radioDtoBase.Location = new System.Drawing.Point(104, 24);
            this.radioDtoBase.Name = "radioDtoBase";
            this.radioDtoBase.Size = new System.Drawing.Size(100, 16);
            this.radioDtoBase.TabIndex = 18;
            this.radioDtoBase.Text = "&DTO Base";
            this.radioDtoBase.CheckedChanged += new System.EventHandler(this.radioInterfaceBase_CheckedChanged);
            // 
            // linkViewInNotepad
            // 
            this.linkViewInNotepad.Location = new System.Drawing.Point(548, 4);
            this.linkViewInNotepad.Name = "linkViewInNotepad";
            this.linkViewInNotepad.Size = new System.Drawing.Size(120, 16);
            this.linkViewInNotepad.TabIndex = 17;
            this.linkViewInNotepad.TabStop = true;
            this.linkViewInNotepad.Text = "View in External Viewer";
            this.linkViewInNotepad.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkViewInNotepad_LinkClicked);
            // 
            // Generator
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(988, 633);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.splitterX);
            this.Controls.Add(this.panel1);
            this.Menu = this.mainMenu1;
            this.Name = "Generator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BootFX DbUtil";
            this.Load += new System.EventHandler(this.Generator_Load);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			// setup...
			var startup = new Startup("BootFX", "BootFX DBUtil", "Client", Runtime.Version);
			startup.StartApplication += new EventHandler(startup_StartApplication);
			startup.DefaultFormIcon = DesktopResourceHelper.GetIcon("BootFX.Common.DbUtil.App.ico");
			startup.Run();
		}

		private void Generator_Load(object sender, System.EventArgs e)
		{
			this.UpdateCaption();
			this.RefreshProjectMruMenu();

			// update...
			if(this.ProjectToLoadOnStart != null && this.ProjectToLoadOnStart.Length > 0)
				this.OpenProject(this.ProjectToLoadOnStart);
			if(this.PromptToLoad)
				this.OpenProject();
		}

		/// <summary>
		/// Loads the schema.
		/// </summary>
		private void LoadSchema(IOperationItem op)
		{
			// load...
			this.CreateSchema(op);
		}

		/// <summary>
		/// Returns true if the schema has been created.
		/// </summary>
		private bool IsSchemaCreated
		{
			get
			{
				if(this.Schema == null)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void RefreshView()
		{
			// caption...
			this.UpdateCaption();

			// Assign the custom properties
			//			this.extendedProperties.Enabled = true;
			//			this.extendedProperties.ConnectionSettings = LocalSettings.ConnectionSettings;
			//			this.extendedProperties.ExtendedPropertySettings = Project.ExtendedPropertySettings;

			// tree...
			this.treeObjects.Nodes.Clear();
			if(this.Schema != null)
			{
				//this.extendedProperties.Tables = this.Schema.Tables;
				foreach(SqlTable table in this.Schema.Tables)
					this.treeObjects.Nodes.Add(new SqlTableNode(table));

				// mbr - 15-06-2006 - sprocs...
				this.treeObjects.Nodes.Add(new SqlProceduresNode(this.Schema));
			}

			// show...
			this.ShowPreview();
		}

		/// <summary>
		/// Shows the not generated message.
		/// </summary>
		private void ShowNotGeneratedMessage(ISqlProgrammable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			this.textCode.Text = string.Format("'{0}' is not included in the generation set.", table.Name);
		}

		/// <summary>
		/// Gets the namespacename.
		/// </summary>
		private string NamespaceName
		{
			get
			{
				// returns the value...
				if(Settings == null)
					throw new InvalidOperationException("Settings is null.");
				if(Settings.NamespaceName == null)
					throw new InvalidOperationException("'Settings.NamespaceName' is null.");
				if(Settings.NamespaceName.Length == 0)
					throw new InvalidOperationException("'Settings.NamespaceName' is zero-length.");

				// return...
				return this.Settings.NamespaceName;
			}
		}

        private string DtoNamespaceName
        {
            get
            {
                return this.NamespaceName + ".Dto";
            }
        }

		private void buttonGenerate_Click(object sender, System.EventArgs e)
		{
			Generate();
		}

		/// <summary>
		/// Checks the state before generation or compilation occurs.
		/// </summary>
		/// <returns></returns>
		private bool CheckPreGenerate()
		{
			// loadeD?
			if(this.IsSchemaCreated == false)
			{
				Alert.ShowWarning(this, "You must connect to the database before you can generate entities.");
				return false;
			}
			if(this.Schema.Tables.Count == 0)
			{
				Alert.ShowWarning(this, "The schema contains no tables.");
				return false;
			}
			if(this.NamespaceName == null || this.NamespaceName.Length == 0)
			{
				Alert.ShowWarning(this, "The namespace has not been specified.");
				return false;
			}

			// ok...
			return true;
		}

		/// <summary>
		/// Generates the selected tables.
		/// </summary>
		private void Generate()
		{
			if(this.CheckPreGenerate() == false)
				return;

			// check...
			if(LocalSettings == null)
				throw new InvalidOperationException("LocalSettings is null.");

			// mjr - 07-07-2005 - check...
			while(!(this.LocalSettings.AreFolderPathsSet()))
			{
				if(!(this.ShowSettings()))
					return;
			}

			// ask...
			this.LazyGenerate(this.LocalSettings.EntitiesFolderPath, this.LocalSettings.DtoFolderPath, 
				this.LocalSettings.ProceduresFolderPath);
		}

		/// <summary>
		/// Generates in a thread.
		/// </summary>
		/// <param name="entitiesFolderPath"></param>
		/// <param name="servicesFolderPath"></param>
		private void LazyGenerate(string entitiesFolderPath, string servicesFolderPath, string proceduresFolderPath)
		{
			if(entitiesFolderPath == null)
				throw new ArgumentNullException("entitiesFolderPath");
			if(entitiesFolderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'entitiesFolderPath' is zero-length.");
			if(proceduresFolderPath == null)
				throw new ArgumentNullException("proceduresFolderPath");
			if(proceduresFolderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'proceduresFolderPath' is zero-length.");			
		
			if(this.CheckPreGenerate() == false)
				return;
			
			// running?
			if(this.GenerateThread != null && this.GenerateThread.IsAlive)  
			{
				Alert.ShowWarning(this, "Generation is already running.");
				return;
			}

			// create...
			GenerateState state = new GenerateState();
			state.Main = this;
			state.EntitiesFolderPath = entitiesFolderPath;
			state.ServicesFolderPath = servicesFolderPath;
			state.ProceduresFolderPath = proceduresFolderPath;
			state.Dialog = new FloatingOperationDialog("Generating entities...");

			// run...
			this.DisposeGenerateThread();
			_generateThread = new Thread(new ThreadStart(state.ThreadEntryPoint));
			_generateThread.IsBackground = true;
			_generateThread.Name = "Generation Thread";
			_generateThread.Start();

			// show...
			state.Dialog.ShowDialog(this);
		}

		private class GenerateState
		{
			internal Generator Main;
			internal string EntitiesFolderPath;
			internal string ServicesFolderPath;
			internal string ProceduresFolderPath;
			internal FloatingOperationDialog Dialog;

			internal void ThreadEntryPoint()
			{
				try
				{
					if(Main == null)
						throw new InvalidOperationException("Main is null.");

					bool cancelled = false;
					while(true)
					{
						// run...
						ArrayList lockedFiles = new ArrayList();
						this.Engine.Generate(this.Main.Project, this.EntitiesFolderPath, this.ServicesFolderPath, this.ProceduresFolderPath,
							lockedFiles, this.Dialog.CurrentItem);

						// if?
						if(lockedFiles.Count > 0)
						{
							// what now?
							if(!(this.Main.GenerateNeedsCheckout(this, (string[][])lockedFiles.ToArray(typeof(string[])))))
							{
								cancelled = true;
								break;
							}
						}
						else
							break;
					}

					// ok...
					this.Main.GenerateOK(this, cancelled);
				}
				catch(Exception ex)
				{
					this.Main.GenerateFailed(this, ex);
				}
			}

			private GenerateEngine Engine
			{
				get
				{
					if(Main == null)
						throw new InvalidOperationException("Main is null.");
					return this.Main.Engine;
				}
			}
		}

		/// <summary>
		/// Called when generation needs check out.
		/// </summary>
		/// <param name="files"></param>
		private bool GenerateNeedsCheckout(GenerateState state, string[][] files)
		{
			if(files == null)
				throw new ArgumentNullException("files");
			
			// flip...
			if(this.InvokeRequired)
			{
				GenerateNeedsCheckoutDelegate d = new GenerateNeedsCheckoutDelegate(this.GenerateNeedsCheckout);
				return (bool)this.Invoke(d, new object[] { state, files });
			}

			// show...
			CheckoutDialog dialog = new CheckoutDialog();
			dialog.SetFiles(files);
			DialogResult result = dialog.ShowDialog(this);
			if(result == DialogResult.Cancel)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Generation finished OK.
		/// </summary>
		private void GenerateOK(GenerateState state, bool cancelled)
		{
			if(state == null)
				throw new ArgumentNullException("state");
			
			// flip?
			if(this.InvokeRequired)
			{
				GenerateOKDelegate d = new GenerateOKDelegate(this.GenerateOK);
				this.Invoke(d, new object[] { state, cancelled });
				return;
			}

			// alert...
			if(state.Dialog == null)
				throw new InvalidOperationException("state.Dialog is null.");
			Alert.ShowInformation(state.Dialog, "Generation completed OK.");

			// close...
			state.Dialog.Close();
		}

		/// <summary>
		/// Generation failed.
		/// </summary>
		/// <param name="ex"></param>
		private void GenerateFailed(GenerateState state, Exception ex)
		{
			if(ex == null)
				throw new ArgumentNullException("ex");
			
			// flip...
			if(this.InvokeRequired)
			{
				GenerateFailedDelegate d = new GenerateFailedDelegate(this.GenerateFailed);
				this.Invoke(d, new object[] { state, ex });
				return;
			}

			// alert...
			if(state.Dialog == null)
				throw new InvalidOperationException("state.Dialog is null.");
			Alert.ShowWarning(this, "Entity generation failed.", ex);

			// close...
			state.Dialog.Close();
		}

		/// <summary>
		/// Disposes the generate thread.
		/// </summary>
		private void DisposeGenerateThread()
		{
			if(_generateThread != null)
			{
				if(_generateThread.IsAlive)
					_generateThread.Abort();
				_generateThread.Join();
				_generateThread = null;
			}
		}

		/// <summary>
		/// Gets the generatethread.
		/// </summary>
		private Thread GenerateThread
		{
			get
			{
				// returns the value...
				return _generateThread;
			}
		}

		private ISqlProgrammable SelectedProgrammableItem
		{
			get
			{
				if(this.SelectedTable != null)
					return this.SelectedTable;
				if(this.SelectedProcedure != null)
					return this.SelectedProcedure;
				else
					return null;
			}
		}

		/// <summary>
		/// Shows the given preview.
		/// </summary>
		/// <param name="fileType"></param>
		private void ShowPreview(EntityCodeFileType fileType)
		{
			ISqlProgrammable table = this.SelectedProgrammableItem;
			if(table == null)
			{
				this.textCode.Text = string.Empty;
				return;
			}

			// check...
            var generate = table.Generate;
            if (generate)
            {
                if ((fileType == EntityCodeFileType.Dto || fileType == EntityCodeFileType.DtoBase) && !(this.SelectedTable.GenerateDto))
                    generate = false;
            }

            // show...
			if(generate)
			{
				try
				{
					// mbr - 21-09-2007 - context
					EntityGenerationContext context = null;
					var generator = this.Engine.GetEntityGenerator(this.Project, ref context);
					if(generator == null)
						throw new InvalidOperationException("generator is null.");
					if(context == null)
						throw new InvalidOperationException("context is null.");

					// get...
					var compileUnit = generator.GetCompileUnit(table, fileType, context);

                    // namespace...
                    var ns = this.NamespaceName;
                    if (fileType == EntityCodeFileType.Dto || fileType == EntityCodeFileType.DtoBase)
                        ns = this.DtoNamespaceName;

					// show...
					string code = CodeDomExtender.ToString(generator.GetNamespace(ns, compileUnit, context), Language.CSharp);
					if(this.Settings.TargetVersion != DotNetVersion.V1)
						code = CodeDomExtender.AddPartialKeyword(code, this.Settings.Language);

					// set...
					this.textCode.Text = code;
				}
				catch(Exception ex)
				{
					this.textCode.Text = ex.ToString();
				}
			}
			else
				this.ShowNotGeneratedMessage(table);
		}

		private void buttonSave_Click(object sender, System.EventArgs e)
		{
			this.SaveSchemaAs();
		}

		/// <summary>
		/// Saves the schema.
		/// </summary>
		private void SaveSchemaAs()
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = SchemaFileFilter;
			if(dialog.ShowDialog(this) == DialogResult.OK)
				this.SaveSchemaAs(dialog.FileName);
		}

		/// <summary>
		/// Saves the schema.
		/// </summary>
		private void SaveSchemaAs(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");

			// saves the project to the given file path...
			if(Schema == null)
				throw new InvalidOperationException("Schema is null.");
			this.Schema.SaveXml(filePath);
		}

		/// <summary>
		/// Gets the schema.
		/// </summary>
		public SqlSchema Schema
		{
			get
			{
				if(Project == null)
					throw new InvalidOperationException("Project is null.");
				return this.Project.Schema;
			}
		}
		
		/// <summary>
		/// Creates an instance of Schema.
		/// </summary>
		/// <remarks>This does not assign the instance to the _schema field</remarks>
		private void CreateSchema(IOperationItem op)
		{
			if(Project == null)
				throw new InvalidOperationException("Project is null.");

			// go...
			this.Project.Schema = Database.GetSchema(new GetSchemaArgs(op));
		}

		/// <summary>
		/// Called when the schema has been retrieved.
		/// </summary>
		/// <param name="result"></param>
		private void GetSchemaCallback(IAsyncResult result)
		{
			Alert.ShowInformation(this, "Done.");
		}

		private void buttonLoad_Click(object sender, System.EventArgs e)
		{
			this.MergeSchema();
		}

		/// <summary>
		/// Loads a project.
		/// </summary>
		private void MergeSchema()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = SchemaFileFilter;
			if(dialog.ShowDialog(this) == DialogResult.OK)
				MergeSchema(dialog.FileName);
		}

		private void menuItem10_Click(object sender, System.EventArgs e)
		{
			this.radioSchemaXml.Checked = true;
		}

		private void radioSchemaXml_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void menuViewCollection_Click(object sender, System.EventArgs e)
		{
			this.radioCollection.Checked = true;
		}

		private void menuViewCollectionBase_Click(object sender, System.EventArgs e)
		{
			this.radioCollectionBase.Checked = true;
		}

		private void menuViewEntity_Click(object sender, System.EventArgs e)
		{
			this.radioEntity.Checked = true;
		}

		private void menuViewEntityBase_Click(object sender, System.EventArgs e)
		{
			this.radioEntityBase.Checked = true;
		}

		private void menuViewRefreshPreview_Click(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void radioEntityBase_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void radioEntity_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void radioCollectionBase_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void radioCollection_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void linkRefresh_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			this.ShowPreview();
		}

		/// <summary>
		/// Shows the schema XML.
		/// </summary>
		private void ShowSchemaXml()
		{
			if(this.Schema != null)
				this.textCode.Text = this.Schema.ToXml();
			else
				this.textCode.Text = string.Empty;
		}

		/// <summary>
		/// Shows the selected preview.
		/// </summary>
		private void ShowPreview()
		{
			if(this.radioSchemaXml.Checked)
				this.ShowSchemaXml();
			else
				this.ShowPreview(this.SelectedCodeFileType);
		}

		/// <summary>
		/// Gets the selected code file type.
		/// </summary>
		private EntityCodeFileType SelectedCodeFileType
		{
			get
			{
				if(this.radioEntityBase.Checked)
					return EntityCodeFileType.EntityBase;
                else if (this.radioEntity.Checked)
					return EntityCodeFileType.Entity;
                else if (this.radioCollectionBase.Checked)
					return EntityCodeFileType.CollectionBase;
                else if (this.radioCollection.Checked)
					return EntityCodeFileType.Collection;
                else if (this.radioDto.Checked)
					return EntityCodeFileType.Dto;
                else if (this.radioDtoBase.Checked)
                    return EntityCodeFileType.DtoBase;
                else
					throw new NotSupportedException("Unhandled file type.");
			}
		}

		private void menuProjectSettings_Click(object sender, System.EventArgs e)
		{
			ShowSettings();
		}

		/// <summary>
		/// Shows the settings dialog.
		/// </summary>
		private bool ShowSettings()
		{
			SettingsDialog dialog = new SettingsDialog();
			dialog.Settings = this.Settings;
			dialog.LocalSettings = this.LocalSettings;
			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				this.RefreshView();
				return true;
			}
			else
				return false;
		}

		private void menuProjectGenerate_Click(object sender, System.EventArgs e)
		{
			this.Generate();
		}

		private void menuFileSaveSchema_Click(object sender, System.EventArgs e)
		{
			this.SaveSchemaAs();
		}

		private void menuDatabaseMerge_Click(object sender, System.EventArgs e)
		{
			this.MergeSchema();
		}

		private void menuFileConnect_Click(object sender, System.EventArgs e)
		{
			Connect();
		}

		/// <summary>
		/// Connects to a database.
		/// </summary>
		private bool Connect()
		{
			// open...
			ConnectToDatabaseDialog dialog = new ConnectToDatabaseDialog();

			// mjr - 07-07-2005 - added...
			dialog.NameRequired = false;

			// set...
			if(Database.HasDefaultDatabaseSettings())
				dialog.ConnectionSettings = Database.DefaultConnectionSettings;

			// show...
			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				// set the primary connection...
				if(dialog.ConnectionSettings == null)
					throw new InvalidOperationException("dialog.ConnectionSettings is null.");
				Database.SetDefaultDatabase(dialog.ConnectionSettings);
				LocalSettings.ConnectionType = dialog.ConnectionSettings.ConnectionType;
				LocalSettings.ConnectionString = dialog.ConnectionSettings.ConnectionString;

				// reload...
				this.ReloadSchema();
				this.RefreshView();

				// ok...
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Reloads the schema from the database.
		/// </summary>
		private void ReloadSchema()
		{
			Cursor oldCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			try
			{
				// mbr - 2008-09-02 - changed to thread...

//				// existing...
//				XmlDocument existingSchemaXml = null;
//				if(this.Schema != null)
//					existingSchemaXml = this.Schema.ToXmlDocument();
//
//				// build the schema...
//				this.LoadSchema();
//
//				// merge...
//				if(Schema == null)
//					throw new InvalidOperationException("Schema is null.");
//				if(existingSchemaXml != null)
//					this.Schema.Merge(existingSchemaXml);

				// create...
				using(OperationDialog dialog = new OperationDialog())
				{
					dialog.Run(new ReloadSchemaOperation(new OperationContext(null, dialog), this));
					dialog.ShowDialog(this);
				}

				// refresh...
				this.ShowPreview();
			}
			finally
			{
				this.Cursor = oldCursor;
			}
		}

		internal void DoReloadSchema(IOperationItem op)
		{
			// existing...
			XmlDocument existingSchemaXml = null;
			if(this.Schema != null)
				existingSchemaXml = this.Schema.ToXmlDocument();

			// build the schema...
			this.LoadSchema(op);

			// merge...
			if(Schema == null)
				throw new InvalidOperationException("Schema is null.");
			if(existingSchemaXml != null)
				this.Schema.Merge(existingSchemaXml);
		}

		private void menuHelpAbout_Click(object sender, System.EventArgs e)
		{
			this.ShowAbout();
		}

		/// <summary>
		/// Shows the 'About' box.
		/// </summary>
		private void ShowAbout()
		{
			DesktopRuntime.Current.ShowAbout(this);
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Loads a project.
		/// </summary>
		private void MergeSchema(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			
			// check...
			if(File.Exists(filePath) == false)
			{
				Alert.ShowWarning(this, string.Format("The file '{0}' does not exist.", filePath));
				return;
			}

			// make sure we have a schema...
			if(Schema == null)
				throw new InvalidOperationException("Schema is null.");
			this.Schema.Merge(filePath);

			// refresh...
			this.RefreshView();
		}

		/// <summary>
		/// Gets the selected table.
		/// </summary>
		private SqlProcedure SelectedProcedure
		{
			get
			{
				// get the object...
				object obj = ViewItemFactory.Unwrap(this.SelectedViewItem);
				if(obj is SqlProcedure)
					return (SqlProcedure)obj;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the selected table.
		/// </summary>
		private SqlTable SelectedTable
		{
			get
			{
				// get the object...
				object obj = ViewItemFactory.Unwrap(this.SelectedViewItem);
				if(obj is SqlTable)
					return (SqlTable)obj;
				if(obj is SqlColumn)
					return ((SqlColumn)obj).Table;
				if(obj is SqlChildToParentLink)
					return ((SqlChildToParentLink)obj).Table;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the project.
		/// </summary>
		private Project Project
		{
			get
			{
				// returns the value...
				return _project;
			}
			set
			{
				if(_project != value)
				{
					// unsub...
					if(_project != null)
						_project.IsDirtyChanged -= new EventHandler(_project_IsDirtyChanged);

					// set...
					_project = value;

					// sub...
					if(_project != null)
						_project.IsDirtyChanged += new EventHandler(_project_IsDirtyChanged);
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

			// update...
			UpdateCaption();
		}

		/// <summary>
		/// Updates the caption.
		/// </summary>
		private void UpdateCaption()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(BaseCaption);

			// setup...
			if(this.Project != null)
			{
				builder.Append(" - ");
				if(this.Project.IsNew)
					builder.Append("(Untitled)");
				else
					builder.Append(this.Project.Name);

				// dirty?
				if(this.Project.IsDirty)
					builder.Append("*");
			}

			// set...
			this.Text = builder.ToString();
		}

		private void menuNewProject_Click(object sender, System.EventArgs e)
		{
			NewProject();
		}

		/// <summary>
		/// Creates a new project.
		/// </summary>
		private void NewProject()
		{
			if(this.CheckSave() == false)
				return;

			// create...
			this.Project = new Project();
			this.RefreshView();
		}

		/// <summary>
		/// Checks to see if we need to save the project.
		/// </summary>
		/// <returns>True if the operation can proceed.</returns>
		private bool CheckSave()
		{
			if(this.Project == null)
				return true;

			// dirty?
			if(this.Project.IsDirty == false)
				return true;

			// prompt...
			DialogResult result = Alert.AskYesNoQuestion(this, "Do you want to save changes to the project?", true);
			if(result == DialogResult.Cancel)
				return true;

			// save if we want to, otherwise ditch...
			if(result == DialogResult.Yes)
				return this.SaveProject();
			else
				return true;
		}

		/// <summary>
		/// Saves the project.
		/// </summary>
		/// <returns></returns>
		private bool SaveProject()
		{
			if(Project == null)
				throw new InvalidOperationException("Project is null.");
			
			// new?
			if(this.Project.IsNew)
				return SaveProjectAs();

			// readonly?
			bool makeReadOnly = false;
			if(ProjectStore.CurrentStore.IsReadOnly(this.Project.FilePath))
			{
				if(Alert.AskYesNoQuestion(this, "The project file is read-only.  Do you want to overwrite the file?") != DialogResult.Yes)
					return false;

				// make...
				File.SetAttributes(Project.FilePath, File.GetAttributes(Project.FilePath) ^ FileAttributes.ReadOnly);
				makeReadOnly = true;
			}

			// busy...
			// save it...
			try
			{
				Project.Save();
			}
			finally
			{
				// readonly?
				if(makeReadOnly)
					File.SetAttributes(Project.FilePath, File.GetAttributes(Project.FilePath) | FileAttributes.ReadOnly);
			}

			// update...
			this.UpdateCaption();

			// add...
			this.AddToProjectMru(this.Project.FilePath);

			// return...
			return true;
		}

		/// <summary>
		/// Saves the project after prompting for a name.
		/// </summary>
		private bool SaveProjectAs()
		{
			if(Project == null)
				throw new InvalidOperationException("Project is null.");

			// open...
			string path = ProjectStore.CurrentStore.BrowseForProject(this, true);
			if(path != null)
				return this.SaveProjectAs(path);
			else
				return false;
		}

		private void menuFileSaveProjectAs_Click(object sender, System.EventArgs e)
		{
			this.SaveProjectAs();
		}

		private void menuFileOpenProject_Click(object sender, System.EventArgs e)
		{
			OpenProject();
		}

		/// <summary>
		/// Loads a project.
		/// </summary>
		private void OpenProject()
		{
			if(this.CheckSave() == false)
				return;

			// open...
			string path = ProjectStore.CurrentStore.BrowseForProject(this, false); 
			if(path != null)
				this.OpenProject(path);
		}

		/// <summary>
		/// Opens the given project.
		/// </summary>
		/// <param name="filePath"></param>
		internal void OpenProject(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");

			// open it...
			this.Project = Project.LoadXml(filePath);

			// set...
			if(Project.Settings == null)
				throw new InvalidOperationException("Project.Settings is null.");
			if(this.Project.LocalSettings.ConnectionType != null && this.Project.LocalSettings.ConnectionString != null && this.Project.LocalSettings.ConnectionString.Length > 0)
				Database.SetDefaultDatabase(this.Project.LocalSettings.ConnectionType, this.Project.LocalSettings.ConnectionString);
			else
			{
				Alert.ShowInformation(this, string.Format("As this is the first time you have opened the '{0}' project on this computer, you must configure a database connection.", this.Project.DisplayName));
				if(!this.Connect())
				{
					Alert.ShowWarning(this, "You must configure a database connection before you can merge in the database schema.");
					return;
				}
			}

			// try...
			try
			{
				// schema?
				ReloadSchema();
			}
			finally
			{
				// update...
				this.RefreshView();
			}

			// add...
			AddToProjectMru(filePath);
		}

		/// <summary>
		/// Adds the file to the project MRU.
		/// </summary>
		/// <param name="path"></param>
		private void AddToProjectMru(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");

			//			// get...
			//			MruList projects = ProjectStore.CurrentStore.GetProjectMruList();
			//			if(projects == null)
			//				throw new InvalidOperationException("projects is null.");
			//
			//			// set...
			//			projects.Push(path);
			//
			//			// save...
			//			Runtime.Current.UserSettings.SetValue(ProjectMruListKey, projects);

			ProjectStore.CurrentStore.AddMruItem(path);
		}

		/// <summary>
		/// Updates the project MRU list menu.
		/// </summary>
		private void RefreshProjectMruMenu()
		{
			// check...
			if(menuFile == null)
				throw new InvalidOperationException("menuFile is null.");
			if(menuExit == null)
				throw new InvalidOperationException("menuExit is null.");

			// remove existing items...
			ArrayList toRemove = new ArrayList();
			foreach(MenuItem item in menuFile.MenuItems)
			{
				if(item is ProjectMruMenuItem || item is EmptyProjectMruMenuItem)
					toRemove.Add(item);
			}
			foreach(MenuItem item in toRemove)
				menuFile.MenuItems.Remove(item);

			// get the list...
			MruList list = ProjectStore.CurrentStore.GetMruItems();
			if(list == null)
				throw new InvalidOperationException("list is null.");

			// go back up the menu...
			MenuItem firstSep = null;
			MenuItem secondSep = null;
			for(int index = menuFile.MenuItems.Count - 2; index >= 0; index--)
			{
				if(menuFile.MenuItems[index].Text == "-")
				{
					if(secondSep == null)
						secondSep = menuFile.MenuItems[index];
					else
					{
						firstSep = menuFile.MenuItems[index];
						break;
					}
				}
			}

			// check...
			if(firstSep == null)
				throw new InvalidOperationException("firstSep is null.");

			// add...
			if(list.Count > 0)
			{
				int itemIndex = 0;
				foreach(string path in list)
				{
					menuFile.MenuItems.Add(firstSep.Index + itemIndex + 1, new ProjectMruMenuItem(this, itemIndex, path));
					itemIndex++;
				}
			}
			else
				menuFile.MenuItems.Add(firstSep.Index + 1, new EmptyProjectMruMenuItem());

			// set...
			secondSep.Index = menuExit.Index - 1;
		}

		private void menuFileSaveProject_Click(object sender, System.EventArgs e)
		{
			this.SaveProject();
		}

		/// <summary>
		/// Saves the project to the given path.
		/// </summary>
		/// <param name="filePath"></param>
		private bool SaveProjectAs(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			
			// check...
			if(Project == null)
				throw new InvalidOperationException("Project is null.");

			// set...
			this.Project.FilePath = filePath;

			// return...
			return this.SaveProject();
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		private LocalSettings LocalSettings
		{
			get
			{
				if(Project == null)
					throw new InvalidOperationException("Project is null.");
				return this.Project.LocalSettings;
			}
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		private Settings Settings
		{
			get
			{
				if(Project == null)
					throw new InvalidOperationException("Project is null.");
				return this.Project.Settings;
			}
		}

		private void _project_IsDirtyChanged(object sender, EventArgs e)
		{
			this.UpdateCaption();
		}

		private void menuProjectReloadSchema_Click(object sender, System.EventArgs e)
		{
			// load...
			try
			{
				// schema?
				ReloadSchema();
			}
			finally
			{
				// update...
				this.RefreshView();
			}

			// tell...
			Alert.ShowInformation(this, "The schema has been reloaded.");
		}

		/// <summary>
		/// Tests that we can connect to the database.
		/// </summary>
		/// <returns></returns>
		private bool TestDatabaseSettings()
		{
			if(Settings == null)
				throw new InvalidOperationException("Settings is null.");

			// check...
			if(this.LocalSettings.ConnectionSettings == null)
			{
				Alert.ShowWarning(this, "The connection settings have not been specified.");
				return false;
			}

			// try...
			try
			{
				Connection.TestConnection(this.LocalSettings.ConnectionSettings);
				return true;
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "A connection to the database could not be established.", ex);
				return false;
			}
		}

		/// <summary>
		/// Gets the appdomainordinal.
		/// </summary>
		private int RevisionOrdinal
		{
			get
			{
				// returns the value...
				return _revisionOrdinal;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing (e);

			// check...
			if(this.CheckSave() == false)
				e.Cancel = true;
		}

		private void linkViewInNotepad_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			ViewExternal();
		}

		/// <summary>
		/// Views the contents of the file in notepad.
		/// </summary>
		private void ViewExternal()
		{
			// get a path...
			string path = Runtime.Current.GetTempTextFilePath();
			using(StreamWriter writer = new StreamWriter(path))
			{
				writer.Write(this.textCode.Text);
			}

			// show...
			System.Diagnostics.Process.Start(path);
		}

		private void radioInterfaceBase_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void radioInterface_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private static void startup_StartApplication(object sender, EventArgs e)
		{
			// mbr - 2008-08-31 - find any .dbu files in the start path...
			foreach(string path in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dbu"))
			{
				// try...
				string asmPath = null;
				try
				{
					asmPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

					// load...
					Assembly.LoadFile(asmPath);
				}
				catch(Exception ex)
				{
					Alert.ShowWarning(null, string.Format("The assembly '{0}' could not be loaded.", asmPath), ex);
				}
			}

			// first...
			WhatFirst dialog = new WhatFirst();
			if(dialog.ShowDialog() == DialogResult.Cancel)
				return;

			// show...
			Application.Run(new Generator(dialog.ProjectToLoad, dialog.PromptForProject));
		}

		private void radioServiceBase_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void radioService_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();
		}

		private void menuDatabaseGetFromScratchScript_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.ShowFromScratchScript();
			}
			catch (Exception ex)
			{
				Alert.ShowError(this, "Failed to generate database script.", ex);
			}
		}

		/// <summary>
		/// Shows the 'from scratch' script.
		/// </summary>
		private void ShowRecreateSprocsScript()
		{
            //SqlServerDialect dialect = new SqlServerDialect();
            //string script = dialect.GetRecreateSprocsScript(this.Schema, SqlTableScriptFlags.IncludeDropObject | SqlTableScriptFlags.IgnoreExistenceErrors);
            //this.ShowScript(script);
		}

		/// <summary>
		/// Shows the 'from scratch' script.
		/// </summary>
		private void ShowFromScratchScript()
		{
			SqlServerDialect dialect = new SqlServerDialect();
			string script = dialect.GetCreateDatabaseScript(this.Schema, SqlTableScriptFlags.IncludeDropObject | SqlTableScriptFlags.IgnoreExistenceErrors);
			this.ShowScript(script);
		}

		/// <summary>
		/// Shows the given script.
		/// </summary>
		/// <param name="script"></param>
		private void ShowScript(string script)
		{
			if(script == null)
				throw new ArgumentNullException("script");
			if(script.Length == 0)
				throw new ArgumentOutOfRangeException("'script' is zero-length.");
			
			// create...
			ScriptDialog dialog = new ScriptDialog();
			dialog.Script = script;
			dialog.ShowDialog(this);
		}

		private void menuDatabaseShowPopulateData_Click(object sender, System.EventArgs e)
		{
			try
			{
				ShowPopulateDataScript();
			}
			catch (Exception ex)
			{
				Alert.ShowError(this, "Failed to show data population script.", ex);
			}
		}

		/// <summary>
		/// Shows the script for table population.
		/// </summary>
		private void ShowPopulateDataScript()
		{
			if(this.SelectedTable == null)
			{
				Alert.ShowWarning(this, "You must select a table.");
				return;
			}
			else
				this.ShowPopulateDataScript(this.SelectedTable);
		}

		private void menuFile_Popup(object sender, System.EventArgs e)
		{
			this.RefreshProjectMruMenu();
		}

		private void gridProperties_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			this.ShowPreview();

			// update...
			if(Project == null)
				throw new InvalidOperationException("Project is null.");
			this.Project.SetDirty(true);
		}

		private void gridProperties_SelectedViewItemsChanged(object sender, System.EventArgs e)
		{
			this.ShowPreview();

			// ok, now what do we have?
			object obj = ViewItemFactory.Unwrap(this.SelectedViewItem);
			string pattern = null;
			if(obj is SqlColumn)
				pattern = string.Format(@"\[\s*EntityField\s*\(\s*\""{0}\""", ((SqlColumn)obj).Name);
			else if(obj is SqlChildToParentLink)
				pattern = string.Format(@"\[\s*EntityLinkToParent\s*\(\s*\""{0}\""", ((SqlChildToParentLink)obj).Name);

			// match...
			if(pattern != null && pattern.Length > 0)
			{
				Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
				Match match = regex.Match(this.textCode.Text);
				if(match.Success)
					this.SelectCode(match);
			}
		}

		private void SelectCode(Match match)
		{
			if(match == null)
				throw new ArgumentNullException("match");
			
			// select...
			this.textCode.Select(match.Index + 400, 0);
			this.textCode.Focus();
			this.textCode.ScrollToCaret();
		}

		/// <summary>
		/// Gets the selected object.
		/// </summary>
		private object SelectedViewItem
		{
			get
			{
				if(this.treeObjects.SelectedNode is ObjectTreeNode)
				{
					object obj = ((ObjectTreeNode)this.treeObjects.SelectedNode).InnerObject;
					if(obj != null)
						return ViewItemFactory.CreateViewItem(obj);
					else
						return null;
				}
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the selected object.
		/// </summary>
		private object SelectedObject
		{
			get
			{
				if(this.treeObjects.SelectedNode is ObjectTreeNode)
					return ((ObjectTreeNode)this.treeObjects.SelectedNode).InnerObject;
				else
					return null;
			}
		}

		private void menuItem17_Click(object sender, System.EventArgs e)
		{
			this.radioDto.Checked = true;
		}

		private void menuItem18_Click(object sender, System.EventArgs e)
		{
		}

		private void menuItem19_Click(object sender, System.EventArgs e)
		{
		}

		private void menuItem15_Click(object sender, System.EventArgs e)
		{
			this.radioDtoBase.Checked = true;
		}

		/// <summary>
		/// Browse for the folder.
		/// </summary>
		private string Browse()
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Choose the folder for the extended property settings.";
			if(dialog.ShowDialog(this) == DialogResult.OK)
				return dialog.SelectedPath;
			else
				return null;
		}

		/// <summary>
		/// Shows the script for table population.
		/// </summary>
		private void ShowPopulateDataScript(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// create...
			SqlDialect dialect = new SqlServerDialect();
			string script = dialect.GetPopulateDataScript(table);
			this.ShowScript(script);
		}

		protected override void OnPositionRestored(EventArgs e)
		{
			base.OnPositionRestored (e);

			// save...
			if(Runtime.Current.UserSettings.Contains(XSplitterKey))
				this.splitterX.SplitPosition = (int)Runtime.Current.UserSettings[XSplitterKey];
			if(Runtime.Current.UserSettings.Contains(YSplitterKey))
				this.splitterY.SplitPosition = (int)Runtime.Current.UserSettings[YSplitterKey];
		}

		protected override void OnPositionSaved(EventArgs e)
		{
			base.OnPositionSaved (e);

			// save...
			Runtime.Current.UserSettings[XSplitterKey] = this.splitterX.SplitPosition;
			Runtime.Current.UserSettings[YSplitterKey] = this.splitterY.SplitPosition;
		}

		private void menuSetAllGenerated_Click(object sender, System.EventArgs e)
		{
			SetAllGenerated(true);		
		}

		private void menuSetAllNotGenerated_Click(object sender, System.EventArgs e)
		{
			SetAllGenerated(false);
		}

		private void SetAllGenerated(bool generate)
		{
			if(Alert.AskYesNoQuestion(this, "Are you sure?", false) != DialogResult.Yes)
				return;

			// walk...
			foreach(SqlTable table in this.Schema.Tables)
				table.Generate = generate;

			// update...
			this.RefreshView();

			// set...
			this.Project.SetDirty(true);
		}

		/// <summary>
		/// Fires when the show exended properties is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItemShowExtendedProperties_Click(object sender, System.EventArgs e)
		{
			//			if(extendedProperties.Visible)
			//			{
			//				textCode.Height += 286;
			//				menuShowExtendedProperties.Text = "Show Extended &Properties";
			//			}
			//			else
			//			{
			//				textCode.Height -= 286;
			//				menuShowExtendedProperties.Text = "Hide Extended &Properties";
			//			}
			//
			//			extendedProperties.Visible = !extendedProperties.Visible;
		}

		private void menuDatabaseSprocs_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.ShowRecreateSprocsScript();
			}
			catch (Exception ex)
			{
				Alert.ShowError(this, "Failed to generate database script.", ex);
			}
		}

		/// <summary>
		/// Gets the engine.
		/// </summary>
		private GenerateEngine Engine
		{
			get
			{
				// returns the value...
				return _engine;
			}
		}

		/// <summary>
		/// Gets the projecttoloadonstart.
		/// </summary>
		private string ProjectToLoadOnStart
		{
			get
			{
				// returns the value...
				return _projectToLoadOnStart;
			}
		}

		/// <summary>
		/// Shows adhoc files.
		/// </summary>
		/// <param name="path"></param>
		public void ShowAdhocFiles(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// open...
			using(FolderViewDialog dialog = new FolderViewDialog())
			{
				dialog.FolderPath = path;
				dialog.ShowDialog(this);
			}
		}

        private void treeObjects_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                // set...
                this.gridProperties.SelectedObject = this.SelectedViewItem;
            }
            catch (Exception ex)
            {
                Alert.ShowWarning(this, "Failed to handle selection change.", ex);
            }
        }
	}
}
