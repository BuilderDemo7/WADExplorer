﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using System.Threading;
using System.Runtime.InteropServices;

namespace WADExplorer
{
    public partial class Window : Form
    {
        AudioPlayer audioPlayer = new AudioPlayer();
        public static bool ConsoleAllocated = false;
        public bool showErrors = true;
        //public Thread LoadThread;
        //private string LoadThread_FileName;

        public void OnFileBufferLoadFail(PackageLoadFileBufferFailureEventArgs e)
        {
            if (!showErrors)
                return;

            if (MessageBox.Show($"The file {e.File.Name} (0x{e.File.CRC:X4}/0x{e.File.Offset:X4}) has failed to load it's buffer\nExpect some errors for now on!\n\nShow more errors?","Warning",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.No)
            {
                showErrors = false;
            }
        }

        public Window()
        {
            InitializeComponent();
#if DEBUG
            showMoreInfo = true;
            ShowMoreInfoBTN.Checked = true;

            // TODO: Make it enabled for release when the converter is done
            ConfigsConverterItem.Visible = true;
#endif


            audioPlayer.Visible = false;
            audioPlayer.Location = this.PicturePanel.Location;
            audioPlayer.Size = new Size(357, 74);
            audioPlayer.TabIndex = 1;
            audioPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));

            this.SplitPanel.Panel2.Controls.Add(audioPlayer);

            //LoadThread = new Thread(new ThreadStart(delegate
            //{
            //    OpenPackage.Load(LoadThread_FileName);
            //}));

            FileTree.AfterSelect += new TreeViewEventHandler(ItemSelected);
            // right clicked or mouse down at
            /*
            FileTree.MouseDown += (sender, args) =>
            {
                TreeNode pointedNode = FileTree.GetNodeAt(args.X, args.Y);
                if (FileTree.SelectedNode!=pointedNode)
                   FileTree.SelectedNode = pointedNode;
            };
            */

            PicturePanel.Scroll += (sender, e) => {
                switch (e.ScrollOrientation) {
                    case ScrollOrientation.HorizontalScroll:
                        PreviewPictureBox.Location = new Point(-e.NewValue, PreviewPictureBox.Location.Y);
                        int newval1 = e.NewValue; //Math.Abs(PreviewPictureBox.Location.X);
                        if (newval1 < PicturePanel.HorizontalScroll.Maximum)
                            PicturePanel.HorizontalScroll.Value = newval1;
                        else
                            PreviewPictureBox.Location = new Point(PicturePanel.HorizontalScroll.Maximum, PreviewPictureBox.Location.Y);
                        break;
                    case ScrollOrientation.VerticalScroll:
                        PreviewPictureBox.Location = new Point(PreviewPictureBox.Location.X, -e.NewValue);
                        int newval2 = e.NewValue; //Math.Abs(PreviewPictureBox.Location.Y);
                        if (newval2 < PicturePanel.VerticalScroll.Maximum)
                            PicturePanel.VerticalScroll.Value = newval2;
                        else
                            PreviewPictureBox.Location = new Point(PreviewPictureBox.Location.X, PicturePanel.VerticalScroll.Maximum);
                        break;
                }
            };
        }

        public Package OpenPackage = new Package();
        bool firstTimeOpening = true;
        bool loadedOldFormat = false;
        bool saveInNewFormat = true;
        int SelectedIndex = -1;
        bool DontSort = false;
        bool showMoreInfo = false;

        public List<InsideItem> CheckedItems = new List<InsideItem>();

        public void InitTools()
        {
            FilePG.Enabled = true;
            ExtractButton.Enabled = true;
            ReplaceButton.Enabled = true;
            DeleteButton.Enabled = true;
            AddButton.Enabled = true;
            NewFolderButton.Enabled = true;

            addFileToolStripMenuItem.Enabled = true;
            addFileToolStripMenuItem1.Enabled = true;
            ToolStripAddFileButton.Enabled = true;
            deleteToolStripMenuItem.Enabled = true;
            deleteToolStripMenuItem1.Enabled = true;
            ToolStripDeleteButton.Enabled = true;
            newFolderToolStripMenuItem.Enabled = true;
            ToolStripNewFolderButton.Enabled = true;
            replaceWithToolStripMenuItem.Enabled = true;
            replaceWithToolStripMenuItem1.Enabled = true;
            ToolStripReplaceButton.Enabled = true;
            extractToToolStripMenuItem.Enabled = true;
            extractToToolStripMenuItem1.Enabled = true;
            ToolStripExtractButton.Enabled = true;

            SaveBTNTS.Enabled = true;

            FileTree.Enabled = true;
            this.Text = this.Tag + " - " + OpenPackage.FileName;

            UpdateTools();
        }

        public void UpdateTools()
        {
            StatusLabel.Text = String.Format("{0} Files loaded", OpenPackage.Items.Count);
        }

        public void ExtractItemChildrenTo(InsideItem item, string path, bool chain = false, string associatedPath = "")
        {
            // not a folder? cancel
            if (item.IsFolder == false)
                return;
            foreach (InsideItem child in item.Children)
            {
                string childPath = path + "/" + associatedPath + child.Name;

                if (child.IsFolder | (child.Offset == 0 & child.Size == 0))

                {
                    if (!Directory.Exists(childPath))
                        Directory.CreateDirectory(childPath);
                }
                else
                {
                    if (!Directory.Exists(Path.GetDirectoryName(childPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(childPath));

                    FileStream childStream = new FileStream(childPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    childStream.SetLength(child.Buffer.Length);
                    childStream.Write(child.Buffer, 0, child.Buffer.Length);
                    childStream.Dispose();
                }

                if (chain)
                    ExtractItemChildrenTo(child, childPath, true, child.IsFolder ? "" : $"{child.Name}/");
            }
        }

        public void ItemSelected(object sender, TreeViewEventArgs e)
        {
            InsideItem item = (InsideItem)(e.Node.Tag);
            SelectedIndex = item.Index;
            FilePG.SelectedObject = item;
            if (item.Name.ToLower().Contains(".bmp") | item.Name.ToLower().Contains(".png"))
            {
                Bitmap texture;
                using (var ms = new MemoryStream(item.Buffer))
                {
                    try
                    {
                        texture = new Bitmap(ms);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image!\n\n{ex.Message}\n- {ex.Source}\n\nBitmap corrupted or unsupported", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        PreviewUnavailableLabel.Visible = true;
                        return;
                    }
                }
                audioPlayer.Visible = false;
                TextPreview.Visible = false;
                AnyLabel.Visible = false;
                PreviewUnavailableLabel.Visible = false;

                PreviewPictureBox.Visible = true;
                PicturePanel.Visible = true;

                PreviewPictureBox.Image = (Image)texture;
                PreviewPictureBox.Location = new Point(0, 0);

                PicturePanel.HorizontalScroll.Value = 0;
                PicturePanel.VerticalScroll.Value = 0;
                
                PicturePanel.AutoScroll = false;
                PicturePanel.VerticalScroll.Enabled = true;
                PicturePanel.VerticalScroll.Visible = true;
                PicturePanel.HorizontalScroll.Enabled = true;
                PicturePanel.HorizontalScroll.Visible = true;

                PicturePanel.HorizontalScroll.Maximum = PreviewPictureBox.Image.Width;
                PicturePanel.VerticalScroll.Maximum = PreviewPictureBox.Image.Height;
                //PicturePanel.AutoScroll = true;
            }
            else if (item.Name.ToLower().Contains(".wav") | item.Name.ToLower().Contains(".ogg") | item.Name.ToLower().Contains(".mp3"))
            {
                PreviewUnavailableLabel.Visible = false;

                PreviewPictureBox.Visible = false;
                PicturePanel.Visible = false;

                var ms = new MemoryStream(item.Buffer);
                try
                {
                    audioPlayer.SetSoundFromStream(ms);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading sound!\n\n{ex.Message}\n- {ex.Source}\n\nSound corrupted or unsupported", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    PreviewUnavailableLabel.Visible = true;
                    return;
                }
                PreviewUnavailableLabel.Visible = false;
                audioPlayer.AudioDescription.Text = item.Name;
                audioPlayer.Visible = true;
                TextPreview.Visible = false;
                AnyLabel.Visible = false;
            }
            // special sound format
            else if (item.Name.ToLower().Contains(".vag"))
            {
                PreviewUnavailableLabel.Visible = false;

                PreviewPictureBox.Visible = false;
                PicturePanel.Visible = false;

                //try
                //{
                    var ms = new MemoryStream(VAGDecoder.GetWAVFromVagData( item.Buffer ));
                    audioPlayer.SetSoundFromStream(ms);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show($"Error loading VAG format sound!\n\n{ex.Message}\n- {ex.Source}\n\nSound corrupted or unsupported", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                audioPlayer.AudioDescription.Text = item.Name;
                audioPlayer.Visible = true;
                TextPreview.Visible = false;
                AnyLabel.Visible = false;
            }
            else if (item.Name.ToLower().Contains(".txt") | item.Name.ToLower().Contains(".cfg") | item.Name.ToLower().Contains(".dir"))
            {
                PreviewUnavailableLabel.Visible = false;

                PreviewPictureBox.Visible = false;
                PicturePanel.Visible = true; // necessary
                audioPlayer.Visible = false;
                PicturePanel.AutoScroll = true;
                PicturePanel.VerticalScroll.Enabled = false;
                PicturePanel.VerticalScroll.Visible = false;
                PicturePanel.HorizontalScroll.Enabled = false;
                PicturePanel.HorizontalScroll.Visible = false;


                TextPreview.Text = Encoding.Default.GetString(item.Buffer);
                TextPreview.ReadOnly = true;
                TextPreview.Visible = true;
                AnyLabel.Visible = false;
            }
            else if (item.IsFolder)
            {
                PreviewUnavailableLabel.Visible = false;

                PreviewPictureBox.Visible = false;
                PicturePanel.Visible = true; // necessary
                audioPlayer.Visible = false;
                PicturePanel.AutoScroll = true;
                TextPreview.Visible = false;

                AnyLabel.Visible = true;
                AnyLabel.Text = Package.GetItemFullPath(item);
            }
            else
            {
                PreviewPictureBox.Visible = false;
                PicturePanel.Visible = true;
                audioPlayer.Visible = false;
                TextPreview.Visible = false;
                AnyLabel.Visible = false;

                PreviewUnavailableLabel.Visible = true;
                PreviewUnavailableLabel.Update();
            }
        }

        public void GenerateList()
        {
            FileTree.Nodes.Clear();
            if (OpenPackage != null)
            {
                if (!DontSort)
                    CreateItems(OpenPackage.Items[0]);
                else
                    CreateItemsUnSorted();
            }
            UpdateTools();
        }

        public void SetIconForNode(TreeNode node)
        {
            InsideItem citem = node.Tag as InsideItem;
            // root icon
            if (citem.Index == 0)
                node.ImageIndex = 6; // root icon
            if (citem.IsFolder)
                node.ImageIndex = 1; // folder icon
            if (citem.Name.ToLower().Contains(".cfg") | citem.Name.ToLower().Contains(".cfb"))
                node.ImageIndex = 2; // config icon
            if (citem.Name.ToLower().Contains(".wav") | citem.Name.ToLower().Contains(".ogg") | citem.Name.ToLower().Contains(".mp3") | citem.Name.ToLower().Contains(".vag") | citem.Name.ToLower().Contains(".dsp"))
                node.ImageIndex = 3; // audio icon
            if (citem.Name.ToLower().Contains(".bmp") | citem.Name.ToLower().Contains(".png") | citem.Name.ToLower().Contains(".tga") | citem.Name.ToLower().Contains(".dds"))
                node.ImageIndex = 4; // image icon
            if (citem.Name.ToLower().Contains(".dff") | citem.Name.ToLower().Contains(".mdl") | citem.Name.ToLower().Contains(".tpl") | citem.Name.ToLower().Contains(".psf"))
                node.ImageIndex = 5; // model icon
            if (citem.Name.ToLower().Contains(".txt") | citem.Name.ToLower().Contains(".text"))
                node.ImageIndex = 7; // text icon
            
            // fix on selecting
            node.SelectedImageIndex = node.ImageIndex;
        }

        public string GetItemDisplayName(InsideItem item)
        {
            if (showMoreInfo)
            {
                if (item.Index != 0)
                    return $"[{item.Index}] {item.Name} (0x{item.CRC:X8})";
                else
                    return $"[{item.Index}] (Root) (0x{item.CRC:X8})";
            }
            else
            {
                if (item.Index != 0)
                    return $"{item.Name}";
                else
                    return $"(Root)";
            }
        }

        public void CreateItems(InsideItem item, bool createNewNode = true, TreeNodeCollection collection = null)
        {
            if (OpenPackage.Items.Count == 0 | OpenPackage == null)
                return;
            //int index = startindex;

            //TreeNode parentistic = null;
            TreeNodeCollection col = FileTree.Nodes;
            if (collection != null)
                col = collection;
            //if (parentistic != null && NoParents == false)
            //    col = parentistic.Nodes;
            TreeNode node = null;
            if (createNewNode)
            {
                string name = OpenPackage.GetItemName(item.Index);
                node = col.Add(GetItemDisplayName(item));
                node.Tag = item; // source
                // root icon
                if (item.Index == 0)
                    node.ImageIndex = 6; // root icon

                node.SelectedImageIndex = node.ImageIndex;
            }
            if (item.IsFolder)
            {
                foreach (InsideItem citem in item.Children)
                {
                    string itemname = OpenPackage.GetItemName(citem.Index);
                    string name = GetItemDisplayName(citem); //$"[{citem.Index}] {itemname} (0x{citem.CRC:X8})";
                    TreeNode cnode = null;
                    if (node != null)
                        cnode = node.Nodes.Add(name);
                    else
                        cnode = col.Add(name);

                    cnode.Tag = citem;
                    SetIconForNode(cnode);
                    if (citem.IsFolder)
                        CreateItems(citem, false, cnode.Nodes);
                }
            }
        }

        public void CreateItemsUnSorted()
        {
            foreach (InsideItem item in OpenPackage.Items)
            {
                string name = OpenPackage.GetItemName(item.Index);
                TreeNode node = FileTree.Nodes.Add(GetItemDisplayName(item));
                node.Tag = item; // source
                SetIconForNode(node);
                CreateItems(item, false, node.Nodes);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog() {
                Filter = "RenderWare Package Files|*.wad",
                Title = "Open RenderWare Package File "+ (loadedOldFormat ? "(Old Format)" : "(New Format)")
            };


            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (OpenPackage != null)
                    OpenPackage.Dispose();

                // old format detection
                FileStream fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read);
                using (var f = new BinaryReader(fs, Encoding.Default, false))
                {
                    fs.Position = 0x13;
                    short vertest = f.ReadInt16();
                    if (vertest == -1 & loadedOldFormat)
                    {
                        if (MessageBox.Show("The WAD file seems to be in new format\nDo you want to open it in the new format?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            loadedOldFormat = false;
                            saveInNewFormat = true;
                            updateOldNewFMTButtons();
                            updateOldNewFMTButtons2();
                        }
                    }
                    if (vertest == 255 & !loadedOldFormat)
                    {
                        if (MessageBox.Show("The WAD file seems to be in old format\nDo you want to open it in the old format?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            loadedOldFormat = true;
                            saveInNewFormat = false;
                            updateOldNewFMTButtons();
                            updateOldNewFMTButtons2();
                        }
                    }
                }

                showErrors = true;
                ProgressBar.Enabled = true;
                if (!loadedOldFormat)
                    OpenPackage = new Package();
                else
                    OpenPackage = new PackageOld();

                OpenPackage.OnLoadFileBufferFail += OnFileBufferLoadFail;
                OpenPackage.FileLoaded += delegate (PackageFileLoadedEventArgs ev) {
                    float pg = (float)(ev.File.Index) / ev.ToLoad;
                    ProgressBar.Value = (int)(pg*100);
                    ProgressBar.Update();
                    //StatusLabel.Text = "Loading... " + ev.File.Index.ToString() + " / " + ev.ToLoad.ToString();
                    //StatusStripbar.Update();
                };
                OpenPackage.Loaded += delegate {
                    ProgressBar.Enabled = false;
                    ProgressBar.Value = 0;
                    GenerateList();
                    firstTimeOpening = true;
                    OpenPackage.FileName = fileDialog.FileName;
                    InitTools();
                };


                OpenPackage.Load(fileDialog.FileName);
                // LoadThread_FileName = fileDialog.FileName;
                // LoadThread.Start();
            }
        }

        public void OpenNew(object sender, EventArgs e)
        {
            loadedOldFormat = false;
            saveInNewFormat = true;
            updateOldNewFMTButtons();
            updateOldNewFMTButtons2();

            openToolStripMenuItem_Click(sender, e);
        }
        public void OpenOld(object sender, EventArgs e)
        {
            loadedOldFormat = true;
            saveInNewFormat = false;
            updateOldNewFMTButtons();
            updateOldNewFMTButtons2();

            openToolStripMenuItem_Click(sender, e);
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SOFTWARE UNDER MIT LICENSE\nCOPYRIGHT 2023 (C) BUILDERDEMO7\n\nAuthor: BuilderDemo7\nGitHub: @BuilderDemo7\n\nPurpose:\nOpen .WAD files from RenderWare", "About", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void ExtractButton_Click(object sender, EventArgs e)
        {
            if (CheckedItems.Count==0 && FilePG.SelectedObject == null)
            {
                MessageBox.Show("There are no specified items", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (CheckedItems.Count>1)
            {
                FolderBrowserDialog folder = new FolderBrowserDialog()
                {
                    Description = "Choose a folder to extract the checked files"
                };
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("=== EXTRACTION STARTED ===");
                    foreach (InsideItem item in CheckedItems)
                    {
                        string itemPath = folder.SelectedPath + @"\" + item.Name;

                        Console.WriteLine(String.Format("Extracting -> {0}",Package.GetItemFullPath(item)));

                        if (item.IsFolder)
                        {
                            ExtractItemChildrenTo(item, itemPath, true);
                            Console.WriteLine("Created directory -> "+itemPath);
                        }
                        else
                        {
                            if (File.Exists(itemPath))
                            {
                                if (MessageBox.Show(String.Format("The file {0} already exists\nOverwrite?", itemPath), "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                                {
                                    continue;
                                }
                            }
                            byte[] buffer = OpenPackage.GetItemBuffer(item.Index);
                            Console.WriteLine(String.Format(" Writing {0} bytes to this", buffer.Length));
                            FileStream file = new FileStream(itemPath, FileMode.OpenOrCreate, FileAccess.Write);
                            file.Write(buffer, 0, buffer.Length);
                            file.Close();
                        }

                    }
                }
                Console.WriteLine("=== EXTRACTION ENDED ===");
                MessageBox.Show(String.Format("Successfully extracted items to folder '{0}'!", folder.SelectedPath), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (FilePG.SelectedObject != null)
            {
                InsideItem item = (InsideItem)FilePG.SelectedObject;
                if (item.Size == 0 && !item.IsFolder)
                {
                    MessageBox.Show("The selected file has no data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (item.IsFolder)
                {
                    FolderBrowserDialog folder = new FolderBrowserDialog()
                    {
                        Description = "Choose a folder to extract the selected folder"
                    };
                    if (folder.ShowDialog() == DialogResult.OK)
                    {
                        ExtractItemChildrenTo(item, folder.SelectedPath+"/"+item.Name, true);
                        MessageBox.Show(String.Format("Successfully extracted folder to '{0}'!", folder.SelectedPath), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                string fileName = OpenPackage.GetItemName(SelectedIndex);
                SaveFileDialog saveFile = new SaveFileDialog()
                {
                    FileName = fileName,
                    Filter = "Any file|*.*",
                    Title = "Extract file to.."
                };

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    byte[] buffer = OpenPackage.GetItemBuffer(SelectedIndex);

                    FileStream file = new FileStream(saveFile.FileName, FileMode.Create, FileAccess.Write);
                    file.Write(buffer, 0, buffer.Length);
                    file.Close();

                    MessageBox.Show(String.Format("Successfully extracted to '{0}'!", saveFile.FileName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //else
            //    MessageBox.Show("No item to extract!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "RenderWare Package File|*.wad",
                Title = "Save RenderWare Package File As " + (saveInNewFormat ? "(Old Format)" : "(New Format)")
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                byte[] buffer = OpenPackage.RegenerateAndReturnBuffer(saveInNewFormat);

                OpenPackage.Dispose(); // dispose old stream
                FileStream file = new FileStream(saveFile.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                file.Write(buffer, 0, buffer.Length);
                file.Close();

                MessageBox.Show(String.Format("Successfully saved to '{0}'!", saveFile.FileName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            if (FilePG.SelectedObject == null)
            {
                MessageBox.Show("No file to replace!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "All Files|*.*",
                Title = "Replace With..."
            };

            if (fileDialog.ShowDialog() == DialogResult.OK && SelectedIndex > 0)
            {
                InsideItem item = OpenPackage.Items[SelectedIndex];

                bool confirm = true;
                if (item.IsFolder)
                {
                    if (MessageBox.Show("The selected item is a folder, do you still want to replace it's buffer anyway?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        confirm = false;
                    }
                }
                if (!confirm)
                    return;

                FileStream repfStream = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[repfStream.Length];
                repfStream.Read(buffer, 0, (int)repfStream.Length);
                repfStream.Dispose(); // close and dispose

                OpenPackage.Items[SelectedIndex].Buffer = buffer;
                OpenPackage.Items[SelectedIndex].Size = (uint)buffer.Length;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will overwrite the old file WITH NO BACKUPS, continue?", "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            byte[] buffer = OpenPackage.RegenerateAndReturnBuffer(saveInNewFormat);

            OpenPackage.Dispose(); // dispose old stream
            FileStream file = new FileStream(OpenPackage.FileName, FileMode.OpenOrCreate, FileAccess.Write);
            file.Write(buffer, 0, buffer.Length);
            file.Close();

            MessageBox.Show(String.Format("Successfully saved to '{0}'!", OpenPackage.FileName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dontSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DontSort = dontSortToolStripMenuItem.Checked;
            GenerateList();
        }

        private void FileTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            InsideItem item = e.Node.Tag as InsideItem;
            if (item.Index == 0)
                return; // don't mess with root
            if (item != null)
            {
                if (item.IsFolder)
                {
                    e.Node.ImageIndex = 9;
                    e.Node.SelectedImageIndex = e.Node.ImageIndex;
                }
            }
        }

        private void FileTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            InsideItem item = e.Node.Tag as InsideItem;
            if (item.Index == 0)
                return; // don't mess with root
            if (item != null)
            {
                if (item.IsFolder)
                {
                    e.Node.ImageIndex = 1;
                    e.Node.SelectedImageIndex = e.Node.ImageIndex;
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (FilePG.SelectedObject == null)
            {
                MessageBox.Show("No file to delete!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (SelectedIndex == 0)
            {
                MessageBox.Show("You can't delete the root of all files!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this item?\nYou can't undo this action!!", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                InsideItem item = FilePG.SelectedObject as InsideItem;
                if (item != null)
                {
                    FileTree.SelectedNode.Remove(); // remove from tree

                    item.Parent.Children.Remove(item);
                    item.Buffer = new byte[0]; // dispose buffer to free memory
                    item.Size = 0;
                    item.Name = null;
                    item = null; // fully dispose it
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (FilePG.SelectedObject == null)
            {
                MessageBox.Show("Please select a item to add your file into", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (FilePG.SelectedObject != null)
            {
                InsideItem item = FilePG.SelectedObject as InsideItem;
                if (!item.IsFolder)
                {
                    MessageBox.Show("The selected item is not a folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Any Files|*.*",
                Title = "Select files to add",
                Multiselect = true
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                InsideItem selectedItem = FilePG.SelectedObject as InsideItem;
                foreach (string filename in fileDialog.FileNames)
                {
                    FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[file.Length];
                    using (var br = new BinaryReader(file, Encoding.Default, false))
                    {
                        buffer = br.ReadBytes(buffer.Length);
                    }
                    int crc = (int)(buffer.Length*32);
                    InsideItem item = new InsideItem(false, 0, crc, 0, (uint)buffer.Length, (uint)buffer.Length, 0, 0, 0)
                    {
                        Name = Path.GetFileName(file.Name),
                        Index = OpenPackage.Items.Count,
                        Buffer = buffer
                    };

                    OpenPackage.Items.Add(item);
                    selectedItem.Children.Add(item);
                }

                // update the list
                GenerateList();
            }
        }

        private void NewFolderButton_Click(object sender, EventArgs e)
        {
            if (FilePG.SelectedObject == null)
            {
                MessageBox.Show("Please select a folder to add your new folder into", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (FilePG.SelectedObject != null)
            {
                InsideItem xitem = FilePG.SelectedObject as InsideItem;
                if (!xitem.IsFolder)
                {
                    MessageBox.Show("The selected item is not a folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            InsideItem selectedItem = FilePG.SelectedObject as InsideItem;
            Random randomCRCMagic = new Random(0xCC * OpenPackage.Items.Count + Package.Magic);
            Random randomCRC = new Random(randomCRCMagic.Next());

            InsideItem folder = new InsideItem(false, 0, randomCRC.Next(0xCCCCCCC, 0xFFFFFFF), 0, 0, 0, 0, 0, 0)
            {
                Name = "New Folder",
                Index = OpenPackage.Items.Count,
                Children = new List<InsideItem>()
            };

            OpenPackage.Items.Add(folder);
            selectedItem.Children.Add(folder);

            GenerateList();
        }

        // not working :(
        private void FilePG_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem!=null & e.ChangedItem.Label=="Name")
            {
                InsideItem item = FilePG.SelectedObject as InsideItem;
                string oldName = e.OldValue as string;
                string newName = item.Name;
                item.Name = oldName;
                string oldDisplayName = GetItemDisplayName(item);
                item.Name = newName;

                TreeNode[] criteria = FileTree.Nodes.Find(GetItemDisplayName(item), true);
                TreeNode sourceNode = null;
                if (criteria != null & criteria.Length > 0)
                    sourceNode = criteria[0];

                if (sourceNode!=null)
                   sourceNode.Name = GetItemDisplayName(item);
            }
        }

        public void updateOldNewFMTButtons()
        {
            NewFormatButton.Checked = saveInNewFormat;
            OldFormatButton.Checked = !saveInNewFormat;
        }

        public void updateOldNewFMTButtons2()
        {
            oldFormatToolStripMenuItem1.Checked = loadedOldFormat;
            newFormatToolStripMenuItem1.Checked = !loadedOldFormat; 
        }

        private void NewFormatButton_Click(object sender, EventArgs e)
        {
            saveInNewFormat = true;
            updateOldNewFMTButtons();
        }

        private void oldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveInNewFormat = false;
            updateOldNewFMTButtons();
        }

        private void showMoreInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMoreInfo = ShowMoreInfoBTN.Checked;
            GenerateList();
        }

        private void FileTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            InsideItem item = e.Node.Tag as InsideItem;
            if (e.Node.Checked)
                CheckedItems.Add(item);
            else
                CheckedItems.Remove(item);
        }

        private void newFormatToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            loadedOldFormat = false;
            updateOldNewFMTButtons2();
        }

        private void oldFormatToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            loadedOldFormat = true;
            updateOldNewFMTButtons2();
        }

        private void showConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllocConsole();
            ConsoleAllocated = true;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void testButtondffFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FilePG.SelectedObject == null)
            {
                MessageBox.Show("Nothing is selected");
                return;
            }

            InsideItem file = FilePG.SelectedObject as InsideItem;
            MemoryStream stream = new MemoryStream(file.Buffer);
            DFF dff = new DFF(stream);
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Save DFF as OBJ",
                Filter = "Wavefront OBJ|*.obj",
                FileName = Path.GetFileNameWithoutExtension(file.Name)+".obj"
            };
            if (saveFileDialog.ShowDialog()==DialogResult.OK)
            {
                FileStream f = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);

                // MTL
                string mtlFileNameNotFull = Path.GetFileNameWithoutExtension(saveFileDialog.FileName) + ".mtl";
                string mtlFileName = Path.GetDirectoryName(saveFileDialog.FileName) + @"\" + mtlFileNameNotFull;

                FileStream fmtl = new FileStream(mtlFileName, FileMode.OpenOrCreate, FileAccess.Write);

                byte[] obj = Encoding.Default.GetBytes(dff.AsOBJ(mtlFileNameNotFull));
                byte[] mtl = Encoding.Default.GetBytes(dff.AsOBJMTL());

                f.Write(obj,0, obj.Length);
                fmtl.Write(mtl, 0, mtl.Length);

                f.Close();
                fmtl.Close();
                MessageBox.Show("Successfully exported as WaveFront OBJ!","Success",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void ImportOBJ_Click(object sender, EventArgs e)
        {
            if (FilePG.SelectedObject == null)
            {
                MessageBox.Show("No file is selected to import .OBJ!");
                return;
            }

            InsideItem file = FilePG.SelectedObject as InsideItem;

            OpenFileDialog openOBJ = new OpenFileDialog()
            {
                Title = "Open WaveFront .OBJ and Convert as DDI RenderWare DFF",
                Filter = "WaveFront OBJ|*.obj|All Files|*.*"
            };
            if (openOBJ.ShowDialog() == DialogResult.OK)
            {
                DFF objDFF = DFF.FromOBJ(openOBJ.FileName);
                file.Buffer = objDFF.GetBytes();
                MessageBox.Show("Successfully imported from WaveFront OBJ!","Success",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void convertdffToobjToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDFF = new OpenFileDialog()
            {
                Title = "Open DDI RenderWare DFF and Convert to WaveFront .OBJ",
                Filter = "Dive File Format|*.dff|All Files|*.*"
            };
            if (openDFF.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(openDFF.FileName, FileMode.Open, FileAccess.Read);
                DFF dff = new DFF(stream);
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Save Converted DFF to WaveFront .OBJ as",
                    Filter = "Wavefront OBJ|*.obj",
                    FileName = Path.GetFileNameWithoutExtension(openDFF.FileName) + ".obj"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileStream f = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);

                    // MTL
                    string mtlFileNameNotFull = Path.GetFileNameWithoutExtension(saveFileDialog.FileName) + ".mtl";
                    string mtlFileName = Path.GetDirectoryName(saveFileDialog.FileName) + @"\" + mtlFileNameNotFull;

                    FileStream fmtl = new FileStream(mtlFileName, FileMode.OpenOrCreate, FileAccess.Write);

                    byte[] obj = Encoding.Default.GetBytes(dff.AsOBJ(mtlFileNameNotFull));
                    byte[] mtl = Encoding.Default.GetBytes(dff.AsOBJMTL());

                    f.Write(obj, 0, obj.Length);
                    fmtl.Write(mtl, 0, mtl.Length);

                    f.Close();
                    fmtl.Close();
                    MessageBox.Show("Successfully exported as WaveFront OBJ!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void converobjTodffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openOBJ = new OpenFileDialog()
            {
                Title = "Open WaveFront .OBJ and Convert to DDI RenderWare DFF",
                Filter = "WaveFront OBJ|*.obj|All Files|*.*"
            };
            if (openOBJ.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(openOBJ.FileName, FileMode.Open, FileAccess.Read);
                DFF dff = DFF.FromOBJ(openOBJ.FileName);
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Save Converted OBJ to DFF as",
                    Filter = "Dive File Format|*.dff",
                    FileName = Path.GetFileNameWithoutExtension(openOBJ.FileName) + ".dff"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] buffer = dff.GetBytes();
                    FileStream f = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                    f.Write(buffer, 0, buffer.Length);
                    f.Close();
                    MessageBox.Show("Successfully exported as DDI RenderWare Dive File Format!","Success",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
        }

        private void convertdffToplyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDFF = new OpenFileDialog()
            {
                Title = "Open DDI RenderWare DFF and Convert to Stanford .PLY",
                Filter = "Dive File Format|*.dff|All Files|*.*"
            };
            if (openDFF.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(openDFF.FileName, FileMode.Open, FileAccess.Read);
                DFF dff = new DFF(stream);
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Save Converted DFF to PLY as",
                    Filter = "Stanford PLY|*.ply",
                    FileName = Path.GetFileNameWithoutExtension(openDFF.FileName) + ".ply"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {

                    byte[] ply = Encoding.Default.GetBytes(dff.AsPLY());

                    FileStream f = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);

                    f.Write(ply, 0, ply.Length);

                    f.Close();
                    MessageBox.Show("Successfully exported as Stanford PLY!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void toCFGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openCFB = new OpenFileDialog()
            {
                Title = "Open Binary Configuration File and Convert to CFG",
                Filter = "Binary Configuration File|*.cfb|All Files|*.*"
            };
            if (openCFB.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(openCFB.FileName, FileMode.Open, FileAccess.Read);
                BinaryConfiguration cfb = new BinaryConfiguration(stream);
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Save Converted CFB to CFG as",
                    Filter = "Configuration File|*.cfg",
                    FileName = Path.GetFileNameWithoutExtension(openCFB.FileName) + ".cfg"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {

                    byte[] cfg = Encoding.Default.GetBytes(cfb.ToString());

                    FileStream f = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);

                    f.Write(cfg, 0, cfg.Length);

                    f.Close();
                    MessageBox.Show("Successfully converted CFB to CFG!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
