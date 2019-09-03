using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class CodeItem : SerializedMonoBehaviour
{
	public CODE_TYPE CodeType;
	public CONDITION Condition;
	public MOVEMENT Movement;
	[HideLabel] public MinMax ValueRange;
	[NonSerialized] public RectTransform RectTransform;
	[NonSerialized] public CodeSlot CodeSlot;
	
	
	private void Awake()
	{
		RectTransform = GetComponent<RectTransform>();
	}

	void Start () 
	{
		CodeSlot = AiEditor.Instance.GetClosestSlot(this);
		CodeSlot.host = this;
	}
	
	void Update () 
	{
		
	}

	public void OnClick()
	{
		AiEditor.Instance.OnSelectCodeItem(this);
	}
	
	public void OnRelease()
	{
		AiEditor.Instance.OnReleaseCodeItem(this);
	}
}

public enum CONDITION
{
	None,
	ThereIsCliff,
	ThereIsWall,
	ThereIsEnemy,
	ThereIsHill,
	ThereIsThwomp,
	ThereIsYellowSign,
	AfterShot,
	RunningToCliff,
	AfterTurn
}

public enum MOVEMENT
{
	None,
	Jump,
	Dash,
	Wait,
	ShootFireBall,
	TurnBack
}

public enum CODE_TYPE
{
	Condition,
	Movement
}

[Serializable]
public class MinMax
{
	[HorizontalGroup("MinMax")] public float Min;
	[HorizontalGroup("MinMax")] public float Max;

	public float GetRandom()
	{
		return Random.Range(Min, Max);
	}

	public bool GetIsIn(float value)
	{
		return value > Min && value < Max;
	}
}