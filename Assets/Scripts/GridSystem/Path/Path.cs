namespace Amegakure.Starkane.GridSystem
{
    [System.Serializable]
    public class Path
    {
        private Tile[] tilesInPath;

        public Tile[] TilesInPath { get => tilesInPath; set => tilesInPath = value; }
    }
}
