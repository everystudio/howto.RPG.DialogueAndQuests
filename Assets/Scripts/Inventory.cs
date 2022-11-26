using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Item[] contents;
    void Start()
    {
        foreach (var item in contents)
        {
            Debug.Log(item.GetItemName());
        }
    }

    void Update()
    {

    }
}
