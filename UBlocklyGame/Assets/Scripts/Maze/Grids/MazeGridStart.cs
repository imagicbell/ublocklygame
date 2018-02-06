using UBlockly;

namespace UBlocklyGame.Maze
{
    public class MazeGridStart : MazeGrid
    {
        public MazeGridStart(GridType type, Vector2<int> xy) : base(type, xy)
        {
        }
        
        public override PassResult CheckPass()
        {
            return PassResult.Pass;
        }

        public override void OnPassGrid(MazeAvatar avatar)
        {
            
        }
    }
}