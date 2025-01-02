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
            menuStrip1 = new MenuStrip();
            loadTerrainToolStripMenuItem = new ToolStripMenuItem();
            runEditorToolStripMenuItem = new ToolStripMenuItem();
            loadTexturesToolStripMenuItem = new ToolStripMenuItem();
            loadTerrainToolStripMenuItem1 = new ToolStripMenuItem();
            treeView1 = new TreeView();
            panel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(treeView1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 24);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 426);
            panel1.TabIndex = 0;
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
            runEditorToolStripMenuItem.Size = new Size(180, 22);
            runEditorToolStripMenuItem.Text = "Run Editor";
            runEditorToolStripMenuItem.Click += runEditorToolStripMenuItem_Click;
            // 
            // loadTexturesToolStripMenuItem
            // 
            loadTexturesToolStripMenuItem.Name = "loadTexturesToolStripMenuItem";
            loadTexturesToolStripMenuItem.Size = new Size(180, 22);
            loadTexturesToolStripMenuItem.Text = "Load Textures";
            loadTexturesToolStripMenuItem.Click += loadTexturesToolStripMenuItem_Click;
            // 
            // loadTerrainToolStripMenuItem1
            // 
            loadTerrainToolStripMenuItem1.Name = "loadTerrainToolStripMenuItem1";
            loadTerrainToolStripMenuItem1.Size = new Size(180, 22);
            loadTerrainToolStripMenuItem1.Text = "Load Terrain";
            loadTerrainToolStripMenuItem1.Click += loadTerrainToolStripMenuItem1_Click;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(60, 49);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(320, 244);
            treeView1.TabIndex = 0;
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
    }
}