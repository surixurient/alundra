namespace Smb3
{
    partial class frmSmb3
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
            this.pctTilesheet = new System.Windows.Forms.PictureBox();
            this.pctMapPalettes = new System.Windows.Forms.PictureBox();
            this.lstMapPalettes = new System.Windows.Forms.ListBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.txtBank = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pctTilesheet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctMapPalettes)).BeginInit();
            this.SuspendLayout();
            // 
            // pctTilesheet
            // 
            this.pctTilesheet.Location = new System.Drawing.Point(36, 164);
            this.pctTilesheet.Name = "pctTilesheet";
            this.pctTilesheet.Size = new System.Drawing.Size(315, 303);
            this.pctTilesheet.TabIndex = 16;
            this.pctTilesheet.TabStop = false;
            // 
            // pctMapPalettes
            // 
            this.pctMapPalettes.Location = new System.Drawing.Point(113, 12);
            this.pctMapPalettes.Name = "pctMapPalettes";
            this.pctMapPalettes.Size = new System.Drawing.Size(92, 121);
            this.pctMapPalettes.TabIndex = 15;
            this.pctMapPalettes.TabStop = false;
            // 
            // lstMapPalettes
            // 
            this.lstMapPalettes.FormattingEnabled = true;
            this.lstMapPalettes.Location = new System.Drawing.Point(36, 12);
            this.lstMapPalettes.Name = "lstMapPalettes";
            this.lstMapPalettes.Size = new System.Drawing.Size(71, 121);
            this.lstMapPalettes.TabIndex = 14;
            this.lstMapPalettes.SelectedIndexChanged += new System.EventHandler(this.lstMapPalettes_SelectedIndexChanged);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(326, 135);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(25, 23);
            this.btnNext.TabIndex = 17;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(269, 136);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(25, 23);
            this.btnPrev.TabIndex = 18;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // txtBank
            // 
            this.txtBank.Location = new System.Drawing.Point(300, 136);
            this.txtBank.Name = "txtBank";
            this.txtBank.Size = new System.Drawing.Size(20, 20);
            this.txtBank.TabIndex = 19;
            this.txtBank.Text = "0";
            this.txtBank.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBank_KeyDown);
            // 
            // frmSmb3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 507);
            this.Controls.Add(this.txtBank);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.pctTilesheet);
            this.Controls.Add(this.pctMapPalettes);
            this.Controls.Add(this.lstMapPalettes);
            this.Name = "frmSmb3";
            this.Text = "frmSmb3";
            this.Load += new System.EventHandler(this.frmSmb3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pctTilesheet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctMapPalettes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pctTilesheet;
        private System.Windows.Forms.PictureBox pctMapPalettes;
        private System.Windows.Forms.ListBox lstMapPalettes;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.TextBox txtBank;
    }
}