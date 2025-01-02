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
            panel1.SuspendLayout();
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(propertyGrid1);
            panel1.Controls.Add(treeView2);
            panel1.Controls.Add(statusStrip1);
            panel1.Controls.Add(treeView1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 24);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 426);
            panel1.TabIndex = 0;
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
            statusStrip1.Location = new Point(0, 404);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
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
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // loadTerrainToolStripMenuItem
            // 
            loadTerrainToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { runEditorToolStripMenuItem, loadTexturesToolStripMenuItem, loadTerrainToolStripMenuItem1 });
            loadTerrainToolStripMenuItem.Name = "loadTerrainToolStripMenuItem";
            loadTerrainToolStripMenuItem.Size = new Size(37, 20);
            loadTerrainToolStripMenuItem.Text = "File";
            loadTerrainToolStripMenuItem.Click += loadTerrainToolStripMenuItem_Click;
            // 
            // runEditorToolStripMenuItem
            // 
            runEditorToolStripMenuItem.Name = "runEditorToolStripMenuItem";
            runEditorToolStripMenuItem.Size = new Size(146, 22);
            runEditorToolStripMenuItem.Text = "Run Editor";
            runEditorToolStripMenuItem.Click += runEditorToolStripMenuItem_Click;
            // 
            // loadTexturesToolStripMenuItem
            // 
            loadTexturesToolStripMenuItem.Name = "loadTexturesToolStripMenuItem";
            loadTexturesToolStripMenuItem.Size = new Size(146, 22);
            loadTexturesToolStripMenuItem.Text = "Load Textures";
            loadTexturesToolStripMenuItem.Click += loadTexturesToolStripMenuItem_Click;
            // 
            // loadTerrainToolStripMenuItem1
            // 
            loadTerrainToolStripMenuItem1.Name = "loadTerrainToolStripMenuItem1";
            loadTerrainToolStripMenuItem1.Size = new Size(146, 22);
            loadTerrainToolStripMenuItem1.Text = "Load Terrain";
            loadTerrainToolStripMenuItem1.Click += loadTerrainToolStripMenuItem1_Click;
            // 
            // GameEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
    }
}