using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class inventoryManagement : MonoBehaviour
{
    public UnityEvent[] itemOnUse;
    public string[] itemName;
    public yOrderSlots[] allSlots;
    public pauseManagerScript pauseManager;
    public GameObject hintButtons;
    public GameObject useHint;
    public GameObject discardHint;
    public craftingSystem crafting;
    public float distanceHintButton;
    public float buttonDelay = 2f;
    public PlayerUIScript playerUIScript;
    public List<SlotScript> slots;
    public List<GameObject> pickUps;
    public Transform Notification;
    public GameObject pickUpPrefab;
    public PlayerStatsScript playerStats;
    public bool justRefunded = false;
    public inventoryScanner scanner;
    public int quantityLooted = 0;
    float timer = 0;
    public Transform itemTransform;
    public GameObject lootPrefab;
    public List<GameObject> rareItems = new List<GameObject>();
    public List<GameObject> commonItems = new List<GameObject>();


    public void doActionOnItem(string name)
    {
        for (int i = 0; i < itemName.Length; i++)
        {
            if (itemName[i] == name)
            {
                itemOnUse[i].Invoke();
            }
        }
    }

    public void dropItem(Vector2 pos)
    {
        

        int i = Random.Range(0, 101);

        if(i <= 80)
        {
            //spawnLoot
            GameObject obj = Instantiate(lootPrefab);
            obj.SetActive(false);
            obj.transform.position = pos;
            

            lootScript ls = obj.GetComponent<lootScript>();
            ls.inventory = itemTransform;

            if(i <= 30)
            {
                //spawnRare
                i = Random.Range(0, rareItems.Count);

                ls.item = rareItems[i];

            }
            else
            {
                //spawnCommon

                i = Random.Range(0, commonItems.Count);

                ls.item = commonItems[i];

            }


            obj.SetActive(true);
       }





    }

    private void Update()
    {
        if(pauseManager.InventoryOpen)
        {
            showButtonHints();
        }
    }

    public void showButtonHints()
    {
        if (pauseManager.pickedItem != null)
        {
            hintButtons.SetActive(true);
            hintButtons.transform.position = pauseManager.pickedItem.transform.position + Vector3.right * distanceHintButton;
            ItemTag tag = pauseManager.pickedItem.GetComponent<ItemTag>();
            if (tag != null)
            {
                if (tag.use)
                {
                    useHint.SetActive(true);
                }
                else
                {
                    useHint.SetActive(false);
                }

                if (tag.discard)
                {
                    discardHint.SetActive(true);
                }
                else
                {
                    discardHint.SetActive(false);
                }

            }

            if (Input.GetMouseButtonDown(1))
            {
                pauseManager.cancellation();

                pauseManager.pickedItem = null;

            }

            if (timer < buttonDelay)
            {
                timer += Time.deltaTime;

            }
            else
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    tag.rotateItem();
                    timer = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                tag.useItem();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if(crafting.craftedItem == tag.gameObject.GetComponent<MoveableObjectScript>())
                {
                    pauseManager.closeInventory();
                }
                else
                {
                    tag.discardItem();
                }
                
            }

        }
        else
        {
            hintButtons.SetActive(false);
        }
    }

   
    public bool lootItem(MoveableObjectScript objScript)
    {
        quantityLooted = 0;
        ItemTag itemTag = objScript.GetComponent<ItemTag>();
        
        int x = itemTag.currentQuantity;
        for(int i = 0; i < x; i++)
        {
            ItemTag unfinishedTag = findNonMaxItem(itemTag.ItemName);

            if (unfinishedTag != null)
            {
                quantityLooted += 1;
                unfinishedTag.currentQuantity += 1;
                itemTag.currentQuantity -= 1;
                if(itemTag.currentQuantity <= 0)
                {
                    Destroy(itemTag.transform.parent.gameObject);
                }
                ItemData data = playerStats.findItemData(unfinishedTag.ItemName, true);
                data.amount += 1;
                
            }
            else
            {
                return findSlots(objScript);
            }
        }

        if(playerUIScript.lootableObjectScript != null)
        {
            Destroy(playerUIScript.lootableObjectScript.gameObject);
            playerUIScript.removeLoot();
            
        }

        
        
        return true;

    }

    private ItemTag findNonMaxItem(string name)
    {
        foreach(GameObject obj in scanner.itemsInInventory)
        {
            ItemTag tag = obj.GetComponent<ItemTag>();
            if(tag.ItemName == name && tag.currentQuantity < tag.maxQuantity)
            {
                return tag;
            }
        }
        return null;
    }

    private bool findSlots(MoveableObjectScript objScript)
    {
        int x = objScript.orderedSpaces.Length;
        int y = objScript.orderedSpaces[0].Yspaces.Length;
        ItemTag tag = objScript.ItemTag;

        for (int i = 0; i < allSlots.Length; i++)
        {
            for (int j = 0; j < allSlots[0].Yslots.Length; j++)
            {
                if (allSlots[i].Yslots[j].unlocked)
                {
                    if (foundEmptySlot(y, x, i, j, objScript))
                    {
                        if (tag.horizontal == false)
                        {
                            tag.rotateItem();
                        }
                        quantityLooted += tag.currentQuantity;
                        placeItem(objScript);
                        
                        return true;

                    }
                    else
                    {

                        if (foundEmptySlot(x, y, i, j, objScript))
                        {
                            if (tag.horizontal == true)
                            {
                                tag.rotateItem();
                            }
                            quantityLooted += tag.currentQuantity;
                            placeItem(objScript);
                            
                            return true;
                        }
                    }


                }
            }
        }

        return false;
    }

    private void placeItem(MoveableObjectScript objScript)
    {
        objScript.isPicked = false;
        objScript.justDropped = true;
        objScript.transform.parent.gameObject.SetActive(true);
        objScript.transform.parent.position = findSlotPosition(objScript);
        ItemData data = playerStats.findItemData(objScript.ItemTag.ItemName, true);
        data.amount += quantityLooted;
        if (playerUIScript.lootableObjectScript != null)
        {
            playerUIScript.removeLoot();
        }
        
    }
    
   

    private bool foundEmptySlot(int x, int y, int i, int j, MoveableObjectScript objScript)
    {
        for (int k = 0; k < x; k++)
        {
            for (int l = 0; l < y; l++)
            {
                if (i + k >= allSlots.Length || j + l >= allSlots[0].Yslots.Length)
                {
                    slots.Clear();
                    return false;
                }

                SlotScript script = allSlots[i + k].Yslots[j + l];
                if (script.unlocked == false || script.heldItem != null)
                {
                    
                    slots.Clear();
                    return false;
                }
                else
                {
                    slots.Add(script);
                }
            }
        }
        
        return true;
    }

    private Vector2 findSlotPosition(MoveableObjectScript script)
    {
        Vector2[] points = new Vector2[slots.Count];
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].heldItem = script.transform.parent.gameObject;
            points[i] = slots[i].transform.position;
        }
        slots.Clear();

        float sumX = 0f;
        float sumY = 0f;

        foreach (Vector2 point in points)
        {
            sumX += point.x;
            sumY += point.y;
        }

        float averageX = sumX / points.Length;
        float averageY = sumY / points.Length;

        return new Vector2(averageX, averageY);
    }

    public void summonPickUpNotif(string name, bool canPickUp)
    {


        foreach (GameObject obj in pickUps)
        {

            if (obj != null && obj.activeSelf == false)
            {
                obj.SetActive(true);

                if (canPickUp == true)
                {
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = "x" + quantityLooted.ToString() + " " + name;
                }
                else
                {
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = name + " Does Not Fit In Inventory";
                }

                
                return;
            }
        }

        if(pickUps.Count < 10)
        {
            GameObject pickUp = Instantiate(pickUpPrefab, Notification);
            pickUp.transform.position = Notification.transform.position;
            pickUp.GetComponentInChildren<TextMeshProUGUI>().text = "x" + quantityLooted.ToString() + " " + name;
            pickUp.SetActive(true);
            pickUps.Add(pickUp);
        }
        

    }

    public void setToMainItems(string name, int quantity)
    {

        playerStats.findItemData(name, true).amount += quantity;

    }

    public void addItemToInventory(string name, int quantity)
    {
        while (quantity > 0)
        {

            GameObject obj = Instantiate(crafting.findItemAfterCrafting(name), crafting.itemsTransform);
            obj.SetActive(false);
            ItemTag tag = obj.transform.GetChild(0).GetComponent<ItemTag>();

            if (quantity > tag.maxQuantity) {

                tag.currentQuantity = tag.maxQuantity;
                quantity -= tag.maxQuantity;

            }
            else
            {
                tag.currentQuantity = quantity;
                quantity = 0;
            }

            lootItem(obj.transform.GetChild(0).GetComponent<MoveableObjectScript>());

        }
        

    }


}

[System.Serializable]
public class yOrderSpaces
{
    public spaceScript[] Yspaces;
}
[System.Serializable]
public class yOrderSlots
{
    public SlotScript[] Yslots;
}