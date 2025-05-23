using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : Character
{
    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject attackArea;
    private IState currentState;

    private bool isRight = true;
    private Character target;
    public Character Target => target;

    // Update is called once per frame
    private void Update()
    {
        Debug.Log(currentState);
        if (currentState != null && !IsDead) {
            currentState.OnExecute(this);
        }
    }

    public override void OnInit()
    {
        base.OnInit();

        ChangeState(new IdleState());
        DeactiveAttack();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        ChangeState(null);
        base.OnDeath();
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null) {
            currentState.OnExit(this);
        }

        currentState = newState;
        if (currentState != null) {
            currentState.OnEnter(this);
        }
    }

    public void Moving()
    {
        ChangeAnim("run");
        rb.linearVelocity = transform.right * moveSpeed;
    }

    public void StopMoving() 
    {
        ChangeAnim("idle");
        rb.linearVelocity = Vector2.zero;
    }

    public void Attack()
    {
        ChangeAnim("attack");
        ActiveAttack();
        Invoke(nameof(DeactiveAttack), 0.5f);
    }

    public bool IsTargetInRange()
    {
        return target != null 
        && Vector2.Distance(target.transform.position, transform.position) <= attackRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyWall")) {
            ChangeDirection(!isRight);
        }
    }

    public void ChangeDirection(bool isRight)
    {
        this.isRight = isRight;
        transform.rotation = isRight ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180);
    }

    internal void SetTarget(Character character)
    {
        this.target = character;
        if (IsTargetInRange()) {
            ChangeState(new AttackState());
        } else if (Target != null) {
            ChangeState(new PatrolState());
        } else {
            ChangeState(new IdleState());
        }
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeactiveAttack()
    {
        attackArea.SetActive(false);
    }
}
