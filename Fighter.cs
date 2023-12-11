using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fighter : MonoBehaviour
{
  // Basic parameters: Health, maximum health, speed recovery coefficient
[Header("------Health System------")]
public int hitPoint = 10;               // Current health
public int maxHitPoint = 10;            // Maximum health
public float pushRecoverySpeed = 0.2f;  // Speed of recovering from being pushed

// Immunity time parameters
protected float ImmuneTime = 0.2f;
protected float lastImmune;

// Parameters for being attacked: Push distance
protected Vector3 pushDirection;

// Function to receive damage
protected virtual void ReceiveDamage(Damag dmg)
{
    // If not in immune time, take damage
    if (Time.time - lastImmune > ImmuneTime)
    {
        // Reset immune time
        lastImmune = Time.time;

        // Inflict damage
        hitPoint -= dmg.damageAmount;

        // Get pushed back
        pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
    }

    // Display damage UI
    // Discarded: Showing the same type of UI for both enemies and players can be confusing,
    // and high-speed collisions may lead to penetration bugs due to the small size of the BoxCollider2D.
    // GameManager.instance.ShowText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.zero, 0.5f);

    // If the object's health is below 0, it dies
    if (hitPoint <= 0)
    {
        hitPoint = 0;
        Death();
    }
}

// Death function
protected virtual void Death()
{

}
}


