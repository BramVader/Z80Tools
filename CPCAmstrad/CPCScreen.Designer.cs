namespace CPCAmstrad
{
    partial class CPCScreen
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
            this.SuspendLayout();
            // 
            // CPCScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 331);
            this.Name = "CPCScreen";
            this.Text = "CPCScreen";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CPCScreen_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CPCScreen_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CPCScreen_KeyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CPCScreen_PreviewKeyDown);
            this.ResumeLayout(false);

        }

        #endregion

    }
}