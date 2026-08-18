using CharacterClass;
using DecoClass;
using NPCClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DnLite
{
    public partial class DnLiteAdmin : Form
    {
        private readonly DnLiteDisplay display;
        private TokenControl selectedToken;

        public DnLiteAdmin(DnLiteDisplay display)
        {
            InitializeComponent();
            this.display = display ?? throw new ArgumentNullException(nameof(display));
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // 0x200 is the Win32 class style constant for CS_NOCLOSE
                const int CS_NOCLOSE = 0x200;

                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_NOCLOSE;
                return cp;
            }
        }

        private void DnLiteAdmin_Load(object sender, EventArgs e)
        {
            // Ensure SelectedToken remove button is wired (designer may have set it)
            try
            {
                SelectedTokenRemoveButton.Click += SelectedTokenRemoveButton_Click;
            }
            catch { }
        }

        // Palette panel inside Admin where tokens/decoration previews live

        // Create a small token in the admin palette (visible only to admins)
        public TokenControl CreatePaletteTokenInAdmin(char letter, Color fillColor, int index, int gridW = 1, int gridH = 1, string imagePath = "")
        {
            if (AdminPalettePanel == null) return null;
            TokenControl token = new TokenControl(letter, fillColor, imagePath);
            int paletteCell = Math.Max(28, DnLiteDisplay.CellSize / 3);
            token.Size = new Size(paletteCell, paletteCell);
            token.GridWidth = Math.Max(1, gridW);
            token.GridHeight = Math.Max(1, gridH);
            int margin = 8;
            int x = (AdminPalettePanel.Width - token.Width) / 2;
            int y = margin + index * (token.Height + margin);
            token.Location = new Point(x, y);
            // Wire display drag handlers so dragging from admin palette creates tokens in the grid
            token.MouseDown += (s, e) => { display.Token_MouseDown(s, e); };
            token.MouseMove += (s, e) => { display.Token_MouseMove(s, e); };
            token.MouseUp += (s, e) => { display.Token_MouseUp(s, e); };
            token.Cursor = Cursors.Hand;

            AdminPalettePanel.Controls.Add(token);
            return token;
        }

        public TokenControl CreatePaletteTokenInAdmin(char letter, Color fillColor)
        {
            if (AdminPalettePanel == null) return null;
            int index = AdminPalettePanel.Controls.Count;
            return CreatePaletteTokenInAdmin(letter, fillColor, index, 1, 1);
        }

        public TokenControl CreatePaletteTokenWithDataInAdmin(char letter, Color fillColor, TokenData data, int gridW = 1, int gridH = 1, string imagePath = "")
        {
            if (AdminPalettePanel == null) return null;
            int index = AdminPalettePanel.Controls.Count;
            var token = CreatePaletteTokenInAdmin(letter, fillColor, index, gridW, gridH, imagePath);
            if (token != null) token.Tag = data;
            return token;
        }

        public DecorationControl CreatePaletteDecoWithDataInAdmin(Decoration deco, int gridW = 1, int gridH = 1)
        {
            if (AdminPalettePanel == null) return null;
            int index = AdminPalettePanel.Controls.Count;
            var ctrl = new DecorationControl(deco.ImgFileLocation, gridW, gridH);
            int paletteCell = Math.Max(28, DnLiteDisplay.CellSize / 3);
            ctrl.Size = new Size(paletteCell, paletteCell);
            int margin = 8;
            int x = (AdminPalettePanel.Width - ctrl.Width) / 2;
            int y = margin + index * (ctrl.Height + margin);
            ctrl.Location = new Point(x, y);
            // Wire the display handlers
            ctrl.MouseDown += (s, e) => { display.Token_MouseDown(s, e); };
            ctrl.MouseMove += (s, e) => { display.Token_MouseMove(s, e); };
            ctrl.MouseUp += (s, e) => { display.Token_MouseUp(s, e); };
            ctrl.Cursor = Cursors.Hand;
            ctrl.Tag = deco;

            AdminPalettePanel.Controls.Add(ctrl);
            return ctrl;
        }

        public void SaveNPC(string Name, string Desc, char Token, int MaxHP, int CurHP, int Lvl, int AC, bool IsHostile, bool IsLarge, string ImgFileLocation)
        {
            NPC newNPC = new NPC(Name, Desc, Token, MaxHP, CurHP, Lvl, AC, IsHostile, IsLarge, ImgFileLocation); //creates NPC object with the given parameters
            var jsonString = JsonSerializer.Serialize(newNPC); //serializes the NPC object to a JSON string

            System.IO.File.WriteAllText($"Token Folder/{Name}.npc", jsonString); //writes the JSON string to a file named after the character's name
            MessageBox.Show($"NPC {Name} saved successfully!");
        }

        public void LoadNPC()
        {
            string NPCLocation = "";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "NPC files (*.npc)|*.npc";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    NPCLocation = openFileDialog.FileName;

                    // Do something with the file path
                    MessageBox.Show($"NPC loaded from file: {NPCLocation}");
                }
            }
            if (string.IsNullOrEmpty(NPCLocation))
            {
                MessageBox.Show("No NPC file selected.");
                return;
            }
            var jsonString = System.IO.File.ReadAllText(NPCLocation);
            NPC newNPC = JsonSerializer.Deserialize<NPC>(jsonString);

            CreatureNameText.Text = newNPC.Name;
            CreatureDescRichText.Text = newNPC.Description;
            CreatureTokenLetter.Text = newNPC.Token.ToString();
            CreatureACNumeric.Text = newNPC.AC.ToString();
            CreatureHPNumeric.Text = newNPC.MaxHP.ToString();
            CreatureLvlNumeric.Value = newNPC.Lvl;
            CreatureHostileCheck.Checked = newNPC.IsHostile;
            CreatureSizeCheck.Checked = newNPC.IsLarge;
            CreatureImgFileLocationText.Text = newNPC.ImgFileLocation ?? "";
        }

        public void CopyAndSaveDecoImage()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Select an Image to Copy";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string destinationDirectory = @"Picture Folder/";

                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        string sourceFilePath = openFileDialog.FileName;
                        string fileName = Path.GetFileName(sourceFilePath);

                        string destinationFilePath = Path.Combine(destinationDirectory, fileName);

                        File.Copy(sourceFilePath, destinationFilePath, overwrite: true);

                        MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        DecoImageFileLocation.Text = destinationFilePath; // Update the text box with the new image file location
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void SelectNewTokenImg()
        {
            if (selectedToken == null)
            {
                MessageBox.Show("No token is currently selected.", "Change Token Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Open file dialog to select an image
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Select an Image for the Token";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string destinationDirectory = @"Picture Folder/";

                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        string sourceFilePath = openFileDialog.FileName;
                        string fileName = Path.GetFileName(sourceFilePath);
                        string destinationFilePath = Path.Combine(destinationDirectory, fileName);

                        // Copy the image to the Picture Folder
                        File.Copy(sourceFilePath, destinationFilePath, overwrite: true);

                        // Check if the selected token is a DecorationControl (decoration) or TokenControl (character/NPC)
                        if (selectedToken is DecorationControl decoControl)
                        {
                            // For DecorationControl: Update the ImagePath property
                            // This will reload the image without rendering a circle border
                            decoControl.ImagePath = destinationFilePath;

                            // Update the Tag if it contains a Decoration object
                            if (decoControl.Tag is Decoration deco)
                            {
                                deco.ImgFileLocation = destinationFilePath;
                            }
                        }
                        else
                        {
                            // For TokenControl (Character/NPC): Update ImagePath
                            // This will render the image with a circle border
                            selectedToken.ImagePath = destinationFilePath;
                        }

                        // Refresh the token display
                        selectedToken.Invalidate();

                        // Refresh the preview in the admin panel
                        DisplayTokenInfo(selectedToken);

                        MessageBox.Show("Token image updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating token image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void CopyAndSaveCreatureImage()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Select an Image to Copy";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string destinationDirectory = @"Picture Folder/";

                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        string sourceFilePath = openFileDialog.FileName;
                        string fileName = Path.GetFileName(sourceFilePath);

                        string destinationFilePath = Path.Combine(destinationDirectory, fileName);

                        File.Copy(sourceFilePath, destinationFilePath, overwrite: true);

                        MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CreatureImgFileLocationText.Text = destinationFilePath; // Update the text box with the new image file location
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void SaveDeco(string Name, string Desc, int Height, int Width, string ImgFileLocation)
        {
            Decoration newDeco = new Decoration(Name, Desc, Height, Width, ImgFileLocation); //creates decoration object with the given parameters
            var jsonString = JsonSerializer.Serialize(newDeco); //serializes the decoration object to a JSON string

            System.IO.File.WriteAllText($"Token Folder/{Name}.deco", jsonString); //writes the JSON string to a file named after the character's name
            MessageBox.Show($"Decoration {Name} saved successfully!");
        }

        public void LoadDeco()
        {
            string decoLocation = "";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Deco files (*.deco)|*.deco";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    decoLocation = openFileDialog.FileName;

                    // Do something with the file path
                    MessageBox.Show($"Deco loaded from file: {decoLocation}");
                }
            }
            if (string.IsNullOrEmpty(decoLocation))
            {
                MessageBox.Show("No Deco file selected.");
                return;
            }
            var jsonString = System.IO.File.ReadAllText(decoLocation);
            Decoration newDeco = JsonSerializer.Deserialize<Decoration>(jsonString);

            DecoNameText.Text = newDeco.Name;
            DecoDescRichText.Text = newDeco.Description;
            DecoImageFileLocation.Text = newDeco.ImgFileLocation;
            DecoTallNumeric.Text = newDeco.Height.ToString();
            DecoWideNumeric.Text = newDeco.Width.ToString();
        }


        private void SetGridButton_Click(object sender, EventArgs e) =>

            display.UpdateGridDimensions((int)GridRowNumeric.Value, (int)GridColumNumeric.Value);

        private void InitiativeClearButton_Click(object sender, EventArgs e) //clean up the initiative list

        => display.ClearInitiativeFunc();

        private void SaveCreatureButton_Click(object sender, EventArgs e)
        {
            SaveNPC(CreatureNameText.Text, CreatureDescRichText.Text, CreatureTokenLetter.Text[0], int.Parse(CreatureHPNumeric.Text), int.Parse(CreatureHPNumeric.Text), (int)CreatureLvlNumeric.Value, int.Parse(CreatureACNumeric.Text), CreatureHostileCheck.Checked, CreatureSizeCheck.Checked, CreatureImgFileLocationText.Text);

            // If a token is selected in the admin, apply changes to it
            if (selectedToken != null)
            {
                ApplyToSelectedToken();
            }
        }

        private void LoadCreatureButton_Click(object sender, EventArgs e)
        {
            LoadNPC();
        }

        private void SaveDecoButton_Click(object sender, EventArgs e)
        {
            SaveDeco(DecoNameText.Text, DecoDescRichText.Text, int.Parse(DecoTallNumeric.Text), int.Parse(DecoWideNumeric.Text), DecoImageFileLocation.Text);
        }

        private void LoadDecoButton_Click(object sender, EventArgs e)
        {
            LoadDeco();
        }

        private void LocateImageForDecoButton_Click(object sender, EventArgs e)
        {
            CopyAndSaveDecoImage();
        }

        private void CreateCreatureButton_Click(object sender, EventArgs e)
        {
            if (display == null)
            {
                MessageBox.Show("Display not available.");
                return;
            }

            if (string.IsNullOrWhiteSpace(CreatureTokenLetter.Text))
            {
                MessageBox.Show("Please provide a token letter for the creature.");
                return;
            }

            char tokenChar = CreatureTokenLetter.Text.Trim()[0];

            // Choose a base color based on hostility
            Color baseColor = Color.Gray;
            if (CreatureHostileCheck.Checked) baseColor = Color.Red;
            else baseColor = Color.Blue;

            // Calculate display color - larger creatures get a lighter tint
            Color displayColor = CreatureSizeCheck.Checked ? ControlPaint.Light(baseColor) : baseColor;

            int gridW = CreatureSizeCheck.Checked ? 2 : 1;
            int gridH = CreatureSizeCheck.Checked ? 2 : 1;

            // Create TokenData for this creature so the admin can show HP/name when placed
            // Get level from the numeric control
            int lvl = (int)CreatureLvlNumeric.Value;

            var td = new TokenData(CreatureNameText.Text ?? string.Empty, (int)CreatureHPNumeric.Value, (int)CreatureHPNumeric.Value, lvl, isPlayer: false, isHostile: CreatureHostileCheck.Checked, isLarge: CreatureSizeCheck.Checked, baseColor: baseColor);

            // Add new palette token inside admin's palette
            CreatePaletteTokenWithDataInAdmin(tokenChar, displayColor, td, gridW, gridH, CreatureImgFileLocationText.Text);
        }

        private void CreateDecoButton_Click(object sender, EventArgs e)
        {
            // Create a Decoration object from the admin fields and add a palette entry that uses its image
            string name = DecoNameText.Text ?? string.Empty;
            string desc = DecoDescRichText.Text ?? string.Empty;
            string img = DecoImageFileLocation.Text ?? string.Empty;
            int h = 1, w = 1;
            try { h = Math.Max(1, int.Parse(DecoTallNumeric.Text)); } catch { h = 1; }
            try { w = Math.Max(1, int.Parse(DecoWideNumeric.Text)); } catch { w = 1; }

            var deco = new Decoration(name, desc, h, w, img);
            try
            {
                CreatePaletteDecoWithDataInAdmin(deco, w, h);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create decoration in palette: {ex.Message}");
            }
        }

        // Called by the display when a token is selected/placed so the admin can show/edit token properties
        public void DisplayTokenInfo(TokenControl token)
        {
            if (token == null) return;
            selectedToken = token;
            // Populate admin UI with token info using SelectedToken-prefixed controls
            // Ensure numeric controls have a sensible maximum before assigning values
            SelectedTokenMaxHPNumeric.Maximum = Math.Max(1000, SelectedTokenMaxHPNumeric.Maximum);
            SelectedTokenCurHPNumeric.Maximum = Math.Max(1000, SelectedTokenCurHPNumeric.Maximum);
            SelectedTokenLvlNumeric.Maximum = Math.Max(1000, SelectedTokenLvlNumeric.Maximum);

            // Name and HP from TokenData if available
            if (token.Tag is TokenData td)
            {
                SelectedTokenNameBox.Text = td.Name ?? string.Empty;
                SelectedTokenMaxHPNumeric.Value = td.MaxHP;
                SelectedTokenCurHPNumeric.Value = td.CurHP;
                SelectedTokenLvlNumeric.Value = td.Lvl;

                // Disable hostile toggle for player characters
                SelectedTokenToggleNPCHostile.Enabled = !td.IsPlayer;
            }
            else
            {
                SelectedTokenNameBox.Text = token.Tag as string ?? string.Empty;
                // Defaults
                SelectedTokenMaxHPNumeric.Value = Math.Max(SelectedTokenMaxHPNumeric.Minimum, Math.Min(SelectedTokenMaxHPNumeric.Maximum, 1));
                SelectedTokenCurHPNumeric.Value = Math.Max(SelectedTokenCurHPNumeric.Minimum, Math.Min(SelectedTokenCurHPNumeric.Maximum, 1));
                SelectedTokenLvlNumeric.Value = Math.Max(SelectedTokenLvlNumeric.Minimum, Math.Min(SelectedTokenLvlNumeric.Maximum, 0));

                // Enable both buttons for tokens without TokenData (assumed to be NPCs)
                SelectedTokenToggleNPCHostile.Enabled = true;
            }

            // Render a small preview of the token inside SelectedTokenViewer
            try
            {
                SelectedTokenViewer.Controls.Clear();
                var preview = new TokenControl(token.Letter, token.FillColor, token.ImagePath)
                {
                    Size = new System.Drawing.Size(SelectedTokenViewer.Width - 6, SelectedTokenViewer.Height - 6),
                    Location = new System.Drawing.Point(3, 3),
                    Enabled = false
                };
                SelectedTokenViewer.Controls.Add(preview);
            }
            catch { }

            // Bring admin window to front so user can see and edit
            this.BringToFront();
            this.Focus();
        }

        // Apply changes currently present in the admin UI back to the selected token
        public void ApplyToSelectedToken()
        {
            if (selectedToken == null) return;

            // Update token metadata from SelectedToken controls
            string name = SelectedTokenNameBox.Text ?? string.Empty;
            int maxHP = (int)SelectedTokenMaxHPNumeric.Value;
            int curHP = (int)SelectedTokenCurHPNumeric.Value;
            int lvl = (int)SelectedTokenLvlNumeric.Value;

            TokenData td = selectedToken.Tag as TokenData;
            if (td == null) td = new TokenData(name, maxHP, curHP, lvl);
            else { td.Name = name; td.MaxHP = maxHP; td.CurHP = curHP; td.Lvl = lvl; }

            selectedToken.Tag = td;

            // Refresh preview in admin and the token itself
            selectedToken.Invalidate();
        }

        private void SelectedTokenSetMaxHPButton_Click(object sender, EventArgs e)
        {
            // Ensure max HP value is applied to the selected token and persisted to disk if possible
            ApplyToSelectedToken();
        }

        private void SelectedTokenSetCurHPButton_Click(object sender, EventArgs e)
        {
            // Ensure current HP value is applied to the selected token and persisted to disk if possible
            ApplyToSelectedToken();
        }

        private void SelectedTokenInitiativeButton_Click(object sender, EventArgs e)
        {
            // Roll initiative for the selected token and add to initiative list
            if (selectedToken == null) return;
            var rnd = new Random();
            int roll = rnd.Next(1, 21);
            string baseName = (selectedToken.Tag as TokenData)?.Name ?? SelectedTokenNameBox.Text ?? selectedToken.Letter.ToString();

            // Get a unique name with numbering if needed (e.g., "Goblin -1-", "Goblin -2-")
            string uniqueName = display.GetUniqueInitiativeName(baseName);

            // Store the unique initiative name in the token's data so we can remove it later
            TokenData td = selectedToken.Tag as TokenData;
            if (td != null)
            {
                td.InitiativeName = uniqueName;
            }

            display.AddInitiativeEntry($"{roll} Initiative | {uniqueName}");
            try { display.SortInitiativeList(); } catch { }
        }

        private void SelectedTokenRemoveButton_Click(object sender, EventArgs e)
        {
            if (selectedToken == null)
            {
                MessageBox.Show("No token selected to remove.", "Remove Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show($"Remove token '{(selectedToken.Tag as TokenData)?.Name ?? selectedToken.Letter.ToString()}' from the grid?", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            bool removed = false;
            try
            {
                removed = display.RemoveToken(selectedToken);
            }
            catch { removed = false; }

            if (removed)
            {
                // Clear admin selected token UI
                SelectedTokenNameBox.Text = string.Empty;
                SelectedTokenMaxHPNumeric.Value = SelectedTokenMaxHPNumeric.Minimum;
                SelectedTokenCurHPNumeric.Value = SelectedTokenCurHPNumeric.Minimum;
                SelectedTokenLvlNumeric.Value = SelectedTokenLvlNumeric.Minimum;
                SelectedTokenViewer.Controls.Clear();
                selectedToken = null;
            }
            else
            {
                //MessageBox.Show("Failed to remove token.", "Remove Token", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectedTokenRemoveIntButton_Click(object sender, EventArgs e)
        {
            // Remove the selected token from the initiative list
            if (selectedToken == null)
            {
                MessageBox.Show("No token selected to remove from initiative.", "Remove Initiative", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Try to get the stored unique initiative name from TokenData
            TokenData td = selectedToken.Tag as TokenData;
            string initiativeName = td?.InitiativeName;

            // Fallback to base name if no initiative name is stored
            if (string.IsNullOrEmpty(initiativeName))
            {
                initiativeName = td?.Name ?? SelectedTokenNameBox.Text ?? selectedToken.Letter.ToString();
            }

            if (string.IsNullOrEmpty(initiativeName))
            {
                MessageBox.Show("Could not determine token name.", "Remove Initiative", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Call the display's remove initiative function with the unique name
                display.RemoveSelectedInitiativeFunc(initiativeName);

                // Clear the stored initiative name since it's no longer in the list
                if (td != null)
                {
                    td.InitiativeName = null;
                }

                MessageBox.Show($"Removed '{initiativeName}' from initiative list.", "Initiative Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing from initiative: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectedTokenToggleNPCHostile_Click(object sender, EventArgs e)
        {
            if (selectedToken == null) return;

            TokenData td = selectedToken.Tag as TokenData;
            if (td == null)
            {
                // Create TokenData if it doesn't exist
                td = new TokenData(SelectedTokenNameBox.Text, (int)SelectedTokenMaxHPNumeric.Value, (int)SelectedTokenCurHPNumeric.Value, (int)SelectedTokenLvlNumeric.Value);
                selectedToken.Tag = td;
            }

            // Don't allow toggling hostility for player characters
            if (td.IsPlayer) return;

            // Toggle hostility
            td.IsHostile = !td.IsHostile;

            // Update base color based on hostility
            Color newBaseColor = td.IsHostile ? Color.Red : Color.Blue;
            td.SetBaseColor(newBaseColor);

            // Update display color based on size
            selectedToken.FillColor = td.IsLarge ? ControlPaint.Light(newBaseColor) : newBaseColor;

            // Refresh the token display
            selectedToken.Invalidate();
            DisplayTokenInfo(selectedToken);
        }

        private void SelectedTokenToggleNPCSize_Click(object sender, EventArgs e)
        {
            if (selectedToken == null) return;

            TokenData td = selectedToken.Tag as TokenData;
            if (td == null)
            {
                // Create TokenData if it doesn't exist
                td = new TokenData(SelectedTokenNameBox.Text, (int)SelectedTokenMaxHPNumeric.Value, (int)SelectedTokenCurHPNumeric.Value, (int)SelectedTokenLvlNumeric.Value);
                selectedToken.Tag = td;
            }

            // Toggle size
            td.IsLarge = !td.IsLarge;

            // Update GridWidth and GridHeight
            selectedToken.GridWidth = td.IsLarge ? 2 : 1;
            selectedToken.GridHeight = td.IsLarge ? 2 : 1;

            // Resize the token on the grid
            int cellSize = DnLiteDisplay.CellSize;
            selectedToken.Size = new Size(selectedToken.GridWidth * cellSize, selectedToken.GridHeight * cellSize);

            // Update token color based on base color and size
            Color baseColor = td.GetBaseColor();
            selectedToken.FillColor = td.IsLarge ? ControlPaint.Light(baseColor) : baseColor;

            // Refresh the token display
            selectedToken.Invalidate();
            DisplayTokenInfo(selectedToken);
        }

        private void ClearPaletteButton_Click(object sender, EventArgs e)
        {
            while (AdminPalettePanel.Controls.Count > 0)
            {
                AdminPalettePanel.Controls[0].Dispose();
            }
        }

        private void ClearCreatureData_Click(object sender, EventArgs e)
        {
            CreatureNameText.Text = string.Empty;
            CreatureHPNumeric.Value = CreatureHPNumeric.Minimum;
            CreatureACNumeric.Value = CreatureACNumeric.Minimum;
            CreatureTokenLetter.Text = string.Empty;
            CreatureDescRichText.Text = string.Empty;
            CreatureHostileCheck.Checked = false;
            CreatureSizeCheck.Checked = false;
            CreatureImgFileLocationText.Text = string.Empty;

        }

        private void ClearDecoData_Click(object sender, EventArgs e)
        {
            DecoNameText.Text = string.Empty;
            DecoDescRichText.Text = string.Empty;
            DecoTallNumeric.Value = DecoTallNumeric.Minimum;
            DecoWideNumeric.Value = DecoWideNumeric.Minimum;
            DecoImageFileLocation.Text = string.Empty;
        }

        private void LocateCreatureImgButton_Click(object sender, EventArgs e)
        {
            CopyAndSaveCreatureImage();
        }

        private void ClearCreatureImgFileLocationButton_Click(object sender, EventArgs e)
        {
            CreatureImgFileLocationText.Text = string.Empty;
        }

        private void ClearDecoImgFileLocationButton_Click(object sender, EventArgs e)
        {
            DecoImageFileLocation.Text = string.Empty;
        }

        private void SelectedTokenChangeTokenImgButton_Click(object sender, EventArgs e)
        {
            SelectNewTokenImg();
        }
    }
}
