using GraphicsTools.Alundra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsTools.Alundra
{
    public class EventHandlers
    {
        GameState gameState;
        System.IO.BinaryReader datasReader;//load this for the handler, keep it open?
        public EventHandlers(GameState gameState, System.IO.BinaryReader datasReader)
        {
            this.gameState = gameState;
        }

        public int __Unknown_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] eventcode)
        {
            Debug.WriteLine("Data Logic Error!");
            return 0;
        }
        public int _02_Goto_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            short offset = (short)(code[exp + 1] + (code[exp + 2] << 8));

            return offset;
        }

        public int _03_BranchIfTrue_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if (eventData.tickprogram.logicResult == 0)
                return 3;

            short offset = (short)(code[exp + 1] + (code[exp + 2] << 8));

            return offset;
        }

        public int _04_BranchIfFalse_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if (eventData.tickprogram.logicResult != 0)
                return 3;

            short offset = (short)(code[exp + 1] + (code[exp + 2] << 8));

            return offset;
        }

        public int _05_FlagOn_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if (eventData.tickprogram.logicResult != 0)
                return 3;

            int flagdata = (code[exp + 1] + (code[exp + 2] << 8));
            //int flag = (flagdata >> 3) & 0xffc;
            int flag = (flagdata >> 5) & 0x3ff;
            int[] flags;
            //if the mapflag bit is set
            if ((flagdata & 0x8000) != 0)
            {
                flags = gameState.GameFlagsMap;
            }
            else//otherwise its a global flag
            {
                flags = gameState.GameFlagsGlobal;
            }

            var bittoset = flagdata & 0x1f;

            //turn on the bit for this flag
            flags[flag] |= 1 << bittoset;

            return 3;
        }

        public int _06_FlagOff_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if (eventData.tickprogram.logicResult != 0)
                return 3;

            int flagdata = (code[exp + 1] + (code[exp + 2] << 8));
            //int flag = (flagdata >> 3) & 0xffc;
            int flag = (flagdata >> 5) & 0x3ff;
            int[] flags;
            //if the mapflag bit is set
            if ((flagdata & 0x8000) != 0)
            {
                flags = gameState.GameFlagsMap;
            }
            else//otherwise its a global flag
            {
                flags = gameState.GameFlagsGlobal;
            }

            var bittoset = flagdata & 0x1f;

            //turn off the bit for this flag
            flags[flag] &= ~(1 << bittoset);

            return 3;
        }

        public int _07_CheckEntityInArea_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            var entityid = code[exp + 1];
            int x1 = code[exp + 2];
            int x2 = code[exp + 3];
            int y1 = code[exp + 4];
            int y2 = code[exp + 5];
            int z1 = code[exp + 6];
            int z2 = code[exp + 7];
            int numentities = gameState.GetEntityFromRefId(entity, entityid);
            for (int dex = 0; dex < numentities; dex++)
            {
                var checkme = gameState.GetEntityList[dex];
                if (checkme.XTile >= x1 && checkme.XTile <= x2
                    && checkme.YTile >= y1 && checkme.YTile <= y2
                    && checkme.ZTile >= z1 && checkme.ZTile <= z2)
                {
                    eventData.tickprogram.logicResult = 1;
                    return 8;
                }
            }

            eventData.tickprogram.logicResult = 0;

            return 8;
        }

        public int _08_Turn_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.TargetDir = (entity.TargetDir + code[exp + 1]) & 0x1f;
            return 2;
        }

        public int _09_SetDir_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.TargetDir = code[exp + 1] & 0x1f;
            return 2;
        }

        public int _0a_Reverse_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.TargetDir = (entity.TargetDir + 0x10) & 0x1f;
            return 1;
        }

        /*public int _0c_SetDirWithMath_Handler(SpriteInstance entity, SpriteInstance entityself, int exp, EventData eventData, byte[] code)
        {
            int i = gameState.SomeVariable;
            int val1 = (int)(i * 0x7d2b89dd);
            int val2 = (int)(0xe06a02e7 + val1);
            int val3 = (int)((val2 * 4) >> 32);
            var dir = gameState.SomeLookupTable[val2];
            gameState.SomeVariable = val2;
            entity.TargetDir = dir;
            return 1;
        }*/

        public int _0d_Dialog_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if ((entity.Flags & 0x800000) != 0)//has portrait
            {
                //TODO get it from memory, not disk
                SIImageSet portrait = entity.Sprite.GetPortraitImageset(datasReader);
                var img = portrait.images[0];
                var bmps = gameState.GetSpriteImages(portrait);
                var bmp = bmps[0];
                //WrapsDialogSetupPortrait(entity.XPos, entity.YPos, entity.ZPos, gameState.CamXPos, gameState.CamYPos, img.sx, img.sy, img.swidth, img.sheight, bmp);
            }
            //SetName(entity.NameId);

            //var ret = SetText(code[exp + 1], code[exp + 2]);

            //if (ret > 0)
            //    return 3;
            return 0;
        }

        public int _10_LoseControl_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            gameState.PlayerControlSetting |= 0x4;
            return 1;
        }

        public int _11_GainControl_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            gameState.PlayerControlSetting &= ~0x4;
            return 1;
        }

        public int _12_PlaySound1_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            //throw new Exception("impliment this one!");
            int soundid = code[exp + 1];
            return 2;
        }

        public int _15_ResetZPos_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if (entity.EntityRecord == null)
            {
                Debug.Print("No InitData");
            }

            entity.ZPos = (entity.EntityRecord.height * 8 - entity.ZMod) << 16;
            return 1;
        }

        public int _16_GravityOn_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.Flags |= 0x100;
            return 1;
        }

        public int _17_GravityOff_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.Flags &= ~0x100;
            return 1;
        }

        public int _19_Deactivate_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.Status = 3;
            return 1;
        }

        public int _1a_SetAnim_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.TargetAnim = code[exp + 1];
            return 2;
        }

        //sets zforce
        public int _1b_Fly_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            int force = code[exp + 1] + (code[exp + 2] << 8);
            force = force << 16;//sign extend
            force = force >> 8;//get it to the correct multiple
            entity.ZForce = force;
            return 2;
        }

        public int _1c_WaitAnim_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if (exp != eventData.tickprogram.evttickprog)
            {
                eventData.tickprogram.evttickprog = exp;
                eventData.tickprogram.evtvars[0] = 0;
                entity.UnknownBeforeBeforeZForce = 0;
                return 0;
            }

            if (entity.UnknownBeforeBeforeBeforeZForce != 0)
            {
                entity.CurAnim = ~entity.TargetAnim;
            }

            if (entity.UnknownBeforeBeforeBeforeZForce != 0 || entity.UnknownBeforeBeforeZForce != 0)
            {
                eventData.tickprogram.evtvars[0]++;
                entity.UnknownBeforeBeforeZForce = 0;
            }


            var towait = code[exp + 1];
            if (eventData.tickprogram.evtvars[0] >= towait)
                return 2;
            else
                return 0;
        }

        //collision ends it
        public int _1d_WaitAnim2_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            var ret = _1c_WaitAnim_Handler(entity, entityself, exp, eventData, code);

            if (ret != 0)
                return ret;

            if (entity.ForceAdjusted != 0)
                return 2;

            return 0;
        }

        //wait until they have walked a certain distance, collision pauses the walk
        public int _1e_WaitWalk_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if (exp != eventData.tickprogram.evttickprog)
            {
                eventData.tickprogram.evttickprog = exp;
                eventData.tickprogram.evtvars[0] = entity.XPos;
                eventData.tickprogram.evtvars[1] = entity.YPos;
                return 0;
            }

            var x = Math.Abs(eventData.tickprogram.evtvars[0] - entity.XPos)>>16;
            var y = Math.Abs(eventData.tickprogram.evtvars[1] - entity.YPos)>>16;

            int distance = code[exp + 1] | (code[exp + 2] << 8);

            if (x >= distance || y >= distance)
                return 3;
            
            return 0;
        }

        //wait until they have walked a certain distance, collision ends the walk
        public int _1f_WaitWalk2_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            var ret = _1e_WaitWalk_Handler(entity, entityself, exp, eventData, code);

            if (ret != 0)
                return ret;

            if (entity.ForceAdjusted != 0)
                return 3;

            return 0;
        }

        //wait until some force acts on the entity, like it bumps into something
        public int _24_WaitForceAdjusted_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            if (entity.ForceAdjusted > 0)
                return 1;

            return 0;
        }

        public int _27_FacePlayer_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.TargetDir = Helper.DirFromVector(gameState.PlayerEntity.XPos - entity.XPos, gameState.PlayerEntity.YPos - entity.YPos);
            return 1;
        }

        public int _28_Flag2On_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.Flags |= 0x8;
            return 1;
        }

        public int _29_Flag2Off_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.Flags &= ~0x8;
            return 1;
        }

        public int _2a_Flag3On_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.Flags |= 0x1;
            return 1;
        }

        public int _2b_Flag3Off_Handler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] code)
        {
            entity.Flags &= ~0x1;
            return 1;
        }

    }

    public delegate int ScriptEventHandler(SpriteInstance entity, SpriteInstance entityself/*?*/, int exp, EventData eventData, byte[] eventcode);



    public static class Helper
    {
        public static int SignExtendWord(int i)
        {
            if ((i & 0x8000) == 0)
                return 0x0000FFFF & i;
            else 
                return (int)(0xFFFF0000 | i);
        }

        public static int DirFromVector(int x, int y)
        {
            int flipper = 0;
            if (y < 1)
                flipper = 2;
            if (x < 0)
                flipper++;

            if (x < 0)
                x = -x;
            if (y < 0)
                y = -y;

            int greatest = x;
            if (x < y)
                y = greatest;

            int div = 0;
            var val = Helper.DivTable[div];
            if (val < greatest)
            {
                do
                {
                    div++;
                    val = DivTable[div];
                } while (val < greatest);
            }
            x = x >> div;
            y = y >> div;

            var direction = (int)DirectionTable[y * 16 + x];

            var ret = direction;
            if (flipper == 1)
                ret = 8 - direction;
            else if (flipper == 2)
                ret = 0x18 - direction;
            else if (flipper == 3)
                ret = 8 + direction;
            else if (flipper == 0)
                ret = 0x18 + direction;

            return ret & 0x1f;
        }

        static short[] DirectionTable = new short[]{
0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
0x8,0x4,0x2,0x2,0x1,0x1,0x1,0x1,0x1,0x1,0x1,0x0,0x0,0x0,0x0,0x0,
0x8,0x6,0x4,0x3,0x2,0x2,0x2,0x1,0x1,0x1,0x1,0x1,0x1,0x1,0x1,0x1,
0x8,0x6,0x5,0x4,0x3,0x3,0x2,0x2,0x2,0x2,0x1,0x1,0x1,0x1,0x1,0x1,
0x8,0x7,0x6,0x5,0x4,0x3,0x3,0x3,0x2,0x2,0x2,0x2,0x2,0x2,0x1,0x1,
0x8,0x7,0x6,0x5,0x5,0x4,0x4,0x3,0x3,0x3,0x2,0x2,0x2,0x2,0x2,0x2,
0x8,0x7,0x6,0x6,0x5,0x4,0x4,0x4,0x3,0x3,0x3,0x3,0x2,0x2,0x2,0x2,
0x8,0x7,0x7,0x6,0x5,0x5,0x4,0x4,0x4,0x3,0x3,0x3,0x3,0x3,0x2,0x2,
0x8,0x7,0x7,0x6,0x6,0x5,0x5,0x4,0x4,0x4,0x3,0x3,0x3,0x3,0x3,0x2,
0x8,0x7,0x7,0x6,0x6,0x5,0x5,0x5,0x4,0x4,0x4,0x3,0x3,0x3,0x3,0x3,
0x8,0x7,0x7,0x7,0x6,0x6,0x5,0x5,0x5,0x4,0x4,0x4,0x4,0x3,0x3,0x3,
0x8,0x8,0x7,0x7,0x6,0x6,0x5,0x5,0x5,0x5,0x4,0x4,0x4,0x4,0x3,0x3,
0x8,0x8,0x7,0x7,0x6,0x6,0x6,0x5,0x5,0x5,0x4,0x4,0x4,0x4,0x4,0x3,
0x8,0x8,0x7,0x7,0x6,0x6,0x6,0x5,0x5,0x5,0x5,0x4,0x4,0x4,0x4,0x4,
0x8,0x8,0x7,0x7,0x7,0x6,0x6,0x6,0x5,0x5,0x5,0x5,0x4,0x4,0x4,0x4,
0x8,0x8,0x7,0x7,0x7,0x6,0x6,0x6,0x6,0x5,0x5,0x5,0x5,0x4,0x4,0x4,
};

        static uint[] DivTable = new uint[]{
0x0000000f,
0x0000001f,
0x0000003f,
0x0000007f,
0x000000ff,
0x000001ff,
0x000003ff,
0x000007ff,
0x00000fff,
0x00001fff,
0x00003fff,
0x00007fff,
0x0000ffff,
0x0001ffff,
0x0003ffff,
0x0007ffff,
0x000fffff,
0x001fffff,
0x003fffff,
0x007fffff,
0x00ffffff,
0x01ffffff,
0x03ffffff,
0x07ffffff,
0x0fffffff,
0x1fffffff,
0x3fffffff,
0x7fffffff,
0xffffffff,
};
    }
}
