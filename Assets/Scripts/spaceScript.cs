using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spaceScript : MonoBehaviour
{
    public bool available = false;
    public GameObject collidedSlot;
    public ItemTag heldItemTag;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slot") && available == false)
        {
            heldItemTag = null;
            SlotScript collisionScript = collision.gameObject.GetComponent<SlotScript>();
            collidedSlot = collision.gameObject;
            if (collisionScript.alreadyInContact == false)
            {
                collisionScript.alreadyInContact = true;
                available = true;
                heldItemTag = null;
                return;
            }else if(collisionScript.heldItem != null && collisionScript.heldItem == transform.parent.gameObject)
            {
                available = true;
                heldItemTag = null;
                return;
            }

            available = false;

            if (collisionScript.alreadyInContact && collidedSlot.GetComponent<SlotScript>().heldItem != null)
            {
                if(collidedSlot.GetComponent<SlotScript>().heldItem.GetComponent<ItemTag>().isFull == false)
                {
                    //CheckIfStackable
                    collisionScript.alreadyInContact = true;
                    heldItemTag = collidedSlot.GetComponent<SlotScript>().heldItem.GetComponent<ItemTag>();
                }
                else
                {
                    heldItemTag = null;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == collidedSlot)
        {
            SlotScript ss = collision.gameObject.GetComponent<SlotScript>();
            if(collision.gameObject.GetComponent<SlotScript>().heldItem == null || collision.gameObject.GetComponent<SlotScript>().heldItem == transform.parent.gameObject)
            {
                ss.alreadyInContact = false;
                ss.heldItem = null;
            }
            ss.changeColorToOriginal();
            available = false;
            heldItemTag = null;
            collidedSlot = null;

        }
    }

}
