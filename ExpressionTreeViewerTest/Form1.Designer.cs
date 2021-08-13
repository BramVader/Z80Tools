namespace ExpressionTreeViewerTest
{
    partial class Form1
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
            this.myRichTextBox1 = new ExpressionTreeViewerTest.MyRichTextBox();
            this.SuspendLayout();
            // 
            // myRichTextBox1
            // 
            this.myRichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.myRichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myRichTextBox1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myRichTextBox1.Location = new System.Drawing.Point(0, 0);
            this.myRichTextBox1.Name = "myRichTextBox1";
            this.myRichTextBox1.Size = new System.Drawing.Size(496, 610);
            this.myRichTextBox1.TabIndex = 1;
            this.myRichTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 610);
            this.Controls.Add(this.myRichTextBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private MyRichTextBox myRichTextBox1;

    }
}

