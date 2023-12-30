using System.Collections.Generic;

namespace Amegakure.Starkane.GridSystem
{
    [System.Serializable]
    public class Frontier 
    {
        private List<Tile> tiles = new();

        public List<Tile> Tiles { get => tiles; set => tiles = value; }
    }
}
