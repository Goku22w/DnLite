using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CharacterClass;
using NPCClass;
using DecoClass;
using System.Windows.Forms;
using System.Collections;

namespace DnLite
{
    public partial class DnLiteDisplay : Form
    {
        private DnLiteAdmin adminForm;
        private DnLiteDice diceFormInstance;
        public DnLiteDisplay()
        {
            InitializeComponent();
        }

        // Add a single initiative entry to the initiative list
        public void AddInitiativeEntry(string entry)
        {
            if (string.IsNullOrEmpty(entry)) return;
            InitiativeList.Items.Add(entry);
        }

        // Sort the initiative list entries (expects entries start with a numeric roll)
        public void SortInitiativeList()
        {
            try
            {
                var sortedItems = InitiativeList.Items.Cast<string>()
                                    .OrderByDescending(x =>
                                    {
                                        var parts = x.Split(' ');
                                        if (parts.Length == 0) return 0;
                                        if (int.TryParse(parts[0], out int v)) return v;
                                        return 0;
                                    })
                                    .ToArray();
                InitiativeList.Items.Clear();
                InitiativeList.Items.AddRange(sortedItems);
            }
            catch { }
        }
        private void DnLiteDisplay_Load(object sender, EventArgs e)
        {
            UpdateGridDimensions(blockWidth, blockHeight); //Initialize the grid panel boundaries on load
            DnLitePC pcForm = new DnLitePC(this);
            adminForm = new DnLiteAdmin(this);
            // No-op touch: added a harmless comment to mark file as edited before adding menu hook
            DnLiteDice diceForm = new DnLiteDice();
            pcForm.Show();
            adminForm.Show(); //Create and show the other forms on load
            diceForm.Show();
            this.Focus(); //Ensure the display form is focused after showing the other forms
        }

        // Forward request to add a palette token into the admin palette
        public void AddPaletteTokenToAdmin(char letter, Color fillColor, TokenData data, int gridW = 1, int gridH = 1)
        {
            try
            {
                adminForm?.CreatePaletteTokenWithDataInAdmin(letter, fillColor, data, gridW, gridH);
            }
            catch { }
        }

        //Grid dimensions defined by total number of block columns and rows
        #pragma warning disable IDE0044 //Disable warning for fields that could be made readonly, as we want to allow dynamic resizing of the grid
        private int blockWidth = 8;
        private int blockHeight = 10;


        public const int CellSize = 75; //Fixed pixel size for each square grid cell

        //Dragging variables
        private bool isDragging = false;
        private Point dragStartMousePos;
        private Point dragStartControlPos;
            
        public void UpdateGridDimensions(int newHeightInBlocks, int newWidthInBlocks) //functionality for setting the grid dynamically from the admin form
        {
            //Dynamically resize the panel based on the block counts
            gridPanel.Width = newWidthInBlocks * CellSize;
            gridPanel.Height = newHeightInBlocks * CellSize;

            gridPanel.Invalidate(); //Force the panel to redraw its gridlines
            foreach (Control token in gridPanel.Controls) //Check if any existing tokens are now out of bounds and push them back in
            {
                ClampTokenToGrid(token);
            }
        }
        private void GridPanel_Paint(object sender, PaintEventArgs e) // utomatically draws the grid lines inside the panel
        {
            Graphics g = e.Graphics;

            using (Pen gridPen = new Pen(Color.Black, 2f))
            {
                //Define Max Size, don't wanna draw the lines *TOO* long
                int maxWidth = blockWidth * CellSize;
                int maxHeight = blockHeight * CellSize;

                for (int i = 0; i <= blockWidth; i++) //Draw vertical column lines
                {
                    int x = i * CellSize;
                    g.DrawLine(gridPen, x, 0, x, maxHeight);
                }
                for (int j = 0; j <= blockHeight; j++) //Draw horizontal row lines
                {
                    int y = j * CellSize;
                    g.DrawLine(gridPen, 0, y, maxWidth, y);
                }
            }
        }

        public void Token_MouseDown(object sender, MouseEventArgs e) //Put em down
        {
            if (e.Button != MouseButtons.Left) return;

            Control src = (Control)sender;

            // If the token came from the palette, create a new token/deco in the grid and begin dragging that copy
            if (src.Parent != gridPanel)
            {
                // Determine grid cell under the current cursor position
                Point clientPos = gridPanel.PointToClient(Cursor.Position);
                int gridX = (int)Math.Round((double)clientPos.X / CellSize);
                int gridY = (int)Math.Round((double)clientPos.Y / CellSize);

                // If the source is a DecorationControl or represents a Decoration, create a DecorationControl on the grid
                if (src is DecorationControl paletteDeco)
                {
                    // use the Deco object stored in Tag if available
                    DecoClass.Decoration decoData = paletteDeco.Tag as DecoClass.Decoration;
                    int gw = Math.Max(1, paletteDeco.GridWidth);
                    int gh = Math.Max(1, paletteDeco.GridHeight);

                    DecorationControl newDeco = CreateDecoToken(decoData?.ImgFileLocation ?? paletteDeco.ImagePath, gridX, gridY, gw, gh);
                    if (decoData != null) newDeco.Tag = decoData;

                    // Start dragging the newly created decoration
                    isDragging = true;
                    dragStartMousePos = Cursor.Position;
                    dragStartControlPos = newDeco.Location;
                    newDeco.BringToFront();
                    newDeco.Capture = true;
                }
                else
                {
                    TokenControl paletteToken = src as TokenControl;
                    int gw = 1, gh = 1;
                    if (paletteToken != null)
                    {
                        gw = Math.Max(1, paletteToken.GridWidth);
                        gh = Math.Max(1, paletteToken.GridHeight);
                    }

                    TokenControl newToken = CreateToken(paletteToken?.Letter ?? '\0', paletteToken?.FillColor ?? Color.Gray, gridX, gridY, gw, gh);
                    // propagate metadata (TokenData or other tag) from the palette token to the placed token
                    if (paletteToken != null && paletteToken.Tag != null)
                    {
                        newToken.Tag = paletteToken.Tag;
                    }

                    // Start dragging the newly created token
                    isDragging = true;
                    dragStartMousePos = Cursor.Position;
                    dragStartControlPos = newToken.Location;
                    newToken.BringToFront();
                    newToken.Capture = true;
                }
            }
            else
            {
                // Existing token in the grid: begin dragging it
                isDragging = true;
                dragStartMousePos = Cursor.Position;
                dragStartControlPos = src.Location;
                // Ensure the token being interacted with is on top of other tokens
                src.BringToFront();
                src.Capture = true;
            }
        }

        public void Token_MouseMove(object sender, MouseEventArgs e) //Drag em around
        {
            if (isDragging)
            {
                Control token = (Control)sender;

                int deltaX = Cursor.Position.X - dragStartMousePos.X;
                int deltaY = Cursor.Position.Y - dragStartMousePos.Y;

                int newX = dragStartControlPos.X + deltaX;
                int newY = dragStartControlPos.Y + deltaY;

                // Snapping logic
                int gridX = (int)Math.Round((double)newX / CellSize);
                int gridY = (int)Math.Round((double)newY / CellSize);

                if (token is TokenControl tct)
                {
                    token.Location = CalculateClampedPosition(gridX, gridY, tct.GridWidth, tct.GridHeight);
                }
                else
                {
                    token.Location = CalculateClampedPosition(gridX, gridY, 1, 1);
                }
            }
        }

        public void Token_MouseUp(object sender, MouseEventArgs e) //Pick em up
        {
            if (e.Button != MouseButtons.Left) return;

            // Stop dragging and release mouse capture
            isDragging = false;
            try
            {
                ((Control)sender).Capture = false;
            }
            catch
            {
                // ignore if sender is not a control or capture can't be released
            }

            // When a token is placed down inside the grid, notify the admin window so it can show/edit stats
            if (sender is TokenControl placedToken && placedToken.Parent == gridPanel)
            {
                try
                {
                    adminForm?.DisplayTokenInfo(placedToken);
                }
                catch
                {
                    // ignore if adminForm not available or method not present
                }
            }
        }
        private void ClampTokenToGrid(Control token) //Helper method to keep a token safely inside the current grid bounds
        {
            int gridX = token.Location.X / CellSize;
            int gridY = token.Location.Y / CellSize;
            if (token is TokenControl tc)
            {
                token.Location = CalculateClampedPosition(gridX, gridY, tc.GridWidth, tc.GridHeight);
            }
            else
            {
                token.Location = CalculateClampedPosition(gridX, gridY, 1, 1);
            }
        }

        public Point CalculateClampedPosition(int gridX, int gridY, int gridW = 1, int gridH = 1) //A function within a Function to calculate the clamped position of a token based on the grid size and cell size
        {
            gridX = Math.Max(0, Math.Min(gridX, blockWidth - gridW));
            gridY = Math.Max(0, Math.Min(gridY, blockHeight - gridH));
            return new Point(gridX * CellSize, gridY * CellSize);
        }

        // Create a circular token with a single letter, placed at the given grid coordinates
        public TokenControl CreateToken(char letter, Color fillColor, int gridX, int gridY, int gridW = 1, int gridH = 1)
        {
            TokenControl token = new TokenControl(letter, fillColor)
            {
                GridWidth = Math.Max(1, gridW),
                GridHeight = Math.Max(1, gridH)
            };

            token.Size = new Size(token.GridWidth * CellSize, token.GridHeight * CellSize);
            token.Location = CalculateClampedPosition(gridX, gridY, token.GridWidth, token.GridHeight);
            token.MouseDown += Token_MouseDown;
            token.MouseMove += Token_MouseMove;
            token.MouseUp += Token_MouseUp;
            token.Cursor = Cursors.Hand;

            gridPanel.Controls.Add(token);
            return token;
        }

        // Create a decoration token that renders an image instead of the circular token
        public DecorationControl CreateDecoToken(string imagePath, int gridX, int gridY, int gridW = 1, int gridH = 1)
        {
            DecorationControl deco = new DecorationControl(imagePath, gridW, gridH);
            deco.Size = new Size(deco.GridWidth * CellSize, deco.GridHeight * CellSize);
            deco.Location = CalculateClampedPosition(gridX, gridY, deco.GridWidth, deco.GridHeight);
            deco.MouseDown += Token_MouseDown;
            deco.MouseMove += Token_MouseMove;
            deco.MouseUp += Token_MouseUp;
            deco.Cursor = Cursors.Hand;

            gridPanel.Controls.Add(deco);
            return deco;
        }

        // Palette functions moved to DnLiteAdmin; display no longer hosts a palette panel.

        public void TestInitiativeFunc(int count)
        {
            Random randomNumber = new Random(); //Roll Dice functionality
            for (int i = 0; i < count; i++) //randomly fill the initiative list with the given number of NPCs
            {
                InitiativeList.Items.Add(randomNumber.Next(1, 21).ToString() + " Initiative | NPC " + (i + 1).ToString());
            }
            var sortedItems = InitiativeList.Items.Cast<string>()
                                .OrderByDescending(x => int.Parse(x.Split(' ')[0]))
                                .ToArray();
            InitiativeList.Items.Clear();
            InitiativeList.Items.AddRange(sortedItems); //sorts the initiative list in descending order
        }

        public void ClearInitiativeFunc()
        {
            InitiativeList.Items.Clear(); //I bet you can guess what this does
        }

        // Remove all tokens and decorations placed on the grid panel.
        // This does not affect the palettePanel.
        public void ClearGridTokens()
        {
            // Make a copy of the controls to avoid modification during enumeration
            var toRemove = gridPanel.Controls.Cast<Control>().ToList();
            foreach (var c in toRemove)
            {
                try
                {
                    gridPanel.Controls.Remove(c);
                    c.Dispose();
                }
                catch
                {
                    // ignore individual dispose errors
                }
            }
        }

        // Remove a specific token/control from the grid panel if present
        public bool RemoveToken(Control token)
        {
            if (token == null) return false;
            if (token.Parent != gridPanel) return false;
            try
            {
                gridPanel.Controls.Remove(token);
                token.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void DnLiteDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); //Ensure the application exits when this form is closed
        }

        private void GridEmptyButton_Click(object sender, EventArgs e)
        {
            ClearGridTokens();
        }

        private void SummonDiceButton_Click(object sender, EventArgs e)
        {
            if (diceFormInstance == null || diceFormInstance.IsDisposed)
            {
                diceFormInstance = new DnLiteDice();
            }
            diceFormInstance.Show();
            diceFormInstance.BringToFront();
        }
    }
}
