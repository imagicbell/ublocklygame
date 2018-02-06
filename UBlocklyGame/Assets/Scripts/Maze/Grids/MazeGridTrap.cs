using UBlockly;

namespace UBlocklyGame.Maze
{
    public class MazeGridTrap : MazeGrid
    {
        public MazeGridTrap(GridType type, Vector2<int> xy) : base(type, xy)
        {
        }
        
        public override PassResult CheckPass()
        {
            return PassResult.Trap;
        }

        public override void OnPassGrid(MazeAvatar avatar)
        {
        }
    }
}