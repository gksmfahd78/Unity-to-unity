
using System.Collections.Generic;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AiEditor : SerializedMonoBehaviour
{
	public static AiEditor Instance;
	private List<CodeItem> _ideaPages;
	private CodeItemSet[] _codingPages;
	[Range(0, 100)][SerializeField] private int _conditionThreshold;
	private CodeSlot[] _codeSlots;
	private CodeItem _selectedCodeItem;
	private CodeSlot _originalSlot;
	private CodeSlot _snappedSlot;
	private int _isSnapped;
	private MovementData _nullMovementData = new MovementData(MOVEMENT.None, 0f);
	[Range(0, 100)][SerializeField] private float _slotSnapDistance;
	[Range(0, 60)][SerializeField] private int _snapWaitCount;
	[SerializeField] private Vector2 _codeSnapScale;
	[Range(0.5f, 2f)][SerializeField] private float _itemScaleDuringDrag;
	[Range(0f, 1000f)][SerializeField] private float _conditionValueScale;
	
	private void Awake()
	{
		Instance = this;

		SetupCodeSlots();
		SetupCodeItems();
	}

	private void SetupCodeSlots()
	{
		_codeSlots = transform.Find("Canvas").GetComponentsInChildren<CodeSlot>();

		var codes = transform.Find("Canvas").Find("CodingPage").Find("Codes");
		for (int i = 0; i < codes.childCount; i++)
			codes.GetChild(i).GetComponent<CodeSlot>().Id = i / 2;
	}

	private void SetupCodeItems()
	{
		// 원래 여기서 생성해야 할 듯
		_ideaPages = new List<CodeItem>();
		var codItems = transform.Find("Canvas").GetComponentsInChildren<CodeItem>();
		for (var i = 0; i < codItems.Length; i++)
			_ideaPages.Add(codItems[i]);
		
		_codingPages = new CodeItemSet[16];
		for (var i = 0; i < _codingPages.Length; i++)
			_codingPages[i] = new CodeItemSet();
	}

	void Start ()
	{
		if (SceneManager.sceneCount > 1)
		{
//			transform.Find("Camera").gameObject.SetActive(false);
//			transform.Find("EventSystem").gameObject.SetActive(false);
			transform.Find("Canvas").gameObject.SetActive(false);
		}
	}
	
	void Update ()
	{
		DragSelectedCodeItem();

		CheckShowHideEditor();
	}

	private void CheckShowHideEditor()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
			ToggleOnOff();
	}

	public void ToggleOnOff()
	{
		transform.Find("Canvas").gameObject.SetActive(!transform.Find("Canvas").gameObject.activeSelf);

		Scroll_Scene.instance.ischeck = transform.Find("Canvas").gameObject.activeSelf;

		SoundManager.Instance.PlaySfx(SFX.Ui_MouseDown);
	}

	public MovementData GetWhatToDo(CONDITION condition, float conditionValue)
	{
		conditionValue /= _conditionValueScale;

		foreach (var code in _codingPages)
		{
			if (code.Condition != null && code.Movement != null && code.Condition.Condition == condition)
			{
				var isIn = code.Condition.ValueRange.GetIsIn(conditionValue);
//				Debug.Log(code.Condition.Condition + " >>> " + code.Condition.ValueRange.Min + " / " +code.Condition.ValueRange.Max + " ???? " + conditionValue + " = " + isIn);
				if (isIn)
				{
					if (Random.Range(0, 100) < _conditionThreshold)
					{
						return new MovementData(code.Movement.Movement, code.Movement.ValueRange.GetRandom());
					}
				}
			}
		}

		return _nullMovementData;
	}

	private void DragSelectedCodeItem()
	{
		if (_selectedCodeItem == null) return;

		_selectedCodeItem.transform.position = GetMousePosition();

		CheckSlotSnap(_selectedCodeItem);
	}

	public void CheckSlotSnap(CodeItem codeItem)
	{
		foreach (var slot in _codeSlots)
		{
			if (slot.host == null || slot.host == codeItem)
			{
				if (GetIsTypeMatch(codeItem, slot))
				{
					var distance = GetDistanceToSlot(codeItem, slot);
					if (distance < _slotSnapDistance)
					{
						_selectedCodeItem.transform.position = slot.transform.position;
						_snappedSlot = slot;
						_isSnapped = _snapWaitCount;
						return;
					}
				}
			}
		}
		_isSnapped = Mathf.Max(_isSnapped - 1, 0);
	}

	private bool GetIsTypeMatch(CodeItem codeItem, CodeSlot slot)
	{
		switch (slot.SlotType)
		{
			case SLOT_TYPE.Idea:
				return true;
			case SLOT_TYPE.ConditionCode:
				return codeItem.CodeType == CODE_TYPE.Condition;
			case SLOT_TYPE.MovementCode:
				return codeItem.CodeType == CODE_TYPE.Movement;
		}

		return false;
	}

	public float GetDistanceToSlot(CodeItem codeItem, CodeSlot codeSlot)
	{
		return Vector2.Scale(codeItem.RectTransform.anchoredPosition - (Vector2)codeSlot.transform.position, _codeSnapScale).magnitude;
	}

	public CodeSlot GetClosestSlot(CodeItem codeItem)
	{
		var closestDistance = float.MaxValue;
		var closestSlot = _codeSlots[0];
		foreach (var codeSlot in _codeSlots)
		{
			var distance = GetDistanceToSlot(codeItem, codeSlot);
			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestSlot = codeSlot;
			}
		}

		return closestSlot;
	}

	public Vector2 GetMousePosition()
	{
		Vector2 pos = Input.mousePosition;
		
		pos.y += Camera.main.transform.position.y; 
		
		return Camera.main.ScreenToWorldPoint(pos);
	}
	
	public void OnSelectCodeItem(CodeItem codeItem)
	{
		_selectedCodeItem = codeItem;
		_originalSlot = GetClosestSlot(_selectedCodeItem);
		_isSnapped = 0;

		_selectedCodeItem.transform.localScale = Vector3.one * _itemScaleDuringDrag;
		
		SoundManager.Instance.PlaySfx(SFX.Ui_MouseDown);
	}

	public void OnReleaseCodeItem(CodeItem codeItem)
	{
		var targetSlot = _isSnapped > 0 ? _snappedSlot : _originalSlot;

		ArrangeCode(_selectedCodeItem, targetSlot);
		
		_selectedCodeItem.transform.localScale = Vector3.one;
		
		_selectedCodeItem.transform.position = targetSlot.transform.position;
		_selectedCodeItem = null;
		
		if(codeItem.CodeSlot.SlotType == SLOT_TYPE.Idea)
			SoundManager.Instance.PlaySfx(SFX.Ui_MouseDown);
		else
			SoundManager.Instance.PlaySfx(SFX.Ui_MouseUp);
	}

	private void ArrangeCode(CodeItem codeItem, CodeSlot targetSlot)
	{
		if (codeItem.CodeSlot == targetSlot) return;

		switch (codeItem.CodeSlot.SlotType)
		{
			case SLOT_TYPE.Idea:
				_ideaPages.Remove(codeItem);
				break;
			case SLOT_TYPE.ConditionCode:
				_codingPages[codeItem.CodeSlot.Id].Condition = null;
				break;
			case SLOT_TYPE.MovementCode:
				_codingPages[codeItem.CodeSlot.Id].Movement = null;
				break;
		}
		
		switch (targetSlot.SlotType)
		{
			case SLOT_TYPE.Idea:
				_ideaPages.Add(codeItem);
				break;
			case SLOT_TYPE.ConditionCode:
				_codingPages[targetSlot.Id].Condition = codeItem;
				break;
			case SLOT_TYPE.MovementCode:
				_codingPages[targetSlot.Id].Movement = codeItem;
				break;
		}
		
		codeItem.CodeSlot.host = null;
		targetSlot.host = codeItem;

		codeItem.CodeSlot = targetSlot;
	}
}

public class MovementData
{
	public MOVEMENT Movement;
	public float MovementValue;

	public MovementData(MOVEMENT movement, float movementValue)
	{
		Movement = movement;
		MovementValue = movementValue;
	}
}

public class CodeItemSet
{
	public CodeItem Condition;
	public CodeItem Movement;
}
