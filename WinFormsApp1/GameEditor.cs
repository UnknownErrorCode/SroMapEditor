using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SimpleGridFly.Texture;
using System.Runtime.InteropServices;

namespace SimpleGridFly
{
    public partial class GameEditor : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        private GridGame gridGame;
        private const string MapPath = "I:\\Clients\\Exay-Origin V1.014\\Map";

        public GameEditor()
        {
            InitializeComponent();
            InitializeGridGame();
            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings
            {
                ClientSize = new Vector2i(1280, 720),
                Title = "Silkroad Map Viewer"
            };
            gridGame = new GridGame(gws, nws, MapPath);
        }

        private void InitializeGridGame()
        {
            // Allocate a console window
            AllocConsole();
            Console.WriteLine("Console window is now visible!");
        }

        private void GameEditor_Load(object sender, EventArgs e)
        {
        }

        private void loadTerrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void runEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridGame.Run();
        }

        private void loadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextureManager.InitializeTextures(MapPath);

            treeView1.Nodes.Clear();
            TreeNode textureNode = new TreeNode("Textures");
            foreach (var item in TextureManager.TextureNames)
            {
                textureNode.Nodes.Add(new TreeNode(item.Value.Replace(Path.Combine(MapPath, "tile2d"), "")) { Tag = item.Key });
            }
            treeView1.Nodes.Add(textureNode);
        }

        private void loadTerrainToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gridGame.InitializeTerrainMeshes();

            treeView2.Nodes.Clear();
            TreeNode meshNode = new TreeNode("TerrainMesh");
            foreach (var item in gridGame.LoadedTerrains)
            {
                meshNode.Nodes.Add(new TreeNode($"Region: {item.Item3.Replace($"{MapPath}\\", "")}") { Tag = item });
            }
            treeView2.Nodes.Add(meshNode);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
        }

        private void loadMapObjectInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridGame.InitializeMapObjectInfo();
            treeView3.Nodes.Clear();

            TreeNode MapObjectNode = new TreeNode("Map Objects");

            foreach (var item in gridGame.LoadedMapObjects)
            {
                MapObjectNode.Nodes.Add(new TreeNode(item.FilePath) { Tag = item });
            }

            treeView3.Nodes.Add(MapObjectNode);
        }

        private void treeView3_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
        }

        private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView3.SelectedNode?.Tag is MapObject obj)
            {
                propertyGrid1.SelectedObject = obj;

                string filePath = Path.Combine("I:\\Clients\\Exay-Origin V1.014\\Data", obj.FilePath);
                if (File.Exists(filePath))
                {
                    JMXFiles.JMXbsrFile bsrFile = new JMXFiles.JMXbsrFile(File.ReadAllBytes(filePath));
                }
            }
        }
    }
}