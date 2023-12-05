﻿namespace WADExplorer
{
    partial class Window
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceWithToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.SplitPanel = new System.Windows.Forms.SplitContainer();
            this.FileTree = new System.Windows.Forms.TreeView();
            this.IconsList = new System.Windows.Forms.ImageList(this.components);
            this.PicturePanel = new System.Windows.Forms.Panel();
            this.PreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.ReplaceButton = new System.Windows.Forms.Button();
            this.ExtractButton = new System.Windows.Forms.Button();
            this.FilePG = new System.Windows.Forms.PropertyGrid();
            this.ListProgress = new System.Windows.Forms.ProgressBar();
            this.dontSortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TextPreview = new System.Windows.Forms.RichTextBox();
            this.AnyLabel = new System.Windows.Forms.Label();
            this.newFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oldFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractToToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceWithToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.NewFolderButton = new System.Windows.Forms.Button();
            this.saveFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OldFormatButton = new System.Windows.Forms.ToolStripMenuItem();
            this.NewFormatButton = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitPanel)).BeginInit();
            this.SplitPanel.Panel1.SuspendLayout();
            this.SplitPanel.Panel2.SuspendLayout();
            this.SplitPanel.SuspendLayout();
            this.PicturePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPictureBox)).BeginInit();
            this.EditContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1108, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFormatToolStripMenuItem,
            this.oldFormatToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToToolStripMenuItem,
            this.replaceWithToolStripMenuItem,
            this.toolStripSeparator2,
            this.dontSortToolStripMenuItem,
            this.saveFormatToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // extractToToolStripMenuItem
            // 
            this.extractToToolStripMenuItem.Name = "extractToToolStripMenuItem";
            this.extractToToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extractToToolStripMenuItem.Text = "Extract To";
            this.extractToToolStripMenuItem.Click += new System.EventHandler(this.ExtractButton_Click);
            // 
            // replaceWithToolStripMenuItem
            // 
            this.replaceWithToolStripMenuItem.Name = "replaceWithToolStripMenuItem";
            this.replaceWithToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.replaceWithToolStripMenuItem.Text = "Replace With";
            this.replaceWithToolStripMenuItem.Click += new System.EventHandler(this.ReplaceButton_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // SplitPanel
            // 
            this.SplitPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitPanel.Location = new System.Drawing.Point(0, 27);
            this.SplitPanel.Name = "SplitPanel";
            // 
            // SplitPanel.Panel1
            // 
            this.SplitPanel.Panel1.Controls.Add(this.FileTree);
            // 
            // SplitPanel.Panel2
            // 
            this.SplitPanel.Panel2.Controls.Add(this.NewFolderButton);
            this.SplitPanel.Panel2.Controls.Add(this.AddButton);
            this.SplitPanel.Panel2.Controls.Add(this.DeleteButton);
            this.SplitPanel.Panel2.Controls.Add(this.PicturePanel);
            this.SplitPanel.Panel2.Controls.Add(this.ReplaceButton);
            this.SplitPanel.Panel2.Controls.Add(this.ExtractButton);
            this.SplitPanel.Panel2.Controls.Add(this.FilePG);
            this.SplitPanel.Size = new System.Drawing.Size(1108, 714);
            this.SplitPanel.SplitterDistance = 690;
            this.SplitPanel.TabIndex = 2;
            // 
            // FileTree
            // 
            this.FileTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FileTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FileTree.ContextMenuStrip = this.EditContextMenu;
            this.FileTree.Enabled = false;
            this.FileTree.ImageIndex = 0;
            this.FileTree.ImageList = this.IconsList;
            this.FileTree.Location = new System.Drawing.Point(0, 0);
            this.FileTree.Name = "FileTree";
            this.FileTree.SelectedImageIndex = 0;
            this.FileTree.Size = new System.Drawing.Size(688, 714);
            this.FileTree.TabIndex = 0;
            this.FileTree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.FileTree_AfterCollapse);
            this.FileTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.FileTree_AfterExpand);
            // 
            // IconsList
            // 
            this.IconsList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IconsList.ImageStream")));
            this.IconsList.TransparentColor = System.Drawing.Color.Transparent;
            this.IconsList.Images.SetKeyName(0, "file.png");
            this.IconsList.Images.SetKeyName(1, "folder.png");
            this.IconsList.Images.SetKeyName(2, "config.png");
            this.IconsList.Images.SetKeyName(3, "audio.png");
            this.IconsList.Images.SetKeyName(4, "bitmap.png");
            this.IconsList.Images.SetKeyName(5, "model.png");
            this.IconsList.Images.SetKeyName(6, "root.png");
            this.IconsList.Images.SetKeyName(7, "text.png");
            this.IconsList.Images.SetKeyName(8, "vehicle.png");
            this.IconsList.Images.SetKeyName(9, "folder_open.png");
            // 
            // PicturePanel
            // 
            this.PicturePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PicturePanel.Controls.Add(this.AnyLabel);
            this.PicturePanel.Controls.Add(this.TextPreview);
            this.PicturePanel.Controls.Add(this.PreviewPictureBox);
            this.PicturePanel.Location = new System.Drawing.Point(0, 468);
            this.PicturePanel.Name = "PicturePanel";
            this.PicturePanel.Size = new System.Drawing.Size(414, 246);
            this.PicturePanel.TabIndex = 4;
            // 
            // PreviewPictureBox
            // 
            this.PreviewPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.PreviewPictureBox.Location = new System.Drawing.Point(0, 0);
            this.PreviewPictureBox.Name = "PreviewPictureBox";
            this.PreviewPictureBox.Size = new System.Drawing.Size(1024, 1024);
            this.PreviewPictureBox.TabIndex = 3;
            this.PreviewPictureBox.TabStop = false;
            // 
            // ReplaceButton
            // 
            this.ReplaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceButton.Enabled = false;
            this.ReplaceButton.Location = new System.Drawing.Point(0, 376);
            this.ReplaceButton.Name = "ReplaceButton";
            this.ReplaceButton.Size = new System.Drawing.Size(414, 23);
            this.ReplaceButton.TabIndex = 2;
            this.ReplaceButton.Text = "Replace With";
            this.ReplaceButton.UseVisualStyleBackColor = true;
            this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
            // 
            // ExtractButton
            // 
            this.ExtractButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExtractButton.Enabled = false;
            this.ExtractButton.Location = new System.Drawing.Point(0, 354);
            this.ExtractButton.Name = "ExtractButton";
            this.ExtractButton.Size = new System.Drawing.Size(414, 23);
            this.ExtractButton.TabIndex = 1;
            this.ExtractButton.Text = "Extract To";
            this.ExtractButton.UseVisualStyleBackColor = true;
            this.ExtractButton.Click += new System.EventHandler(this.ExtractButton_Click);
            // 
            // FilePG
            // 
            this.FilePG.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilePG.Enabled = false;
            this.FilePG.Location = new System.Drawing.Point(0, 0);
            this.FilePG.Name = "FilePG";
            this.FilePG.Size = new System.Drawing.Size(414, 354);
            this.FilePG.TabIndex = 0;
            this.FilePG.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.FilePG_PropertyValueChanged);
            // 
            // ListProgress
            // 
            this.ListProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ListProgress.Location = new System.Drawing.Point(900, 1);
            this.ListProgress.Name = "ListProgress";
            this.ListProgress.Size = new System.Drawing.Size(205, 20);
            this.ListProgress.TabIndex = 2;
            this.ListProgress.Visible = false;
            // 
            // dontSortToolStripMenuItem
            // 
            this.dontSortToolStripMenuItem.CheckOnClick = true;
            this.dontSortToolStripMenuItem.Name = "dontSortToolStripMenuItem";
            this.dontSortToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.dontSortToolStripMenuItem.Text = "Don\'t Sort On View";
            this.dontSortToolStripMenuItem.Click += new System.EventHandler(this.dontSortToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // TextPreview
            // 
            this.TextPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextPreview.Location = new System.Drawing.Point(0, 0);
            this.TextPreview.Name = "TextPreview";
            this.TextPreview.Size = new System.Drawing.Size(411, 246);
            this.TextPreview.TabIndex = 1;
            this.TextPreview.Text = "";
            this.TextPreview.Visible = false;
            // 
            // AnyLabel
            // 
            this.AnyLabel.AutoSize = true;
            this.AnyLabel.Location = new System.Drawing.Point(3, 3);
            this.AnyLabel.Name = "AnyLabel";
            this.AnyLabel.Size = new System.Drawing.Size(0, 13);
            this.AnyLabel.TabIndex = 4;
            this.AnyLabel.Visible = false;
            // 
            // newFormatToolStripMenuItem
            // 
            this.newFormatToolStripMenuItem.Name = "newFormatToolStripMenuItem";
            this.newFormatToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.newFormatToolStripMenuItem.Text = "New Format (PC)";
            this.newFormatToolStripMenuItem.Click += new System.EventHandler(this.OpenNew);
            // 
            // oldFormatToolStripMenuItem
            // 
            this.oldFormatToolStripMenuItem.Name = "oldFormatToolStripMenuItem";
            this.oldFormatToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.oldFormatToolStripMenuItem.Text = "Old Format (PS2, other games, etc.)";
            this.oldFormatToolStripMenuItem.Click += new System.EventHandler(this.OpenOld);
            // 
            // EditContextMenu
            // 
            this.EditContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToToolStripMenuItem1,
            this.replaceWithToolStripMenuItem1});
            this.EditContextMenu.Name = "EditContextMenu";
            this.EditContextMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // extractToToolStripMenuItem1
            // 
            this.extractToToolStripMenuItem1.Name = "extractToToolStripMenuItem1";
            this.extractToToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.extractToToolStripMenuItem1.Text = "Extract To...";
            this.extractToToolStripMenuItem1.Click += new System.EventHandler(this.ExtractButton_Click);
            // 
            // replaceWithToolStripMenuItem1
            // 
            this.replaceWithToolStripMenuItem1.Name = "replaceWithToolStripMenuItem1";
            this.replaceWithToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.replaceWithToolStripMenuItem1.Text = "Replace With...";
            this.replaceWithToolStripMenuItem1.Click += new System.EventHandler(this.ReplaceButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteButton.Enabled = false;
            this.DeleteButton.Location = new System.Drawing.Point(0, 398);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(414, 23);
            this.DeleteButton.TabIndex = 5;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.Enabled = false;
            this.AddButton.Location = new System.Drawing.Point(0, 420);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(414, 23);
            this.AddButton.TabIndex = 6;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // NewFolderButton
            // 
            this.NewFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NewFolderButton.Enabled = false;
            this.NewFolderButton.Location = new System.Drawing.Point(0, 442);
            this.NewFolderButton.Name = "NewFolderButton";
            this.NewFolderButton.Size = new System.Drawing.Size(414, 23);
            this.NewFolderButton.TabIndex = 7;
            this.NewFolderButton.Text = "Create New Folder";
            this.NewFolderButton.UseVisualStyleBackColor = true;
            this.NewFolderButton.Click += new System.EventHandler(this.NewFolderButton_Click);
            // 
            // saveFormatToolStripMenuItem
            // 
            this.saveFormatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OldFormatButton,
            this.NewFormatButton});
            this.saveFormatToolStripMenuItem.Name = "saveFormatToolStripMenuItem";
            this.saveFormatToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveFormatToolStripMenuItem.Text = "Save Format";
            // 
            // OldFormatButton
            // 
            this.OldFormatButton.Name = "OldFormatButton";
            this.OldFormatButton.Size = new System.Drawing.Size(180, 22);
            this.OldFormatButton.Text = "Old";
            this.OldFormatButton.Click += new System.EventHandler(this.oldToolStripMenuItem_Click);
            // 
            // NewFormatButton
            // 
            this.NewFormatButton.Checked = true;
            this.NewFormatButton.CheckOnClick = true;
            this.NewFormatButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NewFormatButton.Name = "NewFormatButton";
            this.NewFormatButton.Size = new System.Drawing.Size(180, 22);
            this.NewFormatButton.Text = "New";
            this.NewFormatButton.Click += new System.EventHandler(this.NewFormatButton_Click);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1108, 741);
            this.Controls.Add(this.ListProgress);
            this.Controls.Add(this.SplitPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Window";
            this.Tag = "WAD Explorer";
            this.Text = "WAD Explorer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.SplitPanel.Panel1.ResumeLayout(false);
            this.SplitPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitPanel)).EndInit();
            this.SplitPanel.ResumeLayout(false);
            this.PicturePanel.ResumeLayout(false);
            this.PicturePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPictureBox)).EndInit();
            this.EditContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.SplitContainer SplitPanel;
        private System.Windows.Forms.PropertyGrid FilePG;
        private System.Windows.Forms.Button ExtractButton;
        public System.Windows.Forms.TreeView FileTree;
        private System.Windows.Forms.ProgressBar ListProgress;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        public System.Windows.Forms.Button ReplaceButton;
        private System.Windows.Forms.PictureBox PreviewPictureBox;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceWithToolStripMenuItem;
        private System.Windows.Forms.Panel PicturePanel;
        public System.Windows.Forms.ImageList IconsList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripMenuItem dontSortToolStripMenuItem;
        public System.Windows.Forms.RichTextBox TextPreview;
        private System.Windows.Forms.Label AnyLabel;
        private System.Windows.Forms.ToolStripMenuItem newFormatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oldFormatToolStripMenuItem;
        public System.Windows.Forms.ContextMenuStrip EditContextMenu;
        private System.Windows.Forms.ToolStripMenuItem extractToToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem replaceWithToolStripMenuItem1;
        public System.Windows.Forms.Button AddButton;
        public System.Windows.Forms.Button DeleteButton;
        public System.Windows.Forms.Button NewFolderButton;
        private System.Windows.Forms.ToolStripMenuItem saveFormatToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem OldFormatButton;
        public System.Windows.Forms.ToolStripMenuItem NewFormatButton;
    }
}

