﻿namespace GraphicsTools
{
    partial class frmFileAnalyzer
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
            this.rtfText = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbl8bit = new System.Windows.Forms.Label();
            this.lbl16bit = new System.Windows.Forms.Label();
            this.lbl32bit = new System.Windows.Forms.Label();
            this.lbl4bita = new System.Windows.Forms.Label();
            this.lblSelLength = new System.Windows.Forms.Label();
            this.lblRelOffset = new System.Windows.Forms.Label();
            this.lblCursorOffset = new System.Windows.Forms.Label();
            this.lbl4bitb = new System.Windows.Forms.Label();
            this.btnViewImage = new System.Windows.Forms.Button();
            this.txtStride = new System.Windows.Forms.TextBox();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.btnJump = new System.Windows.Forms.Button();
            this.btnViewPal = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtStartx = new System.Windows.Forms.TextBox();
            this.txtStarty = new System.Windows.Forms.TextBox();
            this.lstInstructions = new System.Windows.Forms.ListBox();
            this.txtInstructionsOffset = new System.Windows.Forms.TextBox();
            this.txtAddressOffset = new System.Windows.Forms.TextBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnFindInt32 = new System.Windows.Forms.Button();
            this.txtSearchRange = new System.Windows.Forms.TextBox();
            this.frmFindInt16 = new System.Windows.Forms.Button();
            this.lstFunctions = new System.Windows.Forms.ListBox();
            this.txtFunction = new System.Windows.Forms.TextBox();
            this.chk24bpp = new System.Windows.Forms.CheckBox();
            this.btnJumpFunctionList = new System.Windows.Forms.Button();
            this.btnAlundraEventFuncs = new System.Windows.Forms.Button();
            this.btnFunctionTracer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtfText
            // 
            this.rtfText.Location = new System.Drawing.Point(16, 15);
            this.rtfText.Margin = new System.Windows.Forms.Padding(4);
            this.rtfText.Name = "rtfText";
            this.rtfText.Size = new System.Drawing.Size(615, 427);
            this.rtfText.TabIndex = 0;
            this.rtfText.Text = "";
            this.rtfText.SelectionChanged += new System.EventHandler(this.rtfText_SelectionChanged);
            this.rtfText.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.rtfText_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(649, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "cursor offset";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(649, 53);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "relative offset";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(649, 69);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "sel length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(649, 18);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "chunk offset";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(649, 97);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "8 bit";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(649, 113);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 17);
            this.label6.TabIndex = 6;
            this.label6.Text = "16 bit";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(649, 129);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 17);
            this.label7.TabIndex = 7;
            this.label7.Text = "32 bit";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(649, 145);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 17);
            this.label8.TabIndex = 8;
            this.label8.Text = "4 bit";
            // 
            // lbl8bit
            // 
            this.lbl8bit.AutoSize = true;
            this.lbl8bit.Location = new System.Drawing.Point(751, 97);
            this.lbl8bit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl8bit.Name = "lbl8bit";
            this.lbl8bit.Size = new System.Drawing.Size(16, 17);
            this.lbl8bit.TabIndex = 9;
            this.lbl8bit.Text = "0";
            // 
            // lbl16bit
            // 
            this.lbl16bit.AutoSize = true;
            this.lbl16bit.Location = new System.Drawing.Point(751, 113);
            this.lbl16bit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl16bit.Name = "lbl16bit";
            this.lbl16bit.Size = new System.Drawing.Size(16, 17);
            this.lbl16bit.TabIndex = 10;
            this.lbl16bit.Text = "0";
            // 
            // lbl32bit
            // 
            this.lbl32bit.AutoSize = true;
            this.lbl32bit.Location = new System.Drawing.Point(751, 129);
            this.lbl32bit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl32bit.Name = "lbl32bit";
            this.lbl32bit.Size = new System.Drawing.Size(16, 17);
            this.lbl32bit.TabIndex = 11;
            this.lbl32bit.Text = "0";
            // 
            // lbl4bita
            // 
            this.lbl4bita.AutoSize = true;
            this.lbl4bita.Location = new System.Drawing.Point(751, 145);
            this.lbl4bita.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl4bita.Name = "lbl4bita";
            this.lbl4bita.Size = new System.Drawing.Size(16, 17);
            this.lbl4bita.TabIndex = 12;
            this.lbl4bita.Text = "0";
            // 
            // lblSelLength
            // 
            this.lblSelLength.AutoSize = true;
            this.lblSelLength.Location = new System.Drawing.Point(751, 69);
            this.lblSelLength.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSelLength.Name = "lblSelLength";
            this.lblSelLength.Size = new System.Drawing.Size(16, 17);
            this.lblSelLength.TabIndex = 13;
            this.lblSelLength.Text = "0";
            // 
            // lblRelOffset
            // 
            this.lblRelOffset.AutoSize = true;
            this.lblRelOffset.Location = new System.Drawing.Point(751, 53);
            this.lblRelOffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRelOffset.Name = "lblRelOffset";
            this.lblRelOffset.Size = new System.Drawing.Size(16, 17);
            this.lblRelOffset.TabIndex = 14;
            this.lblRelOffset.Text = "0";
            // 
            // lblCursorOffset
            // 
            this.lblCursorOffset.AutoSize = true;
            this.lblCursorOffset.Location = new System.Drawing.Point(751, 37);
            this.lblCursorOffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCursorOffset.Name = "lblCursorOffset";
            this.lblCursorOffset.Size = new System.Drawing.Size(16, 17);
            this.lblCursorOffset.TabIndex = 15;
            this.lblCursorOffset.Text = "0";
            // 
            // lbl4bitb
            // 
            this.lbl4bitb.AutoSize = true;
            this.lbl4bitb.Location = new System.Drawing.Point(795, 145);
            this.lbl4bitb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl4bitb.Name = "lbl4bitb";
            this.lbl4bitb.Size = new System.Drawing.Size(16, 17);
            this.lbl4bitb.TabIndex = 17;
            this.lbl4bitb.Text = "0";
            // 
            // btnViewImage
            // 
            this.btnViewImage.Location = new System.Drawing.Point(672, 187);
            this.btnViewImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnViewImage.Name = "btnViewImage";
            this.btnViewImage.Size = new System.Drawing.Size(107, 31);
            this.btnViewImage.TabIndex = 18;
            this.btnViewImage.Text = "view image";
            this.btnViewImage.UseVisualStyleBackColor = true;
            this.btnViewImage.Click += new System.EventHandler(this.btnViewImage_Click);
            // 
            // txtStride
            // 
            this.txtStride.Location = new System.Drawing.Point(721, 225);
            this.txtStride.Margin = new System.Windows.Forms.Padding(4);
            this.txtStride.Name = "txtStride";
            this.txtStride.Size = new System.Drawing.Size(56, 22);
            this.txtStride.TabIndex = 19;
            this.txtStride.Text = "2048";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(721, 257);
            this.txtWidth.Margin = new System.Windows.Forms.Padding(4);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(56, 22);
            this.txtWidth.TabIndex = 20;
            this.txtWidth.Text = "4096";
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(721, 289);
            this.txtHeight.Margin = new System.Windows.Forms.Padding(4);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(56, 22);
            this.txtHeight.TabIndex = 21;
            this.txtHeight.Text = "512";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(668, 225);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 17);
            this.label9.TabIndex = 23;
            this.label9.Text = "stride";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(668, 257);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 17);
            this.label10.TabIndex = 24;
            this.label10.Text = "width";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(668, 289);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 17);
            this.label11.TabIndex = 25;
            this.label11.Text = "height";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(745, 10);
            this.txtOffset.Margin = new System.Windows.Forms.Padding(4);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(56, 22);
            this.txtOffset.TabIndex = 27;
            this.txtOffset.Text = "0";
            // 
            // btnJump
            // 
            this.btnJump.Location = new System.Drawing.Point(811, 6);
            this.btnJump.Margin = new System.Windows.Forms.Padding(4);
            this.btnJump.Name = "btnJump";
            this.btnJump.Size = new System.Drawing.Size(50, 31);
            this.btnJump.TabIndex = 28;
            this.btnJump.Text = "jump";
            this.btnJump.UseVisualStyleBackColor = true;
            this.btnJump.Click += new System.EventHandler(this.btnJump_Click);
            // 
            // btnViewPal
            // 
            this.btnViewPal.Location = new System.Drawing.Point(787, 187);
            this.btnViewPal.Margin = new System.Windows.Forms.Padding(4);
            this.btnViewPal.Name = "btnViewPal";
            this.btnViewPal.Size = new System.Drawing.Size(97, 31);
            this.btnViewPal.TabIndex = 29;
            this.btnViewPal.Text = "view pal";
            this.btnViewPal.UseVisualStyleBackColor = true;
            this.btnViewPal.Click += new System.EventHandler(this.btnViewPal_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(671, 321);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(42, 17);
            this.label12.TabIndex = 30;
            this.label12.Text = "startx";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(668, 353);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(43, 17);
            this.label13.TabIndex = 31;
            this.label13.Text = "starty";
            // 
            // txtStartx
            // 
            this.txtStartx.Location = new System.Drawing.Point(721, 321);
            this.txtStartx.Margin = new System.Windows.Forms.Padding(4);
            this.txtStartx.Name = "txtStartx";
            this.txtStartx.Size = new System.Drawing.Size(56, 22);
            this.txtStartx.TabIndex = 32;
            this.txtStartx.Text = "0";
            // 
            // txtStarty
            // 
            this.txtStarty.Location = new System.Drawing.Point(721, 353);
            this.txtStarty.Margin = new System.Windows.Forms.Padding(4);
            this.txtStarty.Name = "txtStarty";
            this.txtStarty.Size = new System.Drawing.Size(56, 22);
            this.txtStarty.TabIndex = 33;
            this.txtStarty.Text = "0";
            // 
            // lstInstructions
            // 
            this.lstInstructions.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstInstructions.FormattingEnabled = true;
            this.lstInstructions.ItemHeight = 14;
            this.lstInstructions.Location = new System.Drawing.Point(917, 14);
            this.lstInstructions.Margin = new System.Windows.Forms.Padding(4);
            this.lstInstructions.Name = "lstInstructions";
            this.lstInstructions.Size = new System.Drawing.Size(399, 410);
            this.lstInstructions.TabIndex = 34;
            // 
            // txtInstructionsOffset
            // 
            this.txtInstructionsOffset.Location = new System.Drawing.Point(803, 410);
            this.txtInstructionsOffset.Margin = new System.Windows.Forms.Padding(4);
            this.txtInstructionsOffset.Name = "txtInstructionsOffset";
            this.txtInstructionsOffset.Size = new System.Drawing.Size(105, 22);
            this.txtInstructionsOffset.TabIndex = 35;
            this.txtInstructionsOffset.Text = "0";
            // 
            // txtAddressOffset
            // 
            this.txtAddressOffset.Location = new System.Drawing.Point(672, 410);
            this.txtAddressOffset.Margin = new System.Windows.Forms.Padding(4);
            this.txtAddressOffset.Name = "txtAddressOffset";
            this.txtAddressOffset.Size = new System.Drawing.Size(105, 22);
            this.txtAddressOffset.TabIndex = 36;
            this.txtAddressOffset.Text = "0";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(672, 385);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(83, 22);
            this.txtSearch.TabIndex = 37;
            this.txtSearch.Text = "0";
            // 
            // btnFindInt32
            // 
            this.btnFindInt32.Location = new System.Drawing.Point(803, 379);
            this.btnFindInt32.Margin = new System.Windows.Forms.Padding(4);
            this.btnFindInt32.Name = "btnFindInt32";
            this.btnFindInt32.Size = new System.Drawing.Size(67, 31);
            this.btnFindInt32.TabIndex = 38;
            this.btnFindInt32.Text = "find32";
            this.btnFindInt32.UseVisualStyleBackColor = true;
            this.btnFindInt32.Click += new System.EventHandler(this.btnFindInt32_Click);
            // 
            // txtSearchRange
            // 
            this.txtSearchRange.Location = new System.Drawing.Point(761, 385);
            this.txtSearchRange.Margin = new System.Windows.Forms.Padding(4);
            this.txtSearchRange.Name = "txtSearchRange";
            this.txtSearchRange.Size = new System.Drawing.Size(32, 22);
            this.txtSearchRange.TabIndex = 39;
            this.txtSearchRange.Text = "0";
            // 
            // frmFindInt16
            // 
            this.frmFindInt16.Location = new System.Drawing.Point(803, 346);
            this.frmFindInt16.Margin = new System.Windows.Forms.Padding(4);
            this.frmFindInt16.Name = "frmFindInt16";
            this.frmFindInt16.Size = new System.Drawing.Size(67, 31);
            this.frmFindInt16.TabIndex = 40;
            this.frmFindInt16.Text = "find16";
            this.frmFindInt16.UseVisualStyleBackColor = true;
            this.frmFindInt16.Click += new System.EventHandler(this.frmFindInt16_Click);
            // 
            // lstFunctions
            // 
            this.lstFunctions.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstFunctions.FormattingEnabled = true;
            this.lstFunctions.ItemHeight = 14;
            this.lstFunctions.Location = new System.Drawing.Point(1325, 14);
            this.lstFunctions.Margin = new System.Windows.Forms.Padding(4);
            this.lstFunctions.Name = "lstFunctions";
            this.lstFunctions.Size = new System.Drawing.Size(213, 410);
            this.lstFunctions.TabIndex = 41;
            this.lstFunctions.SelectedIndexChanged += new System.EventHandler(this.lstFunctions_SelectedIndexChanged);
            // 
            // txtFunction
            // 
            this.txtFunction.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFunction.Location = new System.Drawing.Point(1548, 14);
            this.txtFunction.Margin = new System.Windows.Forms.Padding(4);
            this.txtFunction.Multiline = true;
            this.txtFunction.Name = "txtFunction";
            this.txtFunction.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFunction.Size = new System.Drawing.Size(492, 420);
            this.txtFunction.TabIndex = 42;
            this.txtFunction.WordWrap = false;
            // 
            // chk24bpp
            // 
            this.chk24bpp.AutoSize = true;
            this.chk24bpp.Location = new System.Drawing.Point(787, 229);
            this.chk24bpp.Margin = new System.Windows.Forms.Padding(4);
            this.chk24bpp.Name = "chk24bpp";
            this.chk24bpp.Size = new System.Drawing.Size(74, 21);
            this.chk24bpp.TabIndex = 43;
            this.chk24bpp.Text = "24 bpp";
            this.chk24bpp.UseVisualStyleBackColor = true;
            // 
            // btnJumpFunctionList
            // 
            this.btnJumpFunctionList.Location = new System.Drawing.Point(862, 3);
            this.btnJumpFunctionList.Margin = new System.Windows.Forms.Padding(4);
            this.btnJumpFunctionList.Name = "btnJumpFunctionList";
            this.btnJumpFunctionList.Size = new System.Drawing.Size(47, 85);
            this.btnJumpFunctionList.TabIndex = 44;
            this.btnJumpFunctionList.Text = "jump to function list";
            this.btnJumpFunctionList.UseVisualStyleBackColor = true;
            this.btnJumpFunctionList.Click += new System.EventHandler(this.btnJumpFunctionList_Click);
            // 
            // btnAlundraEventFuncs
            // 
            this.btnAlundraEventFuncs.Location = new System.Drawing.Point(862, 89);
            this.btnAlundraEventFuncs.Margin = new System.Windows.Forms.Padding(4);
            this.btnAlundraEventFuncs.Name = "btnAlundraEventFuncs";
            this.btnAlundraEventFuncs.Size = new System.Drawing.Size(47, 97);
            this.btnAlundraEventFuncs.TabIndex = 45;
            this.btnAlundraEventFuncs.Text = "alundra event funcs";
            this.btnAlundraEventFuncs.UseVisualStyleBackColor = true;
            this.btnAlundraEventFuncs.Click += new System.EventHandler(this.btnAlundraEventFuncs_Click);
            // 
            // btnFunctionTracer
            // 
            this.btnFunctionTracer.Location = new System.Drawing.Point(839, 260);
            this.btnFunctionTracer.Margin = new System.Windows.Forms.Padding(4);
            this.btnFunctionTracer.Name = "btnFunctionTracer";
            this.btnFunctionTracer.Size = new System.Drawing.Size(70, 46);
            this.btnFunctionTracer.TabIndex = 46;
            this.btnFunctionTracer.Text = "function tracer";
            this.btnFunctionTracer.UseVisualStyleBackColor = true;
            this.btnFunctionTracer.Click += new System.EventHandler(this.btnFunctionTracer_Click);
            // 
            // frmFileAnalyzer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2057, 456);
            this.Controls.Add(this.btnFunctionTracer);
            this.Controls.Add(this.btnAlundraEventFuncs);
            this.Controls.Add(this.btnJumpFunctionList);
            this.Controls.Add(this.chk24bpp);
            this.Controls.Add(this.txtFunction);
            this.Controls.Add(this.lstFunctions);
            this.Controls.Add(this.frmFindInt16);
            this.Controls.Add(this.txtSearchRange);
            this.Controls.Add(this.btnFindInt32);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.txtAddressOffset);
            this.Controls.Add(this.txtInstructionsOffset);
            this.Controls.Add(this.lstInstructions);
            this.Controls.Add(this.txtStarty);
            this.Controls.Add(this.txtStartx);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnViewPal);
            this.Controls.Add(this.btnJump);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.txtStride);
            this.Controls.Add(this.btnViewImage);
            this.Controls.Add(this.lbl4bitb);
            this.Controls.Add(this.lblCursorOffset);
            this.Controls.Add(this.lblRelOffset);
            this.Controls.Add(this.lblSelLength);
            this.Controls.Add(this.lbl4bita);
            this.Controls.Add(this.lbl32bit);
            this.Controls.Add(this.lbl16bit);
            this.Controls.Add(this.lbl8bit);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtfText);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmFileAnalyzer";
            this.Text = "frmFileAnalyzer";
            this.Load += new System.EventHandler(this.frmFileAnalyzer_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmFileAnalyzer_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtfText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbl8bit;
        private System.Windows.Forms.Label lbl16bit;
        private System.Windows.Forms.Label lbl32bit;
        private System.Windows.Forms.Label lbl4bita;
        private System.Windows.Forms.Label lblSelLength;
        private System.Windows.Forms.Label lblRelOffset;
        private System.Windows.Forms.Label lblCursorOffset;
        private System.Windows.Forms.Label lbl4bitb;
        private System.Windows.Forms.Button btnViewImage;
        private System.Windows.Forms.TextBox txtStride;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Button btnJump;
        private System.Windows.Forms.Button btnViewPal;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtStartx;
        private System.Windows.Forms.TextBox txtStarty;
        private System.Windows.Forms.ListBox lstInstructions;
        private System.Windows.Forms.TextBox txtInstructionsOffset;
        private System.Windows.Forms.TextBox txtAddressOffset;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnFindInt32;
        private System.Windows.Forms.TextBox txtSearchRange;
        private System.Windows.Forms.Button frmFindInt16;
        private System.Windows.Forms.ListBox lstFunctions;
        private System.Windows.Forms.TextBox txtFunction;
        private System.Windows.Forms.CheckBox chk24bpp;
        private System.Windows.Forms.Button btnJumpFunctionList;
        private System.Windows.Forms.Button btnAlundraEventFuncs;
        private System.Windows.Forms.Button btnFunctionTracer;
    }
}