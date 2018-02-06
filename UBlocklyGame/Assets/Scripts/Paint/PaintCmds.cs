using System.Collections;
using UBlockly;
using UnityEngine;

namespace UBlocklyGame.Paint
{
    [CodeInterpreter(BlockType = "paint_move")]
    public class Paint_Move_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            string dirStr = block.GetFieldValue("DIRECTION");
            Direction dir = dirStr.Equals("BACKWARD") ? Direction.Back : Direction.Front;

            string distStr = block.GetFieldValue("PIXEL");
            float dist = float.Parse(distStr);
            
            yield return PaintController.Instance.DoMove(dir, dist);
        }
    }
    
    [CodeInterpreter(BlockType = "paint_turn")]
    public class Paint_Turn_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            float angle = float.Parse(block.GetFieldValue("DEGREE"));
            
            string dirStr = block.GetFieldValue("DIRECTION");
            if (dirStr.Equals("LEFT"))
                angle = -angle;

            yield return PaintController.Instance.DoTurn(angle);
        }
    }

    [CodeInterpreter(BlockType = "paint_turn_ext")]
    public class Paint_Turn_Ext_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            float angle = 90;
            string dirStr = block.GetFieldValue("DIRECTION");
            if (dirStr.Equals("LEFT"))
                angle = -angle;
            
            yield return PaintController.Instance.DoTurn(angle);
        }
    }

    [CodeInterpreter(BlockType = "paint_jump")]
    public class Paint_Jump_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            string dirStr = block.GetFieldValue("DIRECTION");
            Direction dir = dirStr.Equals("BACKWARD") ? Direction.Back : Direction.Front;
            
            string distStr = block.GetFieldValue("PIXEL");
            float dist = float.Parse(distStr);

            yield return PaintController.Instance.DoJump(dir, dist);
        }
    }

    [CodeInterpreter(BlockType = "paint_color")]
    public class Paint_Color_Cmdtor : VoidCmdtor
    {
        protected override void Execute(Block block)
        {
            string colorHex = block.GetFieldValue("COLOR");
            Color color;
            ColorUtility.TryParseHtmlString(colorHex, out color);
            
            PaintController.Instance.DoChangeColor(color);
        }
    }
    
    
}