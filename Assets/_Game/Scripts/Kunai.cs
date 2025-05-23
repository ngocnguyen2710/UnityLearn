using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kunai : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject hitVFX;
    // Start is called before the first frame update
    void Start()
    {
        OnInit();
        rb.linearVelocity = transform.right * 5f;
    }

    public void OnInit() 
    {
        rb.linearVelocity = transform.right * 5f;
        Invoke(nameof(OnDespawn), 4f);
    }
    
    public void OnDespawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Enemy")) {
            collision.GetComponent<Character>().OnHit(30f);
            Instantiate(hitVFX, transform.position, transform.rotation);
            OnDespawn();
        }
    }
}
