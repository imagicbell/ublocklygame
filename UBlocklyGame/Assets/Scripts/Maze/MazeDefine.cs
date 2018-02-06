using System.Collections.Generic;

namespace UBlocklyGame.Maze
{
    public static class MazeDefine
    {
        public static Dictionary<GridType, string> GridTextures = new Dictionary<GridType, string>
        {
            {GridType.Start, "grid_start"},
            {GridType.Terminal, "grid_terminal"},
            {GridType.Road, "grid_road"},
            {GridType.Obstacle, "grid_obstacle"},
            {GridType.Trap, "grid_trap"}
        };

        /// <summary>
        /// The speed factor
        /// </summary>
        public static int SPEED_FACTOR = 1;
        
        /// <summary>
        /// how much time it takes to move a grid
        /// </summary>
        public static float MOVE_SPEED = 1;

        /// <summary>
        /// how much time it takes to turn 90 degrees
        /// </summary>
        public static float TURN_SPEED = 0.5f;
    }

    public enum GridType
    {
        Start = 1,
        Terminal = 2,

        Road = 10,
        Obstacle,
        Trap,
    }
}