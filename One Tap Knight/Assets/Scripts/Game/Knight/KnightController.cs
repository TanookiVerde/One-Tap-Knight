﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class KnightController : MonoBehaviour
{
    public GameObject deathSpawn;
    public KnightSound sound;

    [Header("Preferences")]
    public float movingVelocity;
    public float jumpIntensity;
    public int jumpLimit;
    public float jumpMaxTime = 1f;
    public float jumpMin;
    public float jumpIncreaseTax;
    public float groundPoundStopTime;
    public float lookDownMinTime;

    [Header("Ground Check")]
    public Transform ground;
    public float distanceToGround;
    public Vector2 groundBoxCastSize;

    private Transform followTransform;
    private float followXOffset;
    private bool following = false;

    private Animator animator;
    public new Rigidbody2D rigidbody2D;
    private Collider2D col2D;
    private float currentTax = 1;
    [HideInInspector]
    public int jumpsRemaining;

    public bool isPounding;
    public bool finishedLevel;
    private Vector3 velOnPause;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(ground.position, groundBoxCastSize);
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        jumpsRemaining = jumpLimit;
        rigidbody2D = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
        sound = GetComponent<KnightSound>();
        Time.timeScale = 1f;
    }
    public void Stop(bool value)
    {
        if(value)
        {
            velOnPause = rigidbody2D.velocity;
            print(velOnPause);
            rigidbody2D.Sleep();
            col2D.enabled = false;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.enabled = false;
        }
        else
        {
            print(velOnPause);
            rigidbody2D.WakeUp();
            col2D.enabled = true;
            rigidbody2D.velocity = velOnPause;
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
            animator.enabled = true;
        }
    }
    public void MovementLoop()
    {
        Move();
        animator.SetFloat("XVelocity", rigidbody2D.velocity.x);
        bool grounded = IsGrounded();
        if (grounded || jumpsRemaining > 0)
        {
            Jump();
            jumpsRemaining--;
        }
        if (!grounded && Input.GetButtonDown("Pound") && !isPounding)
        {
            StartCoroutine(PoundCoroutine());
        }
    }
    private void Move()
    {
        if (!isPounding && !following)
        {
            rigidbody2D.velocity = new Vector2(movingVelocity * currentTax, rigidbody2D.velocity.y);
        }
        else if (following)
        {
            transform.position = new Vector3(followTransform.position.x - followXOffset, transform.position.y, transform.position.z);
        }
    }
    public void FollowX(Transform t)
    {
        followTransform = t;
        followXOffset = followTransform.position.x - transform.position.x;
        rigidbody2D.velocity = Vector2.zero;
        following = true;
    }
    public void StopFollowing()
    {
        followTransform = null;
        followXOffset = 0;
        following = false;
    }
    private void Jump()
	{
		if (Input.GetButtonDown("Jump")) {
			GetComponent<Animator>().Play("jump");
			StartCoroutine (SpecialJump ());
            sound.PlaySound(SoundType.JUMP);
        }
	}
	private IEnumerator SpecialJump()
	{
		float time = jumpMaxTime;
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
        rigidbody2D.AddForce(Vector2.up * jumpMin);
		while (Input.GetButton("Jump") && time > 0) {
            //yield return new WaitForEndOfFrame ();
            yield return new WaitForFixedUpdate();
            rigidbody2D.AddForce(Vector2.up * jumpIncreaseTax);
			time -= Time.deltaTime;
		}
	} 
    private void Pound()
    {
        StartCoroutine(PoundCoroutine());
    }
    private IEnumerator PoundCoroutine()
    {
        isPounding = true;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(Vector2.up * jumpIntensity * -1);
        while (!IsGrounded())
            yield return new WaitForEndOfFrame();
        sound.PlaySound(SoundType.FALL);
        yield return new WaitForSeconds(groundPoundStopTime);
        isPounding = false;
    }
    public void Die(bool isSpike = false)
    {
        if (isSpike && rigidbody2D.velocity.y > 2f)
            return;
        Instantiate(deathSpawn, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    public void ModifyVelocity(float tax)
    {
        currentTax = tax;
    }
    private bool IsGrounded()
    {
        if (rigidbody2D.velocity.y > 0) return false;
        RaycastHit2D boxCast = Physics2D.BoxCast(ground.position,
                groundBoxCastSize, 0, Vector2.up, 0, LayerMask.GetMask("Ground", "Stop"));

        if (boxCast.collider != null)
        {
            jumpsRemaining = jumpLimit;
            return true;
        }
        return false;
    }
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("End"))
        {
            finishedLevel = true;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}

