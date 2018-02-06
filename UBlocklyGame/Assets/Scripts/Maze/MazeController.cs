using System.Collections;
using System.Collections.Generic;
using System.IO;
using UToolbox;
using UBlockly;
using UnityEngine;

namespace UBlocklyGame.Maze
{
    public class MazeController : MonoSingleton<MazeController>
    {
        private MazeMap mMap;
        private MazeAvatar mAvatar;

        private MazeGrid mCurGrid;
        private Vector2<int> mCurDir;

        private static List<Vector2<int>> DIRECTIONS = new List<Vector2<int>>
        {
            new Vector2<int>(1, 0),
            new Vector2<int>(0, 1),
            new Vector2<int>(-1, 0),
            new Vector2<int>(0, -1)
        };

        //todo: 入口
        private void Awake()
        {
        }

        //todo: 入口
        private void Start()
        {
            mMap = GetComponentInChildren<MazeMap>();
            mAvatar = GetComponentInChildren<MazeAvatar>();
            
            Reset();
        }

        /*public override void OnSingletonInit()
        {
            mMap = GetComponentInChildren<MazeMap>();
            mAvatar = GetComponentInChildren<MazeAvatar>();
            
            Reset();
        }*/

        public void Reset()
        {
            //todo: configurable

            string savePath = System.IO.Path.Combine(Application.persistentDataPath, "MazeMapDesign");
            savePath = System.IO.Path.Combine(savePath, "Level1.json");
            string jsonText = File.ReadAllText(savePath);
            MazeMapData data = jsonText.FromJson<MazeMapData>();
            mMap.Init(data);

            mCurGrid = mMap.GetStartGrid();
            List<Vector2<int>> dirs = DIRECTIONS.GetRange(0, DIRECTIONS.Count);
            while (dirs.Count > 0)
            {
                //get a random accessible direction
                Vector2<int> dir = dirs[Random.Range(0, dirs.Count)];
                MazeGrid grid = mMap.GetGrid(mCurGrid.XY.x + dir.x, mCurGrid.XY.y + dir.y);
                if (grid.CheckPass() == MazeGrid.PassResult.Pass)
                {
                    mCurDir = dir;
                    break;
                }
                dirs.Remove(dir);
            }

            mAvatar.Init(mCurGrid.Position, new Vector3(mCurDir.x, 0, mCurDir.y));
        }

        public IEnumerator DoMoveForward()
        {
            MazeGrid nextGrid = mMap.GetGrid(mCurGrid.XY.x + mCurDir.x, mCurGrid.XY.y + mCurDir.y);
            yield return mAvatar.Move(nextGrid.Position);
            mCurGrid = nextGrid;
        }

        public IEnumerator DoTurn(Direction turnDir)
        {
            float angle = turnDir == Direction.Right ? 90 : -90;
            yield return mAvatar.Turn(angle);
            mCurDir = GetDirection(turnDir);
        }

        public bool DoCheckAccess(Direction checkDir)
        {
            Vector2<int> dir = GetDirection(checkDir);
            MazeGrid nextGrid = mMap.GetGrid(mCurGrid.XY.x + dir.x, mCurGrid.XY.y + dir.y);
            return nextGrid.CheckPass() == MazeGrid.PassResult.Pass;
        }

        public bool DoCheckAccomplish()
        {
            return mCurGrid.Type == GridType.Terminal;
        }

        private Vector2<int> GetDirection(Direction checkDir)
        {
            int index = DIRECTIONS.IndexOf(mCurDir);
            switch (checkDir)
            {
                case Direction.Right:
                    index += 1;
                    break;
                case Direction.Left:
                    index -= 1;
                    break;
                case Direction.Back:
                    index += 2;
                    break;
            }
            if (index < 0)
                index = DIRECTIONS.Count - 1;
            if (index >= DIRECTIONS.Count)
                index = 0;
            
            return DIRECTIONS[index];
        }
    }
}