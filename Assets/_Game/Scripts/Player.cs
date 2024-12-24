using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 350;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    private bool isGrounded = true;
    private bool isFalling;
    private bool isJumping = false;
    private bool isAttack = false;
    private bool isDead = false;
    private float horizontal;
    private float vertical;
    private int coin = 0;
    private Vector3 savePoint;

    private void Awake()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
    }

    public override void OnInit()
    {
        base.OnInit();
        // isDead = false;
        isAttack = false;
        transform.position = savePoint;
        ChangeAnim("idle");
        DeactiveAttack();

        SavePoint();
        UIManager.instance.SetCoin(coin);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        isGrounded = CheckGrounded();
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (isAttack) {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isGrounded) {
            //jump
            if (isJumping) return;
            if (Input.GetKeyDown(KeyCode.Space)) {
                Jump();
            }

            if (Mathf.Abs(horizontal) > 0.1f) {
                ChangeAnim("run");
            }

            //attack
            if (Input.GetKeyDown(KeyCode.C)) {
                Attack();
                return;
            }
            //throw
            if (Input.GetKeyDown(KeyCode.V)) {
                Throw();
                return;
            }
        }

        //fall
        if (!isGrounded && rb.velocity.y < 0) {
            ChangeAnim("fall");
            isJumping = false;
        }

        //move
        if (Mathf.Abs(horizontal) > 0.1f) {
            // ChangeAnim("run");
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            Debug.Log(horizontal);
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : horizontal*180, 0));
            // transform.localScale = new Vector3(horizontal, 1, 1);
        } else if (isGrounded) {
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }
    }

    private bool CheckGrounded() 
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        // if (hit.collider != null) {
        //     return true;
        // } else {
        //     return false;
        // }

        return hit.collider != null;
    }

    public void Attack()
    {
        isAttack = true;
        rb.velocity = Vector2.zero;
        ChangeAnim("attack");
        
        Invoke(nameof(ResetAttack), 0.5f);
        ActiveAttack();
        Invoke(nameof(DeactiveAttack), 0.5f);
        // rb.AddForce(jumpForce * Vector2.up);
        return;
    }

    private void ResetAttack()
    {
        ChangeAnim("idle");
        isAttack = false;
    }

    public void Throw()
    {
        rb.velocity = Vector2.zero;
        ChangeAnim("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
        // rb.AddForce(jumpForce * Vector2.up);

        Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);
        return;
    }

    public void Jump()
    {
        isJumping = true;
        ChangeAnim("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin") {
            coin++;
            PlayerPrefs.SetInt("coin", coin);
            UIManager.instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }

        if(collision.tag == "DeadZone") {
            // isDead = true;
            ChangeAnim("die");
            Invoke(nameof(OnInit), 1f);
        }
    }

    internal void SavePoint()
    {
        savePoint = transform.position;
        // throw new NotImplementedException();
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeactiveAttack()
    {
        attackArea.SetActive(false);
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }
}
