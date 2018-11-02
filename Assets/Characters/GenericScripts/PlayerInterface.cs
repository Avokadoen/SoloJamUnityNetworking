using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

enum PlayerState { idle, walking, falling, jumping, punch, kick, special }

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class PlayerInterface : NetworkBehaviour
{

    [SyncVar, SerializeField]
    private float health;

    [SyncVar, SerializeField]
    private float punchSpeed;
    private float sincePunchTime;
    [SyncVar, SerializeField]
    private float punchDmg;

    [SyncVar, SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float maxMoveSpeed;

    [SyncVar, SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpCooldown;
    private float sinceJump;

    private Rigidbody2D rigidBo;
    private SpriteRenderer sprite;
    private Animator animator;

    [SerializeField]
    private Collider2D grounded;
    [SerializeField]
    private LayerMask groundLayer;

    private PlayerState playerState;

    private void Start()
    {
        Debug.Log("hello");
        rigidBo = GetComponent<Rigidbody2D>();
        if (rigidBo == null)
            Debug.LogError("failed to retrieve rigidBo");

        sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
            Debug.LogError("failed to retrieve sprite");

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("failed to retrieve animator");

        playerState = PlayerState.idle;
    }

    private void Update()
    {
        sincePunchTime  += Time.deltaTime;
        sinceJump       += Time.deltaTime;

        if (playerState == PlayerState.jumping && rigidBo.velocity.y <= 0)
            playerState = PlayerState.falling;

        if (playerState == PlayerState.falling && rigidBo.velocity.y <= 0 && rigidBo.velocity.y >= -0)
            playerState = PlayerState.idle;

        if (playerState == PlayerState.walking && rigidBo.velocity.x <= 0 && rigidBo.velocity.x >= -0)
            playerState = PlayerState.idle;

        if (playerState == PlayerState.punch && sincePunchTime > punchSpeed)
            playerState = PlayerState.idle;

        animator.SetBool("RestartState", false);
        animator.SetInteger("PlayerState", (int)playerState);
    }

    public void Move(float xAxis)
    {
        sprite.flipX = (xAxis > 0);

        if ((rigidBo.velocity.x > 0 && xAxis < 0) || (rigidBo.velocity.x < 0 && xAxis > 0))
            StopMove(); // todo: solve another way

        if (rigidBo.velocity.x < maxMoveSpeed && rigidBo.velocity.x > -maxMoveSpeed)
            ApplyForce(new Vector2(xAxis, 0) * moveSpeed);

        if (grounded.IsTouchingLayers(groundLayer))
        {
            playerState = PlayerState.walking;

            
        }

    }

    public void StopMove()
    {
        rigidBo.velocity = new Vector2(0, rigidBo.velocity.y);
    }

    public void Jump()
    {

        if (grounded.IsTouchingLayers(groundLayer) && sinceJump > jumpCooldown)
        {
            playerState = PlayerState.jumping;
            sinceJump = 0f;
            ApplyForce(Vector2.up * jumpForce);
        }
        
    }

    public void TakeDmg(float damage)
    {
        health -= damage;
    }

    public void ApplyForce(Vector2 force)
    {
        rigidBo.AddForce(force);
    }

    public void Punch()
    {
        if (sincePunchTime < punchSpeed)
            return;

        animator.SetBool("RestartState", true);
        playerState = PlayerState.punch;
        sincePunchTime = 0;

        Collider2D[] colliders = null;
        Vector3 fistOffset;

        if (sprite.flipX == true)
        {
            fistOffset = new Vector3(0.208f, 0.015f, 0);
        }
        else
        {
            fistOffset = new Vector3(-0.208f, 0.015f, 0);  
        }
        colliders = Physics2D.OverlapBoxAll(transform.position + fistOffset, new Vector2(0.1f * transform.localScale.x, 0.1f * transform.localScale.y), 0);

        foreach(Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            var otherPlayer = collider.gameObject.GetComponent<PlayerInterface>();
            if(otherPlayer != null)
            {
                otherPlayer.TakeDmg(punchDmg);
            }
        }
    }
}
