using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponConfig config;

    void Start()
    {
        Attack();
    }

    public void Attack()
    {
        Debug.Log($"{config.weaponName} で {config.damage} のダメージ");
    }

}
