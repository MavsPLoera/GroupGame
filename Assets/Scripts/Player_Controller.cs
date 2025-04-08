using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player_Controller : MonoBehaviour
{
    [Header("Player Stats")]
    public float playerMovementspeed;
    public float playerRollSpeed;
    public float playerHealth;
    public float swordDamage;
    public float healingAmount;
    public float playerHitKnockBack;
    public int healingPotions = 3;
    public int arrows = 15;
    public int playerLives = 3;
    public int maxHealthPotions = 5;
    public float maxHealth = 100;

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
    public float ultimateDuration;
    public float ultimateCooldown;
    public float invulnerabilityTime;
    public float healthBarEaseTime;

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
    public GameObject arrowSpawn;
    public GameObject respawnPosition;
    public Sword_Controller swordController;
    public Slider healthBarSlider;
    public Slider easeHealthBarSlider;
    public SpriteRenderer spriteRenderer;
    public GameObject arrow;
    public Color hurtColor;
    public BoxCollider2D boxCollider;
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
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (healthBarSlider.value != playerHealth)
        {
            healthBarSlider.value = playerHealth;
        }

        if (easeHealthBarSlider.value != healthBarSlider.value)
        {
            easeHealthBarSlider.value = Mathf.Lerp(easeHealthBarSlider.value, playerHealth, healthBarEaseTime);
        }

        if (!canInput)
        {
            return;
        }

        /*
         *          MAIN MOVEMENT CODE
         *          
         * Get the input values for the Horizontal and Vertical inputs
         * send to update Facing driection to update the value movementDirection
         * 
         * we know if the players input is nothing if this value is == to Vector2.zero
         * we will set the animator the animator to the idle state if this is the case
         * 
         * otherwise we will se the animator to walk, multiply the movement direction by speed since GetAxisRaw returns -1 - 1 values
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

        /*
         *          PLAYER ABILITIES CODE
         *          
         * This is the section of the program where abilities will be implemented        
         * 
         * abilities on fire1 fire2 and fire3
         * 
         * player by default is able to swing a sword, dash, and healing (IF THEY HAVE POTIONS)
         * 
         * players health will not go past MAXHEALTH
         * 
         * secondary ability and ultimate ability will be unlocked once the player completes the dungeons to unlock ability
         * 
         */

        //Dash
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            if (!(movementDirection.x == 0f && movementDirection.y == 0f))
            {
                StartCoroutine(dash(movementDirection));
            }
        }

        //Sword Swing
        if (Input.GetButton("Fire1") && !canSwing)
        {
            StartCoroutine(swing());
        }

        //Secondary
        if (Input.GetButton("Fire2") && unlockedSecondaryMove && canSecondary)
        {
            StartCoroutine(secondaryMove());
        }

        //Ultimate
        if (Input.GetButton("Fire3") && unlockedUltMove && canUlt)
        {
            StartCoroutine(ultimateMove());
        }

        //Healing Self
        if (Input.GetKeyDown(KeyCode.H) && !healingSelf)
        {
            StartCoroutine(healPlayer());
        }
    }

    public IEnumerator swing()
    {
        //Set animator to swing and stop playign from being able to input and swing again.
        playerAnimator.Play("Player_Swing", 0);
        rb.linearVelocity = Vector2.zero;
        canSwing = true;
        canInput = false;

        //Let the full animation play out. I am not sure why getting the length of the animation does not work but .6f does fine.
        yield return new WaitForSeconds(.6f);

        //After the animation finished set the animation state to idle and allow player to be able to swing again and input.
        playerAnimator.Play("Player_Idle", 0);
        canInput = true;
        canSwing = false;

        yield return null;
    }

    public IEnumerator healPlayer()
    {
        //When player is healing we want it to be similar to dark souls. So the players movementspeed is slowed down, and is unable to input until they heal.
        canSwing = false;
        canDash = false;
        float temp = playerMovementspeed;
        playerMovementspeed = temp * .75f;

        //Small duration before the player is actually healed and will not go past Max Health
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

        //Return player movespeed back to normal and allow player to use abilities again. Future notice I will need to disable other abilities as well
        yield return new WaitForSeconds(healingSlowdown);
        playerMovementspeed = temp;
        canSwing = true;
        canDash = true;
    }

    public IEnumerator secondaryMove()
    {
        canSecondary = false;
        rb.linearVelocity = Vector2.zero;
        playerAnimator.Play("Player_Secondary", 0);
        canInput = false;

        yield return new WaitForSeconds(.333f);

        Instantiate(arrow, arrowSpawn.transform.position, gameObject.transform.rotation);
        playerAnimator.Play("Player_Idle", 0);
        canSecondary = true;
        canInput = true;
    }

    public IEnumerator ultimateMove()
    {
        canUlt = false;
        playerHealth = maxHealth;
        yield return new WaitForSeconds(ultimateDuration);
        //Do other stuff

        yield return new WaitForSeconds(ultimateCooldown);
        canUlt = true;
    }

    public IEnumerator dash(Vector2 movement)
    {
        /* 
         * When the player presses space take the direction the player is moving and set the linear velocity of the player to dash
         * 
         * during this dashing, the player cannot input and cannot dash again until a cool down is up
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
        rb.AddForce(-(facingTowards.transform.position - transform.position) * playerHitKnockBack);
        playerAnimator.Play("Player_Hit", 0);
        invincible = true;
        boxCollider.enabled = false;

        canInput = false;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(.5f);

        spriteRenderer.color = Color.white;
        playerAnimator.Play("Player_Idle", 0);
        canInput = true;

        yield return new WaitForSeconds(invulnerabilityTime);

        boxCollider.enabled = true;
        invincible = false;
    }

    //[NOTE] Can add quest completed method that is just a way for a quest to give the reward to the player


    private void updateFacingDirection(float x_raw, float y_raw)
    {
        /*
         * Depending on the input we will se the values for the animator to match the values we are facing
         * 
         * Wanted to have rigidity that you would have in old school games like legend of zelda or pokemon so I am mirroring that movement here.
         * 
         * One of the others things we need to do is change the sword hit box based on the direction the player is facing. Making sure the sword hitbox is consistent no matter the direction.
         */

        if (y_raw == 1f)
        {
            facingTowards.transform.position = new Vector3(0f, 1f, 0f) + transform.position;
            arrowSpawn.transform.position = new Vector3(0f, .6f, 0f) + transform.position;
            movementDirection = new Vector2(0f, 1).normalized;
            playerAnimator.SetFloat("moveX", 0);
            playerAnimator.SetFloat("moveY", 1);
            swordController.updateHitBox(swingUpOffset, swingUpSize);
        }
        else if (y_raw == -1f)
        {
            facingTowards.transform.position = new Vector3(0f, -1f, 0f) + transform.position;
            arrowSpawn.transform.position = new Vector3(0f, -.6f, 0f) + transform.position;
            movementDirection = new Vector2(0f, -1).normalized;
            playerAnimator.SetFloat("moveX", 0);
            playerAnimator.SetFloat("moveY", -1);
            swordController.updateHitBox(swingDownOffset, swingDownSize);
        }
        else if (x_raw == 1f)
        {
            facingTowards.transform.position = new Vector3(1f, 0f, 0f) + transform.position;
            arrowSpawn.transform.position = new Vector3(.6f, 0f, 0f) + transform.position;
            movementDirection = new Vector2(1, 0f).normalized;
            playerAnimator.SetFloat("moveX", 1);
            playerAnimator.SetFloat("moveY", 0);
            swordController.updateHitBox(swingRightOffset, swingRightSize);
        }
        else if (x_raw == -1f)
        {
            facingTowards.transform.position = new Vector3(-1f, 0f, 0f) + transform.position;
            arrowSpawn.transform.position = new Vector3(-.6f, 0f, 0f) + transform.position;
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
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        if(invincible)
        {
            return;
        }

        if((playerHealth - damage) > 0f)
        {
            //Get enemy script component and get the damage value
            playerHealth -= damage;
            rb.linearVelocity = Vector2.zero;
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
            playerHealth = 0f;
            playerLives--;
            // gameObject.transform.position = respawnPosition.transform.position;
        }
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
