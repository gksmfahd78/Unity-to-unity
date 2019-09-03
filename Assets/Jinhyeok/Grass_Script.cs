using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grass_Script : MonoBehaviour {

    int num;
    public int count;
    public int count_present;

    [SerializeField]int num_Death;
    public int count_Death;
    public int count_present_Death;

    private void Awake()
    {
        num = 0;
        count_present = 0;

        num_Death = 0;
        count_present_Death = 0;
    }

    public void CountHit()
    {
        count_present++;
        if(count_present >= count)
        {
            count_present = 0;
            num++;
            int n = GrassManager_Script.instance.sprite_grass.Length;
            if (n > num)
                Upgrade_Grass(num);
            else
                num = n - 1;
        }
    }
    public void CountDeath()
    {
        count_present_Death++;
        if(count_present_Death >= count_Death)
        {
            count_present_Death = 0;
            num_Death++;
            int n = GrassManager_Script.instance.sprite_grass_death.Length;
            if (n > num_Death)
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GrassManager_Script.instance.GetBloodImg(num_Death - 1);
            else
                num = n - 1;
        }
    }

    public virtual void Upgrade_Grass(int num)
    {
        this.GetComponent<SpriteRenderer>().sprite = GrassManager_Script.instance.GetGrassImg(num);
    }
}
