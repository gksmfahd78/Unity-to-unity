using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAi : MonoBehaviour {

    private Vector2 AiVelocity;
    //ai강체
    public Rigidbody2D AiBody;
    //속도
    public float AiMoveSpeed;
    //점프
    public Vector2 AiJumpForce;
    //멈춤 쿨타임
    public float AiAccelTimer;
    //ai 가속도
    public float AiAcceleration;
    //ai 방향
    public bool AiFaceingRight;
    //ai 달리는 속도
    public float AiRunSpeed;
    //ai 걷는 속도
    public float AiWalkSpeed;
    //ai 지 속 멈춤
    public float AiAccelDurationTime;
    //ai 점프 체크
    public bool AiJumpCheck;
    //ai 점프 거리
    public float AiJumpStreet;

    public float RaycastDistance;
    //
    public bool AiDirection= true;
    //
    public float JumpDistanceInput;
    //
    public float JumpHeightInput;

	void Start () 
    {
		
	}
	
	void Update () 
    {
        
        var a = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Cliff")) | (1 << LayerMask.NameToLayer("Enemy"));

        var hit = Physics2D.Raycast(AiBody.position, Vector2.right, RaycastDistance, a);

        if (hit.collider == null)
        {
            
        }
        else
        {
            Debug.Log(hit.collider + " / " + hit.collider.gameObject.layer);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Cliff"))
            {
                Jump(1);
                Debug.Log("점프한다");
            }
            if (AiDirection == true)
            {
                LightMove();
                Debug.Log("오른쪽으로 움직인다");
            }

            if (AiDirection == false)
            {
                LeftMove();
                Debug.Log("왼쪽으로 움직인다");
            }
        }
	}

    void LeftMove() 
    {
        AiVelocity.x = -AiMoveSpeed;
        AiFaceingRight = false;
    }

    void LightMove()
    {
        AiVelocity.x = AiMoveSpeed;
        AiFaceingRight = true;
    }

    void Jump(float JumpForceScale)
    {
        AiBody.AddForce(AiJumpForce * JumpForceScale);
    }



    void StopAccel()
    {
        AiMoveSpeed = AiWalkSpeed;
    }

    void StartAccel()
    {
        AiMoveSpeed = AiRunSpeed;
        AiAccelTimer = AiAccelDurationTime;
    }

    void UpdataAccelTimer()
    {
        if (AiAccelTimer > 0f)
        {
            AiAccelTimer -= Time.deltaTime;
            if (AiAccelTimer <= 0f)
                StopAccel();
        }
    }

    void Dash() 
    {
        StartAccel();

        UpdataAccelTimer();
    }
}
