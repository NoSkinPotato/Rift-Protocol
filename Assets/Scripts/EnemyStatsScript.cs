using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyStatsScript : MonoBehaviour
{
    
    public Collider2D[] colliders;
    public UnityEvent[] DamageEffects;
    public UnityEvent Death;
    public float WeaponDamage;

    public void DealDamageToEnemy(Collider2D hitCollider, float weaponDamage)
    {

        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == hitCollider)
            {
                WeaponDamage = weaponDamage;
                DamageEffects[i].Invoke();
                return;
            }
        }

    }

    

}
