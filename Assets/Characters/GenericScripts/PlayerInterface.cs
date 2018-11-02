using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerInterface : NetworkBehaviour {

    [SyncVar, SerializeField]
    private float health;

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

    [SerializeField]
    private Collider2D grounded;
    [SerializeField]
    private LayerMask groundLayer;

    private void Start()
    {
        rigidBo = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Move(float xAxis)
    {
        sprite.flipX = (xAxis > 0);

        if (grounded.IsTouchingLayers(groundLayer))
        {
            if ((rigidBo.velocity.x > 0 && xAxis < 0) || (rigidBo.velocity.x < 0 && xAxis > 0))
                StopMove();

            if (rigidBo.velocity.x < maxMoveSpeed && rigidBo.velocity.x > -maxMoveSpeed)
                ApplyForce(new Vector2(xAxis, 0) * moveSpeed);
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
            sinceJump = 0f;
            ApplyForce(Vector2.up * jumpForce);
        }
        sinceJump += Time.deltaTime;     
    }

    public void ApplyForce(Vector2 force)
    {
        rigidBo.AddForce(force);
    }

    public void Punch()
    {
        if(sprite.flipX == true)
        {
            // raycast right
        } else
        {
            // raycast left
        }

        // use hit and calc dmg
    }
}
