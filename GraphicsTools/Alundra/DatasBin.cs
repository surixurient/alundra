using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace GraphicsTools.Alundra
{
    public class DatasBin
    {
        public DBHeader header;
        public GameMap[] gamemaps;
        public GameMap alundragamemap;
        public string binfile;
        public DatasBin(string binfile)
        {
            this.binfile = binfile;
            using (var br = new BinaryReader(File.OpenRead(binfile)))
            {
                header = new DBHeader(br);

                alundragamemap = new GameMap(br,header);

                
#if DEBUG       //verify maps
                for (int dex = 0; dex < header.gamemaps.Length; dex++)
                {
                    if (header.gamemaps[dex] > 0)
                    {
                        br.BaseStream.Position = header.gamemaps[dex];
                        if (br.BaseStream.Position != br.BaseStream.Length)
                        {
                            int check = br.ReadInt32();
                            Debug.Assert(check == 28);
                        }
                    }
                }
#endif

                gamemaps = new GameMap[header.gamemaps.Length];
                for (int dex = 0; dex < header.gamemaps.Length; dex++)
                {
                    if (header.gamemaps[dex] > 0 && header.gamemaps[dex] < br.BaseStream.Length)
                    {
                        gamemaps[dex] = new GameMap(br, header.gamemaps[dex]);
                    }
                }


            }
        }

        public BinaryReader OpenBin()
        {
            return new BinaryReader(File.OpenRead(binfile));
        }

    }

    public class GameMap
    {

        public GameMap(BinaryReader br, DBHeader dbheader)
        {//the alundra gamemap (just has sprites)
            
            br.BaseStream.Position = binoffset = 0;
            header = new GameMapHeader(dbheader);
        }
        public GameMap(BinaryReader br, long offset)
        {
            //read header
            br.BaseStream.Position = binoffset = offset;
            header = new GameMapHeader(br);
            
            //just read mapid
            br.BaseStream.Position = binoffset + header.infoblock;
            info = new GameMapInfo(br.ReadInt32(), memaddr + header.infoblock);

        }
        public long binoffset;
        public static int memaddr = 0x153460;// + 0x260;
        public static int eventobjects_memaddr = 0x1ac498;// + 0x260;
        public static int eventobject_size = 0x294;
        public static int EventObjectAddr(int eventobjectid)
        {
            return eventobjects_memaddr + eventobjectid * eventobject_size;
        }
        public GameMapHeader header;
        public GameMapInfo info;
        public SpriteInfo spriteinfo;
        public ScrollScreen scrollscreen;
        public Map map;
        public string[] strings;
        public bool loaded = false;
        byte[] tilesheetimagedata;
        public Bitmap tilesheetbmp;
        byte[] spritesheetimagedata;
        public Bitmap spritesheetbmp;
        int numspritesheets = 8;
        public void Load(BinaryReader br)
        {
            //read info
            if (header.infoblock != -1)
            {
                br.BaseStream.Position = binoffset + header.infoblock;
                info = new GameMapInfo(br, memaddr + header.infoblock);
            }

            //map
            if (header.mapblock != -1)
            {
                br.BaseStream.Position = binoffset + header.mapblock;
                map = new Map(br, memaddr + header.mapblock);
                header.walltilessize = header.tilesheets - (map.walltilesoffset + header.mapblock);
                header.mapsize -= header.walltilessize;
            }

            //tilesheet
            if (header.tilesheets != -1)
            {
                br.BaseStream.Position = binoffset + header.tilesheets + 6;
                byte[] buff = new byte[header.spriteinfo - header.tilesheets];
                br.Read(buff, 0, buff.Length);
                tilesheetimagedata = new byte[256 * 256 * 6 / 2];//6 256x256 4bpp bitmaps
                Utils.Deflate(buff, tilesheetimagedata);
            }

            //spriteinfo
            if (header.spriteinfo != -1)
            {
                br.BaseStream.Position = binoffset + header.spriteinfo;
                spriteinfo = new SpriteInfo(br, memaddr + header.spriteinfo, header.spritesheets);
            }

            //spritesheet
            if (header.spritesheets != -1)
            {
                br.BaseStream.Position = binoffset + header.spritesheets + 6;
                byte[] buff = new byte[header.spritessize - 6];
                br.Read(buff, 0, buff.Length);
                spritesheetimagedata = new byte[256 * 256 * numspritesheets / 2];//numspritesheets 256x256 4bpp bitmaps
                Utils.Deflate(buff, spritesheetimagedata);
            }

            //scrollscreen
            if (header.scrollscreen != -1)
            {
                scrollscreen = new ScrollScreen(br);
            }

            //read string table
            if (header.stringtable != -1)
            {
                br.BaseStream.Position = binoffset + header.stringtable;
                strings = new string[128];
                short[] stringoffsets = new short[128];
                for (int dex = 0; dex < 128; dex++)
                {
                    stringoffsets[dex] = br.ReadInt16();
                }
                for (int dex = 0; dex < 128; dex++)
                {
                    if (stringoffsets[dex] != -1)
                    {
                        strings[dex] = "";
                        br.BaseStream.Position = binoffset + header.stringtable + stringoffsets[dex];
                        char c = br.ReadChar();
                        while (c != '\0')
                        {
                            strings[dex] += c;
                            c = br.ReadChar();
                        }
                    }
                }
                header.stringsize = (int)(br.BaseStream.Position - binoffset) - header.stringtable;
            }
            //loaded = true;
        }

        public Bitmap GenerateSpriteBitmap(SIImage img, Color[] pal)
        {
            bool shiftleft = img.sx % 2 == 1;
            int swidth = img.swidth;
            int readwidth = swidth;
            int outputwidth = img.swidth;
            if (outputwidth % 8 > 0)//make output interval of 8
                outputwidth += 8 - outputwidth % 8;
            if (shiftleft)//make sure theres an extra byte if shifting left
                readwidth++;
            if (readwidth % 2 == 1)// or if odd width
                readwidth++;



            byte[] buff = new byte[outputwidth * img.sheight / 2];
            byte[] readbuff = new byte[readwidth / 2];

            for (int y = 0; y < img.sheight; y++)
            {
                Buffer.BlockCopy(spritesheetimagedata, ((img.spritesheet&0x7)*256 + img.sy + y) * 256 / 2 + img.sx / 2, readbuff, 0, readwidth / 2);

                if (shiftleft)
                {
                    int dex;
                    for (dex = 0; dex < readbuff.Length - 1; dex++)
                    {
                        buff[y * outputwidth / 2 + dex] = (byte)((readbuff[dex] & 0xf0) >> 4 | (readbuff[dex + 1] & 0x0f) << 4);
                    }

                }
                else
                {
                    Buffer.BlockCopy(readbuff, 0, buff, y * outputwidth / 2, readwidth / 2);
                }

                if (swidth % 2 == 1)
                {
                    buff[y * outputwidth / 2 + swidth / 2] = (byte)(buff[y * outputwidth / 2 + swidth / 2] & 0x0f);
                }

            }


            if (outputwidth > swidth)
            {
                img.swidth = (byte)outputwidth;

                float vec1x = (img.x2 - img.x1) / (float)swidth;//get the normalized (normalized to ratio of swidth/outputwidth) vector of point 1
                float vec1y = (img.y2 - img.y1) / (float)swidth;

                float vec3x = (img.x4 - img.x3) / (float)swidth;//get normalized vector of point 3
                float vec3y = (img.y4 - img.y3) / (float)swidth;

                img.x2 = (sbyte)(img.x1 + (vec1x * outputwidth));//extend point 2 to new width
                img.y2 = (sbyte)(img.y1 + (vec1y * outputwidth));

                img.x4 = (sbyte)(img.x3 + (vec3x * outputwidth));//extend point 4 to new width
                img.y4 = (sbyte)(img.y3 + (vec3y * outputwidth));
            }

            return Utils.BitmapFromPsxBuff(buff, outputwidth, img.sheight, 4, pal);
        }

        public Bitmap GenerateTileBitmap(int tile,Color[]pal)
        {
            Debug.Assert(tile < 10 * 16 * 6, "Bad tile index!", "unexpectedly large tile index of {0}", tile);
            byte[] tilebuff = new byte[24 * 16 * 4 / 8];
            int tilex = tile%10 * 24;
            int tiley = tile/10 * 16;
            if (tile < 10 * 16 * 6)
            {
                for (int y = 0; y < 16; y++)
                    Buffer.BlockCopy(tilesheetimagedata, (tiley + y) * 256 / 2 + tilex / 2, tilebuff, y * 24 / 2, 24 / 2);
            }
            return Utils.BitmapFromPsxBuff(tilebuff, 24, 16, 4, pal);
        }
        public Bitmap GenerateTileSheetBmp(Color[] pal)
        {
            tilesheetbmp = Utils.BitmapFromPsxBuff(tilesheetimagedata, 256, 256 * 6, 4, pal);
            return tilesheetbmp;
        }

        public Bitmap GenerateSpriteSheetBmp(Color[] pal)
        {
            spritesheetbmp = Utils.BitmapFromPsxBuff(spritesheetimagedata, 256, 256 * numspritesheets, 4, pal);
            return spritesheetbmp;
        }
    }

    public class Map
    {
        public Map(BinaryReader br, int memaddr)
        {
            this.memaddr = memaddr;
            long binoffset = br.BaseStream.Position;

            width = br.ReadByte();
            height = br.ReadByte();
            width2 = br.ReadByte();
            height2 = br.ReadByte();

            br.BaseStream.Position = binoffset + 1540;//why this number?

            maptiles = new MapTile[width * height];
            for (int dex = 0;dex<maptiles.Length;dex++)
            {
                maptiles[dex] = new MapTile(br);
            }

            walltilesoffset = (int)(br.BaseStream.Position - binoffset);

            //load wall tiles
            for (int dex = 0; dex < maptiles.Length; dex++)
            {
                maptiles[dex].LoadWallTiles(br, binoffset + walltilesoffset);
            }
        }
        public int memaddr;

        public int width;
        public int height;
        public int width2;
        public int height2;

        public int walltilesoffset;

        public MapTile[] maptiles;
        
    }

    public class MapTile
    {
        public MapTile(BinaryReader br)
        {
            Int64 i = br.ReadUInt32();
            walkability = (byte)(i & 0xff);
            i >>= 8;
            groundproperty = (byte)(i & 0xff);
            i >>= 8;
            slope = (byte)(i & 0xff);
            i >>= 8;
            height = (byte)(i & 0xff);

            i = br.ReadUInt16();
            tileid = (short)i;
            if (i == 0xffff)
            {
                palette = -1;
                tile = -1;
            }
            else
            {
                palette = (short)((i & 0xf000) >> 12);
                tile = (short)(i & 0x3ff);
            }
            tilesoffset = br.ReadInt16();
            if (tilesoffset != -1)
                tilesoffset *= 2;

        }
        public byte walkability;
        public byte groundproperty;
        public byte slope;
        public byte height;
        public short tileid;
        public short palette;
        public short tile;
        public short tilesoffset;
        public WallTiles walltiles;
        public void LoadWallTiles(BinaryReader br, long offset)
        {
            if (tilesoffset != -1)
            {
                br.BaseStream.Position = offset + tilesoffset;
                walltiles = new WallTiles(br);
            }
        }
    }

    public class WallTiles
    {
        public WallTiles(BinaryReader br)
        {
            offset = br.ReadSByte();
            count = br.ReadByte();
            tiles = new short[count];
            //if ((flag != 0 && flag != 255) || count==0 || count == 255)
            //{
            //    flag = flag;
            //}
            
            for (int dex =0;dex<count;dex++)
            {
                tiles[dex] = br.ReadInt16();
            }
        }
        public sbyte offset;
        public byte count;
        public short[] tiles;
    }

    public class ScrollScreen
    {
        public ScrollScreen(BinaryReader br)
        {
            unknown1 = br.ReadInt32();
            unknown2 = br.ReadInt32();
            unknown3 = br.ReadInt32();
            unknown4 = br.ReadInt32();
            unknown5 = br.ReadInt32();
            unknown6 = br.ReadInt32();
            unknown7 = br.ReadInt32();
            unknown8 = br.ReadInt32();
        }
        public int unknown1;
        public int unknown2;
        public int unknown3;
        public int unknown4;
        public int unknown5;
        public int unknown6;
        public int unknown7;
        public int unknown8;
    }

    public class SpriteInfo
    {
        public SpriteInfo(BinaryReader br, int memaddr, int sectorend)
        {
            binoffset = br.BaseStream.Position;
            
            header = new SpriteInfoHeader(br, memaddr);
            
            //read sprite table
            br.BaseStream.Position = binoffset + header.spritetablepointer;
            spritetable = new int[0xff];
            for (int dex = 0; dex < spritetable.Length; dex++)
            {
                spritetable[dex] = br.ReadInt32();
            }

            //read palettes
            br.BaseStream.Position = binoffset + header.spritepalettespointer;
            int maxpalettes = 32;
            palettes = new System.Drawing.Color[maxpalettes][];
            byte[] buff = new byte[maxpalettes * 16 * 2];
            br.Read(buff, 0, buff.Length);
            int buffdex = 0;
            for (int dex = 0; dex < maxpalettes; dex++)
            {
                palettes[dex] = new System.Drawing.Color[16];
                for (int cdex = 0; cdex < 16; cdex++)
                {
                    byte b2 = buff[buffdex++];
                    byte b1 = buff[buffdex++];
                    palettes[dex][cdex] = Utils.FromPsxColor((b1 << 8) | b2);
                }
            }
            palettesbitmap = Utils.BitmapFromPsxBuff(buff, 16, maxpalettes, 16, null);

            //read eventcodes
            eventcodes = new SpriteInfoEventCodes(br, binoffset, header);

            //read entities
            br.BaseStream.Position = binoffset + header.entitiespointer;
            entities = new SpriteInfoEntities(br, memaddr + header.entitiespointer);

            //read mapevents
            br.BaseStream.Position = binoffset + header.mapeventspointer;
            mapevents = new SpriteInfoMapEvents(br, binoffset, sectorend);

            //read sprite table records;
            sprites = new SpriteRecord[spritetable.Length];
            for (int dex = 0; dex < sprites.Length; dex++)
            {
                if (spritetable[dex] != -1)
                {
                    br.BaseStream.Position = binoffset + spritetable[dex];
                    sprites[dex] = new SpriteRecord(br, binoffset, dex, memaddr + spritetable[dex], memaddr);
                }
            }
        }

        public SpriteInfoHeader header;
        public SpriteInfoEventCodes eventcodes;
        public SpriteInfoEntities entities;
        public SpriteInfoMapEvents mapevents;

        public int[] spritetable;
        public SpriteRecord[] sprites;

        long binoffset;

        public System.Drawing.Color[][] palettes;
        public Bitmap palettesbitmap;
    }

    public class SpriteRecord
    {
        public SpriteRecord(BinaryReader br, long binoffset, int id, int memaddr, int spriteinfomemaddr)
        {
            
            header = new SpriteTableHeader(br, binoffset, id, memaddr, spriteinfomemaddr);
            animsets = new SIAnimSet[(header.animationspointer - header.animationoffsetspointer) / 14];
            for (int dex = 0; dex < animsets.Length; dex++)
            {
                animsets[dex] = new SIAnimSet(br, memaddr + 32 + dex * 14);
            }
        }

        public SIAnimation GetAnimation(BinaryReader br, int animationoffset)
        {
            br.BaseStream.Position = header.binoffset + header.animationspointer + animationoffset;

            var anim = new SIAnimation(br, header, header.spriteinfomemaddr + header.animationspointer + animationoffset);

            return anim;
        }

        
        public SpriteTableHeader header;
        public SIAnimSet[] animsets;

    }

    public class SIAnimSet
    {
        public SIAnimSet(BinaryReader br, int memaddr)
        {
            this.memaddr = memaddr;
            animoffsets = new int[4];
            for (int dex = 0; dex < animoffsets.Length; dex++)
                animoffsets[dex] = br.ReadInt16();
            speed = br.ReadUInt16();
            u3 = br.ReadByte();
            u4 = br.ReadByte();
            u5 = br.ReadByte();
            u6 = br.ReadByte();
        }
        public int memaddr;
        public int[] animoffsets;//4 of them for each direction
        public ushort speed;
        public byte u3;
        public byte u4;
        public byte u5;
        public byte u6;

        public int downoffset { get { return animoffsets[(int)SIAnimDir.down]; } }
        public int upoffset { get { return animoffsets[(int)SIAnimDir.up]; } }
        public int leftoffset { get { return animoffsets[(int)SIAnimDir.left]; } }
        public int rightoffset { get { return animoffsets[(int)SIAnimDir.right]; } }
    }
    public enum SIAnimDir
    {
        down = 0, 
        up = 1, 
        left = 2, 
        right = 3
    }

    public class SpriteTableHeader
    {
        public SpriteTableHeader(BinaryReader br, long binoffset, int id, int memaddr, int spriteinfomemaddr)
        {
            this.spriteinfomemaddr = spriteinfomemaddr;
            this.memaddr = memaddr;
            this.sector5id = id;
            this.binoffset = binoffset;
            animationoffsetspointer = br.ReadInt32();
            animationspointer = br.ReadInt32();
            unknownpointer = br.ReadInt32();
            framespointer = br.ReadInt32();
            ubuff = new byte[16];
            br.Read(ubuff, 0, ubuff.Length);

            br.BaseStream.Position -= 16;
            u1 = br.ReadByte();
            canpickup = br.ReadByte();
            shadowtype = br.ReadByte();
            u4 = br.ReadByte();
            throwtype = br.ReadByte();
            u6 = br.ReadByte();
            breaksound = br.ReadByte();
            u8 = br.ReadByte();
            u9 = br.ReadByte();
            u10 = br.ReadByte();
            u11 = br.ReadByte();
            u12 = br.ReadByte();
            u13 = br.ReadByte();
            stackable = br.ReadByte();
            breakanim = br.ReadByte();
            contents = br.ReadByte();
        }
        public int sector5id;
        public long binoffset;
        public int memaddr;
        public int spriteinfomemaddr;

        public int animationoffsetspointer;
        public int animationspointer;
        public int unknownpointer;
        public int framespointer;
        public byte[] ubuff;

        public byte u1;
        public byte canpickup;
        public byte shadowtype;
        public byte u4;
        public byte throwtype;
        public byte u6;
        public byte breaksound;
        public byte u8;
        public byte u9;
        public byte u10;
        public byte u11;
        public byte u12;
        public byte u13;
        public byte stackable;
        public byte breakanim;
        public byte contents;
    }


    public class SIAnimation
    {
        public SIAnimation(BinaryReader br, SpriteTableHeader header, int memaddr)
        {
            this.memaddr = memaddr;
            frames = new SIFrame[32];//32 max frames?
            for (int dex = 0; dex < frames.Length; dex++)
            {
                //read two test bytes to check for the end of the list
                short test = br.ReadByte();
                if ((test & 0x80) != 0x80)
                    break;
                numframes++;
                br.BaseStream.Position -= 1;

                frames[dex] = new SIFrame(br, header, memaddr + dex * 5);

                for (int dex2 = 0; dex2 < dex; dex2++)
                {
                    if (frames[dex2].imagesetpointer == frames[dex].imagesetpointer)
                    {
                        frames[dex].images = frames[dex2].images;
                        break;
                    }
                }
            }
        }
        public int memaddr;
        public int numframes;
        public SIFrame[] frames;
    }

    public class SIFrame
    {
        public SIFrame(BinaryReader br, SpriteTableHeader header, int memaddr)
        {
            this.memaddr = memaddr;
            delay = br.ReadByte();

            unknown = br.ReadInt16();
            imagesetpointer = br.ReadUInt16() * 2;
            
            
            //load images
            long savepos = br.BaseStream.Position;

            br.BaseStream.Position = header.binoffset + header.framespointer + imagesetpointer;
            images = new SIImageSet(br, header.sector5id << 16 | imagesetpointer, header.spriteinfomemaddr + header.framespointer + imagesetpointer);

            br.BaseStream.Position = savepos;
        }


        public int memaddr;
        public byte delay;//top bit masked
        public short unknown;//-1
        public int imagesetpointer;
        public SIImageSet images;
    }

    public class SIImageSet
    {
        public SIImageSet(BinaryReader br, int imagesetid, int memaddr)
        {
            this.memaddr = memaddr;
            this.imagesetid = imagesetid;
            unknown = br.ReadByte();//palette?
            numimages = br.ReadByte();
            images = new SIImage[numimages];
            for (int dex = 0; dex < numimages; dex++)
            {
                images[dex] = new SIImage(br);
            }
        }

        public int memaddr;
        public int imagesetid;
        public byte unknown;
        public byte numimages;
        public SIImage[] images;
    }

    public class SIImage
    {
        public SIImage(BinaryReader br)
        {
            spritesheet = br.ReadByte();
            palette = br.ReadByte();
            sx = br.ReadByte();
            sy = br.ReadByte();
            swidth = br.ReadByte();
            sheight = br.ReadByte();
            x1 = br.ReadSByte();
            y1 = br.ReadSByte();
            x2 = br.ReadSByte();
            y2 = br.ReadSByte();
            x3 = br.ReadSByte();
            y3 = br.ReadSByte();
            x4 = br.ReadSByte();
            y4 = br.ReadSByte();

            /*if (rejigger)
            {//byte align
                if (sx % 2 == 1)
                {
                    sx--;
                    swidth++;
                    if (x2 > x1)
                        x1--;
                    else
                        x1++;
                    if (x4 > x3)
                        x3--;
                    else
                        x3++;
                }

                if (swidth % 2 == 1)
                {
                    swidth++;
                    if (x2 > x1)
                        x2++;
                    else
                        x2--;
                    if (x4 > x3)
                        x4++;
                    else
                        x4--;
                }
            }*/
        }

        public byte spritesheet;
        public byte palette;
        public byte sx;
        public byte sy;
        public byte swidth;
        public byte sheight;
        public sbyte x1;
        public sbyte y1;
        public sbyte x2;
        public sbyte y2;
        public sbyte x3;
        public sbyte y3;
        public sbyte x4;
        public sbyte y4;
    }

    public class SpriteInfoHeader
    {
        public SpriteInfoHeader(BinaryReader br, int memaddr)
        {
            
            entitiespointer = br.ReadInt32();
            sector3pointer = br.ReadInt32();
            mapeventspointer = br.ReadInt32();
            spritetablepointer = br.ReadInt32();
            unknown1pointer = br.ReadInt32();
            spritepalettespointer = br.ReadInt32();
            eventcodesapointer = br.ReadInt32();
            eventcodesbpointer = br.ReadInt32();
            eventcodescpointer = br.ReadInt32();
            eventcodesdpointer = br.ReadInt32();
            eventcodesepointer = br.ReadInt32();
            eventcodesfpointer = br.ReadInt32();

            this.memaddr = memaddr;
            this.eventcodeaddr = memaddr + eventcodesapointer;

            entitiessize = sector3pointer - entitiespointer;
            sector3size = mapeventspointer - sector3pointer;
            mapeventssize = -1;// unknown4 - unknown3;
            spritetablesize = unknown1pointer - spritetablepointer;
            unknown1size = spritepalettespointer - unknown1pointer;
            spritepalettessize = eventcodesapointer - spritepalettespointer;
            eventcodesasize = eventcodesbpointer - eventcodesapointer;
            eventcodesbsize = eventcodescpointer - eventcodesbpointer;
            eventcodescsize = eventcodesdpointer - eventcodescpointer;
            eventcodesdsize = eventcodesepointer - eventcodesdpointer;
            eventcodesesize = eventcodesfpointer - eventcodesepointer;
            eventcodesfandremainingsize = entitiespointer - eventcodesfpointer;
        }
        public int memaddr;
        public int eventcodeaddr;

        public int entitiespointer;
        public int entitiessize;
        public int sector3pointer;
        public int sector3size;
        public int mapeventspointer;
        public int mapeventssize;
        public int spritetablepointer;
        public int spritetablesize;
        public int unknown1pointer;//0000333b000e240e0400000000000000
        public int unknown1size;
        public int spritepalettespointer;
        public int spritepalettessize;
        public int eventcodesapointer;
        public int eventcodesasize;
        public int eventcodesbpointer;
        public int eventcodesbsize;
        public int eventcodescpointer;
        public int eventcodescsize;
        public int eventcodesdpointer;
        public int eventcodesdsize;
        public int eventcodesepointer;
        public int eventcodesesize;
        public int eventcodesfpointer;
        public int eventcodesfsize;//calced when reading sector1
        public int eventcodesfandremainingsize;
    }

    public class SpriteInfoEventCodes
    {
        public SpriteInfoEventCodes(BinaryReader br, long binoffset, SpriteInfoHeader header)
        {
            
            int table_size = 0;
            short firstoffset = 0;

            //read sector1a
            br.BaseStream.Position = binoffset + header.eventcodesapointer;
            table_size = header.eventcodesasize / 2;
            eventcodesatable = new short[table_size];
            for (int dex = 0; dex < table_size; dex++)
            {
                eventcodesatable[dex] = br.ReadInt16();
                if (firstoffset == 0 && eventcodesatable[dex] != 0)
                    firstoffset = eventcodesatable[dex];
            }

            //read sector1b
            br.BaseStream.Position = binoffset + header.eventcodesbpointer;
            table_size = header.eventcodesbsize / 2;
            eventcodesbtable = new short[table_size];
            for (int dex = 0; dex < table_size; dex++)
            {
                eventcodesbtable[dex] = br.ReadInt16();
                if (firstoffset == 0 && eventcodesbtable[dex] != 0)
                    firstoffset = eventcodesbtable[dex];
            }

            //read sector1c
            br.BaseStream.Position = binoffset + header.eventcodescpointer;
            table_size = header.eventcodescsize / 2;
            eventcodesctable = new short[table_size];
            for (int dex = 0; dex < table_size; dex++)
            {
                eventcodesctable[dex] = br.ReadInt16();
                if (firstoffset == 0 && eventcodesctable[dex] != 0)
                    firstoffset = eventcodesctable[dex];
            }

            //read sector1d
            br.BaseStream.Position = binoffset + header.eventcodesdpointer;
            table_size = header.eventcodesdsize / 2;
            eventcodesdtable = new short[table_size];
            for (int dex = 0; dex < table_size; dex++)
            {
                eventcodesdtable[dex] = br.ReadInt16();
                if (firstoffset == 0 && eventcodesdtable[dex] != 0)
                    firstoffset = eventcodesdtable[dex];
            }

            //read sector1e
            br.BaseStream.Position = binoffset + header.eventcodesepointer;
            table_size = header.eventcodesesize / 2;
            eventcodesetable = new short[table_size];
            for (int dex = 0; dex < table_size; dex++)
            {
                eventcodesetable[dex] = br.ReadInt16();
                if (firstoffset == 0 && eventcodesetable[dex] != 0)
                    firstoffset = eventcodesetable[dex];
            }

            //read sector1f
            header.eventcodesfsize = (header.eventcodesapointer + firstoffset) - header.eventcodesfpointer;
            br.BaseStream.Position = binoffset + header.eventcodesfpointer;
            table_size = header.eventcodesfsize / 2;
            if (table_size < 0)
                table_size = 16;
            eventcodesftable = new short[table_size];
            for (int dex = 0; dex < table_size; dex++)
            {
                eventcodesftable[dex] = br.ReadInt16();
            }

            //set binoffset for eventcodes
            this.binoffset = binoffset + header.eventcodesapointer;
            this.memaddr = header.memaddr + header.eventcodesapointer;
            this.datasize = header.entitiespointer - header.eventcodesapointer;
        }
        
        public class SICode
        {
            public byte code { get; set; }
            public string name { get; set; }
            public int size { get; set; }
        }
        public static SICode GetCode(byte b)
        {
            int size = 1;
            string name = "";
            switch (b)
            {
                case 0x02:
                    name = "goto";//skips forward or back
                    size = 3;
                    break;
                case 0x03:
                    name = "iffalse";
                    size = 3;
                    break;
                case 0x04:
                    name = "whilefalse";//loops back until condition met
                    size = 3;
                    break;
                case 0x05:
                    name = "flagon";//turn on a logic switch
                    size = 3;
                    break;
                case 0x06:
                    name = "flagoff";//turn off a logic switch
                    size = 3;
                    break;
                case 0x07:
                    name = "checkentityinarea";
                    size = 8;
                    break;
                case 0x08:
                    name = "turn";//turn direction
                    size = 2;
                    break;
                case 0x09:
                    name = "setdir";//set direction
                    size = 2;
                    break;
                case 0x0a:
                    name = "reverse";//switch direction, used for paceing npcs
                    size = 1;
                    break;

                case 0x0d:
                    name = "dialog";//show dialog
                    size = 3;
                    break;

                case 0x10:
                    name = "losecontrol";
                    size = 1;
                    break;
                case 0x11:
                    name = "gaincontrol";
                    size = 1;
                    break;

                case 0x16:
                    name = "highgravity";//fall as normal
                    size = 1;
                    break;
                case 0x17:
                    name = "lowgravity";//used for climbing ladders and flying
                    size = 1;
                    break;

                case 0x19:
                    name = "deactivate?";
                    size = 1;
                    break;
                case 0x1a:
                    name = "setanim";//set animation
                    size = 2;
                    break;

                //CURRENT IMPLEMENTATION PROGRESS
                case 0x1b:
                    name = "fly"; //stop flying 0x0000   flying down 0xff7f     flying foward and up  0x0380
                    size = 3;
                    break;

                case 0x1e:
                    name = "walk2";//collision blocks/pauses the walk
                    size = 3;
                    break;
                case 0x1f:
                    name = "walk";//collision ends the walk
                    size = 3;
                    break;

                case 0x27:
                    name = "faceplayer";
                    size = 1;
                    break;

                case 0x2d:
                    name = "activateentity";//look into this event to study entity type
                    size = 2;
                    break;
                case 0x2e:
                    name = "hide";
                    size = 2;
                    break;
                case 0x2f:
                    name = "checkmovingindir";
                    size = 4;
                    break;
                case 0x30:
                    name = "ifnot";//if a logic switch is on
                    size = 5;
                    break;
                case 0x31:
                    name = "if";//if a logic switch is not on
                    size = 5;
                    break;

                case 0x33:
                    name = "checkflagson";
                    size = 9;
                    break;
                case 0x34:
                    name = "checkflagsoff";
                    size = 9;
                    break;

                case 0x36:
                    name = "until";//block until a logic switch is on
                    size = 3;
                    break;
                case 0x37:
                    name = "wait";//waits for the specified time to pass
                    size = 2;
                    break;
                case 0x38:
                    name = "registersomething?";
                    size = 5;
                    break;

                case 0x3b:
                    name = "checkplayerinarea";
                    size = 7;
                    break;

                case 0x44:
                    name = "waitdialogchoice";
                    size = 1;
                    break;

                case 0x49:
                    name = "restart";//seeks back to the beginning of event program
                    size = 1;
                    break;

                case 0x4b:
                    name = "iffalserestart";
                    size = 1;
                    break;

                case 0x51:
                    name = "getdialogchoice";
                    size = 1;
                    break;

                case 0x54:
                    name = "setwalkable";
                    size = 5;
                    break;
                case 0x55:
                    name = "setunwalkable";
                    size = 5;
                    break;

                case 0x5a:
                    name = "turnentity";
                    size = 3;
                    break;
                case 0x5b:
                    name = "turn";//also has other flags related to on ground or climbing, etc
                    size = 4;
                    break;

                case 0x62:
                    name = "setentitysomething?";
                    size = 4;
                    break;
                case 0x63:
                    name = "setentitygravity";
                    size = 4;
                    break;
                case 0x64:
                    name = "setentityposition";
                    size = 8;
                    break;

                case 0x67:
                    name = "followentity";
                    size = 2;
                    break;

                case 0x70:
                    name = "checksomething?";
                    size = 1;
                    break;

                case 0x85:
                    name = "setmaptiles";
                    size = 7;
                    break;

                case 0x8b:
                    name = "spawnentity";//pulls entity to ones self and activates it at pixel offset
                    size = 9;
                    break;

                case 0xbd:
                    name = "playsound";
                    size = 3;
                    break;


                //non-operations
                case 0x00:
                    name = "break";
                    size = 1;
                    break;
                case 0xff:
                    name = "end";
                    size = 1;
                    break;

                case 0x39://dialog?
                    size = 1;
                    break;
                case 0x50://dialog?
                    size = 2;
                    break;
                case 0x58:
                    name = "directionalbranch";
                    size = 9;
                    break;
                case 0x59://dialog?
                    size = 3;
                    break;
                case 0xac://used on birds
                    size = 4;
                    break;
                default:
                    Debug.Print("Unknown code " + b.ToString("x2"));
                    break;
            }

            return new SICode { code = b, size = size, name = name };
        }
        public List<SICommand> GetCommands(BinaryReader br, int eventcodesoffset, bool stopatff = false, int comandssize = 0)
        {
            var commands = new List<SICommand>();
            //var bytes = GetByteCode(br, sector1offset);
            br.BaseStream.Position = binoffset + eventcodesoffset;
            var bytes = new byte[datasize - eventcodesoffset];
            br.Read(bytes, 0, bytes.Length);
            int dex = 0;
            while (dex < bytes.Length && (comandssize == 0 || dex< comandssize))
            {
                byte b = bytes[dex++];
                
                var sicode = GetCode(b);
                int size = sicode.size;
                string name = sicode.name;
                var parms = new byte[size - 1];
                int pdex = 0;
                while (pdex < size - 1)
                    parms[pdex++] = bytes[dex++];
                SICommand cmd;
                int addr = memaddr + eventcodesoffset + dex - size;
                switch (name)
                {
                    case "walk":
                    case "walk2":
                        cmd = new WalkCommand(b, parms, name, addr);
                        break;
                    case "setentityposition":
                        cmd = new SetPositionCommand(b, parms, name, addr);
                        break;
                    case "flagon":
                    case "flagoff":
                        cmd = new SetFlagCommand(b, parms, name, addr);
                        break;
                    case "if":
                    case "ifnot":
                        cmd = new BranchCommand(b, 5, parms, name, addr);
                        break;
                    case "ifno":
                    case "whilefalse":
                        cmd = new BranchCommand(b, 3, parms, name, addr);
                        break;
                    case "goto":
                        cmd = new JumpCommand(b, parms, name, addr);
                        break;
                    case "directionalbranch":
                        cmd = new DirectionBranchCommand(b,parms, name, addr);
                        break;
                    default:
                        cmd = new SICommand(b, size, parms, name, addr);
                        break;
                }
                
                commands.Add(cmd);
                if (stopatff && b == 0xff)
                    break;
            }

            return commands;
        }

        public byte[] GetByteCode(BinaryReader br, int sector1offset)
        {

            byte[] bytes = new byte[datasize-sector1offset];
            int dex = 0;
            br.BaseStream.Position = binoffset + sector1offset;
            
            while(dex < bytes.Length)
            {
                //Debug.Assert(dex < bytes.Length, "ByteCodes larger than 255");

                byte b = br.ReadByte();
                if (b == 0)//what does 0 mean?
                {
                    bytes[dex++] = b;
                }
                else if (b == 0xff)//end
                {
                    bytes[dex++] = b;
                    return bytes;//for now
                }
                else
                {
                    bytes[dex++] = b;
                    //skip ahead by parameter length
                }
            }
            return bytes;
        }

        long binoffset;
        int datasize;
        int memaddr;
        public short[] eventcodesatable;
        public short[] eventcodesbtable;
        public short[] eventcodesctable;
        public short[] eventcodesdtable;
        public short[] eventcodesetable;
        public short[] eventcodesftable;
    }

    public class SetFlagCommand : SICommand
    {
        public SetFlagCommand(byte command, byte[] parameters, string name, int memaddr)
            : base(command, 3, parameters, name, memaddr)
        {
        }

        public override string PrintParameters(List<SICommand> commands)
        {
            return (parameters[0] | parameters[1] << 8).ToString("x4");
        }
    }

    public class WalkCommand : SICommand
    {
        public WalkCommand(byte command, byte[] parameters, string name, int memaddr)
            : base(command, 3, parameters, name, memaddr)
        {
        }

        public override string PrintParameters(List<SICommand> commands)
        {
            return (parameters[0] | parameters[1] << 8).ToString("x4");
        }
    }

    public class SetPositionCommand : SICommand
    {
        public SetPositionCommand(byte command, byte[] parameters, string name, int memaddr)
            : base(command, 8, parameters, name, memaddr)
        {
        }

        public override string PrintParameters(List<SICommand> commands)
        {
            var parms = new List<string>();

            parms.Add(parameters[0].ToString("x2"));
            parms.Add((parameters[1] | parameters[2] << 8).ToString("x4"));
            parms.Add((parameters[3] | parameters[4] << 8).ToString("x4"));
            parms.Add((parameters[5] | parameters[6] << 8).ToString("x4"));

            return string.Join(", ", parms);
        }
    }

    public class BranchCommand : SICommand
    {
        public BranchCommand(byte command, int size, byte[] parameters, string name, int memaddr)
            : base(command, size, parameters, name, memaddr)
        {
            base.refoffset = (Int16)(parameters[size-3] | parameters[size-2] << 8);
        }

        public override string PrintParameters(List<SICommand> commands)
        {
            var parms = new List<string>();
            if (size == 5)
            {
                parms.Add((parameters[size - 5] | parameters[size - 4] << 8).ToString("x4"));
            }
            else
            {

                for (int dex = 0; dex < size - 3; dex++)
                {
                    parms.Add(parameters[dex].ToString("x2"));
                }
            }
            parms.Add(refoffset.ToString());

            return string.Join(", ",parms);
        }
    }

    public class DirectionBranchCommand : SICommand
    {
        public DirectionBranchCommand(byte command, byte[] parameters, string name, int memaddr)
            : base(command, 9, parameters, name, memaddr)
        {

            offsets[0] = (Int16)(parameters[size - 9] | parameters[size - 8] << 8);
            offsets[1] = (Int16)(parameters[size - 7] | parameters[size - 6] << 8);
            offsets[2] = (Int16)(parameters[size - 5] | parameters[size - 4] << 8);
            offsets[3] = (Int16)(parameters[size - 3] | parameters[size - 2] << 8);
        }

        int[] offsets = new int[4];

        public override string PrintParameters(List<SICommand> commands)
        {
            var parms = new List<string>();
            foreach(var offset in offsets)
            {
                int jumpaddr = this.memaddr + offset;
                int dex;
                for (dex = 0; dex < commands.Count; dex++)
                {
                    if (commands[dex].memaddr == jumpaddr)
                        break;
                }
                parms.Add(dex < commands.Count ? dex.ToString() : "?");
            }

            return string.Join(", ", parms);
        }
    }

    public class JumpCommand : SICommand
    {
        public JumpCommand(byte command, byte[] parameters, string name, int memaddr)
            : base(command, 3, parameters, name, memaddr)
        {
            base.refoffset = (Int16)(parameters[0] | parameters[1] << 8);
        }

        public override string PrintParameters(List<SICommand> commands)
        {
            Int16 jumpamount = (Int16)refoffset;// (Int16)(parameters[0] | parameters[1] << 8);
            int jumpaddr = this.memaddr + jumpamount;
            int dex;
            for (dex = 0;dex<commands.Count;dex++)
            {
                if (commands[dex].memaddr == jumpaddr)
                    break;
            }
            return dex < commands.Count ? dex.ToString() : "?";
        }

    }

    public class SICommand
    {
        public SICommand(byte command, int size, byte[] parameters, string name, int memaddr)
        {
            this.memaddr = memaddr;
            this.command = command;
            this.parameters = parameters;
            this.size = size;
            this.name = name;
        }
        public int memaddr;
        public byte command;
        public byte[] parameters;
        public int size;
        public int refoffset;

        public string name;

        public string PrintName()
        {
            return !string.IsNullOrEmpty(name) ? name : command.ToString("x2");
        }

        public virtual string PrintParameters(List<SICommand> commands)
        {
            return string.Join(", ",parameters.Select(x=> x.ToString("x2")));
        }

        public string Print(int depth, List<SICommand> commands)
        {
            int index = commands.IndexOf(this);
            string output = index.ToString("d3") + " ";
            output += new string(' ', depth * 4);
            output += PrintName();
            if (command != 0 && command != 0xff)
            {
                output += "(";
                output += PrintParameters(commands);
                output += ")";
            }
            return output;
        }
    }

    public class SpriteInfoEntities
    {
        public SpriteInfoEntities(BinaryReader br, int memaddr)
        {
            br.BaseStream.Position += 2;

            entities = new SIEntityRecord[128];
            for (int dex = 0; dex < entities.Length; dex++)
            {
                //read two test bytes to check for the end of the list
                short test = br.ReadInt16();
                if (test == 0)
                    break;
                br.BaseStream.Position -= 2;

                //read the record
                entities[dex] = new SIEntityRecord(br, memaddr + dex * 20);
            }
        }
        public SIEntityRecord[] entities;
    }

    public class SIEntityRecord
    {
        public SIEntityRecord(BinaryReader br, int memaddr)
        {
            this.memaddr = memaddr;
            u1 = br.ReadByte();
            u2 = br.ReadByte();
            u3 = br.ReadByte();
            spritedir = br.ReadByte();
            spritetableindex = br.ReadByte();
            xpos = br.ReadByte();
            ypos = br.ReadByte();
            height = br.ReadByte();
            eventcodesa_load_index = br.ReadByte();
            eventcodesb_unknown_index = br.ReadByte();
            eventcodesc_tick_index = br.ReadByte();
            eventcodesd_touch_index = br.ReadByte();
            eventcodese_unknown_index = br.ReadByte();
            eventcodesf_interact_index = br.ReadByte();
            u7 = br.ReadByte();
            u8 = br.ReadByte();
            u9 = br.ReadByte();
            u10 = br.ReadByte();
            u11 = br.ReadByte();
            u12 = br.ReadByte();
        }

        public SIAnimation GetSprite(BinaryReader br, SpriteInfo si)
        {
            var sector5 = si.sprites[spritetableindex];
            if (sector5 != null && this.spritedir >> 4 != 0x4 && this.spritedir >> 4 != 0x0)
            {
                List<SICommand> commands = new List<SICommand>();
                if (eventcodesa_load_index != 0xff && eventcodesa_load_index != 0)
                    commands.AddRange(si.eventcodes.GetCommands(br, si.eventcodes.eventcodesatable[eventcodesa_load_index & 0x7f], true));
                if (commands.Count == 0 && eventcodesc_tick_index != 0xff && eventcodesc_tick_index != 0)
                    commands.AddRange(si.eventcodes.GetCommands(br, si.eventcodes.eventcodesctable[eventcodesc_tick_index & 0x7f], true));
                foreach (var cmd in commands)
                {
                    if (cmd.command == 0x1a)//set sprite
                    {
                        var animset = sector5.animsets[cmd.parameters[0]];

                        return sector5.GetAnimation(br, animset.animoffsets[spritedir & 0x3]);
                    }
                }
                return sector5.GetAnimation(br, sector5.animsets[0].animoffsets[spritedir & 0x3]);//default anim
            }

            return null;
        }
        public int memaddr;
        public byte u1;//33
        public byte u2;//3b
        public byte u3;//1
        public byte spritedir;//0,c0,c1,c2,c3,80
        public byte spritetableindex;
        public byte xpos;//divide by 2
        public byte ypos;//divide by 2
        public byte height;//divide by 2
        public byte eventcodesa_load_index;
        public byte eventcodesb_unknown_index;
        public byte eventcodesc_tick_index;
        public byte eventcodesd_touch_index;
        public byte eventcodese_unknown_index;
        public byte eventcodesf_interact_index;
        public byte u7;
        public byte u8;
        public byte u9;
        public byte u10;
        public byte u11;
        public byte u12;
    }

    public class SpriteInfoSector3
    {

    }

    public class SpriteInfoMapEvents
    {
        public SpriteInfoMapEvents(BinaryReader br, long sioffset, int sectorend)
        {
            br.BaseStream.Position += 2;

            records = new SISector4Record[64];
            for (int dex = 0; dex < records.Length; dex++)
            {
                //read two test bytes to check for the end of the list
                short test = br.ReadInt16();
                if (test == 0)
                    break;
                br.BaseStream.Position -= 2;

                //read the record
                records[dex] = new SISector4Record(br);
            }


        }

        public SISector4Record[] records;
        
    }

    public class SISector4Record
    {
        public SISector4Record(BinaryReader br)
        {
            u1 = br.ReadByte();
            u2 = br.ReadByte();
            eventcodesbindex = br.ReadByte();
            u4 = br.ReadByte();
            u5 = br.ReadByte();
            u6 = br.ReadByte();
            u7 = br.ReadByte();
            u8 = br.ReadByte();
        }

        public byte u1;
        public byte u2;
        public byte eventcodesbindex;
        public byte u4;
        public byte u5;
        public byte u6;
        public byte u7;
        public byte u8;
    }



    public class GameMapInfo
    {
        public GameMapInfo(int mapid, int memaddr)
        {
            this.memaddr = memaddr;
            this.mapid = mapid;
        }

        public GameMapInfo(BinaryReader br, int memaddr)
        {
            this.memaddr = memaddr;
            long binoffset = br.BaseStream.Position;
            mapid = br.ReadInt32();
            gravity = br.ReadInt16();
            terminal_velocity = br.ReadInt16();
            unknown512 = br.ReadInt16();
            unknown7 = br.ReadInt16();
            unknown3882a = br.ReadByte();
            unknown3882b = br.ReadByte();
            uknown17 = br.ReadInt16();
            //read palettes
            int maxpalettes = 32;
            palettes = new System.Drawing.Color[maxpalettes][];
            byte[] buff = new byte[maxpalettes * 16 * 2];
            br.Read(buff, 0, buff.Length);
            int buffdex = 0;
            for (int dex = 0; dex < maxpalettes; dex++)
            {
                palettes[dex] = new System.Drawing.Color[16];
                for (int cdex = 0; cdex < 16; cdex++)
                {
                    byte b2 = buff[buffdex++];
                    byte b1 = buff[buffdex++];
                    palettes[dex][cdex] = Utils.FromPsxColor((b1 << 8) | b2);
                }
            }
            palettesbitmap = Utils.BitmapFromPsxBuff(buff, 16, maxpalettes, 16, null);

            //read portals
            br.BaseStream.Position = binoffset + 1066;
            portalflag1 = br.ReadByte();
            portalflag2 = br.ReadByte();
            int maxportals = 64;
            portals = new Portal[maxportals];
            for (int dex = 0; dex < portals.Length; dex++)
            {
                portals[dex] = new Portal(br);
            }
        }
        public int memaddr;
        public int mapid;
        public short gravity;
        public short terminal_velocity;
        public short unknown512;
        public short unknown7;
        public byte unknown3882a;
        public byte unknown3882b;
        public short uknown17;
        public System.Drawing.Color[][] palettes;
        public Bitmap palettesbitmap;
        public byte portalflag1;
        public byte portalflag2;
        public Portal[] portals;

    }
    public class Portal
    {
        public Portal(BinaryReader br)
        {
            x1 = br.ReadByte();
            y1 = br.ReadByte();
            x2 = br.ReadByte();
            y2 = br.ReadByte();
            destmapid = br.ReadInt16();
            destx = br.ReadByte();
            desty = br.ReadByte();
            unknown1 = br.ReadByte();
            unknown2 = br.ReadByte();
            unknown3 = br.ReadByte();
            unknown4 = br.ReadByte();
        }
        public byte x1, y1;
        public byte x2, y2;
        public short destmapid;
        public byte destx, desty;
        public byte unknown1, unknown2;
        public byte unknown3, unknown4;
    }
    public class GameMapHeader
    {
        public GameMapHeader(DBHeader header)
        {//alundra gamemap, just has sprites
            infoblock = -1;
            mapblock = -1;
            tilesheets = -1;
            spriteinfo = (int)header.alundraspriteinfo;
            spritesheets = (int)header.alundrasprites;
            scrollscreen = -1;
            stringtable = (int)header.alundrastringtable;

            infosize = 0;
            mapsize = 0;
            tilessize = 0;
            sinfosize = spritesheets - spriteinfo;
            spritessize = (int)header.unknownmapa - spritesheets;
            scrollsize = 0;
        }
        public GameMapHeader(BinaryReader br)
        {
            infoblock = br.ReadInt32();
            mapblock = br.ReadInt32();
            tilesheets = br.ReadInt32();
            spriteinfo = br.ReadInt32();
            spritesheets = br.ReadInt32();
            scrollscreen = br.ReadInt32();
            stringtable = br.ReadInt32();

            infosize = mapblock - infoblock;
            mapsize = tilesheets - mapblock;
            tilessize = spriteinfo - tilesheets;
            sinfosize = spritesheets - spriteinfo;
            spritessize = scrollscreen - spritesheets;
            scrollsize = stringtable - scrollscreen;
            //string table is called later
        }

        public int infosize;
        public int mapsize;
        public int walltilessize;
        public int tilessize;
        public int sinfosize;
        public int spritessize;
        public int scrollsize;
        public int stringsize;

        public int infoblock;
        public int mapblock;
        public int tilesheets;
        public int spriteinfo;
        public int spritesheets;
        public int scrollscreen;//shadow, sky or distant background
        public int stringtable;
    }

    public class DBHeader
    {
        public DBHeader(BinaryReader br)
        {
            alundraspriteinfo = br.ReadUInt32();
            alundrasprites = br.ReadUInt32();
            alundraspritesrepeat = br.ReadUInt32();
            alundrastringtable = br.ReadUInt32();
            alundrastringtablerepeat = br.ReadUInt32();
            unknownmapa = br.ReadUInt32();
            unknownmapb = br.ReadUInt32();
            unknownmapb2 = br.ReadUInt32();
            unknownmapb3 = br.ReadUInt32();
            unknownmapb4 = br.ReadUInt32();
            int maxmaps = 502;
            gamemaps = new UInt32[maxmaps];
            for (int dex = 0; dex < maxmaps; dex++)
            {
                gamemaps[dex] = br.ReadUInt32();
            }
        }

        public UInt32 alundraspriteinfo;
        public UInt32 alundrasprites;
        public UInt32 alundraspritesrepeat;
        public UInt32 alundrastringtable;
        public UInt32 alundrastringtablerepeat;
        public UInt32 unknownmapa;
        public UInt32 unknownmapb;
        public UInt32 unknownmapb2;
        public UInt32 unknownmapb3;
        public UInt32 unknownmapb4;
        public UInt32[] gamemaps;
    }
}
