using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map_Editor : MonoBehaviour {

    public List<Vector2> pos;
    public RectTransform setPos;
    public static Map_Editor instance;
    public Canvas canvas;
    public Sprite[] map_info;
    public List<RectTransform> real_info;
    [SerializeField] private float _mapItemListOffset;
    [SerializeField] private int[] _mapItemCounts;
    [SerializeField] private Text[] _mapItemCountTexts;
    int total;

    public float groupDistance;
    private void Awake()
    {
        canvas.gameObject.SetActive(true);
        
        if (Map_Editor.instance == null)
        {
            Map_Editor.instance = this;
        }
        Init();
        AddMap(0);
        AddMap(1);
        AddMap(2);
        AddMap(3);
        AddMap(4);
        AddMap(5);
    }

    public void Init()
    {
        groupDistance = 0;
        total = 0;
        map_info = Resources.LoadAll<Sprite>("JinHyeok/Blocks");
        real_info = new List<RectTransform>();

        for (int i = 0; i < 6; i++)
            UpdateMapItemCountText(i);
    }

    public bool AddMap(int i)
    {
        if (_mapItemCounts[i] <= 0) return false;

        _mapItemCounts[i]--;
        UpdateMapItemCountText(i);
        
        int height = 100;
        GameObject source = Resources.Load("JinHyeok/Image") as GameObject;
        GameObject newObj = Instantiate(source);
        newObj.GetComponent<Image>().sprite = map_info[i];
        newObj.transform.parent = canvas.transform.GetChild(0);
        
        RectTransform rect = newObj.GetComponent<RectTransform>();
        
        var item = newObj.GetComponent<Map_Move>();
        item.ownNum = total;
        total++;
        item.id = i;
        item.isDocked = true;
        SetPosition(rect, i, 0);


        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(  pos[i].x / 120 * 75, pos[i].y / 120 * 75);
        real_info.Add(rect);
        //newObj.transform.parent = canvas.transform;

        return true;
    }

    private string UpdateMapItemCountText(int i)
    {
        return _mapItemCountTexts[i].text = "x " + (_mapItemCounts[i] + 1);
    }

    public void SetPosition(RectTransform rect, int i, int addCount = 0, float additionalOffset = 0)
    {
        Map_Move move = rect.transform.GetComponent<Map_Move>();
        rect.transform.parent = setPos.transform;
        rect.anchoredPosition = new Vector2((move.id * 240 ) + _mapItemListOffset + additionalOffset, 0);

        _mapItemCounts[i] += addCount;
        UpdateMapItemCountText(i);
        //rect.transform.parent = canvas.transform;
    }
    
}
