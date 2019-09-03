using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixPosition : MonoBehaviour {

    public List<GameObject> list_RealBlock;

    public static FixPosition instance;
    public List<RectTransform> Anchors;
    public RectTransform clicked;
    [SerializeField] private Transform _anchorParent;
    // Use this for initialization
    private void Awake()
    {
        if (FixPosition.instance == null)
            FixPosition.instance = this;
        list_RealBlock = new List<GameObject>();
        Anchors = new List<RectTransform>();
        var anchors = _anchorParent.GetComponentsInChildren<Fixed_Script>();
        foreach (var anchor in anchors)
            Anchors.Add(anchor.GetComponent<RectTransform>());
    }

    // Update is called once per frame
    void Update () {
//        Debug.Log(list_fixed.Count);
        if(clicked != null)
        {
            float minMax;
            int minMaxPos;
            
            minMaxPos = 0;
            minMax = Vector2.Distance(clicked.anchoredPosition, Anchors[0].anchoredPosition);
            for (int i = 1; i < Anchors.Count; i++)
            {
                float distance = Vector2.Distance(clicked.anchoredPosition, Anchors[i].anchoredPosition);
                
                
                if (distance <= 40f && minMax >= distance)
                {
                    minMax = distance;
                    minMaxPos = i;
                    clicked.transform.GetComponent<Map_Move>().check = true;
                    clicked.transform.GetComponent<Map_Move>().fixedNum = i;
                    clicked.anchoredPosition = Anchors[i].anchoredPosition;
                    break;
                }
                else
                {
                    clicked.transform.GetComponent<Map_Move>().check = false;
                }
            }
        }
		
	}
    public void NearDistance()
    {
        for (int i = 0; i < Anchors.Count; i++)
        {
            float distance = Vector2.Distance(clicked.anchoredPosition, Anchors[i].anchoredPosition);
            if (distance <= 40f)
            {
                clicked.transform.GetComponent<Map_Move>().check = true;
                clicked.anchoredPosition = Anchors[i].anchoredPosition;
                break;
            }
        }
    }

    public void Reset_Block()
    {
        int num = list_RealBlock.Count;
        if(num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                GameObject obj = list_RealBlock[0];
                list_RealBlock.RemoveAt(0);
                Destroy(obj);
            }
            num = Map_Editor.instance.real_info.Count;
            for (int i = 0; i < num; i++)
            {
                RectTransform rect = Map_Editor.instance.real_info[i];
                rect.GetComponent<Image>().color = new Color(100, 100, 100, 100);
                var item = rect.GetComponent<Map_Move>();
                item.ResetMap();
                Map_Editor.instance.SetPosition(rect, item.id, 0);
            }
        }
        PlayerSpawnner.instance.Reset_Player();
    }
}
