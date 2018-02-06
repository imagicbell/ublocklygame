using UBlockly;
using UnityEngine;

namespace UBlocklyGame.Maze
{
    public abstract class MazeGrid
    {
        protected readonly Vector2<int> mXY;
        public Vector2<int> XY { get { return mXY; } }

        protected readonly GridType mType;
        public GridType Type { get { return mType; } }

        public GameObject GridObject { get; set; }

        public Vector3 Position
        {
            get { return GridObject.transform.position; }
        }

        public enum PassResult
        {
            Pass,
            Obstruct,
            Trap,
        }

        protected MazeGrid(GridType type, Vector2<int> xy)
        {
            mType = type;
            mXY = xy;
        }

        public abstract PassResult CheckPass();
        public abstract void OnPassGrid(MazeAvatar avatar);
    }
}