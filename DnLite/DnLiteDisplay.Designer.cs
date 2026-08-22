namespace DnLite
{
    partial class DnLiteDisplay
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
            this.gridPanel = new System.Windows.Forms.Panel();
            this.InitiativeList = new System.Windows.Forms.ListBox();
            this.GridEmptyButton = new System.Windows.Forms.Button();
            this.DiceDisplayPanel = new System.Windows.Forms.Panel();
            this.DiceRollOutputLabel = new System.Windows.Forms.Label();
            this.RollingLabel = new System.Windows.Forms.Label();
            this.RoleDieButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gridPanel
            // 
            this.gridPanel.Location = new System.Drawing.Point(12, 12);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(932, 916);
            this.gridPanel.TabIndex = 0;
            this.gridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GridPanel_Paint);
            // 
            // InitiativeList
            // 
            this.InitiativeList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.InitiativeList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InitiativeList.FormattingEnabled = true;
            this.InitiativeList.ItemHeight = 20;
            this.InitiativeList.Location = new System.Drawing.Point(956, 12);
            this.InitiativeList.Name = "InitiativeList";
            this.InitiativeList.Size = new System.Drawing.Size(217, 364);
            this.InitiativeList.TabIndex = 0;
            // 
            // GridEmptyButton
            // 
            this.GridEmptyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GridEmptyButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridEmptyButton.Location = new System.Drawing.Point(956, 386);
            this.GridEmptyButton.Name = "GridEmptyButton";
            this.GridEmptyButton.Size = new System.Drawing.Size(217, 44);
            this.GridEmptyButton.TabIndex = 3;
            this.GridEmptyButton.Text = "Clear the Grid of Tokens";
            this.GridEmptyButton.UseVisualStyleBackColor = true;
            this.GridEmptyButton.Click += new System.EventHandler(this.GridEmptyButton_Click);
            // 
            // DiceDisplayPanel
            // 
            this.DiceDisplayPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DiceDisplayPanel.Location = new System.Drawing.Point(956, 436);
            this.DiceDisplayPanel.Name = "DiceDisplayPanel";
            this.DiceDisplayPanel.Size = new System.Drawing.Size(217, 211);
            this.DiceDisplayPanel.TabIndex = 4;
            // 
            // DiceRollOutputLabel
            // 
            this.DiceRollOutputLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DiceRollOutputLabel.AutoSize = true;
            this.DiceRollOutputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DiceRollOutputLabel.Location = new System.Drawing.Point(1053, 659);
            this.DiceRollOutputLabel.Name = "DiceRollOutputLabel";
            this.DiceRollOutputLabel.Size = new System.Drawing.Size(32, 33);
            this.DiceRollOutputLabel.TabIndex = 7;
            this.DiceRollOutputLabel.Text = "0";
            // 
            // RollingLabel
            // 
            this.RollingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RollingLabel.AutoSize = true;
            this.RollingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RollingLabel.Location = new System.Drawing.Point(953, 666);
            this.RollingLabel.Name = "RollingLabel";
            this.RollingLabel.Size = new System.Drawing.Size(104, 20);
            this.RollingLabel.TabIndex = 6;
            this.RollingLabel.Text = "You Rolled a:";
            // 
            // RoleDieButton
            // 
            this.RoleDieButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RoleDieButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RoleDieButton.Location = new System.Drawing.Point(956, 696);
            this.RoleDieButton.Name = "RoleDieButton";
            this.RoleDieButton.Size = new System.Drawing.Size(217, 40);
            this.RoleDieButton.TabIndex = 5;
            this.RoleDieButton.Text = "Roll the Die";
            this.RoleDieButton.UseVisualStyleBackColor = true;
            this.RoleDieButton.Click += new System.EventHandler(this.RoleDieButton_Click);
            // 
            // DnLiteDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1185, 941);
            this.Controls.Add(this.DiceRollOutputLabel);
            this.Controls.Add(this.RollingLabel);
            this.Controls.Add(this.RoleDieButton);
            this.Controls.Add(this.DiceDisplayPanel);
            this.Controls.Add(this.GridEmptyButton);
            this.Controls.Add(this.InitiativeList);
            this.Controls.Add(this.gridPanel);
            this.Name = "DnLiteDisplay";
            this.Text = "DnLite Viewer Table";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DnLiteDisplay_FormClosed);
            this.Load += new System.EventHandler(this.DnLiteDisplay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.ListBox InitiativeList;
        private System.Windows.Forms.Button GridEmptyButton;
        private System.Windows.Forms.Panel DiceDisplayPanel;
        private System.Windows.Forms.Label DiceRollOutputLabel;
        private System.Windows.Forms.Label RollingLabel;
        private System.Windows.Forms.Button RoleDieButton;
    }
}

