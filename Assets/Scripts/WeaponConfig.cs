using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "SO Demo/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    public string weaponName;
    public float maxAmmo;
    public float damage;
    public bool areaDamage;
}
