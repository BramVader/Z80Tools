namespace CPCAmstrad
{
    partial class Scope
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
            this.scopeBox = new System.Windows.Forms.PictureBox();
            this.scopeOffsetY = new System.Windows.Forms.VScrollBar();
            this.scopeOffsetX = new System.Windows.Forms.HScrollBar();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.scopeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // scopeBox
            // 
            this.scopeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scopeBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scopeBox.Location = new System.Drawing.Point(12, 37);
            this.scopeBox.Name = "scopeBox";
            this.scopeBox.Size = new System.Drawing.Size(597, 401);
            this.scopeBox.TabIndex = 0;
            this.scopeBox.TabStop = false;
            this.scopeBox.Paint += new System.Windows.Forms.PaintEventHandler(this.scopeBox_Paint);
            // 
            // scopeOffsetY
            // 
            this.scopeOffsetY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scopeOffsetY.Location = new System.Drawing.Point(612, 37);
            this.scopeOffsetY.Name = "scopeOffsetY";
            this.scopeOffsetY.Size = new System.Drawing.Size(17, 401);
            this.scopeOffsetY.TabIndex = 1;
            // 
            // scopeOffsetX
            // 
            this.scopeOffsetX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scopeOffsetX.Location = new System.Drawing.Point(12, 441);
            this.scopeOffsetX.Name = "scopeOffsetX";
            this.scopeOffsetX.Size = new System.Drawing.Size(597, 17);
            this.scopeOffsetX.TabIndex = 2;
            // 
            // buttonRecord
            // 
            this.buttonRecord.Location = new System.Drawing.Point(12, 8);
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.Size = new System.Drawing.Size(62, 23);
            this.buttonRecord.TabIndex = 3;
            this.buttonRecord.Text = "Record";
            this.buttonRecord.UseVisualStyleBackColor = true;
            this.buttonRecord.Click += new System.EventHandler(this.buttonRecord_Click);
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.Location = new System.Drawing.Point(149, 8);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(62, 23);
            this.buttonZoomIn.TabIndex = 3;
            this.buttonZoomIn.Text = "Zoom in";
            this.buttonZoomIn.UseVisualStyleBackColor = true;
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.Location = new System.Drawing.Point(217, 8);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(62, 23);
            this.buttonZoomOut.TabIndex = 3;
            this.buttonZoomOut.Text = "Zoom Out";
            this.buttonZoomOut.UseVisualStyleBackColor = true;
            // 
            // buttonPause
            // 
            this.buttonPause.Enabled = false;
            this.buttonPause.Location = new System.Drawing.Point(80, 8);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(62, 23);
            this.buttonPause.TabIndex = 3;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // Scope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 467);
            this.Controls.Add(this.buttonZoomOut);
            this.Controls.Add(this.buttonZoomIn);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonRecord);
            this.Controls.Add(this.scopeOffsetX);
            this.Controls.Add(this.scopeOffsetY);
            this.Controls.Add(this.scopeBox);
            this.Name = "Scope";
            this.Text = "Scope";
            ((System.ComponentModel.ISupportInitialize)(this.scopeBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox scopeBox;
        private System.Windows.Forms.VScrollBar scopeOffsetY;
        private System.Windows.Forms.HScrollBar scopeOffsetX;
        private System.Windows.Forms.Button buttonRecord;
        private System.Windows.Forms.Button buttonZoomIn;
        private System.Windows.Forms.Button buttonZoomOut;
        private System.Windows.Forms.Button buttonPause;
    }
}