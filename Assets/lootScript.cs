using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lootScript : MonoBehaviour
{
    public GameObject item;
    public GameObject instantiatedItem;
    public Transform inventory;
    public int itemQuantity;
    public bool isRare = false;

    

    private void OnEnable()
    {
        instantiatedItem = Instantiate(item, inventory);
        instantiatedItem.SetActive(false);
    }

}
