﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace alundramultitool
{
    public class MIPS
    {
        public static int ValAtOffset(uint instruction, int width, int bitoffset)
        {
            return (int)((instruction & (width << bitoffset)) >> bitoffset);
        }

        public static int SignedValAtOffset(uint instruction, int width, int bitoffset)
        {
            int signoffset = 0;
            while (width >> signoffset > 1)
                signoffset++;
            return (int)(ValAtOffset(instruction, width ^ (1 << signoffset), bitoffset) | (ValAtOffset(instruction, 1, signoffset) == 1 ? (0xffffffff >> signoffset) << signoffset : 0));
        }


        public static string GetRegister(int num)
        {
            return "r" + num;
        }

        public enum InstructionType
        {
            Rtype,
            Jtype,
            Itype
        }
        
        public class Instruction
        {
            public Instruction(uint address, uint instruction)
            {
                this.address = address;
                this.instruction = instruction;

                opcode = MIPS.ValAtOffset(instruction, 0x3f, 26);

                string onevalformat = "{0} {1}";
                string twovalformat = "{0} {1}, {2}";
                string threevalformat = "{0} {1},{2},{3}";
                string threevalmemoryformat = "{0} {1},{3}({2})";

                if (instruction == 0)
                {
                    cmd = "nop";
                    display = cmd;
                }
                else if (opcode == 0x0)//R type instruction
                {
                    type = InstructionType.Rtype;
                    funct = (int)(instruction & 0x3f);
                    rs = ValAtOffset(instruction, 0x1f, 6 + 5 * 3);
                    string srs = GetRegister(rs);
                    rt = ValAtOffset(instruction, 0x1f, 6 + 5 * 2);
                    string srt = GetRegister(rt);
                    rd = ValAtOffset(instruction, 0x1f, 6 + 5 * 1);
                    string srd = GetRegister(rd);
                    shamt = ValAtOffset(instruction, 0x1f, 6 + 5 * 0);

                    switch (funct)
                    {
                        case 0x20://add
                            cmd = "add";
                            display = string.Format(threevalformat, "add", srd, srs, srt);
                            break;
                        case 0x21://add unsigned
                            cmd = "addu";
                            display = string.Format(threevalformat, "addu", srd, srs, srt);
                            break;
                        case 0x22://subtract
                            cmd = "sub";
                            display = string.Format(threevalformat, "sub", srd, srs, srt);
                            break;
                        case 0x23://subtract unsigned
                            cmd = "subu";
                            display = string.Format(threevalformat, "subu", srd, srs, srt);
                            break;
                        case 0x18://multiply
                            cmd = "mult";
                            display = string.Format(twovalformat, "mult", srs, srt);
                            break;
                        case 0x19://multiply unsigned
                            cmd = "multu";
                            display = string.Format(twovalformat, "multu", srs, srt);
                            break;
                        case 0x1a://divide
                            cmd = "div";
                            display = string.Format(twovalformat, "div", srs, srt);
                            break;
                        case 0x1b://divide unsigned
                            cmd = "divu";
                            display = string.Format(twovalformat, "divu", srs, srt);
                            break;
                        case 0x10://move from hi
                            cmd = "mfhi";
                            display = string.Format(onevalformat, "mfhi", srd);
                            break;
                        case 0x12://move from low
                            cmd = "mflo";
                            display = string.Format(onevalformat, "mflo", srd);
                            break;
                        case 0x24://and
                            cmd = "and";
                            display = string.Format(threevalformat, "and", srd, srs, srt);
                            break;
                        case 0x25://or
                            cmd = "or";
                            display = string.Format(threevalformat, "or", srd, srs, srt);
                            break;
                        case 0x26://xor
                            cmd = "xor";
                            display = string.Format(threevalformat, "xor", srd, srs, srt);
                            break;
                        case 0x27://nor
                            cmd = "nor";
                            display = string.Format(threevalformat, "nor", srd, srs, srt);
                            break;
                        case 0x2a://set on less than
                            cmd = "slt";
                            display = string.Format(threevalformat, "slt", srd, srs, srt);
                            break;
                        case 0x0://shift left logical immediate
                            cmd = "sll";
                            display = string.Format(threevalformat, "sll", srd, srt, shamt);
                            break;
                        case 0x2://shift right logical immediate
                            cmd = "srl";
                            display = string.Format(threevalformat, "srl", srd, srt, shamt);
                            break;
                        case 0x3://shift right arithmetic immediate
                            cmd = "sra";
                            display = string.Format(threevalformat, "sra", srd, srt, shamt);
                            break;
                        case 0x4://shift left logical
                            cmd = "sllv";
                            display = string.Format(threevalformat, "sllv", srd, srt, srs);
                            break;
                        case 0x6://shift right logical
                            cmd = "srlv";
                            display = string.Format(threevalformat, "srlv", srd, srt, srs);
                            break;
                        case 0x7://shift right arithmetic
                            cmd = "srav";
                            display = string.Format(threevalformat, "srav", srd, srt, srs);
                            break;
                        case 0x8://jump register
                            cmd = "jr";
                            display = string.Format(onevalformat, "jr", srs);
                            break;
                        case 0x9://jump and link register
                            cmd = "jalr";
                            display = string.Format(twovalformat, "jalr", srs, srd);
                            break;
                        default:
                            cmd = "???";
                            display = "unknown R type funct: " + funct.ToString("x2");
                            break;
                    }
                }
                else if (opcode == 0x2 || opcode == 0x3)//J type instruction
                {
                    type = InstructionType.Jtype;
                    immediateu = (uint)ValAtOffset(instruction, 0x3ffffff, 0);
                    referencedAddress = immediateu << 2;
                    switch (opcode)
                    {
                        case 0x2:
                            cmd = "j";
                            display = string.Format(onevalformat, "j", "0x" + referencedAddress.ToString("x8"));
                            break;
                        case 0x3:
                            cmd = "jal";
                            display = string.Format(onevalformat, "jal", "0x" + referencedAddress.ToString("x8"));
                            break;
                    }
                }
                else//I type instruction
                {
                    type = InstructionType.Itype;
                    rs = ValAtOffset(instruction, 0x1f, 6 + 5 * 3);
                    string srs = GetRegister(rs);
                    rt = ValAtOffset(instruction, 0x1f, 6 + 5 * 2);
                    string srt = GetRegister(rt);
                    immediate = (short)SignedValAtOffset(instruction, 0xffff, 0);
                    string simmediate = "0x" + ((short)immediate).ToString("x4");
                    immediateu = (ushort)ValAtOffset(instruction, 0xffff, 0);
                    string simmediateu = "0x" + ((ushort)immediateu).ToString("x4");

                    switch (opcode)
                    {
                        case 0x8://add immediate
                            cmd = "addi";
                            display =  string.Format(threevalformat, "addi", srt, srs, simmediate);
                            break;
                        case 0x9://add immediate unsigned
                            cmd = "addiu";
                            display = string.Format(threevalformat, "addiu", srt, srs, simmediateu);
                            break;
                        case 0x23://load word
                            cmd = "lw";
                            display = string.Format(threevalmemoryformat, "lw", srt, srs, simmediate);
                            break;
                        case 0x21://load halfword
                            cmd = "lh";
                            display = string.Format(threevalmemoryformat, "lh", srt, srs, simmediate);
                            break;
                        case 0x25://load halfword unsigned
                            cmd = "lhu";
                            display = string.Format(threevalmemoryformat, "lhu", srt, srs, simmediate);
                            break;
                        case 0x20://load byte
                            cmd = "lb";
                            display = string.Format(threevalmemoryformat, "lb", srt, srs, simmediate);
                            break;
                        case 0x24://load byte unsigned
                            cmd = "lbu";
                            display = string.Format(threevalmemoryformat, "lbu", srt, srs, simmediate);
                            break;
                        case 0x2b://store word
                            cmd = "sw";
                            display = string.Format(threevalmemoryformat, "sw", srt, srs, simmediate);
                            break;
                        case 0x29://store halfword
                            cmd = "sh";
                            display = string.Format(threevalmemoryformat, "sh", srt, srs, simmediate);
                            break;
                        case 0x28://store byte
                            cmd = "sb";
                            display = string.Format(threevalmemoryformat, "sb", srt, srs, simmediate);
                            break;
                        case 0xf://load upper immediate
                            cmd = "lui";
                            display = string.Format(twovalformat, "lui", srt, simmediate);
                            break;
                        case 0xc://and immediate
                            cmd = "andi";
                            display = string.Format(threevalformat, "andi", srt, srs, simmediate);
                            break;
                        case 0xd://or immediate
                            cmd = "ori";
                            display = string.Format(threevalformat, "ori", srt, srs, simmediate);
                            break;
                        case 0xa://set on less than immediate
                            cmd = "slti";
                            display = string.Format(threevalformat, "slti", srt, srs, simmediate);
                            break;
                        case 0xb://set on less than immediate unsigned
                            cmd = "sltiu";
                            display = string.Format(threevalformat, "sltiu", srt, srs, simmediateu);
                            break;
                        case 0x4://branch on equal
                            cmd = "beq";
                            referencedAddress = (uint)(address + 4 + (immediate << 2));
                            display = string.Format(threevalformat, "beq", srs, srt, "0x" + referencedAddress.ToString("x8"));
                            break;
                        case 0x1://branch on greater than or equal to zero
                            if (rt == 0x11)
                                cmd = "bgezal";
                            else if (rt == 0x1)
                                cmd = "bgez";
                            else if (rt == 0)
                                cmd = "bltz";
                            else if (rt == 0x10)
                                cmd = "bltzal";
                            referencedAddress = (uint)(address + 4 + (immediate << 2));
                            display = string.Format(twovalformat, cmd, srs, "0x" + referencedAddress.ToString("x8"));
                            break;
                        case 0x7://branch on greater than zero
                            cmd = "bgtz";
                            referencedAddress = (uint)(address + 4 + (immediate << 2));
                            display = string.Format(twovalformat, cmd, srs, "0x" + referencedAddress.ToString("x8"));
                            break;
                        case 0x6://branch on less than or equal to zero
                            cmd = "blez";
                            referencedAddress = (uint)(address + 4 + (immediate << 2));
                            display = string.Format(twovalformat, "blez", srs, "0x" + referencedAddress.ToString("x8"));
                            break;
                        case 0x5://branch on not equal
                            cmd = "bne";
                            referencedAddress = (uint)(address + 4 + (immediate << 2));
                            display = string.Format(threevalformat, "bne", srs, srt, "0x" + referencedAddress.ToString("x8"));
                            break;
                        default:
                            cmd = "?";
                            display = "unknown I type opcode: " + opcode.ToString("x2");
                            break;

                    }
                }
            }
            public string cmd;
            public uint referencedAddress;
            public string display;

            public InstructionType type;
            public uint instruction;
            public int opcode;
            public int funct;
            public int rs;
            public int rt;
            public int rd;
            public int shamt;
            public uint address;
            public int immediate;
            public uint immediateu;

            public bool IsCall
            {
                get
                {
                    return new[] { "jal", "jalr"}.Contains(cmd);
                }
            }

            public bool IsBranch
            {
                get
                {
                    return new[] { "beq", "bgezal", "bgez", "bltz", "bltzal", "bgtz", "blez", "bne" }.Contains(cmd);
                }
            }

            public bool IsJump
            {
                get
                {
                    return cmd == "j";
                }
            }

            public bool IsReturn
            {
                get
                {
                    return cmd == "jr" && rs == 31;
                }
            }
            
        }

        public class BranchOperation
        {
            public BranchOperation(Instruction instruction, CodeBlock block)
            {
                this.Instruction = instruction;
                this.Block = block;
                if (instruction.cmd == "beq" || instruction.cmd == "bne")
                {
                    Comp1 = GetRegister(instruction.rs);
                    Comp2 = GetRegister(instruction.rt);
                    if (Comp1 == "r0")
                        Comp1 = "0";
                    if (Comp2 == "r0")
                        Comp2 = "0";
                }
                else
                {
                    Comp1 = GetRegister(instruction.rs);
                    Comp2 = "0";
                }
            }
            public Instruction Instruction;
            public CodeBlock Block;
            public string Comp1;
            public string Comp2;

            public string Print()
            {
                string text = "if";

                Instruction previnst = null;
                var prevdex = Block.Instructions.IndexOf(Instruction) - 1;
                if (prevdex >=  0)
                previnst = Block.Instructions[prevdex];
                //"beq", "bgezal", "bgez", "bltz", "bltzal", "bgtz", "blez", "bne"
                switch (Instruction.cmd)
                {
                    case "beq":
                        if (Comp2 == "0" && previnst != null && previnst.rd == Instruction.rs && previnst.cmd == "slt")
                        {
                            Comp1 = GetRegister(previnst.rs);
                            Comp2 = GetRegister(previnst.rt);
                            if (Comp1 == "r0")
                                Comp1 = "0";
                            if (Comp2 == "r0")
                                Comp2 = "0";
                            text += string.Format("({0} < {1})", Comp1, Comp2);
                        }
                        else
                        {
                            text += string.Format("({0} != {1})", Comp1, Comp2);
                        }
                        
                        break;
                    case "bne":
                        text += string.Format("({0} == {1})", Comp1, Comp2);
                        break;
                    case "bgez":
                        text += string.Format("({0} < {1})", Comp1, Comp2);
                        break;
                    case "bltz":
                        text += string.Format("({0} >= {1})", Comp1, Comp2);
                        break;
                    case "bgtz":
                        text += string.Format("({0} <= {1})", Comp1, Comp2);
                        break;
                    case "blez":
                        text += string.Format("({0} > {1})", Comp1, Comp2);
                        break;
                }

                return text;
            }
        }

        public class CodeBlock
        {
            public List<Instruction> Instructions = new List<Instruction>();

            public BlockType BlockType;
            public List<CodeBlock> InEdges = new List<CodeBlock>();
            public List<CodeBlock> OutEdges = new List<CodeBlock>();
            public List<uint> OutAddresses = new List<uint>();


            public uint Address
            {
                get
                {
                    return Instructions.FirstOrDefault().address;
                }
            }

            public uint EndAddress
            {
                get
                {
                    return Instructions.LastOrDefault().address + 4;
                }
            }

            public bool IsJumpTarget
            {
                get
                {
                    foreach (var edge in InEdges)
                    {
                        if (edge.EndAddress != Address)
                            return true;
                    }
                    return false;
                }
            }

            public bool BeginsLoop
            {
                get
                {
                    foreach (var edge in InEdges)
                    {
                        if (edge.Address >= Address)
                            return true;
                    }
                    return false;
                }
            }

            public bool EndsLoop
            {
                get
                {
                    foreach (var edge in OutEdges)
                    {
                        if (edge.Address <= Address)
                            return true;
                    }
                    return false;
                }
            }

            public BranchOperation BranchOperation
            {
                get
                {
                    if (BlockType == BlockType.TwoWay && Instructions[Instructions.Count - 2].IsBranch)
                    {
                        return new BranchOperation(Instructions[Instructions.Count - 2], this);
                    }
                    return null;
                }
            }

            
            
        }

        public enum BlockType
        {
            OneWay,
            TwoWay,
            NWay,
            Call,
            Return,
            FallThrough
        }
    }
}