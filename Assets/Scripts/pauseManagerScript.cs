using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class pauseManagerScript : MonoBehaviour
{
    public bool InventoryOpen = true;
    public MoveableObjectScript pickedItem;
    public GameObject inventoryExclusives;
    public Animator inventoryAnimator;
    public CinemachineVirtualCamera cam;
    public Transform cameraPosWhenInventory;
    public Transform playerTransform;
    public Transform lootItemPos;
    public PlayerUIScript playerUIScript;
    public float orthoSize_Inventory = 3.5f;
    public float orthoSize_Idle = 8f;
    public bool lootAttempt = false;
    public MoveableObjectScript lootItem;
    public GameObject discardSlot;
    public inventoryScanner scanner;
    public GameObject playerUI;
    public PlayerStatsScript playerStats;
    public Texture2D gunCursor;
    public Texture2D inventoryCursor;
    public GameObject storageSlots;
    public GameObject centerNavigator;
    public bool storageOpen = false;
    public bool navigate = false;
    public TextMeshProUGUI centerText;
    public Transform rightSide;
    public Transform leftSide;
    public centerNavigatorScript centerNavigatorScript;
    public Transform originalPosition;
    public craftingSystem craftingSystem;
    public PlayerAimScript playerAimScript;
    public inventoryManagement inventoryManagement;
    public GameObject consumableShortcut;
    public GameObject itemDescriptionUI;
    public GameObject pauseBox;
    public bool pause = false;
    private audioManager audioManager;
    private MoveableObjectScript unlootable;

    private void Start()
    {
        Time.timeScale = 1;
        navigate = inventoryAnimator.GetBool("NavigateToInventory");
        audioManager = FindObjectOfType<audioManager>();
    }

    public void inventoryOpen(bool x)
    {
        if (x)
        {
            Cursor.SetCursor(inventoryCursor, new Vector2(20f, 20f), CursorMode.Auto);
            cam.Follow = cameraPosWhenInventory;
            cam.m_Lens.OrthographicSize = orthoSize_Inventory;
            playerUI.transform.position = leftSide.position;
            playerAimScript.stopAim = true;
            consumableShortcut.SetActive(false);

        }
        else
        {
            Cursor.SetCursor(gunCursor, new Vector2(20f, 20f), CursorMode.Auto);
            cam.Follow = playerTransform;
            cam.m_Lens.OrthographicSize = orthoSize_Idle;
            playerUI.transform.position = rightSide.position;
            consumableShortcut.SetActive(true);
            playerAimScript.stopAim = false;
        }
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {

            if (pause)
            {
                pauseBox.SetActive(false);
                Time.timeScale = 1;
                pause = false;
            }
            else
            {
                pauseBox.SetActive(true);
                Time.timeScale = 0;
                pause = true;
            }
            

        }


        if (InventoryOpen)
        {
            inventoryAnimator.SetBool("InventoryOn", true);
            

        }
        else
        {
            inventoryAnimator.SetBool("InventoryOn", false);
            craftingSystem.toCraftingButton.SetActive(false);
            
        }

        setItemDescription();

        if (lootAttempt)
        {
            if (lootItem == null)
            {

                playerUIScript.removeLoot();
                StartCoroutine(scanBeforeClosing());
                lootAttempt = false;
            }
            else if (lootItem.gameObject.activeSelf == true)
            {
                MoveableObjectScript moveableObjectScript = lootItem.GetComponent<MoveableObjectScript>();
                if (moveableObjectScript.suitableToDrop == true && moveableObjectScript.isPicked == false)
                {
                    playerUIScript.removeLoot();
                    StartCoroutine(scanBeforeClosing());
                    
                    lootItem = null;
                    lootAttempt = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!playerUIScript.canUseCraftingTable && craftingSystem.CraftingMenu.activeSelf)
            {
                StartCoroutine(scanBeforeClosing());
                craftingSystem.craftingMenuOpen = false;
                craftingSystem.checkCraftables = true;
                playerUIScript.block = false;

            }else if (storageSlots.activeSelf)
            {
                StartCoroutine(scanBeforeClosing());
            }
            else
            {
                if (InventoryOpen)
                {
                    
                    closeInventory();
                }
                else if (InventoryOpen == false)
                {
                    StartCoroutine(scanBeforeOpening(true));

                }
            }

        }



        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerUIScript.canUseCraftingTable && craftingSystem.CraftingMenu.activeSelf == false)
            {
               
                StartCoroutine(openCrafting());

            }
            else if (!playerUIScript.canUseCraftingTable && craftingSystem.CraftingMenu.activeSelf)
            {
                closeInventory();
                craftingSystem.craftingMenuOpen = false;
                craftingSystem.checkCraftables = true;
                playerUIScript.block = false;

            }
            else if(playerUIScript.canUseStorage)
            {
                StartCoroutine(scanBeforeOpening(false));

            }
            else if (playerUIScript.canLoot)
            {
                StartCoroutine(lootItemInstant());

            }else if (playerUIScript.onDoor == true)
            {
                playerUIScript.doorScript.openDoor();
                audioManager.Play("OpenDoor");
            }
            
        }


        
    }

    private IEnumerator openCrafting()
    {
        
        StartCoroutine(scanBeforeOpening(true));
        
        yield return null;
        InventoryOpen = false;
        craftingSystem.craftingMenuOpen = true;
        craftingSystem.checkCraftables = true;
        playerUIScript.canUseCraftingTable = false;
        playerUIScript.block = true;
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void closeInventory()
    {
        cancellation();
        inventoryOpen(false);
        playerUIScript.block = false;
        StartCoroutine(scanBeforeClosing());
    }


    public IEnumerator scanBeforeClosing()
    {
        if (pickedItem != null && pickedItem.isPicked)
        {
            pickedItem.transform.position = originalPosition.position;
            pickedItem.suitableToDrop = false;
            pickedItem.isPicked = false;
        }

        yield return null;

        playerStats.setConsumableArray();

        if (inventoryAnimator.GetBool("NavigateToInventory") == false)
        {
            inventoryAnimator.SetBool("NavigateToInventory", true);
        }

        if (storageSlots.activeSelf)
        {
            storageSlots.SetActive(false);
            centerNavigator.SetActive(false);

        }
        else
        {
            cancellation();
        }

        yield return null;

        StartCoroutine(scanner.scanInvent());

        yield return null;

        InventoryOpen = false;
        inventoryOpen(false);

        

    }

   
    public void navigateFunction()
    {
        if(centerNavigatorScript.canChange == true)
        {
            navigate = !navigate;
            if (navigate == true)
            {
                centerText.text = "Storage";
            }
            else
            {
                centerText.text = "Inventory";
            }
            inventoryAnimator.SetBool("NavigateToInventory", navigate);
            centerNavigatorScript.canChange = false;
        }
        
    }

    public void cancellation()
    {
        lootAttempt = false;

        if(pickedItem != null)
        {

            if (lootItem != null)
            {
                
                if(pickedItem == lootItem)
                {
                    deactivateItem();

                }
                else
                {
                    Destroy(pickedItem.transform.parent.gameObject);
                    playerUIScript.removeLoot();
                    
                    StartCoroutine(scanBeforeClosing());
                }
            }
            else
            {
                StartCoroutine(tryToFindSlot());
            }
        }

    }

    private IEnumerator tryToFindSlot()
    {
        bool x = inventoryManagement.lootItem(pickedItem);
        yield return null;

        if(x == false)
        {
            
            Destroy(pickedItem.transform.parent.gameObject);
            pickedItem = null;
            playerUIScript.removeLoot();
            StartCoroutine(scanBeforeClosing());
        }


    }

    private void deactivateItem()
    {
        lootItem.GetComponent<MoveableObjectScript>().isPicked = false;
        lootItem.transform.parent.gameObject.SetActive(false);
        StartCoroutine(scanBeforeClosing());
        pickedItem = null;
        lootItem = null;

    }

    public IEnumerator scanBeforeOpening(bool x)
    {
        playerStats.enableConsumable = false;
        navigate = true;
        StartCoroutine(playerStats.scanDataToInventory());

        if (x == false)
        {
            storageSlots.SetActive(true);
            centerNavigator.SetActive(true);
        }
        else
        {
            storageSlots.SetActive(false);
            centerNavigator.SetActive(false);
        }

        yield return null;

        InventoryOpen = true;
        inventoryOpen(true);

        yield return null;
    }

    private IEnumerator lootItemInstant()
    {
        scanBeforeOpening(false);

        yield return null;

        InventoryOpen = false;
        inventoryOpen(false);

        MoveableObjectScript script = playerUIScript.lootableObjectScript.instantiatedItem.transform.GetChild(0).GetComponent<MoveableObjectScript>();
        string title = script.ItemTag.ItemName;
        bool x = inventoryManagement.lootItem(script);
        yield return null;
        if (x == true)
        {
            inventoryManagement.summonPickUpNotif(title, true);

        }
        else
        {

            if(unlootable == null || unlootable != script)
            {
                unlootable = script;
                inventoryManagement.summonPickUpNotif(title, false);
            }

            
        }

    }


    private void setItemDescription()
    {
        if(pickedItem != null && storageSlots.activeSelf == false)
        {
            itemDescriptionUI.SetActive(true);
            TextMeshProUGUI itemName = itemDescriptionUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI hintText = itemDescriptionUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = itemDescriptionUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            string name = pickedItem.gameObject.GetComponent<ItemTag>().ItemName;
            Item item = craftingSystem.findItemInDatabase(name);

            itemName.text = item.name;
            if(item.forConsumption && item.forCrafting)
            {
                hintText.text = "Used For Consumption & Crafting";

            }else if(item.forConsumption && item.forCrafting == false)
            {
                hintText.text = "Used Only For Consumption";
            }
            else if(item.forConsumption == false && item.forCrafting)
            {
                hintText.text = "Used Only For Crafting";
            }
            else
            {
                hintText.text = " ";
            }

            descriptionText.text = item.description;
        }
        else
        {
            itemDescriptionUI.SetActive(false);
        }
    }

}
