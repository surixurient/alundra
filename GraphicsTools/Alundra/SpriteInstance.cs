using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsTools.Alundra
{
    public class SpriteInstance
    {
		
        

		public SpriteInstance UnknownBeforeStatus;
		public int Status;
		public SpriteInstance PlatformEntity;

		public SIEntityRecord EntityRecord;
		public int EntityRefId;

		public SpriteRecord Sprite;
		public int NameId;
		public int Flags;//0x800000 = portrait,0x0100 = gravity,0xf = ?, 0x1 = ?

		public int TargetAnim;
		public int TargetDir;
		public int CurAnim;
		public int CurDir;
		public int UnknownBeforeBeforeBeforeZForce;
		public int UnknownBeforeBeforeZForce;
		public int UnknownBeforeZForce;
		public int ZForce;//rise/fall speed


		public int XPos;
		public int YPos;
		public int ZPos;
		public int XTile;
		public int YTile;
		public int ZTile;
		public SpriteInstance RidingEntity;
		public SpriteInstance XCollisionEntity;

		public int ForceAdjusted;



		public int XMod;
		public int YMod;
		public int ZMod;

		public EventData eventdata = new EventData();
	}

	public class EventData
    {
		public int maploadprogram;//program that runs on map load
		public EventProgramState tickprogram;//event program that runs every tick
		public int interactprogram;
	}

	public class EventProgramState
    {
		public int sp;
		public int exp;
		public ushort elapsedMs;
		public int evttickprog;
		public byte isWaiting;
		public int logicResult;
		public int[] evtvars = new int[8];
    }
}
