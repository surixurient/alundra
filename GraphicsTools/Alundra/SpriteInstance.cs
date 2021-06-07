using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsTools.Alundra
{
    public class SpriteInstance
    {


		public int Index;
		public int UnknownAfterIndex;
		public SpriteInstance UnknownBeforeOwnerEntity;
		public SpriteInstance OwnerEntity;
		public int Status;
		public int HP;
		public int MaxHp;
		public SpriteInstance PlatformEntity;

		public SIEntityRecord EntityRecord;
		public int EntityRefId;
		public int[] Program_Indexes = new int[6];
		//public int ProgramA_Load;
		//public int ProgramB_Map;
		//public int ProgramC_Tick;
		//public int ProgramD_Touch;
		//public int ProgramE_Unknown;
		//public int ProgramF_Interact;
		public SpriteRecord Sprite;
		public int SpriteTableIndex;
		public int Flags;//0x800000 = portrait,0x0100 = gravity,0xf = ?, 0x1 = ?
		public int SpriteU4;
		public int UnkownBeforeThrowType;
		public int ThrowType;
		public int SpriteU6;
		public int BreakSound;
		public int SpriteU8;

		public int TargetAnim;
		public int TargetDir;
		public int CurAnim;
		public int CurDir;
		public int FrameDex;
		public SIAnimSet AnimSet;
		public SIFrame FirstFrame;
		public SIFrame Frame;
		public int NextFrameDelay;
		public int WierdNextFrameDelayFlag;
		public int AnimCompleteCounter;
		public int AnimFlags;
		public int ZForce;//rise/fall speed
		public int TargetXForce, TargetYForce, XForce, YForce;
		//?cc
		//?d0
		public int XForceStep, YForceStep;

		public int AppliedZForce;//this is probably named wrong, has to do with animation
		public int ScreenClipX, ScreenClipY, ScreenClipZ;
		public int NegXMod, NegYMod;
		public int XPos;
		public int YPos;
		public int ZPos;
		public int XTile;
		public int YTile;
		public int ZTile;
		public SpriteInstance RidingEntity;
		public SpriteInstance XCollisionEntity;
		public int ZEntityCollision;
		public int ZMapCollision;//map collision

		public int ForceAdjusted;
		public int CollidedWithEntityZ;//some boolean that has to do with if moddedzpos is greater than hity from collideentitiesz
		public int _144;//collided with something
		public MapTile[] MapTiles = new MapTile[4];
		public int[] MapHeights = new int[4];

		public int _180,_184,_188,_18c,_190;
		public SpriteRef SpriteRef;//194 
		

		public int AddedToSheet, AddedToPalette;//represents offset where the pallets and sheets are in memory for map vs global sprites, prob not used with my engine

		public int DepthSortVal;

		public int _1d4, ModdedXPos, ModdedYPos;
		public int ModdedZPos, XMod, YMod;
		public int ZMod, Width, Depth;
		public int Height, _1fc, _200;//this set of vars is set when an animation has a frame with attached data
		public int _204, _208, _20c;
		public int _210;

		public int MapEventId;//228
		public int MapEventProgramId;//22c
		public SpriteInstance EntitySelf;
		public EventProgramState eventdata = new EventProgramState();
		public EventProgramState eventdata2 = new EventProgramState();
		public bool IsMapSprite { get
            {
				return (EntityRecord.spritedir & 0x80) != 0;
			}
        }
	}
	public class SpriteRef
    {
		public SIImage[] Images;		//c
		public int X;//4				//10
		public int Y;//8				//14
		public int Z;//c				//18
		public int DepthSortVal;//0x10		//1c
		public int NumImages;//0x14		//20
    }
	//public class EventData
    //{
	//	public int maploadprogram;//program that runs on map load
	//	public EventProgramState tickprogram;//event program that runs every tick
	//	public int interactprogram;
	//}

	public class EventProgramState
    {
		public int sp;
		public int exp;
		public int evttickprog;
		public int[] evtvars = new int[8];
		public int logicResult;

		public ushort elapsedMs;
		public byte isWaiting;
		public byte[] code;
	}
}
