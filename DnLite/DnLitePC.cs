using CharacterClass;
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
    public partial class DnLitePC : Form
    {
        private DnLiteDisplay parentDisplay;
        private DnLiteAdmin parentAdmin;

        public DnLitePC(DnLiteDisplay display)
        {
            InitializeComponent();
            parentDisplay = display;
        }

        public DnLitePC()
        {
            InitializeComponent();
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

        private void DnLitePC_Load(object sender, EventArgs e)
        {

        }

        public void SaveCharacter(string Name, string Class, string Desc, char Token, int MaxHP, int CurHP, int Lvl, string ImgFileLocation)
        {
            Character newCharacter = new Character(Name, Class, Desc, Token, MaxHP, CurHP, Lvl, ImgFileLocation); //creates character object with the given parameters
            var jsonString = JsonSerializer.Serialize(newCharacter); //serializes the character object to a JSON string

            System.IO.File.WriteAllText($"Token Folder/{Name}.char", jsonString); //writes the JSON string to a file named after the character's name
            MessageBox.Show($"Character {Name} saved successfully!");
        }

        public void LoadCharacter()
        {
            string characterLocation = "";
            
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Character files (*.char)|*.char";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    characterLocation = openFileDialog.FileName;

                    // Do something with the file path
                    MessageBox.Show($"Character loaded from file: {characterLocation}");
                }
            }
            if(string.IsNullOrEmpty(characterLocation))
            {
                MessageBox.Show("No character file selected.");
                return;
            }
            var jsonString = System.IO.File.ReadAllText(characterLocation);
            Character newCharacter = JsonSerializer.Deserialize<Character>(jsonString);

            CharacterNameText.Text = newCharacter.Name;
            CharacterClassCombo.Text = newCharacter.Class;
            CharacterDescRichText.Text = newCharacter.Description;
            CharacterTokenLetter.Text = newCharacter.Token.ToString();
            CharacterImgFileLocationText.Text = newCharacter.ImgFileLocation ?? "";

            // Also create a palette token with the loaded character's data so it can be placed on the grid
            try
            {
                if (parentDisplay != null)
                {
                    char tokenChar = newCharacter.Token;
                    // Map class selection to a default color
                    Color tokenColor = Color.Gray;
                    string cls = (newCharacter.Class ?? "").ToLowerInvariant();
                    if (cls.Contains("melee")) tokenColor = Color.DarkGreen;
                    else if (cls.Contains("ranger")) tokenColor = Color.Green;
                    else if (cls.Contains("caster")) tokenColor = Color.LightGreen;

                    var td = new TokenData(newCharacter.Name ?? string.Empty, newCharacter.MaxHP, newCharacter.CurHP, newCharacter.Lvl);
                    parentDisplay.AddPaletteTokenToAdmin(tokenChar, tokenColor, td, 1, 1, newCharacter.ImgFileLocation ?? "");
                }
            }
            catch { }
        }

        public void CopyAndSaveImage()
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

                        CharacterImgFileLocationText.Text = destinationFilePath; // Update the text box with the new image file location
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveCharacterButton_Click(object sender, EventArgs e)
        {
            SaveCharacter(CharacterNameText.Text, CharacterClassCombo.Text, CharacterDescRichText.Text, CharacterTokenLetter.Text[0], 3, 3, 0, CharacterImgFileLocationText.Text);
        }

        private void LoadCharacterButton_Click(object sender, EventArgs e)
        {
            LoadCharacter();
        }

        private void CreateCharacterButton_Click(object sender, EventArgs e)
        {
            if (parentDisplay == null)
            {
                MessageBox.Show("Display not available.");
                return;
            }

            if (string.IsNullOrWhiteSpace(CharacterTokenLetter.Text))
            {
                MessageBox.Show("Please provide a token letter.");
                return;
            }

            char tokenChar = CharacterTokenLetter.Text.Trim()[0];

            // Map class selection to a default color
            Color tokenColor = Color.Gray;
            string cls = (CharacterClassCombo.Text ?? "").ToLowerInvariant();
            if (cls.Contains("melee")) tokenColor = Color.Green;
            else if (cls.Contains("ranger")) tokenColor = Color.DarkGreen;
            else if (cls.Contains("caster")) tokenColor = Color.LightGreen;

            // Add this token to the admin palette via the display
            var td = new TokenData(CharacterNameText.Text ?? string.Empty, 3, 3, 0);
            parentDisplay.AddPaletteTokenToAdmin(tokenChar, tokenColor, td, 1, 1, CharacterImgFileLocationText.Text);
        }

        private void ClearCharacterButton_Click(object sender, EventArgs e)
        {
            CharacterNameText.Text = string.Empty;
            CharacterClassCombo.Text = "--Pick a Class--";
            CharacterDescRichText.Text = string.Empty;
            CharacterTokenLetter.Text = string.Empty;
            CharacterImgFileLocationText.Text = string.Empty;
        }

        private void CharacterFindImgButton_Click(object sender, EventArgs e)
        {
           CopyAndSaveImage();
        }

        private void CharacterClearImgFileLocation_Click(object sender, EventArgs e)
        {
            CharacterImgFileLocationText.Text = string.Empty;
        }
    }
}
