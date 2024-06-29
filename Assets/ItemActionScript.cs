using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActionScript : MonoBehaviour
{
    public PlayerStatsScript playerStatsScript;

    public void useMeds()
    {
        playerStatsScript.playerCurrentMaxHealth += 0.2f * playerStatsScript.playerCurrentMaxHealth;
        playerStatsScript.currentPlayerHealth = playerStatsScript.playerCurrentMaxHealth;
    }

    public void useGauze()
    {


    }

    public void useBooster()
    {

    }

    public void useDonut()
    {
        playerStatsScript.currentPlayerHealth += 200f;
        if(playerStatsScript.currentPlayerHealth > playerStatsScript.playerCurrentMaxHealth)
        {
            playerStatsScript.currentPlayerHealth = playerStatsScript.playerCurrentMaxHealth;
        }
    }

    public void useHerbs()
    {
        playerStatsScript.currentPlayerHealth += 1000f;
        if (playerStatsScript.currentPlayerHealth > playerStatsScript.playerCurrentMaxHealth)
        {
            playerStatsScript.currentPlayerHealth = playerStatsScript.playerCurrentMaxHealth;
        }
    }

}
