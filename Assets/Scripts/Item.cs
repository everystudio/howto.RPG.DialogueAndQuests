using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "アイテム名", menuName = "インベントリ/アイテム")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private float itemWeight;

    public string GetItemName()
    {
        return itemName;
    }


}
