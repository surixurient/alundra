using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsTools.Alundra
{
    public class GameState
    {
        public GameMap gameMap;
        public GameMap global;
        public GameState(GameMap global)
        {
            this.global = global;
        }
        public void LoadMap(GameMap map)
        {
            this.gameMap = map;


            //load MapEvents
            MapEvents = new List<MapEvent>();
            for (int dex = 0; dex < map.spriteinfo.mapevents.records.Length; dex++)
            {
                var record = map.spriteinfo.mapevents.records[dex];
                if (record != null)
                {
                    var me = new MapEvent { id = dex };
                    me.MapEventRecord = record;
                    me.ProgramB_Map = record.eventcodesbindex;
                    //TODO special logic if the eventcodesindex is 0
                    me.Entity = PlayerEntity;
                    MapEvents.Add(me);
                }
            }
        }

        public int Seed = 42;
        public int[] GameFlagsMap = new int[1024];
        public int[] GameFlagsGlobal = new int[1024];
        public short[] PlayerInput = new short[16];//no idea how many there are
        public int PlayerControlSetting;
        public int CamXPos;
        public int CamYPos;
        public bool BreakoutGameLoop;

        //for event processing
        public int ActiveEventCode, PrevEventCode, ActiveEventProgramType, ActiveEventProgIndex, ActiveEntityRefId;

        public int NumSprites;
        public SpriteRef[] SpriteRefs = new SpriteRef[2048];
        public SpriteEffect[] SpriteEffects = new SpriteEffect[0x80];

        public int EventProgsSet;//a prog was set by an event, main event handler will repond

        public int UnknownCounter = 0;
        //int NumEntities;
        public int MaxEntity = 0;//higest index entity that is activated
        //64 max
        public List<SpriteInstance> Entities = new List<SpriteInstance>();
        public SpriteInstance PlayerEntity;

        public SpriteInstance ActiveCollisionEntity;
        public SpriteInstance CamFollowEntity;

        public EventProgramState GlobalEventData = new EventProgramState();


        public SpriteInstance[] GetEntityList = new SpriteInstance[128];

        public int AList1Count = 0;
        public List<SpriteInstance> AList1 = new List<SpriteInstance>();

        public List<MapEvent> MapEvents = new List<MapEvent>();


        public SIEntityRecord GetInitData(int entityid)
        {
            //if (NumEntities)
            //todo impliment it
            return null;
        }
        private SIEntityRecord CheckValidEntityId(int entityid)
        {
            var rec = GetInitData(entityid);

            return rec;
        }

        //TODO: impliment/figure out the alterior hide code
        public void HideEntity(SpriteInstance entity)
        {
            //var something = entity._1b8;
            entity.Status = 4;
            /*entity._228 = -1;
            if (something != 0)
            {
                something[68] = 0;
                entity._1b8 = 0;
            }
            if (entity.PlatformEntity != null)
            {
                entity._2c = 0;
            }*/
        }

        public int TurnEntity(SpriteInstance entity, int turnCode)
        {
            int turndir = turnCode & 0x1f;
            int turntype = turnCode >> 5;
            if (turntype >= 8)
                return 0;
            switch(turntype)
            {
                case 1:
                    return (entity.TargetDir + turndir) & 0x1f;
                case 2:
                    return Helper.CardinalDirTable[turndir & 0x3];
                case 3:
                    var dfv = Helper.DirFromVector(PlayerEntity.XPos - entity.XPos, PlayerEntity.YPos - entity.YPos);
                    return (dfv + turndir) & 0x1f;
                case 4:
                    {
                        int i = Seed;
                        int val1 = (int)(i * 0x7d2b89dd);
                        int val2 = (int)(0xe06a02e7 + val1);
                        int val3 = (int)(((long)val2 * 4) >> 32);
                        Seed = val2;
                        var dir = Helper.CardinalDirTable[val3];//val3 here is a number between 0 and 3
                        return dir;
                    }
                case 5:
                    {
                        int i = Seed;
                        int val1 = (int)(i * 0x7d2b89dd);
                        int val2 = (int)(0xe06a02e7 + val1);
                        int val3 = (int)(((long)val2 * 0x20) >> 32);
                        Seed = val2;
                        return val2;
                    }
                case 6:
                    return (PlayerEntity.TargetDir + turndir) & 0x1f;
                case 7:
                    var ret = GetCardialDirToPlayer(entity);
                    if (ret != -1)
                        return (ret + turndir) & 0x1f;
                    break;
                case 0:
                    break;
            }
            return turndir;
        }

        public int GetCardialDirToPlayer(SpriteInstance entity)
        {
            if (entity == ActiveCollisionEntity)
                return -1;

            var difx = PlayerEntity.ModdedXPos - entity.ModdedXPos;

            if ((difx >= 0 && entity.Width < difx)
                || (difx < 0 && PlayerEntity.Width < -difx))
            {
                //checkx
                if (PlayerEntity.XPos < entity.XPos)
                    return 0x08;
                else
                    return 0x18;
            }
            else
            {
                //checky
                if (PlayerEntity.YPos < entity.YPos)
                    return 0x10;
                else
                    return 0x00;
            }

        }

        public int GetEntityFromRefId(SpriteInstance ownerEntity, int entityid)
        {
            int numgot = 0;
            if ((entityid & 0x80) == 0)
            {
                CheckValidEntityId(entityid);//calls getinitrecord which is a 20 byte datarecord SIEntityRecord
                foreach (var entity in Entities)
                {
                    if ((ownerEntity.Status-1<2 || ownerEntity.Status == 3) && entity.EntityRefId == entityid)
                    {
                        GetEntityList[numgot++] = entity;
                    }
                }
                return numgot;
            }
            else
            {
                var functionid = entityid & 0x7f;
                switch (functionid)
                {
                    case 0://get owner
                        GetEntityList[numgot++] = ownerEntity;
                        return numgot;
                    case 1://get player
                        GetEntityList[numgot++] = PlayerEntity;
                        return numgot;
                    case 2://get all entities
                        foreach (var entity in Entities)
                        {
                            if (entity.Status - 1 < 2 || entity.Status == 3)
                            {
                                GetEntityList[numgot++] = entity;
                            }
                        }
                        return numgot;
                    case 3://get all entities except player
                        foreach (var entity in Entities.Skip(1))
                        {
                            if (entity.Status - 1 < 2 || entity.Status == 3)
                            {
                                GetEntityList[numgot++] = entity;
                            }
                        }
                        return numgot;
                    case 4://all entities on the ground
                        foreach (var entity in Entities)
                        {
                            if ((ownerEntity.Status - 1 < 2 || ownerEntity.Status == 3)
                                && (entity.Flags & 0x80) != 0
                                && (entity.AnimFlags & 0x80) == 0
                                && entity.PlatformEntity == null)
                            {
                                GetEntityList[numgot++] = entity;
                            }
                        }
                        return numgot;
                    case 5://all entities besides player that the ownerentity is riding on
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && ownerEntity.RidingEntity == entity)
                            {
                                GetEntityList[numgot++] = entity;
                            }

                        }
                        return numgot;
                    case 6://all entities besides player that are riding on the ownerentity
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && entity.RidingEntity == ownerEntity)
                            {
                                GetEntityList[numgot++] = entity;
                            }

                        }
                        return numgot;
                    case 7://all entities besides player where ownerentity.xcollision? == entity
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && ownerEntity.XCollisionEntity == entity)
                            {
                                GetEntityList[numgot++] = entity;
                            }

                        }
                        return numgot;
                    case 8://all entities besides player where entity.xcollision? == ownerentity
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && entity.XCollisionEntity == ownerEntity)
                            {
                                GetEntityList[numgot++] = entity;
                            }

                        }
                        return numgot;
                    case 9://all entities besides player where entity.ownerentity [c] == ownerentity
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && entity.OwnerEntity == ownerEntity )
                            {
                                GetEntityList[numgot++] = entity;
                            }

                        }
                        return numgot;
                    case 10://all entities besides player where ownerentity.ownerentity [c] == entity
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && ownerEntity.OwnerEntity == entity)
                            {
                                GetEntityList[numgot++] = entity;
                            }

                        }
                        return numgot;
                    case 11://all entities besides player that are on a platform
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && entity.PlatformEntity != null)
                            {
                                GetEntityList[numgot++] = entity;
                            }

                        }
                        return numgot;
                }
            }

            return numgot;
        }

        public SpriteInstance ActivateEntity(SpriteInstance ownerEntity, int entityid, int forceactivate)
        {
            var data = GetInitData(entityid);

            if (data == null)
            {
                return null;
            }

            if (forceactivate == 0)
            {
                //if player is outside of the activation zone dont activate
                //  this is used when a map has multiple rooms, the activate zone is set to the room where
                //  the entity is, if the player loads in a different room then the entity wont activate
                if (PlayerEntity.XTile < data.minx)
                    return null;
                if (data.maxx < PlayerEntity.XTile)
                    return null;
                if (PlayerEntity.YTile < data.miny)
                    return null;
                if (data.maxy < PlayerEntity.YTile)
                    return null;
            }

            if ((data.spritedir & 0x40) == 0 && forceactivate == 0)
                return null;

            int addedtosheet, addedtopalette;
            bool isMapSprite = (data.spritedir & 0x80) != 0;
            var sprite = GetSpriteFromSpriteTable(isMapSprite, data.spritetableindex, out addedtosheet, out addedtopalette);

            if (sprite == null)
                return null;

            var entity = GetNextAvailableEntity();

            if (entity == null)
                return null;

            int x = (data.xpos * 12 + 12) << 16;
            int y = (data.xpos * 8 + 8) << 16;
            int z = data.height << 19;

            int[] dirtable = new[] { 0x00, 0x10, 0x08, 0x18 };
            var dir = dirtable[data.spritedir & 0x3];

            var spritetable = (int)data.spritetableindex;
            if ((data.spritedir & 0x80) != 0)
                spritetable += 0x100;

            InitEntity(entity, ownerEntity, sprite, data, spritetable, entityid, x, y, z, 0, dir, addedtosheet, addedtopalette);

            return entity;
        }

        public void InitEntity(SpriteInstance entity, SpriteInstance ownerentity, SpriteRecord sprite, SIEntityRecord initdata, int spritetableindex, int entityid, int x, int y, int z, int anim, int dir, int addedtosheet, int addedtopalette)
        {
            if (MaxEntity < entity.Index)
                MaxEntity = entity.Index;
            entity.OwnerEntity = ownerentity;

            if (ownerentity != null)
            {
                if (ownerentity.UnknownBeforeOwnerEntity != null)
                    entity.UnknownBeforeOwnerEntity = ownerentity.UnknownBeforeOwnerEntity;
                else
                    entity.UnknownBeforeOwnerEntity = ownerentity;
            }

            entity.Sprite = sprite;
            entity.EntityRecord = initdata;
            entity.SpriteTableIndex = spritetableindex;

            if (initdata!= null)
            {
                entity.EntityRefId = entityid;
            }
            else
            {
                entity.EntityRefId = -1;
            }

            entity.Status = 1;

            entity.UnknownAfterIndex = ++UnknownCounter;

            entity.CurAnim = ~anim;
            entity.CurDir = ~dir;
            entity.TargetAnim = anim;
            entity.TargetDir = dir;
            entity.Flags = sprite.header.moreflags | sprite.header.canpickup << 8 | sprite.header.flags_portrait_shadowtype << 16;

            entity.UnkownBeforeThrowType = 0;
            entity.SpriteU4 = sprite.header.u4;
            entity.ThrowType = sprite.header.throwtype;
            entity.SpriteU6 = sprite.header.u6;
            entity.BreakSound = sprite.header.breaksound;
            entity.SpriteU8 = sprite.header.u8;

            entity.AddedToSheet = addedtosheet;
            entity.AddedToPalette = addedtopalette;

            //TODO impliment this unknown thing that has hp on it
            //var ret = getunknownthingfromtableindex(spritetableindex);
            //entity.unkonwnthing[1c4] = ret;
            //entity.HP = ret.hp;
            //entity.MaxHp = ret.hp;

            InitCodePrograms(entity);

            InitEntityDimensions(entity, sprite.header.xmod, sprite.header.ymod, sprite.header.zmod, sprite.header.width, sprite.header.depth, sprite.header.height);

            entity.XPos = x;
            entity.YPos = y;
            entity.ZPos = (z - entity.ZMod) + 1;
            
            UpdateAnims(entity);

            entity.ModdedXPos = entity.XPos + entity.XMod;
            entity.ModdedYPos = entity.YPos + entity.YMod;
            entity.ModdedZPos = entity.ZPos + entity.ZMod;

            var zhit = CollideWithMap(entity);

            entity.ZMapCollision = zhit;
            if (zhit+1 >= entity.ZPos)
            {
                entity.ZPos = zhit + 1;
                entity.ModdedXPos = entity.XPos + entity.XMod;
                entity.ModdedYPos = entity.YPos + entity.YMod;
                entity.ModdedZPos = entity.ZPos + entity.ZMod;
            }

            UpdateTile(entity);

            //TODO impliment this, maybe its contents
            //UpdateUnknown(entity);
        }

        //TODO all the slope stuff
        public void UpdateTile(SpriteInstance entity)
        {
            //set of variables set by certain special frames of animation
            if (entity._1d4 != 0)
            {
                entity._1fc = entity.XPos + entity._208;
                entity._200 = entity.YPos + entity._20c;
                entity._204 = entity.ZPos + entity._210;
            }

            entity.ZTile = entity.ZPos >> 20; //(z >> 16) / 16
            entity.XTile = (entity.XPos >> 16) / 24;
            entity.YTile = entity.YPos >> 20;

            var hitz = CollideOnEntitiesZ(entity);
            int tohit;
            entity.ZEntityCollision = hitz;
            entity.CollidedWithEntityZ = hitz < entity.ZPos ? 0 : 1;
            if ((entity.Flags & 0x100) != 0)
            {
                tohit = 0xe00;
                int[] somevals = new int[4];
                for (int dex=0;dex<4;dex++)
                {
                    var tl = entity.MapTiles[dex];
                    var fullval = tl.walkability | tl.groundproperty << 8 | tl.slope << 16 | tl.height << 24;
                    if (entity.MapHeights[dex]+1 == entity.ModdedZPos)
                    {
                        
                        //var val = (tl.groundproperty & 0xe) << 8;
                        if ((fullval & 0xe00) < tohit)
                        {
                            somevals[dex] = fullval;
                            tohit = (fullval & 0xe00);
                        }
                    }
                    else
                    {
                        somevals[dex] = 0;
                        tohit = 0;
                    }
                }

                entity._180 = somevals[0] | somevals[1] | somevals[2] | somevals[3];
                entity._184 = somevals[0] & somevals[1] & somevals[2] & somevals[3];

                var tilex = entity.XTile;

                if (tilex > 0)
                {
                    if (tilex >= 0x34)
                        tilex = 0x33;
                }
                else
                {
                    tilex = 0;
                }
                var tiley = entity.YTile;
                if (tiley > 0)
                {
                    if (tiley >= 0x3c)
                        tiley = 0x3b;
                }
                else
                {
                    tiley = 0;
                }

                var tile = gameMap.map.maptiles[tilex + tiley * 52];
                var fullval2 = tile.walkability | tile.groundproperty << 8 | tile.slope << 16 | tile.height << 24;
                var height = (int)(fullval2 & 0xff000000 >> 4) + 1;
                var r3 = height ^ entity.ModdedZPos;
            }
            else
            {
                tohit = 0;
                entity._180 = 0;
                entity._184 = 0;
            }

            //all that slope code is for setting this value
            entity._188 = 0;

            var prevtohit = entity._18c;
            entity._18c = tohit;
            entity._190 = prevtohit;
        }

        public int CollideOnEntitiesZ(SpriteInstance entity)
        {
            var collision = entity.ZMapCollision + 1;
            if ((entity.Flags & 0x80) == 0)
                return collision;
            if ((entity.AnimFlags & 0x80) != 0)
                return collision;
            if (entity.PlatformEntity != null)
                return collision;

            if (AList1Count <= 0)
                return collision;

            foreach (var checkme in AList1)
            {
                if (checkme == entity)
                    continue;
                
                if (checkme.ModdedZPos + checkme.Height >= entity.ModdedZPos
                 || checkme.ModdedZPos + checkme.Height < collision)
                    continue;

                if (checkme.ModdedXPos-entity.ModdedXPos >= 0)
                {
                    if (checkme.ModdedXPos - entity.ModdedXPos >= entity.Width + 1)
                        continue;
                }
                else
                {
                    if (entity.ModdedXPos - checkme.ModdedXPos >= checkme.Width + 1)
                        continue;
                }

                if (checkme.ModdedYPos-entity.ModdedYPos >= 0)
                {
                    if (checkme.ModdedYPos - entity.ModdedYPos < entity.Depth + 1)
                        collision = checkme.ModdedZPos + checkme.Height;
                }
                else
                {
                    if (entity.ModdedYPos-checkme.ModdedYPos < checkme.Depth+1)
                        collision = checkme.ModdedZPos + checkme.Height;
                }

            }
            return collision;
        }

        public int CollideWithMap(SpriteInstance entity)
        {
            int[] xs = new int[4];
            int[] ys = new int[4];
            int x1 = (entity.XPos + entity.XMod) >> 16;
            int x2 = (entity.XPos + entity.XMod + entity.Width) >> 16;
            int y1 = (entity.YPos + entity.YMod) >> 16;
            int y2 = (entity.YPos + entity.YMod + entity.Depth) >> 16;
            xs[0] = x1;
            ys[0] = y1;
            xs[1] = x2;
            ys[1] = y1;
            xs[2] = x1;
            ys[2] = y2;
            xs[3] = x2;
            ys[3] = y2;
            int highest = 0;
            int slopes_hit = 0;
            for (int dex=0;dex<4;dex++)
            {
                var x = xs[dex];
                var y = ys[dex];
                var tilex = x / 24;
                if (tilex > 0)
                {
                    if (tilex >= 0x34)
                        tilex = 0x33;
                    tilex = tilex << 16;
                    tilex = tilex >> 16;
                }
                else
                {
                    tilex = 0;
                }
                var tiley = y / 16;
                if (tiley > 0)
                {
                    if (tiley >= 0x3c)
                        tiley = 0x3b;
                    tiley = tiley << 16;
                    tiley = tiley >> 16;
                }
                else
                {
                    tiley = 0;
                }
                //int offset = (tilex * 8) + (tiley * 8 * 52);
                var tile = gameMap.map.maptiles[tiley * 52 + tilex];
                entity.MapTiles[dex] = tile;
                int height;
                if ((tile.slope & 0x3) != 0)
                {
                    height = tile.height * 16;//puts it in pixels
                    //bunch of slope stuff
                    switch(tile.slope & 0x3)
                    {
                        case 1:
                            if ((slopes_hit & 6) != 0)//it already hit 2 or 3
                            {
                                height += 0x10;//add a tile;
                            }
                            else
                            {
                                var my = ys[dex];
                                var result = height + 0x10;
                                var my2 = my;
                                if (my < 0)
                                    my2 = my + 15;
                                my2 = my2 / 16;
                                my2 = my2 * 16;
                                var remainder = my - my2;
                                height = result - remainder;
                            }
                            slopes_hit |= 1;
                            break;
                        case 2:
                            if ((slopes_hit & 5) != 0)//it already hit 1 or 3
                            {
                                height += 0x10;//add a tile;
                            }
                            else
                            {
                                var mx = xs[dex];
                                var mx2 = mx / 24;
                                mx2 = mx2 * 24;
                                var remainder = mx - mx2;
                                remainder = 0x17 - remainder;

                                int result = (int)(((float)remainder / 0x18) * 0x10);
                                /*var result = (int)((mx * (long)0x2aaaaaab)>>32);//get the high dword
                                int neg = result >> 31;
                                int res2 = result >> 2;//divide by 4
                                res2 = res2 - neg;
                                res2 = res2 * 3;
                                res2 = mx - res2;
                                res2 = 0x17 - res2;
                                res2 = res2 * 4;
                                //result = 0x236d4[res2];some lookuptable of heights based on width*/
                                height += result;
                            }
                            slopes_hit |= 2;
                            break;
                        case 3:
                            if ((slopes_hit & 3) != 0)//it already hit 1 or 2
                            {
                                height += 0x10;
                            }
                            else
                            {
                                var mx = xs[dex];
                                var mx2 = mx / 24;
                                mx2 = mx2 * 24;
                                var remainder = mx - mx2;
                                //remainder = 0x17 - remainder;

                                int result = (int)(((float)remainder / 0x18) * 0x10);
                                /*var result = (int)((mx * (long)0x2aaaaaab) >> 32);//get the high dword
                                int neg = result >> 31;
                                int res2 = result >> 2;//divide by 4
                                res2 = res2 - neg;
                                res2 = res2 * 3;
                                res2 = mx - res2;
                                //res2 = 0x17 - res2; (only diff with other slope is subtracting it from 23, which is tilewidth-1)
                                res2 = res2 * 4;
                                //result = 0x236d4[res2];some lookuptable*/
                                height += result;
                            }
                            slopes_hit |= 4;
                            break;
                    }

                    height = height << 16;//shift it over to fixed float
                }
                else
                {
                    height = (tile.height*16) << 16;//put in pixels then shift over to fixed float
                }

                entity.MapHeights[dex] = height;
                if (highest < height)
                    highest = height;
            }
            return highest;
        }

        static int[] FrameDexTable = new int[]{
0x00000000,
0x00000000,
0x00000002,
0x00000001,
0x00000001,
0x00000001,
0x00000003,
0x00000000,
0x00000000,
0x00000000,
0x00000002,
0x00000001,
0x00000001,
0x00000001,
0x00000003,
0x00000000,
0x00000000,
0x00000002,
0x00000002,
0x00000002,
0x00000001,
0x00000003,
0x00000003,
0x00000003,
0x00000000,
0x00000002,
0x00000002,
0x00000002,
0x00000001,
0x00000003,
0x00000003,
0x00000003,
};

        public void UpdateAnims(SpriteInstance entity)
        {
            //TODO
            var dirdex = ((entity.TargetDir + 2) & 0x1c) >> 2;
            var fdex = FrameDexTable[dirdex + (entity.FrameDex << 3)];
            SIFrame frame = null;
            entity.AppliedZForce = 0;
            if (entity.TargetAnim != entity.CurAnim
                || fdex!= entity.FrameDex)
            {
                entity.AnimSet = entity.Sprite.animsets[entity.TargetAnim];
                entity.FrameDex = fdex;
                //SIAnimSet se;
                //se.animoffsets[]

                entity.NextFrameDelay = 0;
                entity.CurAnim = entity.TargetAnim;
                //TODO: look into if these indexes are correct
                frame = entity.AnimSet.preloaded_anims[entity.TargetDir].frames[fdex];

                entity.FirstFrame = frame;
                entity.Frame = frame;

                entity.AppliedZForce = entity.AnimSet.speed;

                entity.WierdNextFrameDelayFlag = 0;
                entity.AnimFlags = entity.AnimSet.flags;
                //TODO: figure this part out
                /*var numanims = entity._1c4.numanims;
                int specialanim = 0;
                if (numanims != 0)
                {
                    if (entity.CurAnim + 1 < numanims)
                    {
                       specialanim  = &entity._1c4.anims[entity.CurAnim + 1];
                    }
                    else
                    {
                        specialanim = &entity._1c4.anims[0];
                    }
                }
                entity._1c8 = specialanim;*/

                int sfx = entity.AnimSet.sfx;
                if ((entity.AnimSet.flags & 0x20) != 0)
                {
                    sfx += 0x100;
                }
                //TODO:enable sfx
                //PlaySoundEffect(sfx);
            }
            else
            {
                if (--entity.NextFrameDelay != 0)
                    return;
                //time to change the frame
                frame = entity.Frame;
            }
            do
            {
                //if it has a next frame
                if ((frame.delay & 0x80) != 0)
                {
                    entity.NextFrameDelay = frame.delay & 0x7f;
                    //TODO: better way to do this
                    entity.Frame = entity.AnimSet.preloaded_anims[entity.TargetDir].frames[entity.FrameDex + 1];

                    if (frame.unknown != -1)
                    {
                        //TODO load up the unknown data,
                    }
                    else
                    {
                        entity._1d4 = 0;
                    }

                    if (frame.imagesetpointer != -1)
                    {
                        entity.SpriteRef.Images = frame.images.images;
                        entity.SpriteRef.DepthSortVal = frame.images.unknown;
                        entity.SpriteRef.NumImages = frame.images.numimages;
                        return;
                    }
                    entity.SpriteRef.Images = null;
                    entity.SpriteRef.DepthSortVal = 0;
                    entity.SpriteRef.NumImages = 0;
                    return;
                }

                if (frame.delay != 0)
                {
                    if (frame.delay != 1)
                    {
                        //this is a bad state, output debug info
                        throw new Exception("this is a bad animation state");
                    }
                }
                else
                {//frame.delay = 0, non repeating animation?
                    if ((frame.unknown & 0x80) != 0)//why, it doesnt relaly make sense
                    {
                        entity.NextFrameDelay = 0x7fffffff;//what will this mean
                        entity.WierdNextFrameDelayFlag = 1;
                        return;
                    }
                    fdex = entity.FrameDex;
                    entity.TargetAnim = frame.unknown & 0xff;
                    entity.AnimCompleteCounter++;////we get here when the animation is nonrepeating and is finished, so it switches back to some other animation
                                                 //call recursivly?
                    UpdateAnims(entity);
                }

                entity.AnimCompleteCounter++;
                entity.Frame = entity.FirstFrame;//the anim repeats

            } while (true);//will this ever be an infinite loop
        }

        public void InitEntityDimensions(SpriteInstance entity, int xmod, int ymod, int zmod, int width, int depth, int height)
        {
            entity.NegXMod = -(xmod << 16);
            entity.NegYMod = -(ymod << 16);
            entity.XMod = (xmod << 16);
            entity.YMod = (ymod << 16);
            entity.ZMod = (zmod << 16);

            entity.ScreenClipX = 0x4e00000 - ((xmod + width) << 16);
            entity.ScreenClipY = 0x3c00000 - ((ymod + depth) << 16);
            entity.ScreenClipZ = 0x7800000 - ((zmod + height) << 16);

            if (width != 0)
            {
                entity.Width = (width << 16) - 1;
            }
            else
            {
                entity.Width = 0;
            }

            if (depth != 0)
            {
                entity.Depth = (depth << 16) - 1;
            }
            else
            {
                entity.Depth = 0;
            }

            if (height != 0)
            {
                entity.Height = (height << 16) - 1;
            }
            else
            {
                entity.Height = 0;
            }
        }

        public void InitCodePrograms(SpriteInstance entity)
        {
            entity.EntitySelf = entity;
            if (entity.EntityRecord != null)
            {
                entity.Program_Indexes[Helper.PROGRAM_A_LOAD] = entity.EntityRecord.eventcodesa_load_index;
                entity.Program_Indexes[Helper.PROGRAM_B_MAP] = entity.EntityRecord.eventcodesb_unknown_index;
                entity.Program_Indexes[Helper.PROGRAM_C_TICK] = entity.EntityRecord.eventcodesc_tick_index;
                entity.Program_Indexes[Helper.PROGRAM_D_TOUCH] = entity.EntityRecord.eventcodesd_touch_index;
                entity.Program_Indexes[Helper.PROGRAM_E_UNKNOWN] = entity.EntityRecord.eventcodese_unknown_index;
                entity.Program_Indexes[Helper.PROGRAM_F_INTERACT] = entity.EntityRecord.eventcodesf_interact_index;
            }
        }

        SpriteRecord GetSpriteFromSpriteTable(bool isMapSprite, int spritetableindex, out int addedtosheet, out int addedtopallette)
        {
            SpriteInfo si;
            if (isMapSprite)
            {
                si = gameMap.spriteinfo;
                addedtosheet = 0;
                addedtopallette = 0x20;
            }
            else
            {
                si = global.spriteinfo;
                addedtosheet = 0xb;
                addedtopallette = 0x60;
            }
            if (spritetableindex < 0)
                throw new Exception("Illegal Character Race!");
            if (spritetableindex >= si.spritetable.Length)
                throw new Exception("Illegal Character Race!");

            var sprite = si.sprites[spritetableindex];
            return sprite;
        }

        public SpriteEffectRecord GetEffectSpriteFromSpriteTable(bool isMapSprite, int spritetableindex, out int addedtosheet, out int addedtopallette)
        {
            SpriteInfo si;
            if (isMapSprite)
            {
                si = gameMap.spriteinfo;
                addedtosheet = 0;
                addedtopallette = 0x20;
            }
            else
            {
                si = global.spriteinfo;
                addedtosheet = 0xb;
                addedtopallette = 0x60;
            }
            if (spritetableindex >= 0 && spritetableindex < si.spritetable.Length)
                return si.spriteeffects[spritetableindex];

            return null;
        }

        public SpriteEffect GetNextAvailableEffect()
        {
            foreach(var effect in SpriteEffects)
            {
                if (effect.Status == 0)
                    return effect;
            }
            return null;
        }

        public void InitEffect(SpriteEffect effect, MapEffectRecord mapEffectRecord, int mapeffectid, int effecttype, byte ismapeffect, byte effectid, byte animid, int x, int y, int z)
        {
            //initialize
            effect.MapEffectRecord = null;
            effect.SpriteEffectRecord = null;
            effect.SpriteRef = new SpriteRef();
            effect.AddToSheet = 0;
            effect.AddToSheet = 0;
            effect.MapEffectId = 0;
            effect.EffectType = 0;
            effect.EntityRef = null;
            effect.X = 0;
            effect.Y = 0;
            effect.Z = 0;
            effect.XOff = 0;
            effect.YOff = 0;
            effect.ZOff = 0;
            effect.XMod = 0;
            effect.YMod = 0;
            effect.ZMod = 0;
            effect.DepthSortMod = 0;
            effect.DepthSortVal = 0;
            effect.Status = 0;
            effect.TargetIsMapSprite = 0;
            effect.CurIsMapSprite = 0;
            effect.TargetSpriteTableIndex = 0;
            effect.CurSpriteTableIndex = 0;
            effect.TargetAnim = 0;
            effect.CurAnim = 0;
            effect.Frame = null;
            effect.FirstFrame = null;
            effect.Delay = 0;
            effect.DestroyFlag = 0;

            effect.animdex = 0;


            effect.MapEffectRecord = mapEffectRecord;
            if (mapEffectRecord != null)
            {
                effect.MapEffectId = mapeffectid;
            }
            else
            {
                effect.MapEffectId = -1;
            }

            effect.Status = 2;
            effect.CurSpriteTableIndex = (byte)~effectid;
            effect.TargetIsMapSprite = ismapeffect;
            effect.CurIsMapSprite = (byte)~ismapeffect;
            effect.TargetSpriteTableIndex = effectid;
            effect.TargetAnim = animid;
            effect.CurAnim = (byte)~animid;
            effect.X = x;
            effect.Y = y;
            effect.Z = z;
        }

        public MapEffectRecord GetMapEffectRecord(int id, bool checkBoundingBox)
        {
            if (id < gameMap.spriteinfo.mapeffectrecords.Length)
            {
                var record = gameMap.spriteinfo.mapeffectrecords[id];
                if (checkBoundingBox)
                {
                    var p = PlayerEntity;
                    if (p.XTile < record.x1 || p.XTile > record.x2
                        || p.YTile < record.y1 || p.YTile > record.y2)
                        return null;
                }
                
                return record;
            }
            return null;
        }

        public SpriteEffect CreateEffect_Type0(byte ismapeffect, byte effectid, byte animid, int x, int y, int z)
        {
            var effect = GetNextAvailableEffect();

            if (effect != null)
            {
                InitEffect(effect, null, -1, 0, ismapeffect, effectid, animid, x, y, z);
                return effect;
            }
            return null;
        }


        public SpriteEffect CreateEffect_Type1(byte ismapeffect, byte effectid, byte animid, SpriteInstance entity, int depthsortmod, int xoff, int yoff, int zoff)
        {
            var effect = GetNextAvailableEffect();

            if (effect != null)
            {
                InitEffect(effect, null, -1, 1, ismapeffect, effectid, animid, entity.XPos, entity.YPos, entity.ZPos);
                effect.EntityRef = entity;
                effect.DepthSortMod = depthsortmod;
                effect.XOff = xoff;
                effect.YOff = yoff;
                effect.ZOff = zoff;
                return effect;
            }
            return null;
        }

        public SpriteEffect CreateEffect_Type3(byte ismapeffect, byte effectid, byte animid, SpriteInstance entity, int depthsortmod, int x, int y, int z)
        {
            var effect = GetNextAvailableEffect();

            if (effect != null)
            {
                InitEffect(effect, null, -1, 3, ismapeffect, effectid, animid, x, y, z);
                effect.EntityRef = entity;
                effect.DepthSortMod = depthsortmod;
                return effect;
            }
            return null;
        }

        public SpriteEffect CreateEffect_MapType(byte mapeffectid, bool checkBoundingbox)
        {
            var record = GetMapEffectRecord(mapeffectid, checkBoundingbox);
            if (record != null)
            {
                if (!checkBoundingbox && (record.flags & 0x40) == 0)
                    return null;
                var effect = GetNextAvailableEffect();

                if (effect != null)
                {
                    InitEffect(effect, record, mapeffectid, 0,
                        (byte)((record.flags & 0x80) >> 7), record.effectid, record.animid,
                        ((record.x * 12) + 12) << 16, ((record.y * 8) + 8) << 16, record.z << 19);

                    return effect;
                }
            }
            return null;
        }









        SpriteInstance GetNextAvailableEntity()
        {
            foreach (var entity in Entities)
            {
                if (entity.Status == 0)
                    return entity;
            }
            var newentity = new SpriteInstance { Index = Entities.Count + 1 };
            Entities.Add(newentity);
            return newentity;
        }






        //sprite stuff
        //Dictionary<int, List<Bitmap>> CachedSprites;
        
        
        /*public List<Bitmap> GetSpriteImages(SIImageSet imgset, int sheetmod=0, int palmod=0)
        {

            if (!CachedSprites.ContainsKey(imgset.imagesetid))
            {
                var list = new List<Bitmap>();
                for (int dex = 0; dex < imgset.numimages; dex++)
                    list.Add(gameMap.GenerateSpriteBitmap(imgset.images[dex], gameMap.spriteinfo.palettes[(imgset.images[dex].palette & 0x1f)]));
                CachedSprites.Add(imgset.imagesetid, list);
            }


            return CachedSprites[imgset.imagesetid];
        }*/
    }
}
