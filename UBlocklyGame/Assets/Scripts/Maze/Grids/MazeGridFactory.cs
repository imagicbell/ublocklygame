using UBlockly;

namespace UBlocklyGame.Maze
{
    public static class MazeGridFactory
    {
        public static MazeGrid Create(GridType gridType, Vector2<int> xy)
        {
            switch (gridType)
            {
                case GridType.Start: return new MazeGridStart(gridType, xy);
                case GridType.Terminal: return new MazeGridTerminal(gridType, xy);
                case GridType.Road: return new MazeGridRoad(gridType, xy);
                case GridType.Obstacle: return new MazeGridObstacle(gridType, xy);
                case GridType.Trap: return new MazeGridTrap(gridType, xy);
            }
            return null;
        }
    }
}