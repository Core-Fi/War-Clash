using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class UIMapEditor : MonoBehaviour
{
    public int tileSize = 20;
    public int width = 30;
    public int height = 30;
    UnityEngine.UI.Button save_btn_button;

    private GameObject TileBtns_go;
    private UnityEngine.UI.VerticalLayoutGroup TileBtns_verticallayoutgroup;
    private GameObject Template_go;
    private UIContainer<TileBtn> Container;
    private GameObject tileGo;
    private RectTransform tileContent;
    private StageType[,] tiles;
    private GameObject[,] gos;
    private StageType curBrush = StageType.Clear;
    
    void Start()
    {
        FingerGestures.OnDragMove += OnDragMove;
        FingerGestures.OnFingerDown += OnFingerDown;
        tiles = new StageType[height, width];
        gos = new GameObject[height, width];
        var save_btn_go = gameObject.transform.Find("Controls/save_btn").gameObject;
        save_btn_button = save_btn_go.GetComponent<UnityEngine.UI.Button>();
        save_btn_button.onClick.AddListener(OnSaveBtnClick);
        TileBtns_go = gameObject.transform.Find("Controls/ScollView/TileBtns").gameObject;
        TileBtns_verticallayoutgroup = TileBtns_go.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        Container = new UIContainer<TileBtn>(TileBtns_verticallayoutgroup.gameObject);
        Template_go = gameObject.transform.Find("Controls/ScollView/TileBtns/Template").gameObject;
        tileGo = gameObject.transform.Find("ScrollView/Content/tile").gameObject;
        tileGo.GetComponent<Image>();
        tileContent = gameObject.transform.Find("ScrollView/Content").gameObject.GetComponent<RectTransform>();
        tileContent.sizeDelta = new Vector2(width*tileSize, height * tileSize);
        var brushs = Enum.GetValues(typeof(StageType));
        Container.Resize(brushs.Length);
        for (int i = 0; i < Container.ChildCount; i++)
        {
            var t = Container.GetChild(i);
            t.UpdateInfo((StageType)i);
            t.OnClickCallback = OnClick;
        }
        
    } 
    private void OnSaveBtnClick()
    {
        List<StageData> stageData = new List<StageData>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(tiles[y,x] != StageType.Clear)
                {
                    stageData.Add(new StageData() {X = x, Y = y, Type = tiles[y,x] });
                }
            }
        }
        MapData data = new MapData() { Data = stageData, Width = width, Height = height };
        var str = Newtonsoft.Json.JsonConvert.SerializeObject(data, Logic.Skill.SkillUtility.settings);
        System.IO.File.WriteAllText("D://stageConf.txt", str);
    }
    public void OnClick(StageType index)
    {
        curBrush = index;
    }
    private void OnFingerDown(int fingerIndex, Vector2 fingerPos)
    {
        Draw(fingerPos);
    }
    private void OnDragMove(Vector2 fingerPos, Vector2 delta)
    {
        Draw(fingerPos);
    }
    private void Draw(Vector2 fingerPos)
    {
        fingerPos -= tileContent.anchoredPosition;
        int x = ((int)(fingerPos.x / tileSize));
        int y = ((int)(fingerPos.y / tileSize));
        if (x < width && y < height && x>=0 & y>=0)
        {
            if (tiles[y, x] != (curBrush))
            {
                tiles[y, x] = (curBrush);
                if (gos[y, x] != null)
                {
                    GameObject.Destroy(gos[y,x]);
                    gos[y, x] = null;
                }
                if (curBrush != StageType.Clear)
                {
                    var g = GameObject.Instantiate(tileGo);
                    g.transform.SetParent(tileContent.transform);
                    g.transform.localScale = Vector3.one;
                    g.transform.localPosition = Vector3.zero;
                    var rt = g.GetComponent<RectTransform>();
                    Vector2 v = new Vector2(x, y) * tileSize;
                    rt.anchoredPosition = v;
                    gos[y, x] = g;
                }
            }
        }
    }
}
public class TileBtn : UITemplate
{
    public StageType BrushType { get; private set; }
    public Action<StageType> OnClickCallback;
    private UnityEngine.UI.Button Template_button;
    private GameObject Text_go;
    private UnityEngine.UI.Text Text_text;
    public TileBtn() { }
    public TileBtn(GameObject go) : base(go)
    {}
    protected override void OnInit()
    {
        base.OnInit();
        Template_button = Go.GetComponent<UnityEngine.UI.Button>();
        Text_go = Go.transform.Find("Text").gameObject;
        Text_text = Text_go.GetComponent<UnityEngine.UI.Text>();
        Template_button.onClick.AddListener(OnClick);
    }
    public void UpdateInfo(StageType brushType)
    {
        BrushType = brushType;
        Text_text.text = brushType + "";
    }

    private void OnClick()
    {
        OnClickCallback.Invoke(BrushType);
    }

}
public enum StageType : byte
{
    Clear,
    WoodStage,
    StoneStage,
    UpExit,
    DownExit,
    LeftExit,
    RightExit,
}
public struct MapData
{
    public List<StageData> Data;
    public int Width;
    public int Height;
}
public struct StageData
{
    public int X, Y;
    public StageType Type;
}