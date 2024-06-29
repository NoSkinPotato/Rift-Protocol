using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderDamageEffects : MonoBehaviour
{
    public float enemyHealth = 500;
    public EnemyStatsScript enemyStats;
    public EnemyMovement movement;
    public ParticleSystem Blood;
    public Color headShotColor;
    public Color bodyShotColor;
    public float headShotMultiplier = 2f;
    public float bodyShotMultiplier = 1f;
    public float spiderDamage = 75f;

    

    public void HeadShot()
    {
        movement.shot = true;
        enemyHealth -= enemyStats.WeaponDamage * headShotMultiplier;
        var blood = Blood.main;
        blood.startColor = headShotColor;
    }

    public void BodyShot()
    {
        movement.shot = true;
        enemyHealth -= enemyStats.WeaponDamage * bodyShotMultiplier;
        var blood = Blood.main;
        blood.startColor = bodyShotColor;
    }



}
