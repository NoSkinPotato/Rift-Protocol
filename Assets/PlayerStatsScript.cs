using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsScript : MonoBehaviour
{
    public inventoryScanner scanner;
    public PlayerMovementScript playerMovementScript;
    public float playerMaxLevelHealth = 2000;
    public float playerCurrentMaxHealth = 500;
    public float currentPlayerHealth;
    public float currentPlayerStamina;
    public Image healthBar;
    public Image staminaBar;
    public PlayerGunScript pistolScript;
    public ItemData[] mainItems;
    public ItemData[] subItems;
    public float staminaGenerationRate = 2f;
    public int inventoryLevel = 1;
    public TextMeshProUGUI onMagText;
    public TextMeshProUGUI onInventoryText;
    public GameObject[] inventoryLevels;
    public bool scanComplete = false;
    public int currentIndexConsumable = 0;
    public List<ItemData> consumableItems;
    public inventoryManagement inventoryManagement;
    public Image consumableSprite;
    public TextMeshProUGUI quantityTP;
    bool startScanConsumable = false;
    public bool enableConsumable = false;
    public bool dead = false;
    public GameObject deathScreen;
    
    private void Start()
    {
        currentPlayerHealth = playerCurrentMaxHealth;
        currentPlayerStamina = playerCurrentMaxHealth;


    }

    private void Update()
    {
        float health = currentPlayerHealth/ playerMaxLevelHealth;
        healthBar.fillAmount = health;
        float stamina = currentPlayerStamina/ playerMaxLevelHealth;
        staminaBar.fillAmount = stamina;
        


        if(currentPlayerStamina < playerCurrentMaxHealth && playerMovementScript.PlayerSprint == false)
        {
            currentPlayerStamina += Time.deltaTime * staminaGenerationRate;
            if(currentPlayerStamina > playerCurrentMaxHealth)
            {
                currentPlayerStamina = playerCurrentMaxHealth;
            }
        }

        if(startScanConsumable)
        {
            setConsumableArray();
            setInventoryLevel();
            startScanConsumable = false;
        }
        

        if (enableConsumable)
        {
            if (consumableItems.Count > 0)
            {
                consumableSprite.sprite = consumableItems[currentIndexConsumable].image;
                quantityTP.text = consumableItems[currentIndexConsumable].amount.ToString();

            }
            else
            {
                consumableSprite.transform.parent.gameObject.SetActive(false);
            }

            if(consumableItems.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (currentIndexConsumable == 0)
                    {
                        currentIndexConsumable = consumableItems.Count - 1;

                    }
                    else
                    {
                        currentIndexConsumable--;
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (currentIndexConsumable == consumableItems.Count - 1)
                    {
                        currentIndexConsumable = 0;

                    }
                    else
                    {
                        currentIndexConsumable++;
                    }
                }

                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (consumableItems[currentIndexConsumable].amount > 0)
                    {
                        inventoryManagement.doActionOnItem(consumableItems[currentIndexConsumable].name);
                        consumableItems[currentIndexConsumable].amount--;


                    }

                    if (consumableItems[currentIndexConsumable].amount <= 0)
                    {
                        setConsumableArray();
                        if (consumableItems.Count > 0)
                        {
                            consumableSprite.transform.parent.gameObject.SetActive(true);
                        }
                        else
                        {
                            consumableSprite.transform.parent.gameObject.SetActive(false);
                        }
                        currentIndexConsumable = 0;
                    }

                }
            }

            
        }
        
        if(currentPlayerHealth <= 0 && dead == false)
        {
            playerMovementScript.audioManager.Play("PlayerDead");
            deathScreen.SetActive(true);
            Time.timeScale = 0;
            dead = true;
        }

        GunAmmoDisplay();

        
    }

    public void damagePlayer(float damage)
    {
        int x = UnityEngine.Random.Range(1, 4);
        string b = "Hurt" + x.ToString();
        playerMovementScript.audioManager.Play(b);
        currentPlayerHealth -= damage;
    }

    public void setConsumableArray()
    {
        currentIndexConsumable = 0;
        consumableItems.Clear();
        

        for(int i = 0; i < mainItems.Length; i++)
        {
            if (mainItems[i] != null && mainItems[i].consumable && mainItems[i].amount > 0)
            {
                consumableItems.Add(mainItems[i]);
            }
            
        }

        

        enableConsumable = true;
    }

    public void setInventoryLevel()
    {
        for (int i = 0; i < inventoryLevels.Length; i++)
        {
            if (i < inventoryLevel)
            {
                inventoryLevels[i].SetActive(true);
                setSlotUnlocked(inventoryLevels[i].GetComponentsInChildren<SlotScript>(), true);

            }
            else
            {
                inventoryLevels[i].SetActive(false);
                setSlotUnlocked(inventoryLevels[i].GetComponentsInChildren<SlotScript>(), false);
            }
        }
    }

    private void setSlotUnlocked(SlotScript[] array, bool x)
    {

        if (x)
        {
            foreach(SlotScript slot in array)
            {
                slot.unlocked = true;
            }

        }
        else
        {
            foreach (SlotScript slot in array)
            {
                slot.unlocked = false;
            }
        }

    }

    private void GunAmmoDisplay()
    {
        onMagText.text = pistolScript.currentMag.ToString();
        onInventoryText.text = "/ " + findItemData("Handgun Ammo", true).amount.ToString();

    }

    public int findItemQuantity(string name)
    {
        ItemData item = Array.Find(mainItems, item => item.name == name);

        return item.amount;
    }

    public ItemData findItemData(string name, bool isMain)
    {
        ItemData itemData = null;
        if (isMain)
        {
            itemData = Array.Find(mainItems, item => item.name == name);
        }
        else
        {
            itemData = Array.Find(subItems, item => item.name == name);
        }
        
        if(itemData == null)
        {
            Debug.Log("Item Not Found");
            return null;
        }

        return itemData;
    }

    public IEnumerator scanInventory()
    {
        foreach (ItemData itemData in mainItems)
        {
            itemData.amount = 0;
        }

        foreach (GameObject item in scanner.itemsInInventory)
        {
            ItemTag tag = item.GetComponent<ItemTag>();
            string x = tag.ItemName.ToString();
            ItemData i = findItemData(x, true);
            if (i != null)
            {
                i.amount += tag.currentQuantity;
            }

        }

        yield return null;

        startScanConsumable = true;

    }


    public IEnumerator scanSubInventory()
    {
        foreach (ItemData itemData in subItems)
        {
            itemData.amount = 0;
        }

        yield return null;

        foreach (GameObject item in scanner.itemsInInventory)
        {
            ItemTag tag = item.GetComponent<ItemTag>();
            string x = tag.ItemName;
            ItemData i = findItemData(x, false);
            if (i != null)
            {
                
                i.amount += tag.currentQuantity;
            }

        }

        
    }

    public IEnumerator scanDataToInventory()
    {
        
        StartCoroutine(scanSubInventory());
        yield return null;

        for (int i = 0; i < mainItems.Length; i++)
        {
            int excess = subItems[i].amount - mainItems[i].amount;
            if (excess > 0)
            {
                removeProcess(mainItems[i].name, excess);

            }else if(excess < 0)
            {  
                inventoryManagement.addItemToInventory(mainItems[i].name, excess*-1);
            }
        }

    }

    private void removeProcess(string name, int excess)
    {
        while(excess > 0)
        {
            ItemTag tag = findGameObject(name);
            if(tag != null)
            {
                tag.currentQuantity -= 1;
                excess -= 1;
                if (tag.currentQuantity <= 0)
                {
                    Destroy(tag.transform.parent.gameObject);
                }
            }
            else
            {
                Debug.Log("Not Found");
                return;
            }
        }
    }

    private ItemTag findGameObject(string name)
    {
        foreach(GameObject item in scanner.itemsInInventory)
        {
            ItemTag tag = item.GetComponent<ItemTag>();
            if(tag != null && tag.ItemName == name)
            {
                return tag;
            }
            
        }

        return null;

    }
    
}
