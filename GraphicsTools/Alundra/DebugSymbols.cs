using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsTools.Alundra
{
    public static class DebugSymbols
    {

        public static string[] EntityVarOffsets = new string[GameMap.eventobject_size];
        public static Dictionary<uint, string> FunctionNames = new Dictionary<uint, string>();
        public static Dictionary<uint, string> GlobalVariableNames = new Dictionary<uint, string>();
        public static string[] MapNames = new string[502];

        public static uint Adjustment = 0 + 0xdc;

        public static void Init()
        {
            FunctionNames.Clear();
            FunctionNames.Add(0x36d30, "setridingentities");
            FunctionNames.Add(0x36e80, "setxyforces");
            FunctionNames.Add(0x36f64, "setadjustedxyforces");
            FunctionNames.Add(0x37040, "incrementforce");
            FunctionNames.Add(0x37078, "updateforces");
            FunctionNames.Add(0x374a8, "collidewithentitiesjumping");
            FunctionNames.Add(0x37640, "collidewithentities");
            FunctionNames.Add(0x377E0, "collidewithentities2");
            FunctionNames.Add(0x3797c, "collidewithmap");
            FunctionNames.Add(0x37c28, "updateridingentity");
            FunctionNames.Add(0x37eb8, "movez");
            FunctionNames.Add(0x38008, "movexy");
            FunctionNames.Add(0x38708, "moveentity");
            FunctionNames.Add(0x38800, "calcytile");
            FunctionNames.Add(0x38948, "processtile");
            FunctionNames.Add(0x38c44, "dophysics");


            NudgeAddresses();


            EntityVarOffsets[0x0] = "entityid";

            EntityVarOffsets[0x28] = "refentity";//platform entity
            EntityVarOffsets[0x30] = "refxoff";//platformxoff
            EntityVarOffsets[0x34] = "refyoff";//platformyoff
            EntityVarOffsets[0x38] = "refzoff";//platformzoff

            EntityVarOffsets[0x44] = "datasbinrecord";
            EntityVarOffsets[0x64] = "portraitsprite";
            EntityVarOffsets[0x68] = "name";
            EntityVarOffsets[0x6c] = "gravityflags";

            EntityVarOffsets[0x8c] = "targetdir";

            EntityVarOffsets[0x94] = "dir";
            EntityVarOffsets[0x98] = "framedex";
            EntityVarOffsets[0x9c] = "animset";
            EntityVarOffsets[0xa0] = "curframe";
            EntityVarOffsets[0xa4] = "nextframe";

            EntityVarOffsets[0xb8] = "zforce";//risefall speed
            EntityVarOffsets[0xbc] = "targetxforce";
            EntityVarOffsets[0xc0] = "targetyforce";
            EntityVarOffsets[0xc4] = "xforce";
            EntityVarOffsets[0xc8] = "yforce";
            EntityVarOffsets[0xcc] = "";
            EntityVarOffsets[0xd0] = "";
            EntityVarOffsets[0xd4] = "xforcestep";
            EntityVarOffsets[0xd8] = "yforcestep";
            EntityVarOffsets[0xdc] = "adjustedxforce";
            EntityVarOffsets[0xe0] = "adjustedyforce";
            EntityVarOffsets[0xe4] = "finalxforce";
            EntityVarOffsets[0xe8] = "finalyforce";
            EntityVarOffsets[0xec] = "finalzforce";

            EntityVarOffsets[0xf0] = "acceleration";
            EntityVarOffsets[0xf4] = "speed";
            EntityVarOffsets[0xf8] = "appliedzforce";
            EntityVarOffsets[0xfc] = "aboveabovexpos";
            EntityVarOffsets[0x100] = "aboveaboveypos";
            EntityVarOffsets[0x108] = "abovexpos";
            EntityVarOffsets[0x10c] = "aboveypos";

            EntityVarOffsets[0x114] = "xpos";
            EntityVarOffsets[0x118] = "ypos";
            EntityVarOffsets[0x11c] = "zpos";
            EntityVarOffsets[0x120] = "xtile";
            EntityVarOffsets[0x124] = "ytile";
            EntityVarOffsets[0x128] = "ztile";
            EntityVarOffsets[0x12c] = "ridingentity";

            EntityVarOffsets[0x130] = "?xcollision?";
            EntityVarOffsets[0x134] = "?ycollision?";
            EntityVarOffsets[0x138] = "zcollision";
            EntityVarOffsets[0x13c] = "forceadjusted";
            EntityVarOffsets[0x140] = "entitycollision";
            EntityVarOffsets[0x144] = "";
            EntityVarOffsets[0x148] = "maptiletl";
            EntityVarOffsets[0x14c] = "maptiletr";
            EntityVarOffsets[0x150] = "maptilebl";
            EntityVarOffsets[0x154] = "maptilebr";
            EntityVarOffsets[0x158] = "mapheighttl";
            EntityVarOffsets[0x15c] = "mapheighttr";
            EntityVarOffsets[0x160] = "mapheightbl";
            EntityVarOffsets[0x164] = "mapheightbr";
            EntityVarOffsets[0x168] = "donemoving";

            EntityVarOffsets[0x180] = "";
            EntityVarOffsets[0x184] = "";
            EntityVarOffsets[0x188] = "";
            EntityVarOffsets[0x18c] = "";
            EntityVarOffsets[0x190] = "";
            EntityVarOffsets[0x194] = "imageset";
            EntityVarOffsets[0x198] = "xpos2?";
            EntityVarOffsets[0x19c] = "ypos2?";
            EntityVarOffsets[0x1a0] = "zpos2?";

            EntityVarOffsets[0x1b0] = "addedtossheet?";
            EntityVarOffsets[0x1b4] = "addedtopallete?";

            EntityVarOffsets[0x1d4] = "";
            EntityVarOffsets[0x1d8] = "adjustedxpos";
            EntityVarOffsets[0x1dc] = "adjustedypos";
            EntityVarOffsets[0x1e0] = "adjustedzpos";
            EntityVarOffsets[0x1e4] = "xmod";
            EntityVarOffsets[0x1e8] = "ymod";
            EntityVarOffsets[0x1ec] = "zmod";
            EntityVarOffsets[0x1f0] = "width";
            EntityVarOffsets[0x1f4] = "depth";
            EntityVarOffsets[0x1f8] = "height";
            EntityVarOffsets[0x1fc] = "";
            EntityVarOffsets[0x200] = "";
            EntityVarOffsets[0x204] = "";
            EntityVarOffsets[0x208] = "";
            EntityVarOffsets[0x20c] = "";
            EntityVarOffsets[0x210] = "";

            EntityVarOffsets[0x230] = "entity(self)";
            EntityVarOffsets[0x234] = "tickprogsp";
            EntityVarOffsets[0x238] = "tickprogexp";

            EntityVarOffsets[0x260] = "logicresult";


            MapNames[162] = "inoa";
            MapNames[163] = "jess house";
            MapNames[164] = "beaumont/septimus/fortuneteller house";
            MapNames[165] = "wendell/giles house";
            MapNames[166] = "lutas/bonaire house";
            MapNames[167] = "nadia/gustav house";
            MapNames[168] = "klein/sybill house";
            MapNames[169] = "inoa 2";
            MapNames[176] = "inoa 3";
            MapNames[183] = "inoa 4";
            MapNames[186] = "inoa 5";
            MapNames[193] = "inoa 6";
            MapNames[198] = "inoa 7";
            MapNames[205] = "inoa 8";
            MapNames[212] = "inoa 9";
            MapNames[219] = "inoa 10";
            MapNames[226] = "inoa 11";
            MapNames[230] = "inoa 12";
            MapNames[237] = "inoa 13";
            MapNames[241] = "inoa 14";
            MapNames[248] = "inoa 15";
            MapNames[252] = "inoa 16";
            MapNames[259] = "inoa 17";
            MapNames[266] = "inoa 18";
            MapNames[271] = "inoa 19";
            MapNames[275] = "inoa 20";
            MapNames[282] = "inoa 21";
            MapNames[289] = "inoa 22";
            MapNames[293] = "inoa burning";
            MapNames[297] = "inoa burned";
            MapNames[301] = "inoa burned 2";

            MapNames[303] = "miner/graveyard keeper";

            MapNames[310] = "inoa w portal and meia";
            MapNames[311] = "a house and a well";
            MapNames[312] = "ship interior no entities";
            MapNames[313] = "ship interior no entities";
            MapNames[314] = "unfinished coast? no entities";
            MapNames[315] = "unfinished coast no entities one gaincontrol event";
            MapNames[320] = "unfinished ship map";

            MapNames[321] = "zorgia boss map";
            MapNames[322] = "boss map";
            MapNames[323] = "boss map dragon";
            MapNames[324] = "boss map bridge";
            MapNames[325] = "boss map fire dragon";
            MapNames[326] = "boss map priest";
            MapNames[327] = "boss map";

            MapNames[330] = "inoa";

            MapNames[337] = "lake shrine int";
            MapNames[338] = "lake shrine int";
            MapNames[339] = "lake shrine int";
            MapNames[353] = "lake shrine int";
            MapNames[354] = "lake shrine roof";
            MapNames[355] = "lake shrine roof";
            MapNames[356] = "lake shrine roof";


            MapNames[357] = "inoa";
            MapNames[364] = "inoa";
            MapNames[367] = "inoa";

            MapNames[368] = "great tree";
            MapNames[369] = "great tree int";

            MapNames[371] = "inoa";

            MapNames[389] = "ship";
            MapNames[390] = "ship captains";
            MapNames[391] = "ship night";
            MapNames[392] = "ship int";

            GlobalVariableNames.Clear();
            GlobalVariableNames.Add(0x9b5b4, "eventhandlers");
            GlobalVariableNames.Add(0x1e6118, "mapgameflags");
            GlobalVariableNames.Add(0x1ed0d4, "globalgameflags");
            GlobalVariableNames.Add(0x1bc498, "playerentity");

        }

        static void NudgeAddresses()
        {
            var backup = FunctionNames;
            FunctionNames = new Dictionary<uint, string>();
            foreach (var ent in backup)
            {
                FunctionNames.Add(ent.Key - Adjustment, ent.Value);
            }
        }
    }
}
