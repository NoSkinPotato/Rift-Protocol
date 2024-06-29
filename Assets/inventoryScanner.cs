using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryScanner : MonoBehaviour
{
    public LayerMask layerMask; 
    public Color gizmoColor = Color.green;
    public Vector3 size = new Vector3(1f, 1f, 1f);
    public List<GameObject> itemsInInventory;
    public GameObject inventory;
    public bool startScan = false;
    public PlayerStatsScript playerStats;
    public pauseManagerScript pauseManager;
    public bool subScan = true;
    public craftingSystem crafting;


    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, size);
    }

   

    private void Update()
    {
        if(startScan == true && inventory.activeSelf)
        {
            StartCoroutine(scanInvent());
            
            
        }

        scanner();

    }

   

    public IEnumerator scanInvent()
    {
        StartCoroutine(playerStats.scanSubInventory());
        StartCoroutine(playerStats.scanInventory());

        yield return null;

        if (startScan)
        {
            startScan = false;
        }
        
    }

    private void scanner()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, size, 0, Vector3.forward, 0f, layerMask);
        setInInventory(hits);
    }

    private void setInInventory(RaycastHit2D[] hits)
    {
        itemsInInventory.Clear();
        foreach(RaycastHit2D hit in hits)
        {
            
            if(hit.collider.gameObject.transform.parent.GetComponent<MoveableObjectScript>().isPicked == false)
            {
                GameObject obj = hit.collider.transform.parent.gameObject;

                bool check = checkRepetition(obj);
                if (check)
                {
                    itemsInInventory.Add(obj);
                }
            }
            
            
        }

    }

    private bool checkRepetition(GameObject x)
    {
        foreach(GameObject item in itemsInInventory)
        {
            if(x == item)
            {
                return false;
            }
        }
        return true;

    }

}
