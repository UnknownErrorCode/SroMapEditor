namespace SimpleGridFly
{
    partial class GameEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            panel1 = new Panel();
            treeView3 = new TreeView();
            propertyGrid1 = new PropertyGrid();
            treeView2 = new TreeView();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelInfo = new ToolStripStatusLabel();
            treeView1 = new TreeView();
            menuStrip1 = new MenuStrip();
            loadTerrainToolStripMenuItem = new ToolStripMenuItem();
            runEditorToolStripMenuItem = new ToolStripMenuItem();
            loadTexturesToolStripMenuItem = new ToolStripMenuItem();
            loadTerrainToolStripMenuItem1 = new ToolStripMenuItem();
            loadMapObjectInfoToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tabPageTerrain = new TabPage();
            tabPageBsr = new TabPage();
            panel1.SuspendLayout();
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPageBsr.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(splitContainer1);
            panel1.Controls.Add(propertyGrid1);
            panel1.Controls.Add(treeView2);
            panel1.Controls.Add(statusStrip1);
            panel1.Controls.Add(treeView1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 24);
            panel1.Name = "panel1";
            panel1.Size = new Size(998, 592);
            panel1.TabIndex = 0;
            // 
            // treeView3
            // 
            treeView3.Location = new Point(6, 6);
            treeView3.Name = "treeView3";
            treeView3.Size = new Size(285, 244);
            treeView3.TabIndex = 4;
            treeView3.AfterSelect += treeView3_AfterSelect;
            treeView3.NodeMouseClick += treeView3_NodeMouseClick;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Location = new Point(501, 11);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(264, 248);
            propertyGrid1.TabIndex = 3;
            // 
            // treeView2
            // 
            treeView2.Location = new Point(303, 12);
            treeView2.Name = "treeView2";
            treeView2.Size = new Size(182, 244);
            treeView2.TabIndex = 2;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelInfo });
            statusStrip1.Location = new Point(0, 570);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(998, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelInfo
            // 
            toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            toolStripStatusLabelInfo.Size = new Size(118, 17);
            toolStripStatusLabelInfo.Text = "toolStripStatusLabel1";
            // 
            // treeView1
            // 
            treeView1.Location = new Point(12, 12);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(285, 244);
            treeView1.TabIndex = 0;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { loadTerrainToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(998, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // loadTerrainToolStripMenuItem
            // 
            loadTerrainToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { runEditorToolStripMenuItem, loadTexturesToolStripMenuItem, loadTerrainToolStripMenuItem1, loadMapObjectInfoToolStripMenuItem });
            loadTerrainToolStripMenuItem.Name = "loadTerrainToolStripMenuItem";
            loadTerrainToolStripMenuItem.Size = new Size(37, 20);
            loadTerrainToolStripMenuItem.Text = "File";
            loadTerrainToolStripMenuItem.Click += loadTerrainToolStripMenuItem_Click;
            // 
            // runEditorToolStripMenuItem
            // 
            runEditorToolStripMenuItem.Name = "runEditorToolStripMenuItem";
            runEditorToolStripMenuItem.Size = new Size(183, 22);
            runEditorToolStripMenuItem.Text = "Run Editor";
            runEditorToolStripMenuItem.Click += runEditorToolStripMenuItem_Click;
            // 
            // loadTexturesToolStripMenuItem
            // 
            loadTexturesToolStripMenuItem.Name = "loadTexturesToolStripMenuItem";
            loadTexturesToolStripMenuItem.Size = new Size(183, 22);
            loadTexturesToolStripMenuItem.Text = "Load Textures";
            loadTexturesToolStripMenuItem.Click += loadTexturesToolStripMenuItem_Click;
            // 
            // loadTerrainToolStripMenuItem1
            // 
            loadTerrainToolStripMenuItem1.Name = "loadTerrainToolStripMenuItem1";
            loadTerrainToolStripMenuItem1.Size = new Size(183, 22);
            loadTerrainToolStripMenuItem1.Text = "Load Terrain";
            loadTerrainToolStripMenuItem1.Click += loadTerrainToolStripMenuItem1_Click;
            // 
            // loadMapObjectInfoToolStripMenuItem
            // 
            loadMapObjectInfoToolStripMenuItem.Name = "loadMapObjectInfoToolStripMenuItem";
            loadMapObjectInfoToolStripMenuItem.Size = new Size(183, 22);
            loadMapObjectInfoToolStripMenuItem.Text = "Load MapObjectInfo";
            loadMapObjectInfoToolStripMenuItem.Click += loadMapObjectInfoToolStripMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Location = new Point(352, 267);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new Size(490, 195);
            splitContainer1.SplitterDistance = 163;
            splitContainer1.TabIndex = 5;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPageTerrain);
            tabControl1.Controls.Add(tabPageBsr);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(323, 195);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(179, 106);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(0, 0);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPageTerrain
            // 
            tabPageTerrain.Location = new Point(4, 24);
            tabPageTerrain.Name = "tabPageTerrain";
            tabPageTerrain.Padding = new Padding(3);
            tabPageTerrain.Size = new Size(315, 167);
            tabPageTerrain.TabIndex = 2;
            tabPageTerrain.Text = "M file";
            tabPageTerrain.UseVisualStyleBackColor = true;
            // 
            // tabPageBsr
            // 
            tabPageBsr.Controls.Add(treeView3);
            tabPageBsr.Location = new Point(4, 24);
            tabPageBsr.Name = "tabPageBsr";
            tabPageBsr.Padding = new Padding(3);
            tabPageBsr.Size = new Size(315, 167);
            tabPageBsr.TabIndex = 3;
            tabPageBsr.Text = "BSR";
            tabPageBsr.UseVisualStyleBackColor = true;
            // 
            // GameEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(998, 616);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "GameEditor";
            Text = "GameEditor";
            Load += GameEditor_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPageBsr.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem loadTerrainToolStripMenuItem;
        private ToolStripMenuItem runEditorToolStripMenuItem;
        private ToolStripMenuItem loadTexturesToolStripMenuItem;
        private ToolStripMenuItem loadTerrainToolStripMenuItem1;
        private TreeView treeView1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelInfo;
        private PropertyGrid propertyGrid1;
        private TreeView treeView2;
        private ToolStripMenuItem loadMapObjectInfoToolStripMenuItem;
        private TreeView treeView3;
        private SplitContainer splitContainer1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPageTerrain;
        private TabPage tabPageBsr;
    }
}