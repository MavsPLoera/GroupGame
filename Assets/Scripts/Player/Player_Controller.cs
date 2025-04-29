using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    [Header("Player Stats")]
    public float playerMovementspeed;
    public float playerMovementSpeedUnchanging = 5f;
    public float playerRollSpeed;
    public float playerHealth;
    public float swordDamage;
    public float swordDamageUltIncrease = 5f;
    public float healingAmount;
    public float playerHitKnockBack;
    public int healingPotions = 3; //Make UI element
    public int arrows = 15; //Make UI Element
    public int maxArrows = 15;
    public int playerLives = 3; //Make UI Element
    public int maxHealthPotions = 5;
    public float maxHealth = 100;
    public float gold = 0;
    public float flickerAmount = 10;


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
    public float ultimateDuration;
    public float ultimateCooldown;
    public float invulnerabilityTime;
    public float healthBarEaseTime;
    public float flickerDuration;
    public float reloadCoolDown;

    [Header("Player Quests")]
    public List<Quest> quests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    [Header("Player Audio")]
    public AudioSource playerAudioSource;
    public AudioSource playerChangingAudioSource;
    public AudioClip goldCollect;
    public AudioClip playerTakeDamage;
    public AudioClip dashSound;
    public AudioClip healingSound;
    public AudioClip swordSwingSound;
    public AudioClip bowShootSound;
    public AudioClip arrowsRefilledSound;
    public AudioClip ultimateMoveSound;
    public AudioClip ultimateRefreshedSound;
    public AudioClip unlockedNewAbilitySound;

    [Header("Player Particle Systems")]
    public ParticleSystem healingParticles;
    public ParticleSystem dashDustParticles;
    public ParticleSystem dashCooldonRefreshedParticles;
    public ParticleSystem ultimateParticles;
    public ParticleSystem ultimateRefreshedParticles;
    public ParticleSystem DustFX;

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
    public GameObject ultLight;
    public float timer = 0.0f;
    public Slider healthBarSlider;
    public Slider easeHealthBarSlider;
    public SpriteRenderer spriteRenderer;
    public GameObject arrow;
    public BoxCollider2D boxCollider;
    public GameObject playerUI;
    public Animator playerAnimator;
    public GameObject crossFadeIn;
    public Rigidbody2D rb;
    private Vector2 movementDirection;

    //Use this to access the player. No need to drag player gameobject to retrieve the script. There is only one player so this should be fine.
    public static Player_Controller instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        arrows = maxArrows;

        //Default amount of healing potions
        healingPotions = 3;
        playerMovementSpeedUnchanging = playerMovementspeed;

        quests.Add(new Quest("TestTitle", "Test Description", "Test Reward"));
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBarSlider.value != playerHealth)
        {
            healthBarSlider.value = playerHealth;
        }

        //Fix this, slider is not lerpign properlly
        if (easeHealthBarSlider.value != playerHealth)
        {
            timer += Time.deltaTime;
            easeHealthBarSlider.value = Mathf.Lerp(easeHealthBarSlider.value, healthBarSlider.value, timer/healthBarEaseTime);
        }
        else
        {
            timer = 0.0f;
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

        if(canSwing)
        {
            updateFacingDirection(x_raw, y_raw);

            if (movementDirection != Vector2.zero)
            {
                playerAnimator.Play("Player_Walk", 0);

                if (!DustFX.isPlaying)
                {
                    DustFX.Play();
                }
            }
            else
            {
                if (DustFX.isPlaying)
                {
                    DustFX.Stop();
                }

                playerAnimator.Play("Player_Idle", 0);
            }

            rb.linearVelocity = movementDirection * playerMovementspeed;
        }
        else
        {
            rb.linearVelocity = new Vector2(x_raw,y_raw) * playerMovementspeed;
        }

        if(healingSelf)
        {
            return;
        }


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

        //Ultimate
        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.C)) && unlockedUltMove && canUlt)
        {
            StartCoroutine(ultimateMove());
        }

        //Dash
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            if (!(movementDirection.x == 0f && movementDirection.y == 0f))
            {
                StartCoroutine(dash(movementDirection));
            }
        }

        //Sword Swing
        if ((Input.GetButton("Fire1") || Input.GetKeyDown(KeyCode.Z)) && canSwing)
        {
            StartCoroutine(swing());
        }

        //Secondary
        if ((Input.GetButton("Fire2") || Input.GetKeyDown(KeyCode.X)) && unlockedSecondaryMove && canSecondary && !(arrows <= 0))
        {
            StartCoroutine(secondaryMove());
        }

        //Healing Self
        //Need to change key that player uses to heal.
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.V)) && !healingSelf && (healingPotions != 0))
        {
            StartCoroutine(healPlayer());
        }
    }

    public IEnumerator swing()
    {
        playerMovementspeed = playerMovementSpeedUnchanging * .3f;

        //Set animator to swing and stop player from being able to input and swing again.
        playerAnimator.Play("Player_Swing", 0);
        rb.linearVelocity = Vector2.zero;
        canSwing = false;
        playerAudioSource.PlayOneShot(swordSwingSound);

        //Let the full animation play out. I am not sure why getting the length of the animation does not work but .6f does fine.
        yield return new WaitForSeconds(.6f);

        //After the animation finished set the animation state to idle and allow player to be able to swing again and input.
        playerAnimator.Play("Player_Idle", 0);
        playerMovementspeed = playerMovementSpeedUnchanging;
        canSwing = true;
    }

    public IEnumerator healPlayer()
    {
        //When player is healing we want it to be similar to dark souls. So the players movementspeed is slowed down, and is unable to input until they heal.
        healingSelf = true;

        float temp = playerMovementspeed;
        playerMovementspeed = temp * .75f;

        //Small duration before the player is actually healed and will not go past Max Health
        yield return new WaitForSeconds(timeBeforeHealing);

        if (playerHealth + healingAmount > maxHealth)
        {
            playerHealth = maxHealth;
        }
        else
        {
            playerHealth += healingAmount;
        }
        healingPotions--;

        playChangingPitchSound(healingSound);
        //Return player movespeed back to normal and allow player to use abilities again. Future notice I will need to disable other abilities as well
        yield return new WaitForSeconds(healingSlowdown);

        healingSelf = false;
        playerMovementspeed = temp;
    }

    public IEnumerator secondaryMove()
    {
        //Prevent the player from shooting multiple arrows at the same time, let the animation fully play out before creating an arrow.
        canSecondary = false;
        rb.linearVelocity = Vector2.zero;
        playerAnimator.Play("Player_Secondary", 0);
        canInput = false;
        playerAudioSource.PlayOneShot(bowShootSound);


        yield return new WaitForSeconds(.333f);
        playChangingPitchSound(bowShootSound);
        Instantiate(arrow, arrowSpawn.transform.position, gameObject.transform.rotation);
        arrows--;
        
        //If the player is out of arrows the magical bow will magically reload.
        if(arrows <= 0)
        {
            StartCoroutine(reloadArrows());
        }

        //Set player back to idle.
        playerAnimator.Play("Player_Idle", 0);
        canSecondary = true;
        canInput = true;
    }

    public IEnumerator reloadArrows()
    {
        //Short duration before the player is able to shoot again. The magically reloading bow takes time to reload.
        yield return new WaitForSeconds(reloadCoolDown);
        playerAudioSource.PlayOneShot(arrowsRefilledSound);
        arrows = maxArrows;
    }

    public IEnumerator ultimateMove()
    {
        //Same as previous coroutines but for ULT. 
        canUlt = false;
        canInput = false;
        playerHealth = maxHealth;
        rb.linearVelocity = Vector2.zero;
        playerAnimator.Play("Player_Ult", 0);
        ultLight.gameObject.SetActive(true);

        //Increase damage during ult
        swordDamage += swordDamageUltIncrease;

        yield return new WaitForSeconds(.333f);

        //Player is able to input after intimidation animation
        canInput = true;

        if(ultimateParticles != null)
            ultimateParticles.Play();
        playChangingPitchSound(ultimateMoveSound);
        playerAnimator.Play("Player_Idle", 0);

        yield return new WaitForSeconds(ultimateDuration);

        ultLight.gameObject.SetActive(false);

        //Removed increase damage when ult is on cooldown
        swordDamage -= swordDamageUltIncrease;

        yield return new WaitForSeconds(ultimateCooldown);
        playerAudioSource.PlayOneShot(ultimateRefreshedSound);
        if (ultimateRefreshedParticles != null)
            ultimateRefreshedParticles.Play();
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
        playerAudioSource.PlayOneShot(dashSound);

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
    //Need to make the collisions between enemies and player last longer to prevent multiple hits.
    private IEnumerator temporaryInvulnerability()
    {
        //Fix to resolve player inputs not being detected
        canSwing = true;
        canSecondary = true;
        canUlt = true;

        //BECAREFUL ABOUT THIS LINE, IF WE CHANGE THE LAYERS THIS WILL BE WRONG
        Physics2D.IgnoreLayerCollision(7, 8, true);
        
        rb.AddForce(-(facingTowards.transform.position - transform.position) * playerHitKnockBack);
        playerAnimator.Play("Player_Hit", 0);
        invincible = true;
        canInput = false;

        yield return new WaitForSeconds(.333f);
        playerAnimator.Play("Player_Idle", 0);
        canInput = true;

        yield return StartCoroutine(flickerSprite());
        invincible = false;

        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

    private IEnumerator flickerSprite()
    {
        for (int i = 0; i < flickerAmount; i++)
        {
            spriteRenderer.color = new Color(255f, 0f, 0f, 255f);
            yield return new WaitForSeconds(flickerDuration);
            spriteRenderer.color = new Color(0f, 0f, 0f, 255f);
            yield return new WaitForSeconds(flickerDuration);
        }
        spriteRenderer.color = Color.white;
    }

    //Used to change the direction of a player during warp
    public void changeFacingDirectionWarp(Warp_Controller.destinationFacingDirection direction)
    {
        switch(direction)
        {
            case Warp_Controller.destinationFacingDirection.Up:
                facingTowards.transform.position = new Vector3(0f, 1f, 0f) + transform.position;
                arrowSpawn.transform.position = new Vector3(0f, .6f, 0f) + transform.position;
                playerAnimator.SetFloat("moveX", 0);
                playerAnimator.SetFloat("moveY", 1);
                swordController.updateHitBox(swingUpOffset, swingUpSize);
                break;
            case Warp_Controller.destinationFacingDirection.Down:
                facingTowards.transform.position = new Vector3(0f, -1f, 0f) + transform.position;
                arrowSpawn.transform.position = new Vector3(0f, -.7f, 0f) + transform.position;
                playerAnimator.SetFloat("moveX", 0);
                playerAnimator.SetFloat("moveY", -1);
                swordController.updateHitBox(swingDownOffset, swingDownSize);
                break;
            case Warp_Controller.destinationFacingDirection.Right:
                facingTowards.transform.position = new Vector3(0f, -1f, 0f) + transform.position;
                arrowSpawn.transform.position = new Vector3(0f, -.7f, 0f) + transform.position;
                playerAnimator.SetFloat("moveX", 0);
                playerAnimator.SetFloat("moveY", -1);
                swordController.updateHitBox(swingDownOffset, swingDownSize);
                break;
            case Warp_Controller.destinationFacingDirection.Left:
                facingTowards.transform.position = new Vector3(-1f, 0f, 0f) + transform.position;
                arrowSpawn.transform.position = new Vector3(-.6f, 0f, 0f) + transform.position;
                playerAnimator.SetFloat("moveX", -1);
                playerAnimator.SetFloat("moveY", 0);
                swordController.updateHitBox(swingLeftOffset, swingLeftSize);
                break;
        }
    }

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
            arrowSpawn.transform.position = new Vector3(0f, -.7f, 0f) + transform.position;
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

    //Sample game win
    private IEnumerator gameWin()
    {
        canInput = false;
        rb.linearVelocity = Vector2.zero;

        //play animation
        playerAnimator.Play("Player_Ult", 0);
        crossFadeIn.SetActive(true);

        yield return new WaitForSeconds(1f);

        //trigger gameover on UI

        //destroy gameobject
        Destroy(gameObject);
    }

    private IEnumerator gameOver()
    {
        canInput = false;
        rb.linearVelocity = Vector2.zero;

        //play animation
        playerAnimator.Play("Player_Hit", 0);
        crossFadeIn.SetActive(true);

        yield return new WaitForSeconds(1f);

        //trigger gameover on UI

        //destroy gameobject
        Destroy(gameObject);
    }

    private IEnumerator loseLife()
    {
        canInput = false;
        invincible = true;

        rb.linearVelocity = Vector2.zero;

        //play animation
        playerAnimator.Play("Player_Hit", 0);
        crossFadeIn.SetActive(true);

        Music_Controller.instance.warpChangeMusic(Warp_Controller.destinationMusic.Tavern);
        yield return new WaitForSeconds(1f);

        //Update the players spawn to the town, move camera after the player has been moved to the same position.
        gameObject.transform.position = respawnPosition.transform.position;
        Camera_Controller.instance.gameObject.transform.position = gameObject.transform.position;

        //If the player was in the dungeon then we have to let the controllers know the player is no longer in the dungeon
        if (Dungeon_Controller.instance.inDungeon || Camera_Controller.instance.inDungeon)
        {
            Dungeon_Controller.instance.inDungeon = false;
            Dungeon_Controller.instance.ResetRoom();
            Camera_Controller.instance.inDungeon = false;
        }

        //Minus the player lives and set maxHealth back to full
        playerLives--;
        playerHealth = maxHealth;

        yield return new WaitForSeconds(.25f);

        //Disable the crossfade to let the player see again and allow them to input.
        crossFadeIn.SetActive(false);
        canInput = true;
        invincible = false;
    }

    private void playChangingPitchSound(AudioClip sound)
    {
        float temp = Random.Range(.9f, 1.1f);
        playerChangingAudioSource.pitch = temp;
        playerChangingAudioSource.PlayOneShot(sound);
    }

    public void TakeDamage(float damage)
    {
        if(invincible)
        {
            return;
        }

        if ((playerHealth - damage) > 0f)
        {
            invincible = true;
            rb.linearVelocity = Vector2.zero;
            playerAudioSource.PlayOneShot(playerTakeDamage);
            StartCoroutine(temporaryInvulnerability());
        }
        else if (((playerHealth - damage) <= 0f) && ((playerLives - 1) <= 0))
        {
            //Game over sequence
            //Update slider
            playerAudioSource.PlayOneShot(playerTakeDamage);
            StartCoroutine(gameOver());
        }
        else
        {
            StartCoroutine(loseLife());
        }
        playerHealth -= damage;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Need to call to update UI when I collide with different things
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
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("OtherPickUp")) //change the name of otherpickup to what the name 
        {
            gold++;

            //Maybe do check in here to see if gold % 5 is true then give player a life

            playerAudioSource.PlayOneShot(goldCollect);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.CompareTag("UnlockSecondary"))
        {
            //Baseline unlock ability can add more to this like lights and what not.
            unlockedSecondaryMove = true;
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.CompareTag("UnlockUlt"))
        {
            //Baseline unlock ability can add more to this like lights and what not.
            unlockedUltMove = true;
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.CompareTag("Win")) //Doesnt have to be like this but you get the idea
        {
            gameWin();
            Destroy(collision.gameObject);
            canInput = false;
        }
    }
}

[System.Serializable]
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
