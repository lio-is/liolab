using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Xml;

public class SaveManager : MonoBehaviour
{
   private SpriteRenderer spriteRenderer;      // Current player sprite
 public bool isAlive = true;                 // Is the player alive

 [Header("------Rage System------")]
 public float rage = 0;                      // Rage
 public float maxRage = 50;                  // Maximum rage value

 // Turning attack correction system
 private float temp = 0f;

 protected override void Start()
 {
     base.Start();
     GetComponent<BoxCollider2D>().enabled = true;
     spriteRenderer = GetComponent<SpriteRenderer>();
     ImmuneTime = 0.75f;
     Player.DontDestroyOnLoad(gameObject);
     OnRageChange(0f);
 }

 private void FixedUpdate()
 {
     // Get movement values, use the common movement function UpdateMotor(), move according to the specified position/movement speed multiplier
     if (isAlive)
     {
         float x = Input.GetAxisRaw("Horizontal");
         float y = Input.GetAxisRaw("Vertical");

         // Check if the player's direction before and after movement is the same
         // If different, immediately stop the Swing animation of the Weapon and switch to idle
         // Otherwise, proceed normally and wait for the Swing animation to finish before switching to idle
         // Functionality: Stop attacking immediately if turning during the attack
         if (transform.localScale.x == temp)
             GameManager.instance.weapon.animator.SetBool("SameDirection", true);
         else GameManager.instance.weapon.animator.SetBool("SameDirection", false);
         temp = transform.localScale.x;

         UpdateMotor(new Vector3(x, y, 0), 1);

         // Note: Using the following method to read input would significantly increase GCAlloc within FixedUpdate
         //
         // moveTo.x = Input.GetAxisRaw("Horizontal");
         // moveTo.y = Input.GetAxisRaw("Vertical");
         // UpdateMotor(moveTo, 1);
     }
     else
         pushDirection = Vector3.zero;
     // The above else statement has a problem:
     // Problem description: When the player dies and respawns, they are immediately pushed back a certain distance
     // Problem analysis: The reason is that the player dies from a fatal blow, but is not pushed back, so when pushDirection persists after respawn
     // Solution: Add the else statement above to reset pushDirection, similarly for Enemy
 }

 // Sprite change function
 public void SwapSprite(int SkinID)
 {
     GetComponent<SpriteRenderer>().sprite = GameManager.instance.playerSprites[SkinID];
 }

 // Level up function: Increase maximum health and restore current health
 public void OnLevelUp()
 {
     maxHitPoint += 10;
     hitPoint = maxHitPoint;

     GameManager.instance.OnUIChange();
 }

 // Set level function (only for internal use in GameManager)
 public void SetLevel(int level)
 {
     for (int i = 0; i < level; i++)
         OnLevelUp();
 }

 // Player damage function: Reduce health, increase rage, refresh health UI
 protected override void ReceiveDamage(Damag dmg)
 {
     if (!isAlive)
         return;

     // If not in immune time, take damage
     if (Time.time - lastImmune > ImmuneTime)
     {
         lastImmune = Time.time;
         hitPoint -= dmg.damageAmount;
         pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

         // Rage system:
         // If currently using a skill, rage cannot accumulate
         if (!GameManager.instance.weapon.raging)
             OnRageChange(dmg.damageAmount);
     }

     if (hitPoint <= 0)
     {
         hitPoint = 0;
         Death();
     }

     GameManager.instance.OnUIChange();
 }

 // Rage accumulation system
 public void OnRageChange(float alter)
 {
     if (rage < maxRage)
         rage += alter;
     if (rage >= maxRage)
         rage = maxRage;

     if (rage == maxRage)
         GameManager.instance.weapon.CanRageSkill = true;
 }

 // Heal function: Health restoration, display healing UI text, and refresh health UI
 public void Heal(int healingAmount)
 {
     if (hitPoint == maxHitPoint)
         return;

     hitPoint += healingAmount;
     if (hitPoint > maxHitPoint)
         hitPoint = maxHitPoint;

     GameManager.instance.ShowText("+" + healingAmount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
     GameManager.instance.OnUIChange();
 }

 // Player death function
 protected override void Death()
 {
     // Change life state and lie down
     isAlive = false;
     transform.localEulerAngles = new Vector3(0, 0, 90);

     // Show death panel
     GameManager.instance.UIManager.ShowDeathAnimation();

     // Wait for a certain time before respawn and restart
     StartCoroutine("WaitingForRespawn");
 }

 // Player respawn function
 public void Respawn()
 {
     // Configure parameters for respawn
     Heal(maxHitPoint);
     isAlive = true;
     transform.localEulerAngles = Vector3.zero;
 }

 IEnumerator WaitingForRespawn()
 {
     yield return new WaitForSeconds(6);
     GameManager.instance.Respawn();
     GameManager.instance.OnUIChange();
 }
}

