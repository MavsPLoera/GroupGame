using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player_Controller : MonoBehaviour
{
    [Header("Player Stats")]
    public float playerMovementspeed;
    public float playerRollSpeed;
    public float playerHealth;
    public float swordDamage;
    public float healingAmount;
    public int healingPotions = 3;
    public int playerLives = 3;
    public int maxHealthPotions = 5;
    public int maxHealth = 100;

    [Header("Player boolean conditions")]
    public bool canInput = true;
    public bool canDash = true;
    public bool healingSelf = false;
    public bool canSecondary = true;
    public bool canUlt = true;
    public bool canSwing = false;
    public bool unlockedSecondaryMove = false;
    public bool unlockedUltMove = false;

    //DONT TOUCH THIS
    public bool invincible = false;

    [Header("Player Cooldowns")]
    public float dashDuration;
    public float dashCooldown;
    public float timeBeforeHealing;
    public float healingSlowdown;
    public float secondaryCooldown;
    public float ultimateCooldown;
    public float invulnerabilityTime;

    [Header("Player Quests")]
    public List<Quest> quests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    [Header("Player Audio")]
    public AudioSource audioSource;
    public AudioClip dashSound;
    public AudioClip playerLandSound;
    public AudioClip healingSound;
    public AudioClip swordSwingSound;
    public AudioClip swordHitSound;
    public AudioClip secondaryMoveSound;
    public AudioClip secondaryMoveHitSound;
    public AudioClip ultimateMoveSound;
    public AudioClip ultimateMoveHitSound;
    public AudioClip unlockedNewAbilitySound;

    [Header("Player Particle Systems")]
    public ParticleSystem healingParticles;
    public ParticleSystem dashDustParticles; //used for when player initially jumps and lands back on the ground
    public ParticleSystem dashCooldonRefreshedParticles;
    public ParticleSystem secondaryMoveParticles;
    public ParticleSystem ultimateParticles;
    public ParticleSystem playerHitParticles;

    [Header("Player Sword Hitboxes")]
    public Vector2 swingUpOffset;
    public Vector2 swingUpSize;
    public Vector2 swingDownOffset;
    public Vector2 swingDownSize;
    public Vector2 swingRightOffset;
    public Vector2 swingRightSize;
    public Vector2 swingLeftOffset;
    public Vector2 swingLeftSize;

    [Header("Player Misc.")]
    public GameObject facingTowards;
    public GameObject respawnPosition;
    public Sword_Controller swordController;
    public GameObject playerUI;
    public Animator playerAnimator;
    private Rigidbody2D rb;
    private Vector2 movementDirection;

    //Use this to access the player. No need to drag player gameobject to retrieve the script. There is only one player so this should be fine.
    public static Player_Controller instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        swordController = facingTowards.GetComponent<Sword_Controller>();
        canInput = true;
        playerHealth = maxHealth;

        //Default amount of healing potions
        healingPotions = 3;

        quests.Add(new Quest("TestTitle", "Test Description", "Test Reward"));
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canInput)
        {
            return;
        }

        /*
         *          MAIN MOVEMENT CODE
         *          
         *          
         * 
         * 
         * 
         */

        float x_raw = Input.GetAxisRaw("Horizontal");
        float y_raw = Input.GetAxisRaw("Vertical");

        updateFacingDirection(x_raw, y_raw);

        if (movementDirection != Vector2.zero)
        {
            playerAnimator.Play("Player_Walk", 0);
        }
        else
        {
            playerAnimator.Play("Player_Idle", 0);
        }

        rb.linearVelocity = movementDirection * playerMovementspeed;



        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            if (!(movementDirection.x == 0f && movementDirection.y == 0f))
            {
                StartCoroutine(dash(movementDirection));
            }
        }

        /*
         *          PLAYER ABILITIES CODE
         *          
         *          
         * 
         * 
         * 
         */

        if (Input.GetButton("Fire1") && !canSwing)
        {
            StartCoroutine(swing());
        }

        if (Input.GetButton("Fire2") && unlockedSecondaryMove && canSecondary)
        {
            StartCoroutine(secondaryMove());
        }

        if (Input.GetButton("Fire3") && unlockedUltMove && canUlt)
        {
            StartCoroutine(ultimateMove());
        }

        /*
         * When payer presses H, check if the player has any potions on hand.
         * If they have a potion on hand, USE IT!
         * 
         * player CANNOT be healed over 100HP
         */

        if (Input.GetKeyDown(KeyCode.H) && !healingSelf)
        {
            StartCoroutine(healPlayer());
        }
    }

    public IEnumerator swing()
    {
        playerAnimator.Play("Player_Swing", 0);
        rb.linearVelocity = Vector2.zero;
        canSwing = true;
        canInput = false;

        yield return new WaitForSeconds(.6f);

        playerAnimator.Play("Player_Idle", 0);
        canInput = true;
        canSwing = false;

        yield return null;
    }

    //Wanting to mirror the way in which a player heals in dark souls. Where the player slows down to heal before recieving it.
    public IEnumerator healPlayer()
    {
        canSwing = false;
        canDash = false;
        float temp = playerMovementspeed;
        playerMovementspeed = temp * .75f;

        yield return new WaitForSeconds(timeBeforeHealing);

        if (healingPotions > 0)
        {
            if (playerHealth + healingAmount > maxHealth)
            {
                playerHealth = maxHealth;
            }
            else
            {
                playerHealth += healingAmount;
            }
            healingPotions--;
        }

        yield return new WaitForSeconds(healingSlowdown);
        playerMovementspeed = temp;
        canSwing = true;
        canDash = true;
    }

    public IEnumerator secondaryMove()
    {
        yield return null;
    }

    public IEnumerator ultimateMove()
    {
        yield return null;
    }

    public IEnumerator dash(Vector2 movement)
    {
        /* 
         * When the player presses space take the direction the player is moving and set the linear velocity of the player to dash
         * 
         * during this dashing time the player cannot input and cannot dash again until a cool down is up
         * 
         * NEED TO ALSO CHANGE THE HITBOXES TO NOT COLLIDE WITH BULLETS like enter the gungeon
         */

        canInput = false;
        canDash = false;
        rb.linearVelocity = movement * playerRollSpeed;
        yield return new WaitForSeconds(.025f);
        rb.linearVelocity /= .9f;
        yield return new WaitForSeconds(dashDuration);
        canInput = true;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    //Change this to turn off collisions between enemies and the player
    private IEnumerator temporaryInvulnerability()
    {
        invincible = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        invincible = false;
    }

    //Can add quest completed method that is just a way for a quest to give the reward to the player


    //Use this for the animator to get the right animation state based on where the player was last left facing.
    private void updateFacingDirection(float x_raw, float y_raw)
    {
        if (y_raw == 1f)
        {
            facingTowards.transform.position = new Vector3(0f, 1f, 0f) + transform.position;
            movementDirection = new Vector2(0f, 1).normalized;
            playerAnimator.SetFloat("moveX", 0);
            playerAnimator.SetFloat("moveY", 1);
            swordController.updateHitBox(swingUpOffset, swingUpSize);
        }
        else if (y_raw == -1f)
        {
            facingTowards.transform.position = new Vector3(0f, -1f, 0f) + transform.position;
            movementDirection = new Vector2(0f, -1).normalized;
            playerAnimator.SetFloat("moveX", 0);
            playerAnimator.SetFloat("moveY", -1);
            swordController.updateHitBox(swingDownOffset, swingDownSize);
        }
        else if (x_raw == 1f)
        {
            facingTowards.transform.position = new Vector3(1f, 0f, 0f) + transform.position;
            movementDirection = new Vector2(1, 0f).normalized;
            playerAnimator.SetFloat("moveX", 1);
            playerAnimator.SetFloat("moveY", 0);
            swordController.updateHitBox(swingRightOffset, swingRightSize);
        }
        else if (x_raw == -1f)
        {
            facingTowards.transform.position = new Vector3(-1f, 0f, 0f) + transform.position;
            movementDirection = new Vector2(-1, 0f).normalized;
            playerAnimator.SetFloat("moveX", -1);
            playerAnimator.SetFloat("moveY", 0);
            swordController.updateHitBox(swingLeftOffset, swingLeftSize);
        }
        else if(y_raw == 0f && x_raw == 0f)
        {
            movementDirection = Vector2.zero;
        }
    }

    private void gameOver()
    {
        //play animation
        //trigger gameover on UI
        //destroy gameobject
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("HealthPickUp"))
        {
            if ((healingPotions + 1) >= maxHealthPotions)
            {
                healingPotions = maxHealthPotions;
            }
            else
            {
                healingPotions++;
            }
        }
        else if (collision.gameObject.CompareTag("OtherPickUp")) //change the name of otherpickup to what the name 
        {
            //do other thing
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //Going to assume collision is just a enemy always
            float damage = 5f;
            if (playerHealth - damage > 0f)
            {
                //Get enemy script component and get the damage value
                playerHealth -= damage;
                StartCoroutine(temporaryInvulnerability());
            }
            else if (((playerHealth - damage) < 0f) && ((playerLives - 1) < 0))
            {
                //Game over sequence
                gameOver();
            }
            else
            {
                //Play animation of some sort then 
                //Make this coroutine
                playerLives--;
                gameObject.transform.position = respawnPosition.transform.position;
            }
        }
    }
}

public class Quest
{
    public int questID;
    public string questTitle;
    public string questDescription;
    public string reward;

    public float maxHealthIncrease;
    public float maxPotionsIncrease;
    public float swordDamageIncrease;

    public Quest(string questTitle, string questDescription, string reward)
    {
        this.questTitle = questTitle;
        this.questDescription = questDescription;
        this.reward = reward;
    }

    public override string ToString()
    {
        return $" {questTitle}, {questDescription}, {reward}";
    }
}
