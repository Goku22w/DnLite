namespace DnLite
{
    partial class DnLitePC
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
            this.CharacterNameText = new System.Windows.Forms.TextBox();
            this.CharacterClassCombo = new System.Windows.Forms.ComboBox();
            this.NameTextLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CharacterDescRichText = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CreateCharacterButton = new System.Windows.Forms.Button();
            this.ClearCharacterButton = new System.Windows.Forms.Button();
            this.CharacterTokenLetter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SaveCharacterButton = new System.Windows.Forms.Button();
            this.LoadCharacterButton = new System.Windows.Forms.Button();
            this.CharacterImgFileLocationText = new System.Windows.Forms.TextBox();
            this.CharacterFindImgButton = new System.Windows.Forms.Button();
            this.CharacterClearImgFileLocation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CharacterNameText
            // 
            this.CharacterNameText.Location = new System.Drawing.Point(15, 25);
            this.CharacterNameText.Name = "CharacterNameText";
            this.CharacterNameText.Size = new System.Drawing.Size(212, 20);
            this.CharacterNameText.TabIndex = 0;
            // 
            // CharacterClassCombo
            // 
            this.CharacterClassCombo.FormattingEnabled = true;
            this.CharacterClassCombo.Items.AddRange(new object[] {
            "Melee Class",
            "Ranger Class",
            "Caster Class"});
            this.CharacterClassCombo.Location = new System.Drawing.Point(15, 64);
            this.CharacterClassCombo.Name = "CharacterClassCombo";
            this.CharacterClassCombo.Size = new System.Drawing.Size(121, 21);
            this.CharacterClassCombo.TabIndex = 1;
            this.CharacterClassCombo.Text = "--Pick a Class--";
            // 
            // NameTextLabel
            // 
            this.NameTextLabel.AutoSize = true;
            this.NameTextLabel.Location = new System.Drawing.Point(12, 9);
            this.NameTextLabel.Name = "NameTextLabel";
            this.NameTextLabel.Size = new System.Drawing.Size(107, 13);
            this.NameTextLabel.TabIndex = 2;
            this.NameTextLabel.Text = "Name your Character";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select your Class";
            // 
            // CharacterDescRichText
            // 
            this.CharacterDescRichText.Location = new System.Drawing.Point(15, 163);
            this.CharacterDescRichText.Name = "CharacterDescRichText";
            this.CharacterDescRichText.Size = new System.Drawing.Size(212, 96);
            this.CharacterDescRichText.TabIndex = 4;
            this.CharacterDescRichText.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Describe your Character";
            // 
            // CreateCharacterButton
            // 
            this.CreateCharacterButton.Location = new System.Drawing.Point(15, 268);
            this.CreateCharacterButton.Name = "CreateCharacterButton";
            this.CreateCharacterButton.Size = new System.Drawing.Size(104, 23);
            this.CreateCharacterButton.TabIndex = 6;
            this.CreateCharacterButton.Text = "Create Character";
            this.CreateCharacterButton.UseVisualStyleBackColor = true;
            this.CreateCharacterButton.Click += new System.EventHandler(this.CreateCharacterButton_Click);
            // 
            // ClearCharacterButton
            // 
            this.ClearCharacterButton.Location = new System.Drawing.Point(152, 268);
            this.ClearCharacterButton.Name = "ClearCharacterButton";
            this.ClearCharacterButton.Size = new System.Drawing.Size(75, 23);
            this.ClearCharacterButton.TabIndex = 7;
            this.ClearCharacterButton.Text = "Clear Data";
            this.ClearCharacterButton.UseVisualStyleBackColor = true;
            this.ClearCharacterButton.Click += new System.EventHandler(this.ClearCharacterButton_Click);
            // 
            // CharacterTokenLetter
            // 
            this.CharacterTokenLetter.Location = new System.Drawing.Point(152, 64);
            this.CharacterTokenLetter.Name = "CharacterTokenLetter";
            this.CharacterTokenLetter.Size = new System.Drawing.Size(35, 20);
            this.CharacterTokenLetter.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(148, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Token Initial";
            // 
            // SaveCharacterButton
            // 
            this.SaveCharacterButton.Location = new System.Drawing.Point(15, 297);
            this.SaveCharacterButton.Name = "SaveCharacterButton";
            this.SaveCharacterButton.Size = new System.Drawing.Size(102, 23);
            this.SaveCharacterButton.TabIndex = 10;
            this.SaveCharacterButton.Text = "Save Character";
            this.SaveCharacterButton.UseVisualStyleBackColor = true;
            this.SaveCharacterButton.Click += new System.EventHandler(this.SaveCharacterButton_Click);
            // 
            // LoadCharacterButton
            // 
            this.LoadCharacterButton.Location = new System.Drawing.Point(125, 297);
            this.LoadCharacterButton.Name = "LoadCharacterButton";
            this.LoadCharacterButton.Size = new System.Drawing.Size(102, 23);
            this.LoadCharacterButton.TabIndex = 11;
            this.LoadCharacterButton.Text = "Load Character";
            this.LoadCharacterButton.UseVisualStyleBackColor = true;
            this.LoadCharacterButton.Click += new System.EventHandler(this.LoadCharacterButton_Click);
            // 
            // CharacterImgFileLocationText
            // 
            this.CharacterImgFileLocationText.Enabled = false;
            this.CharacterImgFileLocationText.Location = new System.Drawing.Point(15, 91);
            this.CharacterImgFileLocationText.Name = "CharacterImgFileLocationText";
            this.CharacterImgFileLocationText.Size = new System.Drawing.Size(212, 20);
            this.CharacterImgFileLocationText.TabIndex = 12;
            // 
            // CharacterFindImgButton
            // 
            this.CharacterFindImgButton.Location = new System.Drawing.Point(15, 117);
            this.CharacterFindImgButton.Name = "CharacterFindImgButton";
            this.CharacterFindImgButton.Size = new System.Drawing.Size(104, 23);
            this.CharacterFindImgButton.TabIndex = 13;
            this.CharacterFindImgButton.Text = "Locate Image";
            this.CharacterFindImgButton.UseVisualStyleBackColor = true;
            this.CharacterFindImgButton.Click += new System.EventHandler(this.CharacterFindImgButton_Click);
            // 
            // CharacterClearImgFileLocation
            // 
            this.CharacterClearImgFileLocation.Location = new System.Drawing.Point(145, 117);
            this.CharacterClearImgFileLocation.Name = "CharacterClearImgFileLocation";
            this.CharacterClearImgFileLocation.Size = new System.Drawing.Size(81, 23);
            this.CharacterClearImgFileLocation.TabIndex = 14;
            this.CharacterClearImgFileLocation.Text = "Clear Search";
            this.CharacterClearImgFileLocation.UseVisualStyleBackColor = true;
            this.CharacterClearImgFileLocation.Click += new System.EventHandler(this.CharacterClearImgFileLocation_Click);
            // 
            // DnLitePC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 332);
            this.Controls.Add(this.CharacterClearImgFileLocation);
            this.Controls.Add(this.CharacterFindImgButton);
            this.Controls.Add(this.CharacterImgFileLocationText);
            this.Controls.Add(this.LoadCharacterButton);
            this.Controls.Add(this.SaveCharacterButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CharacterTokenLetter);
            this.Controls.Add(this.ClearCharacterButton);
            this.Controls.Add(this.CreateCharacterButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CharacterDescRichText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NameTextLabel);
            this.Controls.Add(this.CharacterClassCombo);
            this.Controls.Add(this.CharacterNameText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DnLitePC";
            this.Text = "DnLite Player Creator";
            this.Load += new System.EventHandler(this.DnLitePC_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CharacterNameText;
        private System.Windows.Forms.ComboBox CharacterClassCombo;
        private System.Windows.Forms.Label NameTextLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox CharacterDescRichText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button CreateCharacterButton;
        private System.Windows.Forms.Button ClearCharacterButton;
        private System.Windows.Forms.TextBox CharacterTokenLetter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button SaveCharacterButton;
        private System.Windows.Forms.Button LoadCharacterButton;
        private System.Windows.Forms.TextBox CharacterImgFileLocationText;
        private System.Windows.Forms.Button CharacterFindImgButton;
        private System.Windows.Forms.Button CharacterClearImgFileLocation;
    }
}