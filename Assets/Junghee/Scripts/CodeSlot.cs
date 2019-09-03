using System;
using UnityEngine;

public class CodeSlot : MonoBehaviour
{
	public SLOT_TYPE SlotType;
	[NonSerialized] public int Id;
	[NonSerialized] public CodeItem host;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum SLOT_TYPE
{
	Idea,
	ConditionCode,
	MovementCode
}