using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoveAi : MonoBehaviour {

    //ai강체
    public Rigidbody2D AiBody;
    //점프
    public Vector2 AiJumpForce;
    //ai 달리는 속도
    public float AiRunSpeed;
    //ai 걷는 속도
    public float AiWalkSpeed;
    //Ai가 앞을 보고 RayCast하는 거리
    public float AiLookDistance;
    //Ai가 밑으로 RayCast하는 거리 - 땅에 착지했는지 체크하는 용도
    public float GroundCheckDistance;
    //ai속도 = ai강체.속도
    private Vector2 AiVelocity;
    //속도
    [ReadOnly][ShowInInspector] private float AiMoveSpeed;
    //대쉬 지속시간
    private float _aiDashTimer;
    //멈춤 지속시간
    private float _waitingTimer;
    //뒤돌아본 후 타이머
    private float _turnBackTimer;
    //Ai가 오른쪽을 보고 있는지
    private float _aiFacingSign;
    private bool _gameStarted = false;
    private bool _walkStarted = false;
    private bool _didTurnBack = false;
    
    public int MyBulletIndex;
    public Rigidbody2D[] MyBullet;

    public Vector2 MyShotForceLeft;
    public Vector2 MyShotForceRight;

    private List<AiCheckPair> _checkedTriggers;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private LayerMask _lookMask;
    [ReadOnly] public bool IsGrounded;
    private LayerMask _groundMask;
    [SerializeField] private float _cliffRecheckDistance;

    private void Awake()
    {
        _checkedTriggers = new List<AiCheckPair>();
        _animator = transform.GetComponent<Animator>();
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
        
        _lookMask = (1 << LayerMask.NameToLayer("Warning")) |(1 << LayerMask.NameToLayer("Upstair"))| (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Ground")) |  (1 << LayerMask.NameToLayer("Cliff")) | (1 << LayerMask.NameToLayer("Enemy"));
        _groundMask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Ground"));
    }
    


    void Start ()
	{
	    StartGame();
	    
	    
	    
	    _aiFacingSign = 1f;
        for (int i = 0; i < MyBullet.Length; i++) 
        {
            MyBullet[i].gameObject.SetActive(false);
            MyBullet[i].transform.parent = null;
        }
	}
	
	void Update () 
    {
        if(!_gameStarted) return;


        
        AiVelocity = AiBody.velocity;

        // 처음 바닥에 착지하면 움직이기 시작하자. 시작은 위에서 떨어지는게 재미있음
        if (!_walkStarted && IsGrounded)
        {
            // 일단 오른쪽으로 걷자
            _walkStarted = true;
            AiMoveSpeed = AiWalkSpeed;
        }

        
        // 캐릭터는 일단 이동속도대로 움직인다. 이동속도는 AI코딩에 따라 바뀐다
        AiVelocity.x = AiMoveSpeed * _aiFacingSign;
        
        
        
         var hit = Physics2D.Raycast(AiBody.position, Vector2.right, AiLookDistance, _lookMask);
        if(hit.collider == null) //앞에 아무도 없다
        {
            
        }
        else //앞에 뭐가 있다
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Cliff")) 
            {
                var hit2 = Physics2D.Raycast(hit.collider.transform.position, Vector2.down, _cliffRecheckDistance, _groundMask);
                if (hit2.collider == null)
                {
                    //ai 코드대로 행동하기
                    MoveByAiPattern(CONDITION.ThereIsCliff, hit.distance, hit.collider);
                }
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                //ai 코드대로 행동하기
                MoveByAiPattern(CONDITION.ThereIsWall, hit.distance, hit.collider);
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Upstair")) 
            {
                //ai 코드대로 행동하기
                MoveByAiPattern(CONDITION.ThereIsHill, hit.distance, hit.collider);
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Warning")) 
            {
                //ai 코드대로 행동하기
                MoveByAiPattern(CONDITION.ThereIsYellowSign, hit.distance, hit.collider);
            }
        }

        UpdataAccelTimer();
        UpdataWaitingTimer();
        UpdataTurnBackTimer();
        
        AiBody.velocity = AiVelocity;
	}

    void LateUpdate()
    {
        IsGrounded = GetIsGrounded();

        UpdateGraphic();
    }

    private void UpdateGraphic()
    {
        if (IsGrounded)
        {
            if(_waitingTimer > 0f)
                _animator.Play("Stay", 0);
            else if(_aiDashTimer > 0f)
                _animator.Play("Run", 0);
            else
                _animator.Play("Walk", 0);
        }
        else
        {
            _animator.Play("Jump", 0);
        }

        _spriteRenderer.flipX = _aiFacingSign < 0f;
    }

    public void StartGame()
    {
        _gameStarted = true;
        AiBody.isKinematic = false;
        _animator.Play("Stay", 0);
    }

    private bool GetIsGrounded()
    {
        var hit = Physics2D.Raycast(AiBody.position, Vector2.down, GroundCheckDistance, _groundMask);
        if(hit.collider == null) // 밑에 바닥이 없다. 점프 혹은 낙하 중
        {
            return false;
        }
        else // 밑에 바닥이 있다. 땅에 서 있는 중
        {
            return true;
        }
    }

    private void MoveByAiPattern(CONDITION Condition, float Value, Collider2D collider2D)
    {
        var moveData = AiEditor.Instance.GetWhatToDo(Condition, Value);

        if (_checkedTriggers.Contains(AiCheckPair.Set(collider2D, moveData.Movement))) return;
        
        
        
        switch (moveData.Movement)
        {
            case MOVEMENT.None:
                break;
            case MOVEMENT.Jump:
//                if (IsGrounded)
                    Jump(moveData.MovementValue);
                break;
            case MOVEMENT.Dash:
                if (IsGrounded)
                    Dash(moveData.MovementValue);
                break;
            case MOVEMENT.Wait:
                if (IsGrounded)
                    Wait(moveData.MovementValue);
                break;
            case MOVEMENT.ShootFireBall:
                Shoot(moveData.MovementValue);
                break;
            case MOVEMENT.TurnBack:
                TurnBack();
                break;
        }
        
        
        if(moveData.Movement != MOVEMENT.None)
            _checkedTriggers.Add(new AiCheckPair(collider2D, moveData.Movement));

    }
    void UpdataAccelTimer() 
    {
        //AiAccelTimer가 0보다 클때
        if(_aiDashTimer > 0f)        
        {
            //AiAccelTimer에 시간을 뺀다
            _aiDashTimer -= Time.deltaTime;

            //AiAccelTimer이 0보다 작거나 같으면
            if(_aiDashTimer <= 0f)
                //StopAccel함수 호출
                StopDash();
        }       
    }

    void Wait(float Duration) 
    {
        //저장된 원래 값 복부
        AiMoveSpeed = 0;
        _waitingTimer = Duration;
    }

    void UpdataWaitingTimer()
    {
        //AiAccelTimer가 0보다 클때
        if(_waitingTimer > 0f)        
        {
            //AiAccelTimer에 시간을 뺀다
            _waitingTimer -= Time.deltaTime;

            //AiAccelTimer이 0보다 작거나 같으면
            if (_waitingTimer <= 0f)
                //StopAccel함수 호출
                stopWaiting();
        }       
    }
    
    void UpdataTurnBackTimer()
    {
        if(_didTurnBack && _turnBackTimer <= 0f)        
        {
            _turnBackTimer += Time.deltaTime;

            MoveByAiPattern(CONDITION.AfterTurn, _turnBackTimer, null);

            if (_turnBackTimer > 9f)
                _didTurnBack = false;
        }       
    }

    void stopWaiting()
    {
        AiMoveSpeed = AiWalkSpeed;
    }

    void StopDash() 
    {
        //ai 움직이는 가속도를 0으로 바꾼다
        AiMoveSpeed = AiWalkSpeed;
    }

    void Dash(float Duration) 
    {
        AiMoveSpeed = AiRunSpeed;
        _aiDashTimer = Duration;
    }

    void Jump(float jumpForceScale) 
    {
        AiBody.AddForce(AiJumpForce * jumpForceScale);
        SoundManager.Instance.PlaySfx(SFX.Jump_0);
    }


    void Shoot(float PowerScale) 
    {
        var bullet = MyBullet[MyBulletIndex];
            
        bullet.gameObject.SetActive(true);
        bullet.position = AiBody.position;
        bullet.velocity = Vector2.zero;

        //플레이어방향이 왼쪽이, 왼쪽으로 , 아니면 오른쪽으로 힘이 가해진
        if (_aiFacingSign > 0f)
            bullet.AddForce(MyShotForceLeft * PowerScale);
        else
            bullet.AddForce(MyShotForceRight * PowerScale);
        
        MyBulletIndex++;

        if(MyBulletIndex >= MyBullet.Length)
        {
            MyBulletIndex = 0;
        }
    }

    void TurnBack() 
    {
        AiVelocity.x *= -1f;
        _aiFacingSign *= -1f;

        _didTurnBack = true;
        _turnBackTimer = 0f;
        
        MoveByAiPattern(CONDITION.AfterTurn, 0f, null);
    }
}

public struct AiCheckPair
{
    private static AiCheckPair _temp;
    public Collider2D Trigger;
    public MOVEMENT Movement;

    public AiCheckPair(Collider2D trigger, MOVEMENT movement)
    {
        Trigger = trigger;
        Movement = movement;
    }

    public static AiCheckPair Set(Collider2D trigger, MOVEMENT movement)
    {
        _temp.Trigger = trigger;
        _temp.Movement = movement;
        
        return _temp;
    }
}