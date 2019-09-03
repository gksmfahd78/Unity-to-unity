using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll_Scene : MonoBehaviour {

    public static Scroll_Scene instance;
    private float dist;
    private Vector3 MouseStart;
    private Vector3 derp;
    public bool ischeck;
    public Vector3 startPos;
    private void Awake()
    {
        if (Scroll_Scene.instance == null)
            Scroll_Scene.instance = this;
        ischeck = false;
        startPos = Camera.main.ScreenToWorldPoint(Vector3.zero);
    }
    void Start()
    {
        dist = transform.position.z;
    }

    void Update()
    {
        if(ischeck == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseStart = new Vector3(0, Input.mousePosition.y, dist);
                MouseStart = Camera.main.ScreenToWorldPoint(MouseStart);
                MouseStart.z = transform.position.z;

            }
            else if (Input.GetMouseButton(0))
            {
                var MouseMove = new Vector3(0, Input.mousePosition.y, dist);
                MouseMove = Camera.main.ScreenToWorldPoint(MouseMove);
                MouseMove.z = transform.position.z;
                transform.position = transform.position - (MouseMove - MouseStart);
            }
        }
        
    }
    public void StopScroll()
    {
        ischeck = true;
    }
    public void StartScroll()
    {
        ischeck = false;
    }
}
