using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bigBoySpecific : MonoBehaviour
{
    public EnemyMovement EnemyMovement;
    public GameObject victory;
    public void victoryScreen()
    {
        Time.timeScale = 0;
        victory.SetActive(true);
    }
}
