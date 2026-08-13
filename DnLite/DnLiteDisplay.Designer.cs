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
            this.SuspendLayout();
            // 
            // gridPanel
            // 
            this.gridPanel.Location = new System.Drawing.Point(12, 12);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(883, 600);
            this.gridPanel.TabIndex = 0;
            this.gridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GridPanel_Paint);
            // 
            // InitiativeList
            // 
            this.InitiativeList.FormattingEnabled = true;
            this.InitiativeList.Location = new System.Drawing.Point(901, 12);
            this.InitiativeList.Name = "InitiativeList";
            this.InitiativeList.Size = new System.Drawing.Size(217, 368);
            this.InitiativeList.TabIndex = 0;
            // 
            // GridEmptyButton
            // 
            this.GridEmptyButton.Location = new System.Drawing.Point(901, 386);
            this.GridEmptyButton.Name = "GridEmptyButton";
            this.GridEmptyButton.Size = new System.Drawing.Size(217, 23);
            this.GridEmptyButton.TabIndex = 3;
            this.GridEmptyButton.Text = "Clear the Grid of Tokens";
            this.GridEmptyButton.UseVisualStyleBackColor = true;
            this.GridEmptyButton.Click += new System.EventHandler(this.GridEmptyButton_Click);
            // 
            // DnLiteDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 633);
            this.Controls.Add(this.GridEmptyButton);
            this.Controls.Add(this.InitiativeList);
            this.Controls.Add(this.gridPanel);
            this.Name = "DnLiteDisplay";
            this.Text = "DnLite Viewer Table";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DnLiteDisplay_FormClosed);
            this.Load += new System.EventHandler(this.DnLiteDisplay_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.ListBox InitiativeList;
        private System.Windows.Forms.Button GridEmptyButton;
    }
}

