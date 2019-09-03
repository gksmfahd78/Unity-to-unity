
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]   
public class StickTo : MonoBehaviour
{
    [BoxGroup("위치 따라하기")][SerializeField]private bool _stickToPosition = true;
    [ShowIf("_stickToPosition")][BoxGroup("위치 따라하기")] public Transform Target;
    [ShowIf("_stickToPosition")][BoxGroup("위치 따라하기")][SerializeField] private Vector3 _offset;
    [ShowIf("_stickToPosition")][BoxGroup("위치 따라하기")][SerializeField] private bool _snap;
    [ShowIf("_stickToPosition")][BoxGroup("위치 따라하기")][SerializeField] private bool _usePositionScale = false;
    [ShowIf("_stickToPosition")][BoxGroup("위치 따라하기")][SerializeField] private Vector3 _positionScale;
    
    [BoxGroup("방향 따라하기")][SerializeField]private bool _stickToRotation = false;
    [ShowIf("_stickToRotation")][BoxGroup("방향 따라하기")][SerializeField] private Transform _rotationTarget;
    [ShowIf("_stickToRotation")][BoxGroup("방향 따라하기")][SerializeField] private Vector3 _rotationOffset;
    [ShowIf("_stickToRotation")][BoxGroup("방향 따라하기")][SerializeField] private Vector3 _rotationSnap;


    private void Awake()
	{
		if(_stickToPosition && Target == null) Target = Camera.main.transform;
	}

	protected virtual void LateUpdate ()
	{
	    if (_stickToPosition && Target != null)
	    {
	        var position = Target.transform.position;
            if(_usePositionScale && _positionScale != null)
                position.Scale(_positionScale);
	        position += _offset;

	        if (_snap)
	            position.Set(position .x - position.x % 1, position .y - position.y % 1, position.z - position.z % 1);
	        transform.position = position;
	    }

	    if (_stickToRotation && _rotationTarget != null)
	    {
	        var rotation = _rotationTarget.eulerAngles + _rotationOffset;
	        rotation.x = SnapDegree(rotation.x, _rotationSnap.x);
	        rotation.y = SnapDegree(rotation.y, _rotationSnap.y);
	        rotation.z = SnapDegree(rotation.z, _rotationSnap.z);

	        transform.eulerAngles = rotation;
	    }
	}

    public void SetRotation(float degreeY)
    {
        var rotation = transform.eulerAngles;
        rotation.y = degreeY + _rotationOffset.y;
        rotation.y = SnapDegree(rotation.y, _rotationSnap.y);

        transform.eulerAngles = rotation;
    }

    private float SnapDegree(float degree, float snap)
    {
        return snap <= 0? degree : ((Mathf.Round(degree / snap) * snap) % 360f);
    }
}
