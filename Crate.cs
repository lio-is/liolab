using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script for destroyable objects, can be attached to obstacles like crates.
public class Crate : Fighter
{
    private void Start()
    {
        ImmuneTime = 0.5f;
    }

    protected override void ReceiveDamage(Damag dmg)
    {
        // If not in immune time, the object takes damage
        if (Time.time - lastImmune > ImmuneTime)
        {
            lastImmune = Time.time;
            hitPoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
        }

        // If the object's health drops below 0, it dies
        if (hitPoint <= 0)
        {
            hitPoint = 0;
            Death();
        }
    }

    protected override void Death()
    {
        // Destroy the object
        Destroy(gameObject);
    }
}

