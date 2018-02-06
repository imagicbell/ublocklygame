using System.Collections.Generic;
using System.IO;
using UBlockly.UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UToolbox;

namespace UBlocklyGame.Maze
{
    public class MazeMapBuilder : MonoBehaviour
    {
        [SerializeField] private InputField m_InputWidth;
        [SerializeField] private InputField m_InputHeight;
        [SerializeField] private InputField m_InputName;
        [SerializeField] private Button m_BtnCreate;
        [SerializeField] private Button m_BtnDelete;
        [SerializeField] private Button m_BtnLoad;
        [SerializeField] private Button m_BtnSave;
        [SerializeField] private GameObject m_GridPrefab;
        [SerializeField] private GameObject m_FileNamePrefab;
        [SerializeField] private Transform m_ChoiceGroup;

        private const int MAX_WIDTH = 600;
        private const int MAX_HEIGHT = 600;

        private ScrollRect mScroll;
        private GridLayoutGroup mGridGroup;
        private List<GridType> mGridList;
        private Dictionary<GridType, GameObject> mChoiceDict;

        private int mMapWidth;
        private int mMapHeight;
        private int mGridLength;

        private GameObject mPickedGrid;
        private Canvas mCanvas;
        private GameObject mNameTip;

        private string mSavePath
        {
            get
            {
                string savePath = System.IO.Path.Combine(Application.persistentDataPath, "MazeMapDesign");
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
                return savePath;
            }
        }

        private void Awake()
        {
            mGridList = new List<GridType>();
            mGridGroup = m_GridPrefab.GetComponentInParent<GridLayoutGroup>();
            mScroll = m_ChoiceGroup.GetComponentInParent<ScrollRect>();
            
            m_GridPrefab.SetActive(false);
            m_FileNamePrefab.SetActive(false);

            m_BtnCreate.onClick.AddListener(() => BuildMap());
            m_BtnDelete.onClick.AddListener(ClearMap);
            m_BtnLoad.onClick.AddListener(LoadMap);
            m_BtnSave.onClick.AddListener(SaveMap);

            mChoiceDict = new Dictionary<GridType, GameObject>();
            for (int i = 0; i < m_ChoiceGroup.childCount; i++)
            {
                GameObject choiceObj = m_ChoiceGroup.GetChild(i).gameObject;
                UIEventListener.Get(choiceObj).onBeginDrag = OnBeginDrag;
                UIEventListener.Get(choiceObj).onDrag = OnDrag;
                UIEventListener.Get(choiceObj).onEndDrag = OnEndDrag;

                GridType gridType = (GridType) int.Parse(choiceObj.name.Substring("Choice".Length));
                mChoiceDict[gridType] = choiceObj;
            }

            mNameTip = m_InputName.transform.Find("Tip").gameObject;
            m_InputName.onValueChanged.AddListener((text) =>
            {
                if (!string.IsNullOrEmpty(text) && mNameTip.activeInHierarchy)
                    mNameTip.SetActive(false);
            });
            
            mCanvas = GetComponentInChildren<Canvas>();
        }

        private void Start()
        {
            LoadMapFiles();
        }

        private void BuildMap(List<GridType> gridList = null)
        {
            mMapWidth = int.Parse(m_InputWidth.text);
            mMapHeight = int.Parse(m_InputHeight.text);

            int gridWidth = MAX_WIDTH / mMapWidth;
            int gridHeight = MAX_HEIGHT / mMapHeight;

            mGridLength = Mathf.Min(gridWidth, gridHeight);
            mGridGroup.cellSize = new Vector2(mGridLength, mGridLength);
            if (mGridLength == gridWidth)
            {
                mGridGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                mGridGroup.constraintCount = mMapWidth;
            }
            else
            {
                mGridGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                mGridGroup.constraintCount = mMapHeight;
            }

            mGridList.Clear();
            for (int i = 0; i < mMapHeight; i++)
            {
                for (int j = 0; j < mMapWidth; j++)
                {
                    GameObject gridObj = GameObject.Instantiate(m_GridPrefab, mGridGroup.transform, false);
                    gridObj.SetActive(true);

                    GridType gridType;
                    if (gridList == null)
                    {
                        gridType = GridType.Obstacle;
                    }
                    else
                    {
                        int index = i * mMapWidth + j;
                        gridType = gridList[index];
                    }
                    mGridList.Add(GridType.Obstacle);
                    
                    gridObj = GameObject.Instantiate(mChoiceDict[gridType], gridObj.transform);
                    gridObj.SetActive(true);
                    gridObj.transform.localPosition = Vector3.zero;
                    RectTransform rectTrans = gridObj.transform as RectTransform;
                    rectTrans.anchorMin = rectTrans.anchorMax = 0.5f * Vector2.one;
                    rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mGridLength);
                    rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mGridLength);
                }
            }
        }

        private void ClearMap()
        {
            foreach (Transform trans in mGridGroup.transform)
            {
                if (trans != m_GridPrefab.transform)
                    GameObject.Destroy(trans.gameObject);
            }
            
            mGridList.Clear();
        }

        private void LoadMapFiles()
        {
            foreach (Transform child in m_FileNamePrefab.transform.parent)
            {
                if (child.gameObject != m_FileNamePrefab)
                    GameObject.Destroy(child.gameObject);
            }

            string[] files = Directory.GetFiles(mSavePath, "*.json");
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                GameObject fileObj = GameObject.Instantiate(m_FileNamePrefab, m_FileNamePrefab.transform.parent, false);
                fileObj.SetActive(true);
                fileObj.GetComponentInChildren<Text>().text = fileName;
            }
        }

        private void LoadMap()
        {
            Toggle[] toggles = m_FileNamePrefab.transform.parent.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < toggles.Length; i++)
            {
                if (toggles[i].isOn)
                {
                    ClearMap();
                    
                    string fileName = toggles[i].GetComponentInChildren<Text>().text;
                    string savePath = System.IO.Path.Combine(mSavePath, fileName + ".json");
                    string jsonText = File.ReadAllText(savePath);
                    MazeMapData data = jsonText.FromJson<MazeMapData>();
                    m_InputName.text = data.Name;
                    m_InputWidth.text = data.Width.ToString();
                    m_InputHeight.text = data.Height.ToString();
                    BuildMap(data.Grids);
                    break;
                }
            }
        }

        private void SaveMap()
        {
            if (string.IsNullOrEmpty(m_InputName.text))
            {
                mNameTip.SetActive(true);
                return;
            }
            
            MazeMapData data = new MazeMapData();
            data.Name = m_InputName.text;
            data.Width = mMapWidth;
            data.Height = mMapHeight;
            data.Grids = mGridList;
            
            string savePath = System.IO.Path.Combine(mSavePath, m_InputName.text + ".json");
            string jsonText = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, jsonText);
            
            LoadMapFiles();
        }

        private void OnBeginDrag(PointerEventData data)
        {
            if (data.delta.x > data.delta.y)
            {
                //drag self, create new grid
                mPickedGrid = GameObject.Instantiate(data.pointerDrag, m_ChoiceGroup, false);
                mPickedGrid.name = data.pointerDrag.name;
                mPickedGrid.transform.SetParent(mCanvas.transform);
                RectTransform rectTrans = mPickedGrid.GetComponent<RectTransform>();
                rectTrans.anchorMin = rectTrans.anchorMax = 0.5f * Vector2.one;
                FollowInputPosition(rectTrans);
            }
            else
            {
                mScroll.OnBeginDrag(data);
            }
        }
        
        private void OnDrag(PointerEventData data)
        {
            if (mPickedGrid != null)
            {
                FollowInputPosition(mPickedGrid.GetComponent<RectTransform>());
            }
            else
            {
                mScroll.OnDrag(data);
            }
        }
        
        private void OnEndDrag(PointerEventData data)
        {
            if (mPickedGrid != null)
            {
                GameObject displaceObj;
                AttachToNearestGrid(mPickedGrid, out displaceObj);
                mPickedGrid = displaceObj;
            }
            else
            {
                mScroll.OnEndDrag(data);   
            }
        }

        private void FollowInputPosition(RectTransform rectTrans)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) mCanvas.transform,
                new Vector2(Input.mousePosition.x, Input.mousePosition.y), mCanvas.worldCamera, out localPoint);
            rectTrans.anchoredPosition = localPoint;
        }

        private void AttachToNearestGrid(GameObject obj, out GameObject displaceObj)
        {
            displaceObj = null;
            
            RectTransform parent = mGridGroup.transform as RectTransform;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                mCanvas.worldCamera, out localPoint);
            
            //relative to upper left
            localPoint.x += parent.sizeDelta.x * 0.5f;
            localPoint.y = parent.sizeDelta.y * 0.5f - localPoint.y;
            
            int x = Mathf.CeilToInt(localPoint.x / mGridLength);
            int y = Mathf.CeilToInt(localPoint.y / mGridLength);

            if (x > 0 && x <= mMapWidth &&
                y > 0 && y <= mMapHeight)
            {
                int index = (y - 1) * mMapWidth + (x - 1);
                Transform gridParent = mGridGroup.transform.GetChild(index + 1);
                if (gridParent.childCount > 0)
                {
//                    displaceObj = gridParent.GetChild(0).gameObject;
//                    displaceObj.transform.SetParent(mCanvas.transform);
                    GameObject.Destroy(gridParent.GetChild(0).gameObject);
                }
                
                obj.transform.SetParent(mGridGroup.transform.GetChild(index + 1)); //not consider m_GridPrefab
                obj.transform.localPosition = Vector3.zero;
                RectTransform rectTrans = obj.transform as RectTransform;
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mGridLength);
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mGridLength);
                
                mGridList[index] = (GridType) int.Parse(obj.name.Substring("Choice".Length));
            }
            else
            {
                GameObject.Destroy(obj);
            }
        }
    }
}