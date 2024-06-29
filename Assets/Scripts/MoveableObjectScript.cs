using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MoveableObjectScript : MonoBehaviour, IPointerClickHandler
{
    public bool suitableToDrop = false;
    public bool isPicked = false;
    public bool discard = false;
    public spaceScript[] allSpaces;
    public yOrderSpaces[] orderedSpaces;
    public bool justDropped = false;
    public ItemTag ItemTag;
    public bool mustStack = false;
    private pauseManagerScript pauseManagerScript;
    public bool switchable = false;
    public MoveableObjectScript targetSwitchObject;
    public MoveableObjectScript switchedObject;
    
    

    private void Start()
    {
        pauseManagerScript = FindObjectOfType<pauseManagerScript>();
    }

   
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isPicked == false)
            {
                isPicked = true;
                suitableToDrop = true;
                removeHeldItems();

                pauseManagerScript.pickedItem = this;
                transform.parent.SetAsLastSibling();

            }
            else
            {
                if (suitableToDrop)
                {
                    suitableToDrop = false;
                    transform.parent.position = findSlotPosition();
                    isPicked = false;
                    pauseManagerScript.pickedItem = null;
                    justDropped = true;
                }
                else if (mustStack) {

                    ItemTag theTag = allSpaces[0].heldItemTag;
                    if (ItemTag.currentQuantity <= (theTag.maxQuantity - theTag.currentQuantity))
                    {
                        theTag.currentQuantity += ItemTag.currentQuantity;
                        Destroy(transform.parent.gameObject);
                    }
                    else
                    {
                        int x = theTag.maxQuantity - theTag.currentQuantity;
                        theTag.currentQuantity += x;
                        ItemTag.currentQuantity -= x;

                    }
                }else if (switchable && targetSwitchObject != null)
                {
                    StartCoroutine(switchItems());

                }
            }
        }
        
    }

    private void removeHeldItems()
    {
        foreach(spaceScript space in allSpaces)
        {
            space.collidedSlot.GetComponent<SlotScript>().heldItem = null;
        }
    }

    private IEnumerator switchItems()
    {
        isPicked = false;
        switchable = false;
        transform.parent.position = findSlotPosition();
        justDropped = true;
        

        yield return null;
        availableAllSpaces(targetSwitchObject);
        pauseManagerScript.pickedItem = targetSwitchObject;
        targetSwitchObject.gameObject.transform.parent.SetAsLastSibling();
        targetSwitchObject.isPicked = true;
        targetSwitchObject.switchable = false;
    }


    private void Update()
    {
        if (isPicked)
        {
            transform.parent.position = (Vector2)Input.mousePosition;
            StartCoroutine(checkingStuff());
            SlotColorControlWhenPicked();
        }
        else
        {
            if(justDropped)
            {
                SlotColorControlWhenLeftBe();
                justDropped = false;
            }
            
        }
    }

   
    private IEnumerator checkingStuff()
    {
        checkSuitabilityToDrop();

        yield return null;

        checkStackable();

        yield return null;

        checkSuitabilityToSwitch();
    }

    private void checkSuitabilityToSwitch()
    {
        if(suitableToDrop == false && mustStack == false)
        {
            MoveableObjectScript script = null;
            for(int i = 0; i < allSpaces.Length; i++)
            {
                if (allSpaces[i].collidedSlot == null)
                {
                    switchable = false;
                    targetSwitchObject = null;
                    return;
                }

                GameObject obj = allSpaces[i].collidedSlot.GetComponent<SlotScript>().heldItem;

                if (script == null)
                {
                    if(obj != null )
                    {
                        if(obj != gameObject)
                        {
                            MoveableObjectScript x = allSpaces[i].collidedSlot.GetComponent<SlotScript>().heldItem.GetComponent<MoveableObjectScript>();
                            if (x.allSpaces.Length == allSpaces.Length)
                            {
                                script = x;
                            }
                            else
                            {
                                switchable = false;
                                targetSwitchObject = null;
                                return;
                            }
                        }
                    }
                    else
                    {
                        switchable = false;
                        targetSwitchObject = null;
                        return;

                    }

                }
                else
                {
                    if(obj != null)
                    {
                        if(script.gameObject != obj)
                        {
                            switchable = false;
                            targetSwitchObject = null;
                            return;
                        }
                    }
                    else
                    {
                        switchable = false;
                        targetSwitchObject = null;
                        return;
                    }
                }

            }

            switchable = true;
            targetSwitchObject = script;
            return;
        }
        
    }

    public void availableAllSpaces(MoveableObjectScript script)
    {
        foreach (spaceScript space in script.allSpaces)
        {
            space.available = false;
        }
    }

    private void SlotColorControlWhenPicked()
    {
        if (suitableToDrop == false && switchable == false && mustStack == false)
        {
            foreach (spaceScript space in allSpaces)
            {
                if (space.collidedSlot != null)
                {
                    space.collidedSlot.GetComponent<SlotScript>().changeColorWhenOccupied();
                }
            }
        }
        else if (suitableToDrop || switchable || mustStack)
        {
            foreach (spaceScript space in allSpaces)
            {
                if (space.collidedSlot != null)
                {
                    space.collidedSlot.GetComponent<SlotScript>().ChangeColorWhenAvailable();
                }
            }
        }
    }

    private void SlotColorControlWhenLeftBe()
    {
        foreach (spaceScript space in allSpaces)
        {
            if (space.collidedSlot != null)
            {
                space.collidedSlot.GetComponent<SlotScript>().changeColorToOriginal();
            }
        }
    }

    public void checkSuitabilityToDrop()
    {
        foreach(spaceScript space in allSpaces)
        {
            if(space.available == false)
            {
                suitableToDrop = false;
                return;
            }
        }


        suitableToDrop = true;
        return;

    }

    private void checkStackable()
    {
        if(suitableToDrop == false)
        {
            ItemTag temp = allSpaces[0].heldItemTag;

            if (temp == null || temp.ItemName != ItemTag.ItemName)
            {
                mustStack = false;
                return;
            }

            for(int i = 0; i < allSpaces.Length; i++)
            {
                if (temp != allSpaces[i].heldItemTag)
                {
                    mustStack = false;
                    return;
                }
            }

            if(temp.ItemName == ItemTag.ItemName)
            {
                mustStack = true;
            }
            else
            {
                mustStack = false;
            }

            
            
        }

    }

    private Vector2 findSlotPosition()
    {
        Vector2[] points = new Vector2[allSpaces.Length];
        for(int i = 0; i < allSpaces.Length; i++)
        {
            points[i] = allSpaces[i].collidedSlot.transform.position;
        }

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

    
}
