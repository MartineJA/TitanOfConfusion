using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{

    public enum PlayerState
    {
        LOCOMOTION,
        SPRINT,
        ROLLING
    }

    private PlayerState currentState;
    private Animator animator;

    [Header("Speed")]
    [SerializeField]
    private float runSpeed = 2f;

    [SerializeField]
    private float sprintSpeed = 4f;

    [SerializeField]
    private float rollSpeed = 6f;

    private float currentSpeed;


    [Header("Roll parameters")]
    [SerializeField]
    private float rollDuration = 0.5f;
    private float endRoll;


    private Rigidbody2D rb2D;
    private Vector2 direction = new Vector2();


    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        TransitionToState(PlayerState.LOCOMOTION);
        
    }

    
    private void Update()
    {
        OnStateUpdate();
    }

    private void FixedUpdate()
    {
        rb2D.velocity = direction * currentSpeed;
    }

    void OnStateEnter()
    {
        switch(currentState)
        {
                case PlayerState.LOCOMOTION:
                currentSpeed = runSpeed;
                break;

                case PlayerState.SPRINT:
                currentSpeed = sprintSpeed;
                animator.SetBool("isRunning", true);

                break;

                case PlayerState.ROLLING:
                currentSpeed = rollSpeed;
                animator.SetBool("isRolling", true);
                endRoll = Time.time + rollDuration;

                break;
                default: break;


        }
    }
    void OnStateUpdate()
    {
        switch (currentState)
        {
            case PlayerState.LOCOMOTION:
                SetDirection();

                if (Input.GetButtonDown("Fire3")) { TransitionToState(PlayerState.ROLLING); }

                break;

            case PlayerState.SPRINT:
                SetDirection();

                if (Input.GetButtonUp("Fire3"))
                {
                    TransitionToState(PlayerState.LOCOMOTION);
                }

                break;

            case PlayerState.ROLLING:
                if(Time.time> endRoll)
                {
                    if (Input.GetButton("Fire3")) // si le bouton est maintenu appuyé
                    {
                        TransitionToState(PlayerState.SPRINT);
                    }
                    else
                    {
                        TransitionToState(PlayerState.LOCOMOTION);
                    }

                    
                }

                break;
            default: break;


        }
    }

    void OnStateExit()
    {
        switch (currentState)
        {
            case PlayerState.LOCOMOTION:
                break;

            case PlayerState.SPRINT:
                animator.SetBool("isRunning", false);
                break;

            case PlayerState.ROLLING:
                animator.SetBool("isRolling", false);
                break;
            default: break;


        }
    }

    private void TransitionToState(PlayerState toState )
    {
        OnStateExit();
        currentState = toState;
        OnStateEnter();
    }

    private void SetDirection()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");

        animator.SetFloat("MoveSpeedX", direction.x);
        animator.SetFloat("MoveSpeedY", direction.y);
    }


    // Update is called once per frame

}
