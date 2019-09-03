using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_Script : MonoBehaviour {

    public AnimationCurve curve;
    public float AnimationSpeed;
    public float MovementScale;

    private float timer;

	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime * AnimationSpeed;
        if(timer > 1)
        {
            timer -= 1;
        }
        float y = curve.Evaluate(timer) * MovementScale;
        transform.localPosition = new Vector2(0, y);
    }
}
