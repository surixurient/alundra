using System;
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
                    {
                        fname += "(";
                        var fref = Alundra.DebugSymbols.FunctionNames[functaddr];
                        if (!string.IsNullOrEmpty(fref.name))
                            fname += fref.name;
                        if (!string.IsNullOrEmpty(fref.comment))
                            fname += "//" + fref.comment;
                        fname += ")";
                    }
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
                var blocks = AnalyzeFunction(selectedFunction);
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
                        ccode = "void " + fnames[inst.address].name + "()";
                        if (!string.IsNullOrEmpty(fnames[inst.address].comment))
                            ccode += "//" + fnames[inst.address].comment;
                    }
                    else
                    {
                        switch (inst.cmd)
                        {
                            case "jal":
                                var sname = inst.referencedAddress.ToString("x");
                                var scomment = "";
                                if (fnames.ContainsKey(inst.referencedAddress))
                                {
                                    var fref = fnames[inst.referencedAddress];
                                    if (!string.IsNullOrEmpty(fref.name))
                                        sname = fref.name;
                                    if (!string.IsNullOrEmpty(fref.comment))
                                        scomment = fref.comment;
                                }
                                ccode = sname + "()//" + scomment;
                                break;
                            case "sw":
                            case "lw":
                            case "lhu":
                            case "lh":
                            case "shu":
                            case "sh":
                            case "lb":
                            case "sb":
                            case "lbu":
                            case "sbu":
                                if (inst.rs != 29)//if not local variable declaration
                                {
                                    bool nudgewierdness = false;
                                    //if (inst.rt == 16 || inst.rs == 16)
                                    //    nudgewierdness = true;
                                    if (nudgewierdness)
                                        inst.immediate += 0x134;
                                    //assume entity struct
                                    if (inst.immediate > 0xc && inst.immediate < evars.Length)
                                    { 
                                        
                                        if (!string.IsNullOrEmpty(evars[inst.immediate]))
                                        {
                                           
                                            if (inst.cmd == "lw" || inst.cmd == "lh" || inst.cmd == "lhu" || inst.cmd == "lb" || inst.cmd == "lbu")
                                                ccode = MIPS.GetRegister(inst.rt) + " = " + MIPS.GetRegister(inst.rs) + "." + evars[inst.immediate];
                                            else if (inst.cmd == "sw" || inst.cmd == "sh" || inst.cmd == "shu" || inst.cmd == "sb" || inst.cmd == "sbu")
                                                ccode = MIPS.GetRegister(inst.rs) + "." + evars[inst.immediate] + " = " + MIPS.GetRegister(inst.rt);
                                            if (nudgewierdness)
                                                inst.immediate -= 0x134;
                                            break;
                                        }
                                        
                                    }

                                    var fulladdr2 = inst.GetGlobalVariable(block);
                                    if (fulladdr2 > 0)
                                    {
                                        var name2 = "0x" + fulladdr2.ToString("x");
                                        if (globalvars.ContainsKey(fulladdr2))
                                            name2 = globalvars[fulladdr2].name;
                                        
                                        if (inst.cmd == "lw" || inst.cmd == "lh" || inst.cmd == "lhu" || inst.cmd == "lb" || inst.cmd == "lbu")
                                            ccode = MIPS.GetRegister(inst.rt) + " = *" + name2;
                                        else if (inst.cmd == "sw" || inst.cmd == "sh" || inst.cmd == "shu" || inst.cmd == "sb" || inst.cmd == "sbu")
                                            ccode = "*" + name2 + " = " + MIPS.GetRegister(inst.rt);
                                    }
                                    else
                                    {
                                        if (inst.cmd == "lw" || inst.cmd == "lh" || inst.cmd == "lhu" || inst.cmd == "lb" || inst.cmd == "lbu")
                                            ccode = MIPS.GetRegister(inst.rt) + " = " + MIPS.GetRegister(inst.rs) + "[" + inst.immediate.ToString("x") + "]";
                                        else if (inst.cmd == "sw" || inst.cmd == "sh" || inst.cmd == "shu" || inst.cmd == "sb" || inst.cmd == "sbu")
                                            ccode = MIPS.GetRegister(inst.rs) + "[" + inst.immediate.ToString("x") + "]" + " = " + MIPS.GetRegister(inst.rt);    
                                    }
                                    if (nudgewierdness)
                                        inst.immediate -= 0x134;
                                }
                                break;

                            case "addiu":
                            case "addi":
                            case "ori":
                            //case "lw":
                            //case "sw":
                            //case "lhu":
                            //case "lh":
                            //case "shu":
                            //case "sh":
                                var fulladdr = inst.GetGlobalVariable(block);
                                if (fulladdr != 0)
                                {
                                    if (globalvars.ContainsKey(fulladdr))
                                        ccode = "//" + globalvars[fulladdr].name;
                                    else
                                        ccode = "//0x" + fulladdr.ToString("x");
                                }
                                break;
                            /*case "addiu":
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
                                break;*/
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
                    else if (block.BranchOperation != null)
                    {
                        ftext += RawIndent(codestart) + Indent(indentlevel) + block.BranchOperation.Print()  + "\r\n";
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

        static List<MIPS.CodeBlock> AnalyzeFunction(List<MIPS.Instruction> function)
        {
            List<uint> refs = function.Select(x => x.referencedAddress).Where(x => x != 0).ToList();


            List<MIPS.CodeBlock> blocks = new List<MIPS.CodeBlock>();

            var block = new MIPS.CodeBlock();

            for (int dex = 0; dex < function.Count; dex++)
            {
                var inst = function[dex];
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
                    inst = function[dex];
                    block.Instructions.Add(inst);
                    blocks.Add(block);
                    block = new MIPS.CodeBlock();
                }
                else if (inst.IsReturn)
                {
                    block.BlockType = MIPS.BlockType.Return;
                    block.Instructions.Add(inst);
                    dex++;
                    if (dex < function.Count)
                    {
                        inst = function[dex];
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
                    inst = function[dex];
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
                    inst = function[dex];
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


        public class AnalyzedGlobalVariable
        {
            public AnalyzedGlobalVariable (uint addr, List<AnalyzedGlobalVariable> varlist)
            {
                varlist.Add(this);
                var globalvars = Alundra.DebugSymbols.GlobalVariableNames;
                address = addr;
                if (globalvars.ContainsKey(address))
                    name = globalvars[address].name;
            }
            public uint address;
            public string name;
            public string notes;
            public List<AnalyzedFunction> functions = new List<AnalyzedFunction>();

            public string DisplayName
            {
                get
                {
                    if (!string.IsNullOrEmpty(name))
                        return name + "(" + address.ToString("x") + ")";
                    return address.ToString("x");
                }
            }

            public override string ToString()
            {
                return DisplayName + "[" + functions.Count + "]";
            }
        }
        public class AnalyzedFunction
        {
            public AnalyzedFunction(uint addr, List<AnalyzedFunction> funclist, List<AnalyzedGlobalVariable> varlist, string datafile, Func<List<MIPS.Instruction>,List<MIPS.CodeBlock>> AnalyzeFunction, uint endaddr = 0)
            {
                var fnames = Alundra.DebugSymbols.FunctionNames;
                var evars = Alundra.DebugSymbols.EntityVarOffsets;
                var globalvars = Alundra.DebugSymbols.GlobalVariableNames;
                int chunklength = 1024 * 1024;

                funclist.Add(this);
                address = addr;
                if (fnames.ContainsKey(address))
                {
                    name = fnames[address].name;
                    notes = fnames[address].comment;
                }

                

                var fdat = new byte[chunklength];
                var stream = File.OpenRead(datafile);
                stream.Position = address;
                int numread = stream.Read(fdat, 0, chunklength);
                stream.Close();

                bool exit = false;

                var function = new List<MIPS.Instruction>();

                for (int dex = 0; dex < 10000; dex += 4)
                {
                    var inst = new MIPS.Instruction((uint)(address + dex), (uint)(fdat[dex] | fdat[dex + 1] << 8 | fdat[dex + 2] << 16 | (uint)fdat[dex + 3] << 24));
                    function.Add(inst);
                    if (exit)
                        break;
                    if (inst.IsReturn || ( endaddr != 0 && inst.address == endaddr))
                        exit = true;
                }
                length = (int)(function.Last().address - address);
                var blocks = AnalyzeFunction(function);

                

                foreach (var block in blocks)
                {
                    foreach (var inst in block.Instructions)
                    {
                        switch (inst.cmd)
                        {
                            case "jal":
                                var calledfunc = funclist.FirstOrDefault(x => x.address == inst.referencedAddress);
                                if (calledfunc == null)
                                {
                                    calledfunc = new AnalyzedFunction(inst.referencedAddress, funclist, varlist, datafile, AnalyzeFunction);
                                }
                                if (!calledfunctions.Contains(calledfunc))
                                    calledfunctions.Add(calledfunc);
                                break;
                            case "jalr":
                                callsfunctionpointers = true;
                                break;
                            //TODO record global variables
                            case "addiu":
                            case "addi":
                            case "ori":
                            case "lw":
                            case "sw":
                            case "lhu":
                            case "lh":
                            case "shu":
                            case "sh":
                            case "lb":
                            case "sb":
                            case "lbu":
                            case "sbu":
                                var fulladdr = inst.GetGlobalVariable(block);
                                if (fulladdr != 0)
                                {
                                    var gvar = varlist.FirstOrDefault(x => x.address == fulladdr);
                                    if (gvar == null)
                                    {
                                        gvar = new AnalyzedGlobalVariable(fulladdr, varlist);
                                    }
                                    if (!globalvariables.Contains(gvar))
                                        globalvariables.Add(gvar);
                                }
                                break;
                            

                        }
       
                    }
                    if (block.EndsLoop)
                    {
                        hasloop = true;
                    }
                }

                var debugnames = new[] { "outputdebuginfo", "printdebug", "printdebugparams", "printdebugerror" };
                if (calledfunctions.Any(x => debugnames.Contains(x.name)))
                    hasdebugoutput = true;
            }
            public uint address;
            public int length;
            public string name;
            public string notes;
            public List<string> parameters = new List<string>();
            public string returnval;
            public List<AnalyzedFunction> calledfunctions = new List<AnalyzedFunction>();
            public List<AnalyzedGlobalVariable> globalvariables = new List<AnalyzedGlobalVariable>();
            public List<AnalyzedFunction> calledby = new List<AnalyzedFunction>();
            public bool callsfunctionpointers = false;
            public bool hasloop = false;
            public bool hasdebugoutput = false;

            public List<AnalyzedFunction> stack = new List<AnalyzedFunction>();
            public List<AnalyzedFunction> maxstack = new List<AnalyzedFunction>();

            public List<AnalyzedFunction> GetDepth(List<AnalyzedFunction> depthstack)
            {
                stack = depthstack;
                maxstack = stack;
                //detect recursion
                if (depthstack.Contains(this))
                    return depthstack;

                depthstack.Add(this);


                foreach (var func in calledfunctions)
                {
                    var potentialstack = func.GetDepth(depthstack.ToList());
                    if (potentialstack.Count > maxstack.Count)
                        maxstack = potentialstack;
                }

                return maxstack;
            }

            public string DisplayName
            {
                get
                {
                    if (!string.IsNullOrEmpty(name))
                        return name;
                    return address.ToString("x");
                }
            }

            public override string ToString()
            {
                return DisplayName + "()" + (!string.IsNullOrEmpty(notes) ? $"//{notes}" : "") +
                    " funcs:" + calledfunctions.Count +
                    " calledby:" + calledby.Count +
                    " depth:" + maxstack.Count + 
                    (hasdebugoutput ? "hasdebug" : "");
            }
        }

        List<AnalyzedFunction> analyzedfunctions = new List<AnalyzedFunction>();
        List<AnalyzedGlobalVariable> analyzedglobalvariables = new List<AnalyzedGlobalVariable>();
        AnalyzedFunction root = null;
        
        private void btnFunctionTracer_Click(object sender, EventArgs e)
        {
            this.Height = 1000;
            var address = (uint)ParseNum(txtOffset.Text);

            address = 0x0002c4a4;
            var endaddress = (uint)0x0002c518;
            analyzedfunctions = new List<AnalyzedFunction>();
            analyzedglobalvariables = new List<AnalyzedGlobalVariable>();
            root = new AnalyzedFunction(address, analyzedfunctions, analyzedglobalvariables, datafile, AnalyzeFunction, endaddress);

            foreach(var func in analyzedfunctions)
            {
                //set calledby
                foreach(var testfunc in analyzedfunctions)
                {
                    if (func!=testfunc)
                    {
                        if (testfunc.calledfunctions.Contains(func))
                        {
                            func.calledby.Add(testfunc);
                        }
                    }
                }
                func.calledby = func.calledby.OrderBy(x => x.address).ToList();
            }

            var maxstack = root.GetDepth(new List<AnalyzedFunction>());

            analyzedfunctions = analyzedfunctions.OrderBy(x => x.address).ToList();

            foreach(var gvar in analyzedglobalvariables)
            {
                foreach(var testfunc in analyzedfunctions)
                {
                    if (testfunc.globalvariables.Contains(gvar))
                    {
                        gvar.functions.Add(testfunc);
                    }
                }
            }

            analyzedglobalvariables = analyzedglobalvariables.OrderByDescending(x => x.functions.Count).ToList();

        }
    }
}
