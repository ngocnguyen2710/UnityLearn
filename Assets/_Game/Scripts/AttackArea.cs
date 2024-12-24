using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coliision)
    {
        if (coliision.tag == "Player" || coliision.tag == "Enemy") {
            coliision.GetComponent<Character>().OnHit(30f);
        }
    }
}
