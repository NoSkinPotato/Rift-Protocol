using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class craftingSystem : MonoBehaviour
{
    public List<GameObject> craftableItems;
    public PlayerStatsScript playerStats;
    public pauseManagerScript pauseManager;
    public bool craftingMenuOpen = false;
    public bool checkCraftables = true;
    public TextMeshProUGUI[] availableItemsText;
    public GameObject CraftingMenu;
    public string[] availableItemsNames;
    public Transform craftingContents;
    public float distanceBetweenCraftable = 123f;
    public ItemDatabase ItemDatabase;
    public Transform itemsTransform;
    public MoveableObjectScript craftedItem;
    public bool requestUndo = false;
    public CraftableScript savedCraftableScript;
    public GameObject toCraftingButton;
    public inventoryManagement inventoryManagement;
    public bool startOfCrafting = true;
    public List<GameObject> instantCraftables;


    private void Update()
    {
        if (craftingMenuOpen && checkCraftables)
        {
            StartCoroutine(arrangeCraftables(true));
            checkCraftables = false;

        }else if(craftingMenuOpen == false)
        {
            CraftingMenu.SetActive(false);
        }
        
    }


    public void calculateCost(CraftableScript craftableScript)
    {
        for(int i = 0; i < craftableScript.materialsRequired.Length; i++)
        {
            ItemData item = playerStats.findItemData(craftableScript.materialNames[i], true);
            if (item != null)
            {
                item.amount -= craftableScript.materialsRequired[i];
            }
            else
            {
                Debug.Log("Item Not Found in CalculateCost");
            }
        }
        savedCraftableScript = craftableScript;

    }

    public IEnumerator craftAnItem(CraftableScript craftable, MoveableObjectScript crafted)
    {
        calculateCost(craftable);
        yield return null;
        StartCoroutine(scanBeforePuttingItem(crafted));
    }

    private IEnumerator scanBeforePuttingItem(MoveableObjectScript craftedScript)
    {
        StartCoroutine(playerStats.scanDataToInventory());
        
        yield return null;

        bool x = inventoryManagement.lootItem(craftedScript);
        
        yield return null;

        if (x == false)
        {
            ItemTag tag = craftedScript.ItemTag;
            inventoryManagement.summonPickUpNotif(tag.ItemName, false);
            Destroy(craftedScript.transform.parent.gameObject);
            StartCoroutine(undoCost());
            
        }
        else
        {
            StartCoroutine(arrangeCraftables(false));
            StartCoroutine(playerStats.scanDataToInventory());
            ItemTag tag = craftedScript.ItemTag;
            inventoryManagement.summonPickUpNotif(tag.ItemName, true);

        }
    }

    public IEnumerator undoCost()
    {

        for (int i = 0; i < savedCraftableScript.materialsRequired.Length; i++)
        {
            ItemData item = playerStats.findItemData(savedCraftableScript.materialNames[i], true);
            if (item != null)
            {
                item.amount += savedCraftableScript.materialsRequired[i];
                
            }
            else
            {
                Debug.Log("Item Not Found in UndoCost");
            }

            
        }

        yield return null;

        StartCoroutine(playerStats.scanDataToInventory());

        yield return null;

        StartCoroutine(playerStats.scanInventory());

        yield return null;

        StartCoroutine(arrangeCraftables(true));

    }

    private void sortItems()
    {
        instantCraftables.Sort((x, y) =>
        {
            CraftableScript xScript = x.GetComponent<CraftableScript>();
            CraftableScript yScript = y.GetComponent<CraftableScript>();
            if (xScript.craftable == yScript.craftable)
                return instantCraftables.IndexOf(x).CompareTo(instantCraftables.IndexOf(y));

            return yScript.craftable.CompareTo(xScript.craftable);
        });
    }

    private IEnumerator arrangeCraftables(bool x)
    {
        if(x == true)
        {
            StartCoroutine(playerStats.scanInventory());
        }

        yield return null;

        if (startOfCrafting)
        {
            for(int i = 0; i < craftableItems.Count; i++)
            {
                GameObject obj = Instantiate(craftableItems[i].gameObject, craftingContents);
                instantCraftables.Add(obj);
                yield return null;
            }

            startOfCrafting = false;
        }

        yield return null;


        for(int i = 0; i < availableItemsText.Length; i++)
        {
            availableItemsText[i].text = playerStats.findItemQuantity(availableItemsNames[i]).ToString();
        }

        foreach (GameObject craft in instantCraftables)
        {
            CraftableScript script = craft.GetComponent<CraftableScript>();
            bool check = true;
            for (int i = 0; i < script.materialNames.Length; i++)
            {
                ItemData item = Array.Find(playerStats.mainItems, item => item.name == script.materialNames[i]);
                if (item != null)
                {
                    if (item.amount >= script.materialsRequired[i])
                    {
                        script.materialsText[i].color = Color.white;
                    }
                    else
                    {
                        check = false;
                        script.materialsText[i].color = Color.red;
                    }
                }
                else
                {
                    Debug.Log("Not Found");
                }
            }

            if (check)
            {
                script.craftable = true;
            }
            else
            {
                script.craftable = false;
            }

        }

        sortItems();

        yield return null;
        

        foreach(GameObject craft in instantCraftables)
        {
            craft.transform.SetAsLastSibling();
        }

        yield return null;

        CraftingMenu.SetActive(true);



    }

    
    public GameObject findItemAfterCrafting(string name)
    {
        Item item = Array.Find(ItemDatabase.items, x => x.name == name);
        if(item != null)
        {
            return item.itemObject;
        }
        else
        {
            Debug.Log("Bitch there ain't none");
        }

        return null;
    }

    public Item findItemInDatabase(string name)
    {
        Item item = Array.Find(ItemDatabase.items, x => x.name == name);
        if (item != null)
        {
            return item;
        }
        else
        {
            Debug.Log("Bitch there ain't none");
        }

        return null;
    }

}
