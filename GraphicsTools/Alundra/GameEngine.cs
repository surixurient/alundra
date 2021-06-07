
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GraphicsTools.Alundra
{
    public class GameEngine
    {
        DatasBin datasBin;
        GameMap map;
        Dictionary<int, Bitmap> CachedTiles;
        Dictionary<int, List<Bitmap>> CachedSprites;
        GameState game;
        EventHandlers eventHandlers;
        public GameEngine(DatasBin datasBin)
        {
            this.datasBin = datasBin;
            var reader = datasBin.OpenBin();
            if (!datasBin.alundragamemap.loaded)
                datasBin.alundragamemap.Load(reader, false);
            reader.Close();

            game = new GameState (datasBin.alundragamemap) { CamXPos = 100 << 16, CamYPos = 100 << 16 };

            eventHandlers = new EventHandlers(game);
        }

        const int SCREEN_WIDTH = 320;
        const int SCREEN_HEIGHT = 224;
        public void Render(Graphics g)
        {
            int curxpos = game.CamXPos >> 16;
            int curypos = game.CamYPos >> 16;

            int curxtile = curxpos / 24;

            var sinfo = map.spriteinfo;
            var gensi = datasBin.alundragamemap.spriteinfo;

            for (int y = 0; y < 60; y++)
            {
                //draw tiles on this row
                for (int x = curxtile; x < curxtile + SCREEN_WIDTH / 24 + 2; x++)
                {
                    var tile = map.map.maptiles[y * 52 + x];
                    //render tile
                    int dx = x * 24 - curxpos;
                    int dy = (y - tile.height) * 16 - curypos;

                    if (dy > -16 && dy < SCREEN_HEIGHT && tile.tileid != -1)
                        DrawTile(tile.tileid, dx, dy, g);
                    if (tile.walltiles != null)
                    {
                        var walltiles = tile.walltiles;
                        int dex;
                        dy -= (walltiles.offset) * 16;
                        for (dex = 0; dex < walltiles.count; dex++)
                        {
                            dy += 16;
                            //render wall tile
                            if (dy > -16 && dy < SCREEN_HEIGHT && walltiles.tiles[dex] != -1)
                                DrawTile(walltiles.tiles[dex], dx, dy, g);

                        }
                    }
                }

                //draw sprites who are on this row
                for (int sdex = 0; sdex < game.MaxEntity; sdex++)
                {
                    SpriteInstance si = game.Entities[sdex];
                    if (si.Status == 5)
                        continue;
                    if (si.YTile != y)
                        continue;//if its not in this row, continue


                    //var tile = selectedGame.map.maptiles[sx + sy * selectedGame.map.width];
                    int scx = (si.ModdedXPos>>16) - curxpos;
                    int scy = ((si.ModdedYPos >> 16) - (si.ModdedZPos >> 16)) - curypos;
                    if (si.Sprite != null)
                    {
                        int idex;

                        GameMap gm = datasBin.alundragamemap;//general sprite
                        if (si.IsMapSprite)
                            gm = map;//map sprite

                        var iset = si.Frame.images;
                        for (idex = iset.numimages - 1; idex >= 0; idex--)
                        {
                            var img = iset.images[idex];

                            DrawSprite(gm, img, scx, scy, g);
                        }

                    }
                }
            }
        }

        Point[] pnts = new Point[4];
        private void DrawSprite(GameMap gm, SIImage img, int x, int y, Graphics g)
        {
            var bmp = gm.GetSpriteBitmap(img);
            pnts[0].X = x + img.x1;
            pnts[1].X = x + img.x2;
            pnts[2].X = x + img.x3;
            pnts[3].X = x + img.x4;

            pnts[0].Y = y + img.y1;
            pnts[1].Y = y + img.y2;
            pnts[2].Y = y + img.y3;
            pnts[3].Y = y + img.y4;
            g.DrawImage(bmp, pnts);
        }

        private void DrawTile(int tileid, int x, int y, Graphics g)
        {
            var bmp = map.GetTileBitmap(tileid);
            g.DrawImage(bmp, x, y);
        }

        public void LoadMap(GameMap gmap)
        {
            this.map = gmap;
            if (!map.loaded)
            {
                var reader = datasBin.OpenBin();
                map.Load(reader, true);
                reader.Close();
            }

            game.LoadMap(map);



            //spriteinfo
            for (int dex = 0; dex < map.spriteinfo.entities.entities.Length; dex++)
            {
                var entity = map.spriteinfo.entities.entities[dex];
                if (entity != null)
                {
                    
                }
            }

            
            for (int dex = 0; dex < map.spriteinfo.mapevents.records.Length; dex++)
            {
                var record = map.spriteinfo.mapevents.records[dex];
                if (record != null)
                {
                    
                }
            }


            for (int dex = 0; dex < map.spriteinfo.sprites.Length; dex++)
            {
                var sprite = map.spriteinfo.sprites[dex];
                if (sprite != null)
                {

                }
            }

        }

        public void MainLoop(Graphics g)
        {

            do
            {
                //_2abe8();

                Render(g);

                MainUpdate(false);

                //_86220(game._1e60e8, r19);

                var framestoadvance = 1;
                //if (game._1ac468[0] < 0
                //    && (game._1ac468[4] & 0x4000) != 0)
                //    framestoadvance = game._1ac468[14];

                //PresentFrame(framestoadvance);

                //EmptyFunc();

            } while (!game.BreakoutGameLoop);
        }

        public void MainUpdate(bool force)
        {
            //game._1d7a70 -= 8;
            //if (game._1d7a70 < 0)
            //    game._1d7a70 = 0;

            //game._1d7a78 = 0x140 - (game._1d7a70 * 2);

            //_1d7a74 -= 6;
            //if (game._1d7a74 < 0)
            //    game._1d7a74 = 0;

            //game._1d7a7c = 0xf0 - (game._1d7a74 * 2);

            //GetPlayerInput();

            Update();

            //if (game._1ef998 != 0)
            //{
            //    game._1ef998--;
            //}

            /*if (game.PlayerControlSetting == 0
                && game.PlayerEntity._20 == 0
                && game._1f0fbc == 0
                && (game.1dd7ea & 0x803) != 0
                && game._1ef998 == 0
                && (game.PlayerInput & 0x100) == 0
                && _570f8() == 0)
                game.BreakoutGameLoop = true;*/

            //PlayMusic();
            //advance random seed num
            var rnd = (int)((game.Seed * 0x7d2b89dd) + 0xe06a02e7);
            game.Seed = rnd;

            if (force)
                game.BreakoutGameLoop = false;
        }

        public void Update()
        {

            /*
             a debug check 
            if (0x38800 < game._1dd810) {

            }
             */

            //game._1ef1d0 = 0x1380f0;

            //number of spriterefs that will be rendered this frame, the following functions build up the list
            game.NumSprites = 0;

            UpdateMapEvents();

            UpdateEntities();

            UpdateEffects();

        }

        public void UpdateEntities()
        {

        }

        public void UpdateMapEvents()
        {
            if ((game.PlayerControlSetting & 0x48) != 0)
                return;
            int medex = 0;
            var p = game.PlayerEntity;
            foreach(var mapevent in game.MapEvents)
            {
                var eventcode = mapevent.ProgramB_Map;
                if ((eventcode & 0x7f) == 0)
                    continue;
                var rec = mapevent.MapEventRecord;
                if (p.XTile < rec.x1 || p.XTile > rec.x2 || p.YTile < rec.y1 || p.YTile > rec.y2)
                {
                    p.Program_Indexes[Helper.PROGRAM_B_MAP] = mapevent.ProgramB_Map;
                    p.MapEventProgramId = mapevent.ProgramB_Map;
                    p.eventdata = mapevent.EventData;
                    p.MapEventId = medex;
                    p.EntitySelf = mapevent.Entity;
                    eventHandlers.RunEntityEventScripts(p, Helper.PROGRAM_B_MAP);
                    
                    mapevent.ProgramB_Map = p.Program_Indexes[Helper.PROGRAM_B_MAP];
                    mapevent.EventData = p.eventdata;
                    mapevent.Entity = p.EntitySelf;
                }
                else
                {
                    mapevent.EventData.sp = 0;
                    mapevent.EventData.exp = 0;
                    mapevent.EventData.logicResult = 0;
                    mapevent.Entity = p;
                    mapevent.ProgramB_Map = rec.eventcodesbindex;
                }
                medex++;
            }
        }

        


        //updates effect animations and adds sprites to spritereflist
        public void UpdateEffects()
        {
            foreach (var effect in game.SpriteEffects)
            {
                if (effect.Status != 2)
                    continue;
                if ((game.PlayerControlSetting & 0x48) == 0)
                {
                    if (effect.DestroyFlag != 0)
                    {
                        effect.Status = 0;
                        continue;
                    }

                    UpdateEffectAnim(effect);

                    UpdateEffectByType(effect);
                }

                effect.SpriteRef.DepthSortVal = effect.DepthSortVal;
                effect.SpriteRef.X = effect.X;
                effect.SpriteRef.Y = effect.Y;
                effect.SpriteRef.Z = effect.Z;
                game.SpriteRefs[game.NumSprites++] = effect.SpriteRef;
            }
        }

        public void UpdateEffectAnim(SpriteEffect effect)
        {
            if (effect.CurSpriteTableIndex != effect.TargetSpriteTableIndex
                || effect.CurIsMapSprite != effect.TargetIsMapSprite)
            {
                int addtosheet, addtopal;
                var record = game.GetEffectSpriteFromSpriteTable(effect.TargetIsMapSprite != 0, effect.TargetSpriteTableIndex, out addtosheet, out addtopal);
                if (record == null)
                {
                    effect.DestroyFlag = 1;
                    effect.SpriteRef.Images = null;
                    effect.SpriteRef.NumImages = 0;
                    //field after numimages = 0
                    return;
                }

                effect.SpriteEffectRecord = record;
                effect.CurIsMapSprite = effect.TargetIsMapSprite;
                effect.CurSpriteTableIndex = effect.TargetSpriteTableIndex;

                effect.AddToSheet = addtosheet;
                effect.AddToPalette = addtopal;
                effect.CurAnim = (byte)(~effect.TargetAnim);
            }

            if (effect.CurAnim != effect.TargetAnim)
            {
                var animation = effect.SpriteEffectRecord.preloaded_anims[effect.TargetAnim];
                effect.animdex = 0;
                var nframe = animation.frames[effect.animdex];
                effect.CurAnim = effect.TargetAnim;
                effect.Delay = 0;
                effect.DestroyFlag = 0;
                effect.FirstFrame = nframe;
                effect.Frame = nframe;
            }
            else
            {
                effect.Delay--;
                if ((effect.Delay & 0xff) != 0)
                    return;
            }

            do
            {
                var frame = effect.Frame;
                if ((frame.delay & 0x80) != 00)
                {
                    effect.animdex++;
                    var anim = effect.SpriteEffectRecord.preloaded_anims[effect.TargetAnim];
                    frame = anim.frames[effect.animdex];
                    effect.Frame = frame;
                    if (frame.images != null)
                    {
                        effect.SpriteRef.Images = frame.images.images;
                        effect.SpriteRef.DepthSortVal = frame.images.unknown;
                        effect.SpriteRef.NumImages = frame.images.numimages;
                        return;
                    }
                    effect.SpriteRef.Images = null;
                    effect.SpriteRef.DepthSortVal = 0;
                    effect.SpriteRef.NumImages = 0;
                    return;
                }
                if (frame.delay != 0)
                {
                    if (frame.delay == 1)
                    {
                        //repeat
                        effect.animdex = 0;
                        effect.Frame = effect.FirstFrame;
                        continue;
                    }
                    throw new Exception("Error with Effect Animation!");
                }
                else//its 0  which means its non repeating so flag for destroy
                {
                    effect.Delay = 0xff;
                    effect.DestroyFlag = 1;
                    return;
                }
            } while (true);

        }

        public void UpdateEffectByType(SpriteEffect effect)
        {
            if (effect.EffectType == 0)
            {
                effect.X += effect.XMod; //forces?
                effect.Y += effect.YMod;
                effect.Z += effect.ZMod;
                //some kind of unique id? maybe its used for zsorting
                effect.DepthSortVal = (int)(effect.Y & 0xffff0000) + (effect.Z >> 16) + (effect.SpriteRef.NumImages << 16);
                return;
            }
            else if (effect.EffectType == 1)
            {
                var entity = effect.EntityRef;
                if (entity.Status != 0)
                {
                    effect.X = entity.XPos + effect.XOff;
                    effect.Y = entity.YPos + effect.YOff;
                    effect.Z = entity.ZPos + effect.ZOff;
                    effect.DepthSortVal = entity.DepthSortVal + effect.DepthSortMod;
                    if (entity.Status == 4)
                        effect.EffectType = 2;
                }
                else
                {
                    effect.EffectType = 2;
                }
            }
            else if (effect.EffectType != 3)
            {
                return;
            }
            //param3== 1 falls through to here, and param3 == 3 is here
            effect.X += effect.XMod; //forces?
            effect.Y += effect.YMod;
            effect.Z += effect.ZMod;

            if (effect.EntityRef.Status != 0)
            {
                effect.DepthSortVal = effect.EntityRef.DepthSortVal + effect.DepthSortMod;

                if (effect.EntityRef.Status == 4)
                    effect.EffectType = 2;
            }
            else
            {
                effect.EffectType = 2;
            }
        }
    }
}
