
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

            game = new GameState (datasBin.alundragamemap, datasBin.balancebin) { CamXPos = 100 << 16, CamYPos = 100 << 16 };

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
            if ((game.PlayerControlSetting & 0x48) == 0)
            {
                ProcessDestroyedEntities();

                DoEvents();

                UpdateCounters();



                AddToLists();

                UpdateAnims();

                //DoPhysics();

                UpdateActiveEffects();

                //UpdateBalanceRecords();
            }
            else
            {
                AddToLists();
            }

            if (game.CamFollowEntity != null)
            {
                if (game.CamFollowEntity.Status <= 3)
                {
                    //gets halfwords
                    game.CamTargetX = game.CamFollowEntity.XPos >> 16;
                    game.CamTargetY = game.CamFollowEntity.YPos >> 16;
                    game.CamTargetZ = game.CamFollowEntity.ZPos >> 16;
                }
            }
            //_3a1e4();

            //add spriterefs
            if (game.ToRenderCount > 0)
            {
                for (int dex = 0;dex<game.ToRenderCount;dex++)
                {
                    var entity = game.ToRenderList[dex];

                    entity.SpriteRef.DepthSortVal = entity.DepthSortVal;
                    entity.SpriteRef.X = entity.XPos;
                    entity.SpriteRef.Y = entity.YPos;
                    entity.SpriteRef.Z = entity.ZPos;
                    game.SpriteRefs[game.NumSprites++] = entity.SpriteRef;
                }
            }
        }

        void UpdateActiveEffects()
        {
            if (game.MaxEntity < 0)
                return;
            for (int dex = 0; dex < game.MaxEntity; dex++)
            {
                var entity = game.Entities[dex];
                if(entity.Status-2 >= 2 || (entity.DamagedTickCounter & 3) ==3)
                {
                    if (entity.ActiveEffect != null)
                    {
                        entity.ActiveEffect.Status = 0;
                        entity.ActiveEffect = null;
                    }
                    continue;
                }
                var effect = entity.ActiveEffect;
                if (effect == null)
                {
                    effect = game.CreateEffect_Type3(0, 0, 0, entity, -1, 0, 0, 0);
                    if (effect == null)
                        continue;
                    entity.ActiveEffect = effect;
                }

                if (((entity.Flags>>16) & 7) == 0)
                    continue;

                if ((entity.AnimFlags & 0x10) != 0)
                    continue;

                if (entity.PlatformEntity != null)
                {
                    effect.Status = 1;
                    continue;
                }
                //TODO: what is 18c, something with slope and sliding?
                int animid = -1;
                if ((entity._18c == 4 || entity._190 == 4) 
                    && entity._18c != entity._190)
                {
                    game.CreateEffect_Type0(0, 6, 0, entity.XPos, entity.YPos, entity.CollidedWithEntityZ);
                }

                if (entity._18c >= 8)
                    continue;
                switch(entity._18c)
                {
                    case 1:
                    case 2:
                        effect.TargetIsMapSprite = 0;
                        effect.TargetSpriteTableIndex = 1;
                        effect.TargetAnim = (byte)animid;
                        effect.Status = 2;
                        effect.X = entity.XPos;
                        effect.Y = entity.YPos;
                        effect.Z = entity.CollidedWithEntityZ;
                        continue;
                    case 4:
                        effect.Status = 1;
                        if ((entity.UnknownCounter & 7) != 0)
                            continue;
                        if ((entity.XForce | entity.YForce) == 0)
                            continue;

                        game.CreateEffect_Type0(0, 0x15, 0, entity.XPos, entity.YPos, entity.CollidedWithEntityZ);
                        continue;
                    case 3:
                        if ((entity.UnknownCounter & 0x7) != 0)
                            break;
                        if ((entity.XForce | entity.YForce) == 0)
                            break;
                        game.CreateEffect_Type0(0, game.gameMap.info.slideeffectid, 0, entity.XPos, entity.YPos, entity.CollidedWithEntityZ);
                        break;
                    default:
                        break;
                }

                animid -= ((entity.ZPos - entity.CollidedWithEntityZ) >> 20);

                if (animid >= 6)
                    animid = 5;
                else if (animid < 0)
                    animid = 0;

                effect.TargetIsMapSprite = 0;
                effect.TargetSpriteTableIndex = 0;

                effect.TargetAnim = (byte)animid;
                effect.Status = 2;

                effect.X = entity.XPos;
                effect.Y = entity.YPos;
                effect.Z = entity.CollidedWithEntityZ;
            }
        }

        void UpdateAnims()
        {
            for (int dex =0;dex<game.ToProcessesCount;dex++)
            {
                var entity = game.Entities[dex];
                game.UpdateAnim(entity);
            }
        }

        void MovePlayer()
        {
            //TODO: impliment
            //this function is MASSIVE
            //perhaps the largest in the entire game
        }

        void DoEvents()
        {
            MovePlayer();

            if (game.MaxEntity > 0)
            {
                for (int dex =1;dex<game.MaxEntity;dex++)
                {
                    var entity = game.Entities[dex];
                    int evttype = -1;
                    if (entity._20 == 0 && entity.Status< 5)
                    {
                        switch(entity.Status)
                        {
                            case 1://loading/activating
                                evttype = Helper.PROGRAM_A_LOAD;
                                entity.Status = 2;
                                break;
                            case 2://normal

                                if ((entity.Flags & 0x100000) != 0)
                                {
                                    if (entity._18c == 4)
                                    {
                                        game.DestroyEntity(entity, 6);

                                        evttype = -1;
                                        break;
                                    }
                                }

                                if ((entity.Flags & 0x200000) != 0)
                                {
                                    if ((entity._180 & 0x8004) != 0)
                                    {
                                        game.DestroyEntity(entity, -1);

                                        evttype = -1;
                                        break;
                                    }
                                }

                                if ((entity.Flags & 0x10) != 0)
                                {
                                    if (entity.ForceAdjusted != 0 || entity._144 != 3)
                                    {
                                        entity.Status = 3;//decativate
                                        evttype = Helper.PROGRAM_E_DEACTIVATE;
                                        break;
                                    }
                                }

                                if ((entity.Flags & 0x20) != 0)
                                {
                                    if (entity.HitCounter != 0)
                                    {
                                        entity.Status = 3;//decativate
                                        evttype = Helper.PROGRAM_E_DEACTIVATE;
                                        break;
                                    }
                                }

                                if ((entity.Flags & 0x40) != 0)
                                {
                                    if (entity.WierdNextFrameDelayFlag != 0)
                                    {
                                        entity.Status = 3;//decativate
                                        evttype = Helper.PROGRAM_E_DEACTIVATE;
                                        break;
                                    }
                                }

                                if (entity.TouchingEntity != null)
                                {
                                    evttype = Helper.PROGRAM_D_TOUCH;
                                    break;
                                }

                                //i think this gamevar is more than just activecollitionentity, 
                                //the interact button probabaly has to be down for this to be set
                                if (game.ActiveCollisionEntity != entity)
                                {
                                    evttype = 2;
                                    break;
                                }
                                //if it gets here it means the player is interacting with this entity

                                if (entity.Sprite_Program_Indexes[Helper.PROGRAM_F_INTERACT] != 0)
                                {
                                    evttype = 5;
                                    break;
                                }

                                if (entity.Program_Indexes[Helper.PROGRAM_F_INTERACT] != 0)
                                {
                                    evttype = 5;
                                    break;
                                }
                                evttype = 2;
                                break;
                            case 3://deactivating
                                evttype = Helper.PROGRAM_E_DEACTIVATE;
                                break;
                            case 0:
                            case 4:
                                evttype = -1;
                                break;
                        }
                    }
                    entity.EventTrigger = evttype;
                }
            }

            //run events
            bool keepgoing = false;
            do
            {
                keepgoing = false;
                if (game.MaxEntity > 0)
                {
                    //foreach entity besides player
                    for (int dex = 1; dex < game.MaxEntity; dex++)
                    {
                        var entity = game.Entities[dex];

                        if (entity.EventTrigger != -1)
                        {
                            int progindex = entity.Program_Indexes[entity.EventTrigger] & 0x7f;

                            if (progindex != 0)
                            {
                                //run the eventhandler script
                                eventHandlers.RunEntityEventScripts(entity, entity.EventTrigger);
                                entity.EventTrigger = -1;
                            }
                            else
                            {
                                int eventid = entity.Sprite_Program_Indexes[entity.EventTrigger];
                                //run the sprite event handler
                                eventHandlers.SpriteHandlers.RunSpriteHandler(entity.EventTrigger, eventid, entity);
                                entity.EventTrigger = -1;
                            }

                            keepgoing = true;
                        }
                    }
                }

            } while (keepgoing);
        }

        void UpdateCounters()
        {
            if (game.MaxEntity>=0)
            {
                for (int dex = 0; dex <= game.MaxEntity; dex++)
                {
                    var entity = game.Entities[dex];
                    entity.UnknownCounter++;
                    if (entity.DamagedTickCounter != 0)
                        entity.DamagedTickCounter--;
                    if (entity.FrameColTickCounter != 0)
                        entity.FrameColTickCounter--;
                }
            }

            //displays debug records here

        }

        void ProcessDestroyedEntities()
        {
            int max = 0;
            for (int dex = 0;dex<game.Entities.Length;dex++)
            {
                var entity = game.Entities[dex];
                if (entity.Status == 4)
                {
                    //zero out the properties
                    entity = new SpriteInstance();
                    entity.EntityRefId = -1;
                    game.Entities[dex] = entity;
                    entity.Index = dex;
                }
                if (entity.Status != 0)
                {
                    max = dex;
                }
            }

            game.MaxEntity = max;
        }

        void AddToLists()
        {
            game.ToProcessesCount = 0;
            game.ToCollideCount = 0;
            game.ToRenderCount = 0;

            if (game.MaxEntity < 0)
                return;

            foreach(var entity in game.Entities)
            {
                //processable
                if (entity.Status -2 < 2 && entity._20 == 0)
                {
                    game.ToProcessList[game.ToProcessesCount++] = entity;
                }

                //collidable
                if ((entity.Flags & 0x80) != 0
                    && (entity.AnimFlags & 0x80) == 0
                    && entity.PlatformEntity == null)
                {
                    game.ToCollideList[game.ToCollideCount++] = entity;
                }

                //renderable
                if (entity.Status-2 < 2 && (entity.DamagedTickCounter & 3) != 3)//flicker effect, every 3rd frame when being damaged
                {
                    game.ToRenderList[game.ToRenderCount++] = entity;
                }
            }

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
                    p.EventTrigger = medex;
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
