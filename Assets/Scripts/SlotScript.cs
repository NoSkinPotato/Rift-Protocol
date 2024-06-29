using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    public bool unlocked = false;
    public bool alreadyInContact = false;
    public Image slotRenderer;
    Color original;
    public GameObject heldItem;

    private void Start()
    {
        original = slotRenderer.color;
    }


    public void changeColorToOriginal()
    {
        slotRenderer.color = original;
    }

    public void changeColorWhenOccupied()
    {
        slotRenderer.color = Color.red;
    }

    public void ChangeColorWhenAvailable()
    {
        slotRenderer.color = Color.green;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.parent.CompareTag("Item") && collision.transform.parent.GetComponent<MoveableObjectScript>().isPicked == false)
        {
            MoveableObjectScript script = collision.transform.parent.GetComponent<MoveableObjectScript>();
            if (script != null && script.isPicked == false)
            {
                heldItem = collision.transform.parent.gameObject;
            }

        }
    }



}
