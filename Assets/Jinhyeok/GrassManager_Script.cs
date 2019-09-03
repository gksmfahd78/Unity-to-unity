using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager_Script : MonoBehaviour {
    public static GrassManager_Script instance;
    public Sprite[] sprite_grass;
    public Sprite[] sprite_rise_grass;
    public Sprite[] sprite_grass_death;
    private void Awake()
    {
        if (GrassManager_Script.instance == null)
            GrassManager_Script.instance = this;
        sprite_grass = Resources.LoadAll<Sprite>("JinHyeok/Grass");
        sprite_grass_death = Resources.LoadAll<Sprite>("JinHyeok/Blood");
        sprite_rise_grass = Resources.LoadAll<Sprite>("JinHyeok/GrassRise");
        GetGrassImg(0);
    }
    public Sprite GetGrassImg(int info)
    {
        return sprite_grass[info];
    }
    public Sprite GetBloodImg(int info)
    {
        return sprite_grass_death[info];
    }
    public Sprite GetGrassRiseImg(int info)
    {
        return sprite_rise_grass[info];
    }
}

