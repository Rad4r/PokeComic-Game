using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entitiy : MonoBehaviour
{
    [SerializeField] private int health;

    public int GetDamaged(int attackPower)
    {
        health -= attackPower;
        return health;
    }

    // OnTriggerEnter2D(Collider2D col)
    // OnCollisionEnter2D(Collision2D)
    void OnTriggerEnter2D(Collider2D col)
    {
        // if (col.CompareTag("EnemyAttackTag") || col.CompareTag("MyAttackTag"))
        // {
        //     GetDamaged(2);
        //     Destroy(col);
        // }
        
        if (col.CompareTag("MyAttackTag") && gameObject.CompareTag("EnemyPokemonTag"))
        {
            GetDamaged(2);
        }
        else if (col.CompareTag("EnemyAttackTag") && gameObject.CompareTag("MyPokemonTag"))
        {
            GetDamaged(2);
        }
    }
}
