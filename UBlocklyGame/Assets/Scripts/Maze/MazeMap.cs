using System;
using System.Collections.Generic;
using UBlockly;
using UnityEngine;

namespace UBlocklyGame.Maze
{
    [Serializable]
    public class MazeMapData
    {
        public string Name;
        public int Width;
        public int Height;
        public List<GridType> Grids;
    }

    [Serializable]
    public class GridSprite
    {
        public GridType GridType;
        public string SprteName;
    }
    
    public class MazeMap : MonoBehaviour
    {
        [SerializeField] private Rect m_Boundary;
        [SerializeField] private GameObject m_Background;
        [SerializeField] private GameObject m_GridPrefab;

        private MazeMapData mMapData;
        private MazeGrid[,] mGrids;
        private MazeGrid mStart;
        private MazeGrid mTerminal;
        private float mGridLength;

        public void Init(MazeMapData mapData)
        {
            mMapData = mapData;
            BuildMap();
        }

        private void BuildMap()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject != m_Background && child.gameObject != m_GridPrefab)
                    GameObject.Destroy(child.gameObject);
            }

            float gridWidth = m_Boundary.width / mMapData.Width;
            float gridHeight = m_Boundary.height / mMapData.Height;
            mGridLength = Mathf.Min(gridWidth, gridHeight);

            float mapWidth = mGridLength * mMapData.Width;
            float mapHeight = mGridLength * mMapData.Height;
            m_Background.transform.localPosition = new Vector3(m_Boundary.center.x, m_Boundary.center.y, 1);
            m_Background.transform.localScale = new Vector3(mapWidth+0.2f, mapHeight+0.2f, 1);

            //topleft
            Vector3 startPos = new Vector3(m_Boundary.center.x - 0.5f * mapWidth, m_Boundary.center.y + 0.5f * mapHeight);
            startPos.x += 0.5f * mGridLength;
            startPos.y -= 0.5f * mGridLength;

            Vector3 gridPos = startPos;
            mGrids = new MazeGrid[mMapData.Height, mMapData.Width];
            
            for (int i = 0; i < mMapData.Height; i++)
            {
                for (int j = 0; j < mMapData.Width; j++)
                {
                    int index = i * mMapData.Width + j;
                    GridType gridType = mMapData.Grids[index];

                    GameObject gridObj = GameObject.Instantiate(m_GridPrefab, transform, false);
                    gridObj.SetActive(true);
                    gridObj.name = "Grid" + gridType.ToString();
                    gridObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(MazeDefine.GridTextures[gridType]);

                    gridObj.transform.localPosition = gridPos;
                    gridObj.transform.localScale = new Vector3(mGridLength, mGridLength, 1);

                    if (j < mMapData.Width - 1)
                        gridPos.x += mGridLength;
                    else
                        gridPos.x = startPos.x;

                    MazeGrid grid = MazeGridFactory.Create(gridType, new Vector2<int>(j, i));
                    grid.GridObject = gridObj;
                    mGrids[i, j] = grid;

                    if (gridType == GridType.Start) mStart = grid;
                    else if (gridType == GridType.Terminal) mTerminal = grid;
                }
                gridPos.y -= mGridLength;
            }
        }

        public MazeGrid GetStartGrid()
        {
            return mStart;
        }

        public MazeGrid GetTerminalGrid()
        {
            return mTerminal;
        }

        public MazeGrid GetGrid(int x, int y)
        {
            if (x < mMapData.Width && y < mMapData.Height)
                return mGrids[y, x];
            return null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(m_Boundary.center, m_Boundary.size);
        }
        #endif
    }
}