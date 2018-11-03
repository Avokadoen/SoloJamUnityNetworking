using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum PlayerState { idle, walking, falling, jumping, punch, kick, special, block, dead }

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
    private float kickSpeed;
    private float sinceKickTime;
    [SerializeField]
    private float kickDmg;

    [SerializeField]
    private float blockResistance;
    public bool isBlocking;

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
    private Cinemachine.CinemachineVirtualCamera cinemachineVC;
    private GameObject lookAtPoint;

    [SerializeField]
    private AudioClip tookPureDamage;
    [SerializeField]
    private AudioClip tookBlockDamage;

    private AudioSource audioSource;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GameObject vCamera = GameObject.Find("VirtualCamera");
        if(vCamera == null)
        {
            Debug.LogError("failed to retrieve vCamera");
        }

        cinemachineVC = vCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();

        //cinemachineVC.Follow = transform;
        //cinemachineVC.Follow = lookAtPoint.transform;
        lookAtPoint = transform.Find("LookAtPoint").gameObject;
        cinemachineVC.Follow = lookAtPoint.transform;

    }

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
        if (rigidBo.velocity.x > 0.00001f || rigidBo.velocity.x < -0.00001f)
        {
            spriteRenderer.flipX = rigidBo.velocity.x > 0.00001f;
        }

        if (playerState != PlayerState.dead  && health <= 0)
        {
            playerState = PlayerState.dead;
            animator.SetInteger("PlayerState", (int)playerState);
            rigidBo.isKinematic = true;
            cinemachineVC.Follow = null;
            cinemachineVC.LookAt = null;
        }
        if (playerState == PlayerState.dead || !isLocalPlayer)
        {
            return;
        }

        Vector3 lookatOffset = new Vector3();
        float speedThreshold = 0.2f;
        float offset = 0.8f;
        if(rigidBo.velocity.x > speedThreshold)
        {
            lookatOffset.x = offset * transform.lossyScale.x;
        }
        else if (rigidBo.velocity.x < -speedThreshold)
        {
            lookatOffset.x = -offset * transform.lossyScale.x;
        }
        if (rigidBo.velocity.y > speedThreshold)
        {
            lookatOffset.y = offset * 0.4f * transform.lossyScale.y;
        }
        else if (rigidBo.velocity.y < -speedThreshold)
        {
            lookatOffset.y = -offset * 0.4f * transform.lossyScale.y;
        }
        lookAtPoint.transform.position = transform.position + lookatOffset;

        sinceKickTime   += Time.deltaTime;
        sincePunchTime  += Time.deltaTime;
        sinceJump       += Time.deltaTime;

        if (playerState == PlayerState.jumping && rigidBo.velocity.y > 0.0f)
            playerState = PlayerState.jumping;

        else if ((playerState == PlayerState.jumping || playerState == PlayerState.idle) && rigidBo.velocity.y < 0.0f)
            playerState = PlayerState.falling;

        else if (playerState == PlayerState.falling && rigidBo.velocity.y <= 0.01f && rigidBo.velocity.y >= -0.01f)
            playerState = PlayerState.idle;

        else if(playerState == PlayerState.walking && rigidBo.velocity.x <= 0.01f && rigidBo.velocity.x >= -0.01f)
            playerState = PlayerState.idle;

        else if(playerState == PlayerState.punch && sincePunchTime > punchSpeed)
            playerState = PlayerState.idle;

        else if (playerState == PlayerState.kick && sinceKickTime > punchSpeed)
            playerState = PlayerState.idle;

        animator.SetInteger("PlayerState", (int)playerState);
    }

    public void Move(float xAxis)
    {

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
        float resistance = 0;
        if (isBlocking)
        {
            audioSource.PlayOneShot(tookBlockDamage);
            resistance = blockResistance;
        }
        else
        {
            audioSource.PlayOneShot(tookPureDamage);
        }

       
        health -= damage * (1 - resistance);
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
            fistOffset = new Vector3(0.208f * transform.lossyScale.x, 0.015f * transform.lossyScale.y, 0);
        }
        else
        {
            fistOffset = new Vector3(-0.208f * transform.lossyScale.x, 0.015f * transform.lossyScale.y, 0);  
        }
        colliders = Physics2D.OverlapBoxAll(transform.position + fistOffset, new Vector2(0.1f * transform.lossyScale.x, 0.1f * transform.lossyScale.y), 0, characterLayer);

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

    public void PreKick()
    {
        if (sinceKickTime < kickSpeed)
            return;

        playerState = PlayerState.kick;
        sinceKickTime = 0;
    }

    public void Kick()
    {
        Collider2D[] colliders = null;
        Vector3 kickOffset;

        if (spriteRenderer.flipX == true)
        {
            kickOffset = new Vector3(0.181f * transform.lossyScale.x, -0.1992728f * transform.lossyScale.y, 0);
        }
        else
        {
            kickOffset = new Vector3(-0.181f * transform.lossyScale.x, -0.1992728f * transform.lossyScale.y, 0);
        }
        colliders = Physics2D.OverlapBoxAll(transform.position + kickOffset, new Vector2(0.15f * transform.lossyScale.x, 0.15f * transform.lossyScale.y), 0, characterLayer);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            var otherPlayer = collider.gameObject.GetComponent<PlayerInterface>();
            if (otherPlayer != null)
            {
                otherPlayer.TakeDmg(kickDmg);
            }
        }
    }

    public void PreBlock()
    {
        playerState = PlayerState.block;
    }

    public void ToggleIsBlocking()
    {
        isBlocking = !isBlocking;
    }

    public bool IsLocalPlayer()
    {
        return isLocalPlayer;
    }
}
