using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsTools.Alundra
{
    public class UIHandler
    {
        GameState game;
        EtcStrings etcstrings;
        public short SavedBoxDrawerX, SavedBoxDrawerY;
        public int DialogChoiceUnknown1;//0x1072ec
        public int DialogChoiceUnknown2;//0x1072cc

        byte[] uiimagedata;
        byte[] fontimagedata;
        int fontimageoffset = 64;
        List<Color[]> uipalettes = new List<Color[]>();
        BitmapDrawCommand dialognametextcmd;

        public UIHandler(GameState gameState, EtcStrings etcstrings, string fontfile, string palettesfile, string uifile)
        {
            this.game = gameState;
            this.etcstrings = etcstrings;
            //load palettes
            byte[] buff = File.ReadAllBytes(fontfile);
            for (int pdex = 0; pdex + 32 < buff.Length; pdex += 32)
            {
                Color[] pal = new Color[16];
                for (int dex = 0; dex < pal.Length; dex++)
                {
                    int ddex = pdex + dex * 2;
                    pal[dex] = Utils.FromPsxColor((byte)(buff[ddex + 1] << 8), buff[ddex]);
                }
                uipalettes.Add(pal);
            }

            //load uitex
            uiimagedata = File.ReadAllBytes(uifile);

            //load font image
            fontimagedata = File.ReadAllBytes(fontfile);
            fontimageoffset = 64;//is the first one an rgba palette?
        }
        Dictionary<long, Bitmap> spriteCache = new Dictionary<long, Bitmap>();
        Bitmap GetUIBitmap(UIDrawCmd cmd)
        {
            var pal = uipalettes[cmd.uipaletteindex];
            if (spriteCache.ContainsKey(cmd.signature))
                return spriteCache[cmd.signature];
            var bmp = UIHelper.GenerateBitmap(cmd, pal, uiimagedata);
            spriteCache.Add(cmd.signature, bmp);

            return bmp;
        }
        public void Init()
        {
            foreach (var cmd in BoxDrawer3.boxcommands[0])
            {
                var bmp = GetUIBitmap(cmd);
            }
        }
        public UILerper DialogBoxLerper = new UILerper { };
        public UILerper DialogNameBoxLerper = new UILerper { };

        //TODO how should these be setup
        //
        UIBoxAnimated BoxDrawer1 = new UIBoxAnimated
        {
            x = 0x10,
            y = 0xa8,
            width = 0x24,
            height = 0x7,
            boxcommands = new UIDrawCmd[][] { new UIDrawCmd[] { } }
        };
        UIBoxAnimated BoxDrawer2 = new UIBoxAnimated
        {
            x = 0x10,
            y = 0xa8,
            width = 0x24,
            height = 0x7,
            boxcommands = new UIDrawCmd[][] { new UIDrawCmd[480 / 8 + 240 / 8], new UIDrawCmd[] { } }
        };
        //this one is the dialoboxname
        UIBoxAnimated BoxDrawer3 = new UIBoxAnimated
        {
            x = 0x40,
            y = 0x8c,
            width = 0xe,
            height = 0x4,
            boxcommands = new UIDrawCmd[][] { UIHelper.DialogNameBoxDrawCommands }
        };
        public List<UIRecord> records = new List<UIRecord>();
        public void RegisterUIRecord(UIBoxAnimated boxanimated, short x, short y, short width, short height, UIFunction setup, UIFunction render, int unknown)
        {
            var rec = new UIRecord { boxAnimated = boxanimated, x = x, y = y, width = width, height = height, SetupFunc = setup, RenderFunc = render, UnknownVal = unknown };
            records.Add(rec);
        }
        public UIHandler()
        {
            //0 dialog box
            RegisterUIRecord(BoxDrawer1, 0x10, 5, 0x20, 6, InitUI_1, RenderDialogBox, 0);
            //1 main ui
            RegisterUIRecord(null, 0, 0, 0x40, 4, InitUI_2, RenderMainUI, -1);
            //2
            RegisterUIRecord(BoxDrawer1, 8, 0xc, 0x20, 4, InitUI_1, Render2, 0);
            //3
            RegisterUIRecord(BoxDrawer2, 0x10, 8, 0x20, 4, null, Render3, 5);
            //4
            RegisterUIRecord(null, 0x10, 8, 0x20, 4, InitUI_3, Render4, -1);
            //5
            RegisterUIRecord(BoxDrawer1, 0x10, 0xc, 0x20, 4, InitUI_1, Render5, 0);
            //6 item menu
            RegisterUIRecord(null, 0x10, 8, 0x20, 4, null, RenderItemMenu, 0);
            //7
            RegisterUIRecord(BoxDrawer1, 0x10, 8, 0x20, 4, InitUI_1, null, 0);
            //8
            RegisterUIRecord(BoxDrawer1, 0x10, 0xc, 0x20, 4, InitUI_1, Render8, 0);
            //9
            RegisterUIRecord(BoxDrawer1, 0x10, 0xc, 0x20, 4, InitUI_1, Render9, 0);
            //a
            RegisterUIRecord(null, 0x10, 0xc, 0x20, 4, null, Rendera, 0);
            //b
            RegisterUIRecord(BoxDrawer1, 8, 0xc, 0x20, 4, InitUI_1, Renderb, 0);
            //c dialog name box
            RegisterUIRecord(BoxDrawer3, 0x10, 8, 0x20, 4, InitUI_4, RenderDialogNameBox, 5);
        }

        //0x491a4
        bool InitUI_1(UIRecord ui)
        {
            return true;
        }

        //0x4c998
        bool InitUI_2(UIRecord ui)
        {
            return true;
        }

        //0x550d4
        bool InitUI_3(UIRecord ui)
        {
            return true;
        }

        //0x5c300
        bool InitUI_4(UIRecord ui)
        {
            DialogNameBoxLerper.tickstolinger = 2;
            DialogNameBoxLerper.numticks = 0xf;
            DialogNameBoxLerper.currenttick = 0;
            DialogNameBoxLerper.x1 = 0x140;
            if (ui.boxAnimated.y < 0)
            {
                DialogNameBoxLerper.y1 = (short)(ui.boxAnimated.y - ui.boxAnimated.height * 8);
            }
            else
            {
                DialogNameBoxLerper.y1 = ui.boxAnimated.y;
            }

            if (ui.boxAnimated.x < 0)
            {
                DialogNameBoxLerper.x2 = (short)(ui.boxAnimated.x - ui.boxAnimated.width * 8);
            }
            else
            {
                DialogNameBoxLerper.x2 = ui.boxAnimated.x;
            }

            if (ui.boxAnimated.y < 0)
            {
                DialogNameBoxLerper.y2 = (short)(ui.boxAnimated.y - ui.boxAnimated.height * 8);
            }
            else
            {
                DialogNameBoxLerper.y2 = ui.boxAnimated.y;
            }

            DialogNameBoxLerper.AfterX = ui.boxAnimated.x;
            DialogNameBoxLerper.AfterY = ui.boxAnimated.y;
            game.DialogNameState = 5;

            var name = etcstrings.GetEtcString(game.DialogName);

            dialognametextcmd = RenderText(name, 0, (short)(ui.y + ui.boxAnimated.y), 3);

            return true;
        }

        //0x47de4 TODO: impliment missing functions
        bool RenderDialogBox(UIRecord ui)
        {
            //setdrawarea
            //it sets the clipping for the dialog box


            if ((game.DialogState & 0x3) != 0)
            {
                var finished = LerpUIBox(ui.boxAnimated, DialogBoxLerper);
                if (finished)
                {
                    if ((game.DialogState & 1) != 0)
                        game.DialogState &= ~1;//turn off bit 1 if its on
                    if ((game.DialogState & 2) != 0)
                    {
                        //dialog is finished?
                        BoxDrawer1.x = SavedBoxDrawerX;
                        BoxDrawer1.y = SavedBoxDrawerY;
                        ZeroDialogState(ui);
                        return false;
                    }
                }
            }
            else
            {
                if (DialogChoiceUnknown1 != 0)
                {
                    //46d18()
                }
                else if (DialogChoiceUnknown2 != 0)
                {
                    //46890(ui);
                    return true;
                }
                else
                {
                    //46ed8()
                }
            }

            //464e0(ui)

            return true;
        }

        //0x4d218
        bool RenderMainUI(UIRecord ui)
        {
            return true;
        }
        bool Render2(UIRecord ui)
        {
            return true;
        }
        bool Render3(UIRecord ui)
        {
            return true;
        }
        bool Render4(UIRecord ui)
        {
            return true;
        }
        bool Render5(UIRecord ui)
        {
            return true;
        }
        //0x5695c
        bool RenderItemMenu(UIRecord ui)
        {
            return true;
        }
        bool Render8(UIRecord ui)
        {
            return true;
        }
        bool Render9(UIRecord ui)
        {
            return true;
        }
        bool Rendera(UIRecord ui)
        {
            return true;
        }
        bool Renderb(UIRecord ui)
        {
            return true;
        }
        //0x5c4ac
        bool RenderDialogNameBox(UIRecord ui)
        {
            if ((game.DialogNameState & 3) != 0)
            {
                var finished = LerpUIBox(ui.boxAnimated, DialogNameBoxLerper);
                if (finished)
                {
                    if ((game.DialogNameState & 1) != 0)
                        game.DialogNameState &= ~1;//turn off bit 1 if its on
                    if ((game.DialogNameState & 2) != 0)
                    {
                        //dialog is finished?
                        ui.boxAnimated.x = SavedBoxDrawerX;
                        ui.boxAnimated.y = SavedBoxDrawerY;
                        ZeroDialogNameState(ui);
                        return false;
                    }
                }
            }

            string text = etcstrings.GetEtcString(game.DialogName);
            int width = GetRenderedTextWidth(text);
            width = ui.boxAnimated.width * 8 - width;
            dialognametextcmd.x = (short)(width / 2 + ui.boxAnimated.x);
            dialognametextcmd.y = (short)(ui.y + ui.boxAnimated.y);

            //not sure whats being done down here

            return true;

        }


        int drawAreaId = 0;

        public bool LerpUIBox(UIBoxAnimated boxanim, UILerper lerper)
        {
            if (lerper.tickstolinger == 0)
                return true;//complete

            if (lerper.currenttick != lerper.numticks)
            {
                boxanim.x = (short)((lerper.x2 - lerper.x1) / (float)lerper.numticks * lerper.currenttick);
                boxanim.y = (short)((lerper.y2 - lerper.y1) / (float)lerper.numticks * lerper.currenttick);
                lerper.currenttick++;
            }
            else
            {
                boxanim.x = lerper.x2;
                boxanim.y = lerper.y2;
                lerper.tickstolinger--;
            }
            //commands for the 8x8 portions of the dialog box,  but how are the corners and edges drawn?  must be code elsewhere to set that up, this just sets the positions
            UIDrawCmd[] cmds = boxanim.boxcommands[this.drawAreaId];
            int dex = 0;
            short y = boxanim.y;
            while (y < boxanim.height * 8 + boxanim.y)
            {
                short x = boxanim.x;
                while (x < boxanim.width * 8 + boxanim.x)
                {
                    cmds[dex].x = x;
                    cmds[dex].y = y;
                    x += 8;
                }

                y += 8;
            }

            return false;
        }

        public void ZeroDialogState(UIRecord ui)
        {
            ZeroDialogRecord(ui);
            game.DialogState = 0;
            game.PlayerControlSetting &= 0xffe7;//turn off bits 4 and 5
        }
        public void ZeroDialogNameState(UIRecord ui)
        {
            ZeroDialogRecord(ui);
            game.DialogNameState = 0;
        }
        public bool ZeroDialogRecord(UIRecord ui)
        {
            ui.Status = 0;
            return true;
        }

        public class BitmapDrawCommand
        {
            public Bitmap bmp;
            public short x;
            public short y;
        }
        public BitmapDrawCommand RenderText(string text, short x, short y,int linenum)
        {
            byte[] buff = new byte[0x800];
            if (linenum >= 4)
                linenum++;
            if (linenum > 0xe)
                throw new Exception("set mess over");
            string tempstr = text;
            
            RenderTextInner(tempstr, buff, 0x3c0, linenum * 16 + 0x120, 0, 0, 0x100, 0x10);
            var cmd = new UIDrawCmd { x = x, y = y, u = 0, v = 0, w = 255, h = 16, uipaletteindex = 8 };
            var bdc = new BitmapDrawCommand { x = x, y = y };
            bdc.bmp = UIHelper.GenerateBitmap(cmd, uipalettes[8], buff);
            return bdc;
        }

        public void RenderTextInner(string linetext, byte[]outputbitmap,int vramx, int vramy, int startx, int starty, int outputbitmapwidth, int outputbitmapheight)
        {
            int x = startx;
            int dex = 0;
            char chr = linetext[dex];
            while(chr != 0)
            {
                var info = UIHelper.FontCharInfos[(int)chr];
                int fontstartdex = fontimageoffset + (info.sx / 2) + (info.sy * 128);

                for (int y = 0; y < info.height; y++)
                {
                    int charx = x;
                    int lineoffset = y * 128;
                    for (int xdex = 0; xdex < info.width; xdex++)
                    {
                        if (charx >= 0x100)
                            break;
                        int outputpos = ((starty + info.y) * outputbitmapwidth / 2) + (charx / 2);
                        
                        byte existingnib, sourcenib;

                        if ((charx & 1) != 0)
                        {
                            existingnib = (byte)(outputbitmap[outputpos] & 0xf);
                            if ((xdex & 1) != 0)
                            {
                                sourcenib = fontimagedata[fontstartdex + lineoffset + xdex / 2];
                                sourcenib = (byte)(sourcenib & 0xf0);
                            }
                            else
                            {
                                sourcenib = fontimagedata[fontstartdex + lineoffset + xdex / 2];
                                sourcenib = (byte)((sourcenib & 0xf) << 4);
                            }
                        }
                        else
                        {
                            existingnib = (byte)(outputbitmap[outputpos] & 0xf0);
                            if ((xdex & 1) != 0)
                            {
                                sourcenib = fontimagedata[fontstartdex + lineoffset + xdex / 2];
                                sourcenib = (byte)(sourcenib >> 4);
                            }
                            else
                            {
                                sourcenib = fontimagedata[fontstartdex + lineoffset + xdex / 2];
                                sourcenib = (byte)(sourcenib & 0xf);
                            }
                        }


                        outputbitmap[outputpos] = (byte)(existingnib | sourcenib);
                        charx++;

                    }
                }
                
                x += info.width + 1;
                dex++;
                chr = linetext[dex];
            }
        }

        int GetRenderedTextWidth(string text)
        {
            int width = 0;
            int dex = 0;
            char c = text[dex];
            while(c!=0)
            {
                if(c == '\\')
                {
                    dex++;
                    c = text[dex];
                    switch (c)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            dex++;
                            while (c <= '9')
                            {
                                c = text[dex];
                                dex++;
                            }
                            dex--;
                            break;
                        case 'W':
                            int lookup = text[dex];
                            if (lookup < 'A')
                                lookup -= 0x20;
                            else
                                lookup -= 0x27;
                            dex++;
                            width += UIHelper.FontCharInfos[lookup].width + 1;
                            break;
                        case 'X':
                            dex += 2;
                            break;
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                        case 'G':
                        case 'T':
                        case 'Y':
                            dex++;
                            break;
                        case 'N':
                            return width;
                    }
                }
                else
                {
                    width += UIHelper.FontCharInfos[c].width + 1;
                    dex++;
                }

                c = text[dex];
            }

            return width;
        }

        
    }
}
