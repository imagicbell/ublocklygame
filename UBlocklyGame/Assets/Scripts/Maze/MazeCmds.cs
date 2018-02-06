using System.Collections;
using UBlockly;

namespace UBlocklyGame.Maze
{
    [CodeInterpreter(BlockType = "maze_move")]
    public class Maze_Move_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            yield return MazeController.Instance.DoMoveForward();
        }
    }

    [CodeInterpreter(BlockType = "maze_turn")]
    public class Maze_Turn_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            string dirStr = block.GetFieldValue("DIRECTION");
            Direction dir = dirStr.Equals("LEFT") ? Direction.Left : Direction.Right;
            yield return MazeController.Instance.DoTurn(dir);
        }
    }

    [CodeInterpreter(BlockType = "maze_bool_access")]
    public class Maze_BoolAccess_Cmdtor : ValueCmdtor
    {
        protected override DataStruct Execute(Block block)
        {
            string dirStr = block.GetFieldValue("ACCESS");
            Direction dir = Direction.Front;
            switch (dirStr)
            {
                case "FRONT":
                    dir = Direction.Front;
                    break;
                case "RIGHT":
                    dir = Direction.Right;
                    break;
                case "LEFT":
                    dir = Direction.Left;
                    break;
            }
            bool access = MazeController.Instance.DoCheckAccess(dir);
            return new DataStruct(access);
        }
    }
    
    [CodeInterpreter(BlockType = "maze_reach_terminal")]
    public class Maze_ReachTerminal_Cmdtor : ValueCmdtor
    {
        protected override DataStruct Execute(Block block)
        {
            bool reach = MazeController.Instance.DoCheckAccomplish();
            return new DataStruct(reach);
        }
    }
}