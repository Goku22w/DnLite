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
        private TokenControl currentSelectedToken;
        private DodecahedronControl dodecaControl;
        public int? LastRoll { get; private set; }
        public event Action<int> RollCompleted;

        public DnLiteDisplay()
        {
            InitializeComponent();
        }

        // Property to get the currently selected token
        public TokenControl CurrentSelectedToken
        {
            get { return currentSelectedToken; }
        }

        // Add a single initiative entry to the initiative list
        public void AddInitiativeEntry(string entry)
        {
            if (string.IsNullOrEmpty(entry)) return;
            InitiativeList.Items.Add(entry);
        }

        // Generate a unique name for initiative entries by adding numbering (-1-, -2-, etc.) if duplicates exist
        public string GetUniqueInitiativeName(string baseName)
        {
            if (string.IsNullOrEmpty(baseName)) return baseName;

            // Check if this exact name already exists in the initiative list
            bool baseNameExists = false;
            int highestNumber = 0;

            foreach (var item in InitiativeList.Items)
            {
                string entry = item.ToString();
                if (entry.Contains("Initiative |"))
                {
                    // Extract the name part after "Initiative | "
                    string[] parts = entry.Split(new[] { "Initiative |" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        string entryName = parts[1].Trim();

                        // Check if it matches the base name exactly
                        if (entryName.Equals(baseName, StringComparison.OrdinalIgnoreCase))
                        {
                            baseNameExists = true;
                        }
                        // Check if it matches the base name with a number suffix (e.g., "Name -1-")
                        else if (entryName.StartsWith(baseName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Try to extract the number from the suffix
                            string suffix = entryName.Substring(baseName.Length).Trim();
                            if (suffix.StartsWith("-") && suffix.EndsWith("-"))
                            {
                                string numberPart = suffix.Trim('-');
                                if (int.TryParse(numberPart, out int number))
                                {
                                    highestNumber = Math.Max(highestNumber, number);
                                }
                            }
                        }
                    }
                }
            }

            // If the base name exists, we need to add numbering
            if (baseNameExists || highestNumber > 0)
            {
                // Return the base name with the next available number
                return $"{baseName} -{highestNumber + 1}-";
            }

            // No duplicates found, return the original name
            return baseName;
        }

        // Generate a unique name for tokens on the grid by adding numbering (-1-, -2-, etc.) if duplicates exist
        public string GetUniqueTokenName(string baseName)
        {
            if (string.IsNullOrEmpty(baseName)) return baseName;

            // Check if this exact name already exists in tokens on the grid
            bool baseNameExists = false;
            int highestNumber = 0;

            foreach (Control control in gridPanel.Controls)
            {
                if (control is TokenControl token && token.Tag is TokenData td)
                {
                    string tokenName = td.Name;
                    if (string.IsNullOrEmpty(tokenName)) continue;

                    // Check if it matches the base name exactly
                    if (tokenName.Equals(baseName, StringComparison.OrdinalIgnoreCase))
                    {
                        baseNameExists = true;
                    }
                    // Check if it matches the base name with a number suffix (e.g., "Name -1-")
                    else if (tokenName.StartsWith(baseName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Try to extract the number from the suffix
                        string suffix = tokenName.Substring(baseName.Length).Trim();
                        if (suffix.StartsWith("-") && suffix.EndsWith("-"))
                        {
                            string numberPart = suffix.Trim('-');
                            if (int.TryParse(numberPart, out int number))
                            {
                                highestNumber = Math.Max(highestNumber, number);
                            }
                        }
                    }
                }
            }

            // If the base name exists, we need to add numbering
            if (baseNameExists || highestNumber > 0)
            {
                // Return the base name with the next available number
                return $"{baseName} -{highestNumber + 1}-";
            }

            // No duplicates found, return the original name
            return baseName;
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
            pcForm.Show();
            adminForm.Show(); //Create and show the other forms on load
            this.Focus(); //Ensure the display form is focused after showing the other forms
            DiceRollOutputLabel.Text = "0";
        }

        private void RoleDieButton_Click(object sender, EventArgs e)
        {
            // Disable button to prevent re-entry
            RoleDieButton.Enabled = false;

            // Create dodecahedron animation control and add to DiceDisplayPanel
            try
            {
                if (dodecaControl == null)
                {
                    // Place in the DiceDisplayPanel
                    dodecaControl = new DodecahedronControl()
                    {
                        Location = new System.Drawing.Point(0, 0),
                        Size = new System.Drawing.Size(DiceDisplayPanel.ClientSize.Width, DiceDisplayPanel.ClientSize.Height),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
                    };
                    DiceDisplayPanel.Controls.Add(dodecaControl);
                    dodecaControl.BringToFront();
                }

                // ensure we don't add multiple handlers
                dodecaControl.RollCompleted -= OnRollCompleted;
                dodecaControl.RollCompleted += OnRollCompleted;
                // Start with faster spin: fewer frames and higher speed multiplier
                dodecaControl.StartAnimation(60, 3.5f);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start dodecahedron animation: {ex.Message}");
                RoleDieButton.Enabled = true;
            }
        }

        private void OnRollCompleted(int roll)
        {
            // Unsubscribe to avoid repeated calls
            if (dodecaControl != null) dodecaControl.RollCompleted -= OnRollCompleted;

            // Get level modifier from selected token
            int levelModifier = 0;
            string tokenInfo = "";
            if (CurrentSelectedToken != null)
            {
                if (CurrentSelectedToken.Tag is TokenData tokenData)
                {
                    levelModifier = tokenData.Lvl;
                    tokenInfo = $" ({tokenData.Name ?? CurrentSelectedToken.Letter.ToString()})";
                }
            }

            int totalRoll = roll + levelModifier;
            LastRoll = totalRoll;

            // Update UI label inside this form instead of MessageBox and do not close the form
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((Action)(() => OnRollCompleted(roll)));
                    return;
                }
                // show result in DiceRollOutputLabel with modifier breakdown
                try
                {
                    if (levelModifier > 0)
                    {
                        DiceRollOutputLabel.Text = $"{roll} +{levelModifier}";
                    }
                    else
                    {
                        DiceRollOutputLabel.Text = roll.ToString();
                    }
                }
                catch { }
                // re-enable roll button
                try { RoleDieButton.Enabled = true; } catch { }
                // raise external event so other forms can react
                RollCompleted?.Invoke(totalRoll);
            }
            catch { }
        }

        // Forward request to add a palette token into the admin palette
        public void AddPaletteTokenToAdmin(char letter, Color fillColor, TokenData data, int gridW = 1, int gridH = 1, string imagePath = "")
        {
            try
            {
                adminForm?.CreatePaletteTokenWithDataInAdmin(letter, fillColor, data, gridW, gridH, imagePath);
            }
            catch { }
        }

        //Grid dimensions defined by total number of block columns and rows
        private int blockWidth = 8;
        private int blockHeight = 8;


        public const int CellSize = 75; //Fixed pixel size for each square grid cell

        //Dragging variables
        private bool isDragging = false;
        private Point dragStartMousePos;
        private Point dragStartControlPos;
            
        public void UpdateGridDimensions(int newHeightInBlocks, int newWidthInBlocks) //functionality for setting the grid dynamically from the admin form
        {
            //Update the block dimensions
            blockWidth = newWidthInBlocks;
            blockHeight = newHeightInBlocks;

            //Dynamically resize the panel based on the block counts
            gridPanel.Width = blockWidth * CellSize;
            gridPanel.Height = blockHeight * CellSize;

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
                    if (src is TokenControl paletteToken)
                    {
                        int gw = Math.Max(1, paletteToken.GridWidth);
                        int gh = Math.Max(1, paletteToken.GridHeight);

                        TokenControl newToken = CreateToken(paletteToken.Letter, paletteToken.FillColor, gridX, gridY, gw, gh, paletteToken.ImagePath);

                        // Clone TokenData if present and apply unique naming
                        if (paletteToken.Tag is TokenData sourceData)
                        {
                            // Clone the TokenData to give this token its own independent copy
                            TokenData clonedData = sourceData.Clone();

                            // Generate a unique name for this token based on existing tokens on the grid
                            if (!string.IsNullOrEmpty(clonedData.Name))
                            {
                                clonedData.Name = GetUniqueTokenName(clonedData.Name);
                            }

                            newToken.Tag = clonedData;
                        }
                        else if (paletteToken.Tag != null)
                        {
                            // For non-TokenData tags, just copy the reference as before
                            newToken.Tag = paletteToken.Tag;
                        }

                        // Start dragging the newly created token
                        isDragging = true;
                        dragStartMousePos = Cursor.Position;
                        dragStartControlPos = newToken.Location;
                        newToken.BringToFront();
                        newToken.Capture = true;
                    }
                    else
                    {
                        // Fallback for non-TokenControl types
                        TokenControl newToken = CreateToken('\0', Color.Gray, gridX, gridY, 1, 1, "");

                        // Start dragging the newly created token
                        isDragging = true;
                        dragStartMousePos = Cursor.Position;
                        dragStartControlPos = newToken.Location;
                        newToken.BringToFront();
                        newToken.Capture = true;
                    }
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
                    currentSelectedToken = placedToken;
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
        public TokenControl CreateToken(char letter, Color fillColor, int gridX, int gridY, int gridW = 1, int gridH = 1, string imagePath = "")
        {
            TokenControl token = new TokenControl(letter, fillColor, imagePath)
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

        public void RemoveSelectedInitiativeFunc(string tokenName)
        {
            if (string.IsNullOrEmpty(tokenName)) return;

            try
            {
                // Find and remove all initiative entries that match the token name
                // Initiative entries are formatted as "{roll} Initiative | {name}"
                var itemsToRemove = new List<object>();

                foreach (var item in InitiativeList.Items)
                {
                    string entry = item.ToString();
                    if (entry.Contains("Initiative |"))
                    {
                        // Extract the name part after "Initiative | "
                        string[] parts = entry.Split(new[] { "Initiative |" }, StringSplitOptions.None);
                        if (parts.Length > 1)
                        {
                            string entryName = parts[1].Trim();
                            if (entryName.Equals(tokenName, StringComparison.OrdinalIgnoreCase))
                            {
                                itemsToRemove.Add(item);
                            }
                        }
                    }
                }

                // Remove all matching items
                foreach (var item in itemsToRemove)
                {
                    InitiativeList.Items.Remove(item);
                }
            }
            catch
            {
                // Silently handle any errors
            }
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
    }
}
