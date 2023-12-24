using System.Collections.Generic;

namespace Amegakure.Starkane.GridSystem
{
    [System.Serializable]
    public class Frontier 
    {
        private List<Tile> tiles = new();
        private bool isMovementFrontier = true;

        public List<Tile> Tiles { get => tiles; set => tiles = value; }
        public bool IsMovementFrontier { get => isMovementFrontier; set => isMovementFrontier = value; }
    }
}
