using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check_Player : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(collision.transform.GetComponent<GrassCheck_Script>().prevObj != null)
                collision.transform.GetComponent<GrassCheck_Script>().prevObj.GetComponent<Grass_Script>().CountDeath();
            PlayerSpawnner.instance.Check_Player(collision.gameObject); 
        }
        
       
    }

}
