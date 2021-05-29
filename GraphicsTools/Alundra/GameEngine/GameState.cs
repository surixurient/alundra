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
        public int[] GameFlagsMap = new int[1024];
        public int[] GameFlagsGlobal = new int[1024];
        public int PlayerControlSetting;
        public int CamXPos;
        public int CamYPos;

        //int NumEntities;
        //64 max
        public List<SpriteInstance> Entities = new List<SpriteInstance>();
        public SpriteInstance PlayerEntity;


        public SpriteInstance[] GetEntityList = new SpriteInstance[128];


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
                                && (entity.UnknownBeforeZForce & 0x80) == 0
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
                    case 9://all entities besides player where entity.unknownbeforestatus [c] == ownerentity
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && entity.UnknownBeforeStatus == ownerEntity )
                            {
                                GetEntityList[numgot++] = entity;
                            }

                        }
                        return numgot;
                    case 10://all entities besides player where ownerentity.unknownbeforestatus [c] == entity
                        foreach (var entity in Entities.Skip(1))
                        {
                            if ((entity.Status - 1 < 2 || entity.Status == 3)
                                && ownerEntity.UnknownBeforeStatus == entity)
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

        public SpriteInstance ActivateEntity(SpriteInstance ownerEntity, int entityid, int flag)
        {
            var data = GetInitData(entityid);

            if (data == null)
            {
                return null;
            }

            if (flag == 0)
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

            if ((data.spritedir & 0x40) == 0 && flag == 0)
                return null;

            int outvar1, outvar2;
            bool isMapSprite = (data.spritedir & 0x80) != 0;
            var sprite = GetSpriteFromSpriteTable(isMapSprite, data.spritetableindex, out outvar1, out outvar2);

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

            //InitEntity(entity, ownerEntity, sprite, data, spritetable, entityid, x, y, z, 0, dir, outvar1, outvar2);

            return entity;
        }

        SpriteRecord GetSpriteFromSpriteTable(bool isMapSprite, int spritetableindex, out int outvar1, out int outvar2)
        {
            SpriteInfo si;
            if (isMapSprite)
            {
                si = gameMap.spriteinfo;
                outvar1 = 0;
                outvar2 = 0x20;
            }
            else
            {
                si = global.spriteinfo;
                outvar1 = 0xb;
                outvar2 = 0x60;
            }
            if (spritetableindex < 0)
                throw new Exception("Illegal Character Race!");
            if (spritetableindex >= si.spritetable.Length)
                throw new Exception("Illegal Character Race!");

            var sprite = si.sprites[spritetableindex];
            return sprite;
        }

        SpriteInstance GetNextAvailableEntity()
        {
            foreach (var entity in Entities)
            {
                if (entity.Status == 0)
                    return entity;
            }
            var newentity = new SpriteInstance();
            Entities.Add(newentity);
            return newentity;
        }






        //sprite stuff
        Dictionary<int, List<Bitmap>> CachedSprites;
        GameMap gameMap;
        GameMap global;
        
        public List<Bitmap> GetSpriteImages(SIImageSet imgset, int sheetmod=0, int palmod=0)
        {

            if (!CachedSprites.ContainsKey(imgset.imagesetid))
            {
                var list = new List<Bitmap>();
                for (int dex = 0; dex < imgset.numimages; dex++)
                    list.Add(gameMap.GenerateSpriteBitmap(imgset.images[dex], gameMap.spriteinfo.palettes[(imgset.images[dex].palette & 0x1f) + palmod], sheetmod));
                CachedSprites.Add(imgset.imagesetid, list);
            }


            return CachedSprites[imgset.imagesetid];
        }
    }
}
