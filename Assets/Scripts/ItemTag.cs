using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemTag : MonoBehaviour
{

    public string ItemName;
    public int maxQuantity;
    public bool isStackable;
    public int currentQuantity;
    public bool isFull = false;
    public GameObject quantityTag;
    public TextMeshProUGUI Qtag;
    public bool use = false;
    public bool discard = false;
    private inventoryManagement inventoryManagement;
    private craftingSystem craftingSystem;
    public bool horizontal = true;
    public Transform horiPos;
    public Transform vertiPos;

    private void Start()
    {
        inventoryManagement = FindObjectOfType<inventoryManagement>();  
        craftingSystem = FindObjectOfType<craftingSystem>();
        

    }


    private void Update()
    {
        if(currentQuantity >= maxQuantity)
        {
            isFull = true;
        }
        else
        {
            isFull=false;
        }

        if(isStackable == false)
        {
            quantityTag.SetActive(false);
        }
        else
        {
            quantityTag.SetActive (true);
            Qtag.SetText(currentQuantity.ToString());
        }


    }

    public void rotateItem()
    {
        transform.Rotate(Vector3.forward, 90f);

        
        if(horiPos != null)
        {
            
            horizontal = !horizontal;
            if (horizontal)
            {
                quantityTag.transform.position = horiPos.position;
            }
            else
            {
                quantityTag.transform.position = vertiPos.position;
            }
        }
        
    }

    public void useItem()
    {
        if(use)
        {
            inventoryManagement.doActionOnItem(ItemName);
            if (currentQuantity == 1)
            {
                Destroy(transform.parent.gameObject);
            }
            else{
                currentQuantity -= 1;
            }

            
        }
    }

    public void discardItem()
    {
        if (discard)
        {
            Destroy(transform.parent.gameObject);
        }

    }




}
