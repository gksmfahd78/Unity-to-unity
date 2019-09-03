using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Map_Move : MonoBehaviour {
    RectTransform canvasRect;

    public bool check;
    public int ownNum;
    public int id;
    public bool clicked;
    GameObject newObj;
    [NonSerialized]public bool isDocked = true;

    public int fixedNum;
    public EventTrigger eventTrigger;
    
    //public Vector2
    private void Awake()
    {
        canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();
        check = false;
        clicked = false;
        
    }
    public RectTransform rect;
    public void Clicked()
    {
        clicked = true;
        Scroll_Scene.instance.ischeck = false;
        Scroll_Scene.instance.ischeck = true;
        Move_Drag();
        if (newObj != null)
        {
            ResetMap();
            Destroy(newObj);
            
        }
        if (FixPosition.instance != null)
            FixPosition.instance.clicked = rect;
    }
    public void ResetMap()
    {
        FixPosition.instance.Anchors[fixedNum].GetComponent<Fixed_Script>().ischecked = false;
        
        fixedNum = 0;
        this.GetComponent<Image>().color = new Color(100, 100, 100, 100);
    }
    private void Update()
    {
        if(clicked == true && check == false)
        {

            Vector3 pos = Input.mousePosition;
            pos.y -= Camera.main.transform.position.y;
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = 0;




            pos.y += Camera.main.transform.position.y; 
            
            
            
            
            this.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    public void Move_Drag()
    {
        transform.parent = canvasRect.transform;
        
        if (clicked == true && check == true)
        {

            Vector3 pos = Input.mousePosition;
            pos.y -= Camera.main.transform.position.y;
            pos = Camera.main.ScreenToWorldPoint(pos);
            
            
            
            pos.y += Camera.main.transform.position.y; 
            
            
            
            pos.z = 0;
            this.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
    public void Make_Block()
    {
        bool checkFixed = FixPosition.instance.Anchors[fixedNum].GetComponent<Fixed_Script>().ischecked;
        clicked = false;
        Scroll_Scene.instance.ischeck = false;
        Debug.Log(check + " / " + checkFixed);
        //if (check == true && checkFixed == false && isDocked)
        if (check == true && checkFixed == false)
        {
            Debug.Log("1");

            GameObject resource = Resources.Load("JinHyeok/RealBlock/Block" + id) as GameObject;

            newObj = Instantiate(resource, transform.position, Quaternion.identity);
            if (id == 4)
            {
                newObj.transform.Rotate(new Vector3(0, 180, 0));
            }
            newObj.GetComponent<Block_Script>().check = this;
            FixPosition.instance.list_RealBlock.Add(newObj);
            FixPosition.instance.Anchors[fixedNum].GetComponent<Fixed_Script>().ischecked = true;

            this.GetComponent<Image>().color = new Color(100, 100, 100, 0);
            //isDocked = false;

            Map_Editor.instance.AddMap(id);
        }
        else
        {
            Debug.Log("2");

            var add = isDocked ? 0 : 1;
            Map_Editor.instance.SetPosition(Map_Editor.instance.real_info[id], id, add, 0f);
            isDocked = true;
        }
    }
}
