using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCheck_Script : MonoBehaviour {
    int layermask;
    public float height;
    public float distance;
    public GameObject prevObj;
	// Use this for initialization
	void Start () {
        layermask = 1<<LayerMask.NameToLayer("Wall");
        height = 100f;
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up * -1, height, layermask);
        if(hit.collider != null )
        {
            float dis = Vector2.Distance(transform.position, hit.point);
            if (prevObj != hit.collider.gameObject && dis < distance)
            {
                prevObj = hit.collider.gameObject;
                prevObj.GetComponent<Grass_Script>().CountHit();
            }
        }

        Vector2 pos = transform.position;
        if (pos.y <= -540)
        {
            if(prevObj != null)
                prevObj.GetComponent<Grass_Script>().CountDeath();
            PlayerSpawnner.instance.Check_Player(this.gameObject);
               
        }

    }
}
