using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass_Rise_Script : Grass_Script {

    public override void Upgrade_Grass(int num)
    {
        this.GetComponent<SpriteRenderer>().sprite = GrassManager_Script.instance.GetGrassRiseImg(num);
    }
}
