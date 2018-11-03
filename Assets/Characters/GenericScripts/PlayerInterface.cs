using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum PlayerState { idle, walking, falling, jumping, punch, kick, special, dead }

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class PlayerInterface : NetworkBehaviour
{

    [SyncVar]
    public float health;

    [SerializeField]
    private float punchSpeed;
    private float sincePunchTime;
    [SerializeField]
    private float punchDmg;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float maxMoveSpeed;

    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpCooldown;
    private float sinceJump;

    private Rigidbody2D rigidBo;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField]
    private Collider2D grounded;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask characterLayer;

    private PlayerState playerState;

    private void Start()
    {
        rigidBo = GetComponent<Rigidbody2D>();
        if (rigidBo == null)
            Debug.LogError("failed to retrieve rigidBo");

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("failed to retrieve sprite");

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("failed to retrieve animator");


        playerState = PlayerState.idle;
    }

    private void Update()
    {
        if (playerState != PlayerState.dead  && health <= 0)
        {
            playerState = PlayerState.dead;
            animator.SetInteger("PlayerState", (int)playerState);
            rigidBo.isKinematic = true;
        }
        if (playerState == PlayerState.dead)
        {
            return;
        }
        if(!isLocalPlayer)
        {
            if(spriteRenderer.flipX == false && rigidBo.velocity.x > 0.01f)
            {
                spriteRenderer.flipX = true;
            } else if (spriteRenderer.flipX == true && rigidBo.velocity.x < -0.01f)
            {
                spriteRenderer.flipX = false;
            }
            return;
        }

        sincePunchTime  += Time.deltaTime;
        sinceJump       += Time.deltaTime;

        if (playerState == PlayerState.jumping && rigidBo.velocity.y > 0.0f)
            playerState = PlayerState.jumping;

        else if (playerState == PlayerState.jumping && rigidBo.velocity.y <= 0.0f)
            playerState = PlayerState.falling;

        else if (playerState == PlayerState.falling && rigidBo.velocity.y <= 0.01f && rigidBo.velocity.y >= -0.01f)
            playerState = PlayerState.idle;

        else if(playerState == PlayerState.walking && rigidBo.velocity.x <= 0.01f && rigidBo.velocity.x >= -0.01f)
            playerState = PlayerState.idle;

        else if(playerState == PlayerState.punch && sincePunchTime > punchSpeed)
            playerState = PlayerState.idle;  
        
        animator.SetInteger("PlayerState", (int)playerState);
    }

    public void Move(float xAxis)
    {
        spriteRenderer.flipX = xAxis > 0;

        if ((rigidBo.velocity.x > 0 && xAxis < 0) || (rigidBo.velocity.x < 0 && xAxis > 0))
            StopMove(); // todo: solve another way

        if (rigidBo.velocity.x < maxMoveSpeed && rigidBo.velocity.x > -maxMoveSpeed)
            ApplyForce(new Vector2(xAxis, 0) * moveSpeed);

        if (grounded.IsTouchingLayers(groundLayer | characterLayer))
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

        if (grounded.IsTouchingLayers(groundLayer | characterLayer) && sinceJump > jumpCooldown && rigidBo.velocity.y <= 0)
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

    public bool IsPlayerDead()
    {
        return playerState == PlayerState.dead;
    }


    public void PrePunch()
    {
        if (sincePunchTime < punchSpeed)
            return;

        playerState = PlayerState.punch;
        sincePunchTime = 0;
    }

    public void Punch()
    {  
        Collider2D[] colliders = null;
        Vector3 fistOffset;

        if (spriteRenderer.flipX == true)
        {
            fistOffset = new Vector3(0.208f * transform.localScale.x, 0.015f * transform.localScale.y, 0);
        }
        else
        {
            fistOffset = new Vector3(-0.208f * transform.localScale.x, 0.015f * transform.localScale.y, 0);  
        }
        colliders = Physics2D.OverlapBoxAll(transform.position + fistOffset, new Vector2(0.1f * transform.localScale.x, 0.1f * transform.localScale.y), 0, characterLayer);

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

    public bool IsLocalPlayer()
    {
        return isLocalPlayer;
    }
}
