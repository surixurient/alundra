namespace GraphicsTools
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
            this.picOut = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDATASBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.lblColors = new System.Windows.Forms.Label();
            this.lsvColors = new System.Windows.Forms.ListView();
            this.lblCellColors = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lsvCellColors = new System.Windows.Forms.ListView();
            this.btnProcess = new System.Windows.Forms.Button();
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.hScroll = new System.Windows.Forms.HScrollBar();
            this.chkGrid = new System.Windows.Forms.CheckBox();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.analyzeCarpetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.picOut)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picOut
            // 
            this.picOut.Location = new System.Drawing.Point(239, 27);
            this.picOut.Name = "picOut";
            this.picOut.Size = new System.Drawing.Size(365, 330);
            this.picOut.TabIndex = 0;
            this.picOut.TabStop = false;
            this.picOut.Paint += new System.Windows.Forms.PaintEventHandler(this.picOut_Paint);
            this.picOut.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picOut_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(638, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.analyzeFileToolStripMenuItem,
            this.analyzeCarpetToolStripMenuItem,
            this.openDATASBINToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // analyzeFileToolStripMenuItem
            // 
            this.analyzeFileToolStripMenuItem.Name = "analyzeFileToolStripMenuItem";
            this.analyzeFileToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.analyzeFileToolStripMenuItem.Text = "Analyze File";
            this.analyzeFileToolStripMenuItem.Click += new System.EventHandler(this.analyzeFileToolStripMenuItem_Click);
            // 
            // openDATASBINToolStripMenuItem
            // 
            this.openDATASBINToolStripMenuItem.Name = "openDATASBINToolStripMenuItem";
            this.openDATASBINToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.openDATASBINToolStripMenuItem.Text = "Open DATAS.BIN";
            this.openDATASBINToolStripMenuItem.Click += new System.EventHandler(this.openDATASBINToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "colors:";
            // 
            // lblColors
            // 
            this.lblColors.AutoSize = true;
            this.lblColors.Location = new System.Drawing.Point(48, 72);
            this.lblColors.Name = "lblColors";
            this.lblColors.Size = new System.Drawing.Size(13, 13);
            this.lblColors.TabIndex = 3;
            this.lblColors.Text = "0";
            // 
            // lsvColors
            // 
            this.lsvColors.Location = new System.Drawing.Point(16, 88);
            this.lsvColors.Name = "lsvColors";
            this.lsvColors.Size = new System.Drawing.Size(93, 269);
            this.lsvColors.TabIndex = 5;
            this.lsvColors.UseCompatibleStateImageBehavior = false;
            this.lsvColors.View = System.Windows.Forms.View.Tile;
            // 
            // lblCellColors
            // 
            this.lblCellColors.AutoSize = true;
            this.lblCellColors.Location = new System.Drawing.Point(178, 72);
            this.lblCellColors.Name = "lblCellColors";
            this.lblCellColors.Size = new System.Drawing.Size(13, 13);
            this.lblCellColors.TabIndex = 7;
            this.lblCellColors.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(124, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "cell colors:";
            // 
            // lsvCellColors
            // 
            this.lsvCellColors.Location = new System.Drawing.Point(127, 88);
            this.lsvCellColors.Name = "lsvCellColors";
            this.lsvCellColors.Size = new System.Drawing.Size(93, 269);
            this.lsvCellColors.TabIndex = 8;
            this.lsvCellColors.UseCompatibleStateImageBehavior = false;
            this.lsvCellColors.View = System.Windows.Forms.View.Tile;
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(159, 23);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(61, 21);
            this.btnProcess.TabIndex = 9;
            this.btnProcess.Text = "doit toit";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // vScroll
            // 
            this.vScroll.Location = new System.Drawing.Point(607, 27);
            this.vScroll.Name = "vScroll";
            this.vScroll.Size = new System.Drawing.Size(17, 330);
            this.vScroll.TabIndex = 10;
            this.vScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScroll_Scroll);
            // 
            // hScroll
            // 
            this.hScroll.Location = new System.Drawing.Point(239, 360);
            this.hScroll.Name = "hScroll";
            this.hScroll.Size = new System.Drawing.Size(365, 17);
            this.hScroll.TabIndex = 11;
            this.hScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScroll_Scroll);
            // 
            // chkGrid
            // 
            this.chkGrid.AutoSize = true;
            this.chkGrid.Location = new System.Drawing.Point(16, 27);
            this.chkGrid.Name = "chkGrid";
            this.chkGrid.Size = new System.Drawing.Size(62, 17);
            this.chkGrid.TabIndex = 12;
            this.chkGrid.Text = "cell grid";
            this.chkGrid.UseVisualStyleBackColor = true;
            this.chkGrid.CheckedChanged += new System.EventHandler(this.chkGrid_CheckedChanged);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(72, 50);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(19, 18);
            this.btnZoomIn.TabIndex = 13;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(14, 50);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(19, 18);
            this.btnZoomOut.TabIndex = 14;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "zoom";
            // 
            // analyzeCarpetToolStripMenuItem
            // 
            this.analyzeCarpetToolStripMenuItem.Name = "analyzeCarpetToolStripMenuItem";
            this.analyzeCarpetToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.analyzeCarpetToolStripMenuItem.Text = "Analyze Magic Carpet File";
            this.analyzeCarpetToolStripMenuItem.Click += new System.EventHandler(this.analyzeCarpetToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 381);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.chkGrid);
            this.Controls.Add(this.hScroll);
            this.Controls.Add(this.vScroll);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.lsvCellColors);
            this.Controls.Add(this.lblCellColors);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lsvColors);
            this.Controls.Add(this.lblColors);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picOut);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picOut)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picOut;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblColors;
        private System.Windows.Forms.ListView lsvColors;
        private System.Windows.Forms.Label lblCellColors;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView lsvCellColors;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.VScrollBar vScroll;
        private System.Windows.Forms.HScrollBar hScroll;
        private System.Windows.Forms.CheckBox chkGrid;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem analyzeFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDATASBINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analyzeCarpetToolStripMenuItem;
    }
}

