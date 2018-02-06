using UBlockly;

namespace UBlocklyGame.Maze
{
    public class MazeGridObstacle : MazeGrid
    {
        public MazeGridObstacle(GridType type, Vector2<int> xy) : base(type, xy)
        {
        }
        
        public override PassResult CheckPass()
        {
            return PassResult.Obstruct;
        }

        public override void OnPassGrid(MazeAvatar avatar)
        {
        }
    }
}