using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Enemy base class: includes gaining experience points upon killing, chasing the player functionality, etc.
/* Currently implemented features:
 * - Chasing the player for attacking damage
 * - Respawn after a certain time period after death
 * 
 * Additional features that can be added:
 * - Random movement when not chasing the player
 * - Interactions and combat between enemies
 */
public class Enemy : Mover
{
    private bool isAlive = true;            // Is the enemy alive
    [Header("------Respawn System----")]
    public bool canRespawn = true;          // Can the enemy respawn
    public float timeToRespawn = 10f;       // Time in seconds before enemy respawns

    [Header("------Experience Value----")]
    public int xpValue = 1;                 // Experience points gained upon killing

    // Chasing logic:
    [Header("------Chasing Logic----")]
    public float speedMultiple = 0.75f;     // Enemy speed as a multiple of the normal speed
    public float triggerLength = 1.0f;      // Distance within which chasing is triggered
    public float chaseLength = 1.0f;        // Maximum distance for chasing
    public bool chasing;                    // Is the enemy currently chasing
    public bool collidingWithPlayer;        // Is the enemy colliding with the player (preventing clipping)

    private Transform playTransform;        // Player reference
    private Vector3 startingPosition;       // Original position of the enemy

    // Enemy state indicators
    [Header("------State Indicators----")]
    public SpriteRenderer enemyStateSprite;
    public List<Sprite> stateSprites;

    // Hitbox collider
    //[Header("------Others----")]
    public ContactFilter2D filter;
    private BoxCollider2D hitBox;
    private Collider2D[] hits = new Collider2D[10];

    // DEBUG-related
    public bool drawTriggerLength;          // Draw trigger range

    protected override void Start()
    {
        base.Start();
        playTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;

        // Note: Getting the first child (Hitbox)
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        CloseStateSprite();
    }

    protected virtual void Update()
    {
        collidingWithPlayer = false;

        if (drawTriggerLength)
        {
            // Visualize the triggerLength range
            Debug.DrawLine(transform.position, new Vector3(transform.position.x + triggerLength, transform.position.y, transform.position.z), Color.green);
            Debug.DrawLine(transform.position, new Vector3(transform.position.x - triggerLength, transform.position.y, transform.position.z), Color.green);
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + triggerLength, transform.position.z), Color.green);
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - triggerLength, transform.position.z), Color.green);
        }

        // Get overlapping colliders
        hitBox.OverlapCollider(filter, hits);
        if (hitBox == null)
            return;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            // If a player is detected within the trigger range
            if (hits[i].CompareTag("Fighter") && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }
            hits[i] = null;
        }
    }

    private void FixedUpdate()
    {
        // Chasing logic is only performed when the enemy is alive
        if (isAlive)
            ChasingTarget();
        else
            pushDirection = Vector3.zero;
    }

    // Chasing player function:
    protected virtual void ChasingTarget()
    {
        // If the player is within the chaseLength range of the enemy's starting position and the player is alive
        if ((Vector3.Distance(playTransform.position, startingPosition) < chaseLength) && GameManager.instance.player.isAlive)
        {
            // Further, if the player is too close (within triggerLength), the enemy starts chasing
            if (Vector3.Distance(playTransform.position, startingPosition) < triggerLength)
                chasing = true;

            // Chasing state:
            if (chasing)
            {
                OpenStateSprite();

                // If the enemy and player are not colliding, continue chasing
                // Otherwise, maintain the collision state without further movement
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playTransform.position - transform.position).normalized, speedMultiple);
                }
            }
            else
            {
                // Non-chasing state: Enemy returns to its original position
                UpdateMotor((startingPosition - transform.position), speedMultiple);
                CloseStateSprite();
            }
        }
        else
        {
            // If the enemy and player are too far apart, stop chasing and return to the original position
            UpdateMotor((startingPosition - transform.position), speedMultiple);
            chasing = false;
            CloseStateSprite();
        }
    }

    // Open state display: Half health or above / Below half health
    private void OpenStateSprite()
    {
        enemyStateSprite.enabled = true;
        if ((float)hitPoint / (float)maxHitPoint < 0.5)
            enemyStateSprite.sprite = stateSprites[1];
        else
            enemyStateSprite.sprite = stateSprites[0];
    }

    private void CloseStateSprite()
    {
        enemyStateSprite.enabled = false;
    }

    // Enemy death function
    protected override void Death()
    {
        // Player gains experience, display UI with +xp
        GameManager.instance.GrantXP(xpValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);

        // Decide whether the enemy can respawn or not
        if (canRespawn)
        {
            isAlive = false;
            hitBox.enabled = false;
            CloseStateSprite();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine("WaitingForRespawn");
        }
        else
            Destroy(gameObject);
    }

    IEnumerator WaitingForRespawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        isAlive = true;
        hitBox.enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        OpenStateSprite();
        hitPoint = maxHitPoint;
        gameObject.transform.position = startingPosition;
    }
}


