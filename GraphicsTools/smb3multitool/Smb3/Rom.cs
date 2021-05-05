using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Smb3
{
    public class Rom
    {
        public const int VRAM_ADDR = 0x40010;
        public const int VRAM_SIZE = 0x20000;
        public List<RomBank> banks;
        public Rom(string romfile)
        {
            var br = new BinaryReader(File.OpenRead(romfile));

            //load bitmaps
            br.BaseStream.Position = VRAM_ADDR;
            banks = new List<RomBank>();
            for (int dex = 0; dex < VRAM_SIZE / 1024 * 4; dex++)
            {
                var bank = new RomBank(br);
                banks.Add(bank);
            }
            
            

            br.Close();
        }
    }

    public class RomBank
    {
        public RomBank(BinaryReader br)
        {
            rombitmaps  = new List<RomBitmap>();
            for(int dex = 0;dex<256;dex++)
            {
                var rbmp = new RomBitmap(br);
                rombitmaps.Add(rbmp);
            }

            
        }

        public Bitmap GenerateBitmap(Color[] pal, int tilewidth, int tileheight)
        {
            var rb = new RomBitmap(8 * tilewidth, 8 * tileheight);
            for (int y = 0; y < tileheight; y++)
            {
                for (int x = 0; x <tilewidth; x++)
                {
                    var srb = rombitmaps[y * tilewidth + x];
                    for (int sy = 0; sy < 8; sy++)
                    {
                        int dex = (y * 8 + sy) * rb.rowsize + x * srb.rowsize;
                        int sdex = sy * srb.rowsize;
                        rb.bitmapdata[dex] = srb.bitmapdata[sdex];
                        rb.bitmapdata[dex + 1] = srb.bitmapdata[sdex + 1];
                    }
                }
            }
            return GraphicsTools.Utils.BitmapFromNesBuff(rb.bitmapdata, rb.width, rb.height, pal);
        }

        public List<RomBitmap> rombitmaps;
    }
    public class RomBitmap
    {
        public RomBitmap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.rowsize = width / 4;
            bitmapdata = new byte[rowsize * height];
        }

        public RomBitmap(BinaryReader br)
        {
            width = 8;
            height = 8;
            rowsize = width / 4;
            bitmapdata = new byte[rowsize*height];
            var buff1 = new byte[8];
            var buff2 = new byte[8];
            br.Read(buff1, 0,buff1.Length);
            br.Read(buff2, 0,buff2.Length);
            for (int y = 0; y < 8;y++)
            {
                int s = 0;
                for (int x = 0;x<8;x++)
                {
                    s |= ((buff1[y] >> x) & 1) << (x*2) + 0;
                    s |= ((buff2[y] >> x) & 1) << (x*2) + 1;
                }
                bitmapdata[y * rowsize + 0] = (byte)(s & 0xff);
                bitmapdata[y * rowsize + 1] = (byte)((s & 0xff00) >> 8);
            }
        }
        public int width;
        public int height;
        public int rowsize;
        public byte[] bitmapdata;
        public Bitmap GenerateBitmap(Color[] pal)
        {
            return GraphicsTools.Utils.BitmapFromNesBuff(bitmapdata,width,height,pal);
        }
    }
}
