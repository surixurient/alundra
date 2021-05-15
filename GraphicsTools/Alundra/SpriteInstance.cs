using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsTools.Alundra
{
    public class SpriteInstance
    {
		SIEntityRecord entityrecord;
        SpriteRecord sprite;

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
		public byte logicResult;
		public int[] evtvars;
    }
}
