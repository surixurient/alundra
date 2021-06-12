using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsTools.Alundra
{
    public class SpriteEventHandlers
    {
        GameState gameState;
        Dictionary<int, SpriteEventHandler>[] TypeHandlers = new Dictionary<int, SpriteEventHandler>[6];
        public SpriteEventHandlers(GameState gameState)
        {
            this.gameState = gameState;

            TypeHandlers[Helper.PROGRAM_A_LOAD] = new Dictionary<int, SpriteEventHandler>();
            //there are no spriteevent handlers for map
            TypeHandlers[Helper.PROGRAM_C_TICK] = new Dictionary<int, SpriteEventHandler>();
            TypeHandlers[Helper.PROGRAM_D_TOUCH] = new Dictionary<int, SpriteEventHandler>();
            TypeHandlers[Helper.PROGRAM_E_DEACTIVATE] = new Dictionary<int, SpriteEventHandler>();
            TypeHandlers[Helper.PROGRAM_F_INTERACT] = new Dictionary<int, SpriteEventHandler>();

            //register the ones that have been implimented here
        }

        public void RunSpriteHandler(int eventtype, int eventid, SpriteInstance entity)
        {
            var handlers = TypeHandlers[eventtype];
            if (handlers.ContainsKey(eventid))
            {
                handlers[eventid](entity);
                return;
            }
        }
    }

    public delegate void SpriteEventHandler(SpriteInstance entity);

}
