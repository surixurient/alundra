﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using alundramultitool;

namespace GraphicsTools
{
    public partial class frmFileAnalyzer : Form
    {
        public frmFileAnalyzer()
        {
            InitializeComponent();
            Alundra.DebugSymbols.Init();
        }

        public string datafile;

        public int offset;
        public int memaddress;
        public int startoffset = 0;
        public int startsize = 0;
        int chunklength = 1024 * 1024;
        byte[] data;

        int ParseNum(string num)
        {
            int i = 0;
            if (num.StartsWith("0x"))
            {
                int.TryParse(num.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier, null, out i);
            }
            else
            {
                int.TryParse(num, out i);
            }
            return i;
        }

        private void frmFileAnalyzer_Load(object sender, EventArgs e)
        {
            loadchunk(offset);
        }

        int instOffset = 0;
        List<MIPS.Instruction> instructions = new List<MIPS.Instruction>();
        List<uint> functions = new List<uint>();
        void loadchunk(int offset, bool alundraeventlist = false)
        {
            txtAddressOffset.Text = (memaddress - offset).ToString();
            this.offset = offset;
            data = new byte[chunklength];
            var stream = File.OpenRead(datafile);
            stream.Position = offset;
            int numread = stream.Read(data, 0, chunklength);
            stream.Close();
            rtfText.LoadFile(new MemoryStream(data), RichTextBoxStreamType.PlainText);
            rtfText.Select(startoffset, startsize);

            instOffset = ParseNum(txtInstructionsOffset.Text);
            instructions.Clear();
            lstInstructions.Items.Clear();
            lstFunctions.Items.Clear();
            var mips = new MIPS();
            if (alundraeventlist)
            {
                functions.Clear();
                for (int dex = 0; dex <= 0x3FC; dex += 4)
                {
                    uint addr = (uint)(data[dex] | data[dex + 1] << 8 | data[dex + 2] << 16 | (uint)data[dex + 3] << 24);
                    functions.Add(0xFFFFFFF & addr);
                }

                for (int fdex=0;fdex<functions.Count;fdex++)
                {
                    var functaddr = functions[fdex];
                    var sicode = Alundra.SpriteInfoEventCodes.GetCode((byte)fdex);

                    string fname = $"({sicode.code.ToString("x2")}_{sicode.name}_handler)";

                    lstFunctions.Items.Add("0x"+functaddr.ToString("x") + fname);
                }
            }
            else
            {
                for (int dex = 0; dex < 0x6000; dex += 4)
                {
                    var inst = new MIPS.Instruction((uint)(instOffset + offset + dex), (uint)(data[dex] | data[dex + 1] << 8 | data[dex + 2] << 16 | (uint)data[dex + 3] << 24));
                    if (inst.cmd == "jal")
                        functions.Add(inst.referencedAddress);

                    instructions.Add(inst);
                    lstInstructions.Items.Add(string.Format("{0}:  {1}", inst.address.ToString("x8"), inst.display));
                }
                foreach (var functaddr in functions.Distinct().OrderBy(x => x))
                {
                    string fname = "";
                    if (Alundra.DebugSymbols.FunctionNames.ContainsKey(functaddr))
                        fname = " (" + Alundra.DebugSymbols.FunctionNames[functaddr] + ")";
                    lstFunctions.Items.Add("0x" + functaddr.ToString("x8") + fname);
                }
            }
        }

        void DisplayData(int pos)
        {
            int addrOffset = 0;
            int.TryParse(txtAddressOffset.Text, out addrOffset);
            txtOffset.Text = offset.ToString();
            lblCursorOffset.Text = (pos + offset).ToString() + "(" + (pos + offset + addrOffset).ToString("x6") + ")";
            lblRelOffset.Text = pos.ToString();
            lblSelLength.Text = rtfText.SelectionLength.ToString();

            lbl8bit.Text = data[pos].ToString() + " (" + data[pos].ToString("x2") + ")"; ;
            long l = data[pos] | data[pos + 1] << 8;
            lbl16bit.Text = l.ToString() + " (" + l.ToString("x4") + ")";
            l = data[pos] | data[pos + 1] << 8 | data[pos + 2] << 16 | (long)data[pos + 3] << 24;
            lbl32bit.Text = l.ToString() + " (" + l.ToString("x8") + ")";

            lbl4bita.Text = ((data[pos] & 0xf0) >> 4).ToString();
            lbl4bitb.Text = (data[pos] & 0xf).ToString();
        }

        private void rtfText_SelectionChanged(object sender, EventArgs e)
        {
            if (rtfText.SelectionStart >= 0)
                DisplayData(rtfText.SelectionStart);
        }

        frmViewer viewer;
        frmViewer pal;

        private void btnViewImage_Click(object sender, EventArgs e)
        {
            int stride, width, height, startx, starty;
            int.TryParse(txtStride.Text, out stride);
            int.TryParse(txtWidth.Text, out width);
            int.TryParse(txtHeight.Text, out height);
            int.TryParse(txtStartx.Text, out startx);
            int.TryParse(txtStarty.Text, out starty);

            int imagestart = rtfText.SelectionStart;

            int bpp = 4;
            byte[] imagedata = new byte[width * height * bpp / 8];

            if (stride == -1)
            {
                //compressed
                int imagedex = 0;
                int buffdex = imagestart;
                while (imagedex < imagedata.Length)
                {
                    byte b = data[buffdex++];
                    if (b == 0xad)
                    {
                        int seek = data[buffdex++];
                        if (seek == 0)
                        {
                            imagedata[imagedex++] = b;
                        }
                        else
                        {
                            int len = data[buffdex++];
                            int seekdex = imagedex - seek;
                            while (len-- > 0)
                                imagedata[imagedex++] = imagedata[seekdex++];
                        }
                    }
                    else
                        imagedata[imagedex++] = b;
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    Buffer.BlockCopy(data, imagestart + (y + starty) * stride + (startx / 2), imagedata, y * width * bpp / 8, width * bpp / 8);
                }
            }

            int paloffset = 0;
            Color[] palettes = new Color[(int)Math.Pow(2, bpp)];
            if (Program.palette != null)
                palettes = Program.palette;
            else
            {
                for (int dex = 0; dex < palettes.Length; dex++)
                {
                    int ddex = imagestart + paloffset + dex * 2;
                    palettes[dex] = Utils.FromPsxColor(data[ddex + 1], data[dex]);// Color.FromArgb(255, (data[ddex + 1] & 0x1f) << 3, ((data[ddex + 1] & 0xe0) >> 2) | ((data[ddex] & 0x3) << 6), data[ddex] & 0x7c);
                }
            }
            viewer = new frmViewer();
            viewer.Show();
            viewer.init(imagedata,16,4, width, height, palettes);
        }

        private void btnJump_Click(object sender, EventArgs e)
        {
            offset = ParseNum(txtOffset.Text);
            loadchunk(offset);
        }

        

        private void btnViewPal_Click(object sender, EventArgs e)
        {
            int stride, width, height;
            int.TryParse(txtStride.Text, out stride);
            int.TryParse(txtWidth.Text, out width);
            int.TryParse(txtHeight.Text, out height);

            int palettestart = rtfText.SelectionStart;

            int bpp = 16;
            byte[] imagedata = new byte[width * height * bpp / 8];
            for (int y = 0; y < height; y++)
            {
                Buffer.BlockCopy(data, palettestart + y * stride, imagedata, y * width * bpp / 8, width * bpp / 8);

            }
            var frm = new frmViewer();
            frm.Show();
            frm.initpalette(viewer, imagedata,16,4, width, height);
        }

        private void rtfText_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (rtfText.SelectionStart >= 0)
            {
                lstInstructions.SelectedIndex = (rtfText.SelectionStart) / 4;

            }
        }



        private void btnFindInt32_Click(object sender, EventArgs e)
        {
            long search = ParseNum(txtSearch.Text);
            int range = ParseNum(txtSearchRange.Text);
            for (int dex = rtfText.SelectionStart + 1; dex < data.Length - 3; dex++)
            {
                long num = data[dex] | data[dex + 1] << 8 | data[dex + 2] << 16 | data[dex + 3] << 24;
                if (num >= search && num <= search + range)
                {
                    rtfText.Focus();
                    rtfText.Select(dex, 0);
                    //rtfText.ScrollToCaret();
                    break;
                }
            }
        }

        private void frmFileAnalyzer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
                btnFindInt32_Click(null, null);
        }

        private void frmFindInt16_Click(object sender, EventArgs e)
        {
            long search = ParseNum(txtSearch.Text);
            int range = ParseNum(txtSearchRange.Text);
            for (int dex = rtfText.SelectionStart + 1; dex < data.Length - 3; dex++)
            {
                long num = data[dex] | data[dex + 1] << 8;// | data[dex + 2] << 16 | data[dex + 3] << 24;
                if (num >= search && num <= search + range)
                {
                    rtfText.Focus();
                    rtfText.Select(dex, 0);
                    //rtfText.ScrollToCaret();
                    break;
                }
            }
        }
        List<MIPS.Instruction> selectedFunction = null;
        private void lstFunctions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFunctions.SelectedIndex >= 0)
            {
                int address = ParseNum(lstFunctions.Text.Split('(')[0].Trim());
                var fdat = new byte[chunklength];
                var stream = File.OpenRead(datafile);
                stream.Position = address;
                int numread = stream.Read(fdat, 0, chunklength);
                stream.Close();

                bool exit = false;

                selectedFunction = new List<MIPS.Instruction>();

                for (int dex = 0; dex < 10000; dex += 4)
                {
                    var inst = new MIPS.Instruction((uint)(address + dex), (uint)(fdat[dex] | fdat[dex + 1] << 8 | fdat[dex + 2] << 16 | (uint)fdat[dex + 3] << 24));
                    selectedFunction.Add(inst);
                    if (exit)
                        break;
                    if (inst.IsReturn)
                        exit = true;
                }
                var blocks = AnalyzeFunction();
                txtFunction.Text = PrintFunction(blocks);

            }
        }

        string PrintFunction(List<MIPS.CodeBlock> blocks)
        {
            string ftext = "";

            var fnames = Alundra.DebugSymbols.FunctionNames;
            var evars = Alundra.DebugSymbols.EntityVarOffsets;
            var globalvars = Alundra.DebugSymbols.GlobalVariableNames;

            int indentlevel = 0;
            int codestart = 40;
            foreach (var block in blocks)
            {
                if (block.BeginsLoop)
                {
                    ftext += RawIndent(codestart) + Indent(indentlevel) + "do{\r\n";
                    indentlevel++;
                }
                if (block.IsJumpTarget)
                {
                    ftext += "\r\n";
                }
                foreach (var inst in block.Instructions)
                {
                    string ccode = "";
                    if (fnames.ContainsKey(inst.address))
                    {
                        ccode = "void " + fnames[inst.address] + "()";
                    }
                    else
                    {
                        switch (inst.cmd)
                        {
                            case "jal":
                                if (fnames.ContainsKey(inst.referencedAddress))
                                    ccode = fnames[inst.referencedAddress];
                                else
                                    ccode = inst.referencedAddress.ToString("x");
                                ccode += "()";
                                break;
                            case "sw":
                            case "lw":
                                if (inst.rs != 29)//if not local variable declaration
                                {
                                    bool nudgewierdness = false;
                                    //if (inst.rt == 16 || inst.rs == 16)
                                    //    nudgewierdness = true;
                                    if (nudgewierdness)
                                        inst.immediate += 0x134;
                                    //assume entity struct
                                    if (inst.immediate > 0 && inst.immediate < evars.Length)
                                    { 
                                        
                                        if (!string.IsNullOrEmpty(evars[inst.immediate]))
                                        {
                                           
                                            if (inst.cmd == "lw")
                                                ccode = MIPS.GetRegister(inst.rt) + " = " + MIPS.GetRegister(inst.rs) + "." + evars[inst.immediate];
                                            else if (inst.cmd == "sw")
                                                ccode = MIPS.GetRegister(inst.rs) + "." + evars[inst.immediate] + " = " + MIPS.GetRegister(inst.rt);
                                            if (nudgewierdness)
                                                inst.immediate -= 0x134;
                                            break;
                                        }
                                        
                                    }
                                    if (inst.cmd == "lw")
                                        ccode = MIPS.GetRegister(inst.rt) + " = " + MIPS.GetRegister(inst.rs) + "[" + inst.immediate.ToString("x") + "]";
                                    else if (inst.cmd == "sw")
                                        ccode = MIPS.GetRegister(inst.rs) + "[" + inst.immediate.ToString("x") + "]" + " = " + MIPS.GetRegister(inst.rt);
                                    if (nudgewierdness)
                                        inst.immediate -= 0x134;
                                }
                                break;
                            case "addiu":
                                //get prev lui
                                var mdex = block.Instructions.IndexOf(inst);
                                uint addr = inst.immediateu;
                                var reg = inst.rs;
                                for (int dex = mdex-1;mdex>=dex-3 && dex >=0;dex--)
                                {
                                    var binst = block.Instructions[dex];
                                    if (binst.cmd == "lui" && binst.rt == reg)
                                    {
                                        uint fulladdr = (uint)((UInt32)(((uint)binst.immediate & 0xff) << 16) | addr);

                                        if (globalvars.ContainsKey(fulladdr))
                                            ccode = "//" + globalvars[fulladdr];
                                        else
                                            ccode = "//0x" + fulladdr.ToString("x");
                                        break;
                                    }
                                }
                                
                                //get prev
                                break;
                        }
                    }
                    string asm = string.Format("{0}: {1} {2}", ((block.Instructions.IndexOf(inst) == 0 && block.IsJumpTarget) ? "0x" : "") + inst.address.ToString("x8"), inst.instruction.ToString("x8"), inst.display);
                    ftext += string.Format("{0}{1}{2}{3}\r\n", asm, RawIndent(codestart - asm.Length), Indent(indentlevel), ccode);
                }
                if (block.EndsLoop)
                {
                    indentlevel--;
                    ftext += RawIndent(codestart) + Indent(indentlevel) + "}\r\n";
                }
                if (block.BlockType == MIPS.BlockType.TwoWay)
                {
                    if (block.EndsLoop)
                    {
                        //output nothing special, this jump is just ending a posttested loop
                    }
                    else
                    {
                        ftext += RawIndent(codestart) + Indent(indentlevel) + "if\r\n";
                    }
                }
                if (block.BlockType == MIPS.BlockType.OneWay)
                {
                    if (block.EndsLoop)
                    {
                        //output nothing special, this jump is just ending a pretested or infinite loop
                    }
                    else if (block.OutEdges[0].BlockType == MIPS.BlockType.Return)
                    {
                        ftext += RawIndent(codestart) + Indent(indentlevel) + "return\r\n";
                    }
                    else if (block.OutEdges[0].EndsLoop)
                    {
                        ftext += RawIndent(codestart) + Indent(indentlevel) + "continue\r\n";
                    }
                    //need a way to detect a break
                    else
                    {
                        ftext += RawIndent(codestart) + Indent(indentlevel) + "else\r\n";
                    }
                }

            }

            return ftext;
        }

        string Indent(int indentlevel)
        {
            string indent = "";
            for (int dex = 0; dex < indentlevel; dex++)
            {
                indent += "  ";
            }
            return indent;
        }

        string RawIndent(int indentlevel)
        {
            string indent = "";
            for (int dex = 0; dex < indentlevel; dex++)
            {
                indent += " ";
            }
            return indent;
        }

        List<MIPS.CodeBlock> AnalyzeFunction()
        {
            List<uint> refs = selectedFunction.Select(x => x.referencedAddress).Where(x => x != 0).ToList();


            List<MIPS.CodeBlock> blocks = new List<MIPS.CodeBlock>();

            var block = new MIPS.CodeBlock();

            for (int dex = 0; dex < selectedFunction.Count; dex++)
            {
                var inst = selectedFunction[dex];
                if (refs.Contains(inst.address))
                {
                    if (block.Instructions.Count > 0)
                    {
                        block.BlockType = MIPS.BlockType.FallThrough;
                        block.OutAddresses.Add(inst.address);
                        blocks.Add(block);
                        block = new MIPS.CodeBlock();
                    }
                }
                if (inst.IsCall)
                {
                    block.BlockType = MIPS.BlockType.Call;
                    block.OutAddresses.Add(inst.address + 8);
                    block.Instructions.Add(inst);
                    dex++;
                    inst = selectedFunction[dex];
                    block.Instructions.Add(inst);
                    blocks.Add(block);
                    block = new MIPS.CodeBlock();
                }
                else if (inst.IsReturn)
                {
                    block.BlockType = MIPS.BlockType.Return;
                    block.Instructions.Add(inst);
                    dex++;
                    if (dex < selectedFunction.Count)
                    {
                        inst = selectedFunction[dex];
                        block.Instructions.Add(inst);
                    }
                    blocks.Add(block);
                    break;
                }
                else if (inst.IsBranch)
                {
                    block.BlockType = MIPS.BlockType.TwoWay;
                    block.OutAddresses.Add(inst.referencedAddress);
                    block.OutAddresses.Add(inst.address + 8);
                    block.Instructions.Add(inst);
                    dex++;
                    inst = selectedFunction[dex];
                    block.Instructions.Add(inst);
                    blocks.Add(block);
                    block = new MIPS.CodeBlock();
                }
                else if (inst.IsJump)
                {
                    block.BlockType = MIPS.BlockType.OneWay;
                    block.OutAddresses.Add(inst.referencedAddress);
                    block.Instructions.Add(inst);
                    dex++;
                    inst = selectedFunction[dex];
                    block.Instructions.Add(inst);
                    blocks.Add(block);
                    block = new MIPS.CodeBlock();
                }
                else
                {
                    block.Instructions.Add(inst);
                }

            }

            foreach (var b in blocks)
            {
                b.OutEdges = b.OutAddresses.Select(x => blocks.Where(y => y.Address == x).FirstOrDefault()).ToList();
                b.InEdges = blocks.Where(x => x.OutAddresses.Contains(b.Address)).ToList();
            }


            return blocks;
        }

        private void btnJumpFunctionList_Click(object sender, EventArgs e)
        {
            offset = ParseNum(txtOffset.Text);
            loadchunk(offset, true);
        }

        private void btnAlundraEventFuncs_Click(object sender, EventArgs e)
        {
            lstInstructions.Width -= 100;
            lstFunctions.Left -= 100;
            lstFunctions.Width += 20;
            txtFunction.Left -= 80;
            txtFunction.Width += 10;
            offset = 0x9b5b4;
            loadchunk(offset, true);
        }
    }
}
