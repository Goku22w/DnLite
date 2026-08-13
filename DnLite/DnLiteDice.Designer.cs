namespace DnLite
{
    partial class DnLiteDice
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
            this.RoleDieButton = new System.Windows.Forms.Button();
            this.RollResultLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DiceRollOutputLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RoleDieButton
            // 
            this.RoleDieButton.Location = new System.Drawing.Point(15, 155);
            this.RoleDieButton.Name = "RoleDieButton";
            this.RoleDieButton.Size = new System.Drawing.Size(75, 23);
            this.RoleDieButton.TabIndex = 0;
            this.RoleDieButton.Text = "Roll the Die";
            this.RoleDieButton.UseVisualStyleBackColor = true;
            this.RoleDieButton.Click += new System.EventHandler(this.RoleDieButton_Click);
            // 
            // RollResultLabel
            // 
            this.RollResultLabel.Location = new System.Drawing.Point(12, 12);
            this.RollResultLabel.Name = "RollResultLabel";
            this.RollResultLabel.Size = new System.Drawing.Size(195, 23);
            this.RollResultLabel.TabIndex = 1;
            this.RollResultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(96, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "You Rolled a:";
            // 
            // DiceRollOutputLabel
            // 
            this.DiceRollOutputLabel.AutoSize = true;
            this.DiceRollOutputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DiceRollOutputLabel.Location = new System.Drawing.Point(164, 155);
            this.DiceRollOutputLabel.Name = "DiceRollOutputLabel";
            this.DiceRollOutputLabel.Size = new System.Drawing.Size(21, 24);
            this.DiceRollOutputLabel.TabIndex = 3;
            this.DiceRollOutputLabel.Text = "0";
            // 
            // DnLiteDice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 190);
            this.Controls.Add(this.DiceRollOutputLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RoleDieButton);
            this.Controls.Add(this.RollResultLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DnLiteDice";
            this.Text = "Dice Roll";
            this.Load += new System.EventHandler(this.DnLiteDice_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RoleDieButton;
        private System.Windows.Forms.Label RollResultLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label DiceRollOutputLabel;
    }
}