using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OnInit();
    }
    private float hp;
    private string currentAnimName;
    [SerializeField] private Animator anim;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected CombatText CombatTextPrefab;
    public bool IsDead => hp <= 0;
    public virtual void OnInit()
    {
        hp = 100;
        healthBar.OnInit(100, transform);
    }

    public virtual void OnDespawn()
    {

    }

    public void OnHit(float damage)
    {
        if (!IsDead) {
            hp -= damage;
            if (IsDead) {
                hp = 0;
                OnDeath();
            }
            healthBar.SetNewHp(hp);
            Instantiate(CombatTextPrefab, transform.position + Vector3.up, Quaternion.identity).OnInit(damage);
        }
    }

    protected void ChangeAnim(string animName)
    {
        if(currentAnimName != animName) {
            anim.ResetTrigger(animName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }

    protected virtual void OnDeath()
    {
        ChangeAnim("die");

        Invoke(nameof(OnDespawn), 2f);
    }
}
