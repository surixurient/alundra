
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
        GameMap game;
        Dictionary<int, Bitmap> CachedTiles;
        Dictionary<int, List<Bitmap>> CachedSprites;
        public GameEngine(DatasBin datasBin)
        {
            this.datasBin = datasBin;
        }

        private void loadmap(GameMap gmap)
        {
            this.game = gmap;
            if (!game.loaded)
            {
                var reader = datasBin.OpenBin();
                game.Load(reader);
                reader.Close();
            }
            CachedTiles = new Dictionary<int, Bitmap>();//blow cache
            CachedSprites = new Dictionary<int, List<Bitmap>>();//blow cache




            //spriteinfo
            for (int dex = 0; dex < game.spriteinfo.entities.entities.Length; dex++)
            {
                var entity = game.spriteinfo.entities.entities[dex];
                if (entity != null)
                {
                    
                }
            }

            
            for (int dex = 0; dex < game.spriteinfo.mapevents.records.Length; dex++)
            {
                var record = game.spriteinfo.mapevents.records[dex];
                if (record != null)
                {
                    
                }
            }


            for (int dex = 0; dex < game.spriteinfo.sprites.Length; dex++)
            {
                var sprite = game.spriteinfo.sprites[dex];
                if (sprite != null)
                {

                }
            }

        }
    }
}
