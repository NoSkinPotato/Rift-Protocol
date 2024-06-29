using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIScript : MonoBehaviour
{

    public GameObject InteractUI;
    public lootScript lootableObjectScript;
    public pauseManagerScript pauseScript;
    public float UIDistanceFromObject = 5f;
    public bool canLoot = false;
    public bool onStorage = false;
    public bool canUseStorage = false;
    public GameObject storage;
    public GameObject craftingTable;
    public bool onCraftingTable = false;
    public bool canUseCraftingTable = false;
    public bool block = false;
    public inventoryManagement inventoryManagement;
    public bool onDoor = false;
    private Vector2 doorPos;
    public doorScript doorScript;
    


    private void Update()
    {

        if(onStorage == false && onCraftingTable == false)
        {
            canUseCraftingTable = false;
            canUseStorage = false;

            if(onDoor == true)
            {
                InteractUI.SetActive(true);
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(doorPos);
                InteractUI.transform.position = screenPoint + Vector3.right * UIDistanceFromObject;
            }
            else
            {
                if (lootableObjectScript != null && pauseScript.InventoryOpen == false)
                {
                    InteractUI.SetActive(true);
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(lootableObjectScript.transform.position);
                    InteractUI.transform.position = screenPoint + Vector3.up * UIDistanceFromObject;
                    canLoot = true;
                }
                else if (lootableObjectScript == null || pauseScript.InventoryOpen == true)
                {
                    InteractUI.SetActive(false);
                    canLoot = false;
                }
            }
            

        }else if(onStorage && onCraftingTable == false)
        {
            canUseCraftingTable = false;
            if (pauseScript.InventoryOpen == false)
            {
                InteractUI.SetActive(true);
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(storage.transform.position);
                InteractUI.transform.position = screenPoint + Vector3.up * UIDistanceFromObject;
                canUseStorage = true;
            }
            else
            {
                InteractUI.SetActive(false);
                canUseStorage = false;
            }

        }else if(onStorage == false && onCraftingTable == true)
        {

            if (block == false)
            {
                canUseStorage = false;
                if (pauseScript.InventoryOpen == false)
                {
                    InteractUI.SetActive(true);
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(craftingTable.transform.position);
                    InteractUI.transform.position = screenPoint + Vector3.up * UIDistanceFromObject;
                    canUseCraftingTable = true;
                }
                else
                {
                    InteractUI.SetActive(false);
                    canUseCraftingTable = false;
                }
            }
            else
            {
                InteractUI.SetActive(false);
            }
            
        }


    }

 

    public void removeLoot()
    {
        if(lootableObjectScript != null)
        {
            Destroy(lootableObjectScript.gameObject);

        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Loot"))
        {
            lootableObjectScript = collision.gameObject.GetComponent<lootScript>();
            onCraftingTable = false;
            onDoor = false;

        }
        else if (collision.gameObject.CompareTag("Door"))
        {
            lootableObjectScript = null;
            onCraftingTable = false;
            onDoor = true;
            doorPos = collision.transform.position;
            doorScript = collision.transform.parent.parent.GetComponent<doorScript>();
        }
        else if (collision.gameObject.CompareTag("Crafting"))
        {
            onDoor = false;
            onCraftingTable = true;
            lootableObjectScript = null;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Loot"))
        {
            lootableObjectScript = null;
        }

        if (collision.gameObject.CompareTag("Storage"))
        {
            onStorage = false;
        }

        if (collision.gameObject.CompareTag("Crafting"))
        {
            onCraftingTable = false;
        }

        if (collision.gameObject.CompareTag("Door")) {

            onDoor = false;

        }

    }




}
