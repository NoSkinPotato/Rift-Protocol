using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftableScript : MonoBehaviour
{
    public int[] materialsRequired;
    public string[] materialNames;
    public TextMeshProUGUI[] materialsText;
    public string craftedItemName;
    public bool craftable = false;
    public bool heldDown = false;
    public Slider slider;
    private float timer = 0;
    public float speedRate = 4f;
    private craftingSystem craftingSystem;


    private void Start()
    {
        setMaterialToText();
        craftingSystem = FindObjectOfType<craftingSystem>();
    }

    private void Update()
    {
        
        if (heldDown == true)
        {
            if(timer < 1)
            {
                timer += Time.deltaTime / speedRate;
            }
            else
            {
                timer = 0;
                crafted();
            }

        }
        else
        {
            timer = 0;
        }

        slider.value = timer;
    }

    private void crafted()
    {
        
        GameObject newItem = Instantiate(craftingSystem.findItemAfterCrafting(craftedItemName), craftingSystem.itemsTransform);
        MoveableObjectScript script = newItem.transform.GetChild(0).GetComponent<MoveableObjectScript>();
        craftingSystem.craftedItem = script;
        newItem.SetActive(false);
        StartCoroutine(craftingSystem.craftAnItem(this, script));
    }

    private void setMaterialToText()
    {
        for(int i = 0; i <  materialsRequired.Length; i++)
        {
            materialsText[i].text = "x" + materialsRequired[i].ToString();
        }
    }

    public void onPointerDown()
    {
        if (craftable) 
        {
            heldDown = true;
        }

        
    }
    

    public void onPointerUp()
    {
        heldDown = false;
    }
}
