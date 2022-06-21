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
}
