namespace Z80Sim
{
    partial class ReferencesForm
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
            ReferencesListbox = new System.Windows.Forms.ListBox();
            AddressLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // ReferencesListbox
            // 
            ReferencesListbox.FormattingEnabled = true;
            ReferencesListbox.ItemHeight = 15;
            ReferencesListbox.Location = new System.Drawing.Point(12, 29);
            ReferencesListbox.Name = "ReferencesListbox";
            ReferencesListbox.Size = new System.Drawing.Size(262, 214);
            ReferencesListbox.TabIndex = 0;
            // 
            // AddressLabel
            // 
            AddressLabel.AutoSize = true;
            AddressLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            AddressLabel.Location = new System.Drawing.Point(12, 9);
            AddressLabel.Name = "AddressLabel";
            AddressLabel.Size = new System.Drawing.Size(40, 17);
            AddressLabel.TabIndex = 1;
            AddressLabel.Text = "label1";
            // 
            // ReferencesForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(280, 252);
            Controls.Add(AddressLabel);
            Controls.Add(ReferencesListbox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Name = "ReferencesForm";
            Text = "References";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox ReferencesListbox;
        private System.Windows.Forms.Label AddressLabel;
    }
}