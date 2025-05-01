using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

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
    public bool isPaused = false;
    public bool isTransitioning = false;

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
    public AudioClip healthCollect;
    public AudioClip playerTakeDamage;
    public AudioClip dashSound;
    public AudioClip healingSound;
    public AudioClip extraLifeSound;
    public AudioClip swordSwingSound;
    public AudioClip bowShootSound;
    public AudioClip arrowsRefilledSound;
    public AudioClip ultimateMoveSound;
    public AudioClip ultimateAlmostDone;
    public AudioClip ultimateRefreshedSound;
    public AudioClip unlockedNewAbilitySound;
    public AudioClip fishCollect;

    [Header("Player Particle Systems")]
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
    public GameObject abilityUnlockedLight;
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

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused && !isTransitioning)
        {
            UI_Controller.instance.PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused && !isTransitioning)
        {
            UI_Controller.instance.UnpauseGame();
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
        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.C)) && unlockedUltMove && canUlt && !isMouseOverUI())
        {
            StartCoroutine(ultimateMove());
        }

        //Dash
        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isMouseOverUI())
        {
            if (!(movementDirection.x == 0f && movementDirection.y == 0f))
            {
                StartCoroutine(dash(movementDirection));
            }
        }

        //Sword Swing
        if ((Input.GetButton("Fire1") || Input.GetKeyDown(KeyCode.Z)) && canSwing && !isMouseOverUI())
        {
            StartCoroutine(swing());
        }

        //Secondary
        if ((Input.GetButton("Fire2") || Input.GetKeyDown(KeyCode.X)) && unlockedSecondaryMove && canSecondary && !(arrows <= 0) && !isMouseOverUI())
        {
            StartCoroutine(secondaryMove());
        }

        //Healing Self
        //Need to change key that player uses to heal.
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.V)) && !healingSelf && (healingPotions != 0) && !isMouseOverUI())
        {
            StartCoroutine(healPlayer());
        }
    }

    private bool isMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
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
        playerMovementspeed = playerMovementSpeedUnchanging * .5f;
        playChangingPitchSound(healingSound);

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

        //Return player movespeed back to normal and allow player to use abilities again. Future notice I will need to disable other abilities as well
        yield return new WaitForSeconds(healingSlowdown);

        healingSelf = false;
        UI_Controller.instance.CollectHealth();
        playerMovementspeed = playerMovementSpeedUnchanging;
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

        UI_Controller.instance.ShootArrow();

        //If the player is out of arrows the magical bow will magically reload.
        if (arrows <= 0)
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
        UI_Controller.instance.ShootArrow();
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
        playerAudioSource.PlayOneShot(ultimateMoveSound);

        //Increase damage during ult
        swordDamage += swordDamageUltIncrease;

        yield return new WaitForSeconds(.333f);

        //Player is able to input after intimidation animation
        canInput = true;
      
        playerAnimator.Play("Player_Idle", 0);

        yield return new WaitForSeconds(ultimateDuration - 3f);

        playerAudioSource.PlayOneShot(ultimateAlmostDone);

        yield return new WaitForSeconds(3f);

        ultLight.gameObject.SetActive(false);

        //Removed increase damage when ult is on cooldown
        swordDamage -= swordDamageUltIncrease;

        yield return new WaitForSeconds(ultimateCooldown);
        playerAudioSource.PlayOneShot(ultimateRefreshedSound);

        canUlt = true;
    }

    public IEnumerator dash(Vector2 movement)
    {
        /* 
         * When the player presses space take the direction the player is moving and set the linear velocity of the player to dash
         * 
         * during this dashing, the player cannot input and cannot dash again until a cool down is up
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

    private IEnumerator temporaryInvulnerability()
    {
        //Fix to resolve player inputs not being detected
        //canSwing = true;
        //canSecondary = true;
        //canUlt = true;

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
    [ContextMenu("gameWin")]
    public void test()
    {
        StartCoroutine(gameWin());
    }

    [ContextMenu("gameLose")]
    public void test2()
    {
        StartCoroutine(gameOver());
    }

    public IEnumerator gameWin()
    {
        canInput = false;
        rb.linearVelocity = Vector2.zero;

        //play animation
        playerAnimator.Play("Player_Ult", 0);
        crossFadeIn.SetActive(true);

        Music_Controller.instance.gameCompleteMusic(Music_Controller.instance.gameWinMusic);
        yield return new WaitForSeconds(1.1f);

        crossFadeIn.SetActive(false);

        //trigger gameover on UI
        UI_Controller.instance.GameWin();

        //destroy gameobject
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }

    private IEnumerator gameOver()
    {
        canInput = false;
        invincible = true;

        rb.linearVelocity = Vector2.zero;

        //If the player was in the dungeon then we have to let the controllers know the player is no longer in the dungeon
        if (Dungeon_Controller.instance.inDungeon || Camera_Controller.instance.inDungeon)
        {
            Dungeon_Controller.instance.inDungeon = false;
            Dungeon_Controller.instance.ResetRoom();
            Camera_Controller.instance.inDungeon = false;
        }
        if (Area_Controller.instance.currentArea != null)
        {
            Area_Controller.instance.ResetArea();
        }

        //play animation
        playerAnimator.Play("Player_Hit", 0);
        crossFadeIn.SetActive(true);

        Music_Controller.instance.gameCompleteMusic(Music_Controller.instance.gameOverMusic);
        yield return new WaitForSeconds(1.1f);

        crossFadeIn.SetActive(false);

        //trigger gameover on UI
        UI_Controller.instance.GameOver();

        //destroy gameobject
        //Destroy (gameObject);
        gameObject.SetActive(false);
    }

    private IEnumerator loseLife()
    {
        canInput = false;
        invincible = true;

        rb.linearVelocity = Vector2.zero;

        //play animation
        playerAnimator.Play("Player_Hit", 0);
        crossFadeIn.SetActive(true);

        //THEY ARE GOD GAMERS I BELIEVE
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
        if(Area_Controller.instance.currentArea != null) 
        {
            Area_Controller.instance.ResetArea();
        }

        //Minus the player lives and set maxHealth back to full
        playerLives--;
        playerHealth = maxHealth;

        if(healingPotions < 3)
            healingPotions = 3;

        UI_Controller.instance.CollectHealth();
        UI_Controller.instance.UpdatePlayerLives();

        yield return new WaitForSeconds(1.1f);

        //Disable the crossfade to let the player see again and allow them to input.
        crossFadeIn.SetActive(false);
        canInput = true;
        invincible = false;
    }

    public void playChangingPitchSound(AudioClip sound)
    {
        float temp = Random.Range(.9f, 1.1f);
        playerChangingAudioSource.pitch = temp;
        playerChangingAudioSource.PlayOneShot(sound);
    }

    private IEnumerator unlockedNewAbility(System.Action callback = null)
    {
        canInput = false;
        abilityUnlockedLight.SetActive(true);
        changeFacingDirectionWarp(Warp_Controller.destinationFacingDirection.Down);
        playerAnimator.Play("Player_Ult", 0);
        playerAudioSource.PlayOneShot(unlockedNewAbilitySound);

        yield return new WaitForSeconds(1.5f);

        abilityUnlockedLight.SetActive(false);
        canInput = true;
        callback?.Invoke();
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

    public void giveReward(float maxHealthIncrease, int maxPotionsIncrease, float swordDamageIncrease, float goldReward, int healthPoitionReward)
    {
        maxHealth += maxHealthIncrease;
        maxHealthPotions += maxPotionsIncrease;
        swordDamage += swordDamageIncrease;

        if(goldReward != 0f)
        {
            for (int i = 0; i < goldReward; i++)
            {
                gold++;

                if (gold % 20 == 0 && gold != 0)
                {
                    playerLives++;
                }
            }
        }


        if ((healingPotions + healthPoitionReward) >= maxHealthPotions)
        {
            healingPotions = maxHealthPotions;
        }
        else
        {
            healingPotions += healthPoitionReward;
        }

        UI_Controller.instance.CollectCoin();
        UI_Controller.instance.CollectHealth();
        UI_Controller.instance.UpdatePlayerLives();
        UI_Controller.instance.PopupText("Quest reward received");
        //Can move quest to completed or anything else or remove quest from the player
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

            playerAudioSource.PlayOneShot(healthCollect);
            Destroy(collision.gameObject);

            UI_Controller.instance.CollectHealth();
        }
        else if (collision.gameObject.CompareTag("OtherPickUp"))
        {
            gold++;

            //Maybe do check in here to see if gold % 5 is true then give player a life
            if(gold % 20 == 0 && gold != 0)
            {
                playerLives++;
                playerAudioSource.PlayOneShot(extraLifeSound);
                UI_Controller.instance.UpdatePlayerLives();
            }
            else
            {
                playerAudioSource.PlayOneShot(goldCollect);
            }

            Destroy(collision.gameObject);

            UI_Controller.instance.CollectCoin();
        }
        else if (collision.gameObject.CompareTag("FishPickUp"))
        {
            playerHealth = maxHealth;
            playerAudioSource.PlayOneShot(fishCollect);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("DiamondPickUp"))
        {
            //Bad programming because I am a god level programmer
            bool extraLife = false;

            for(int i = 0; i < 5; i++)
            {
                gold++;

                if (gold % 20 == 0 && gold != 0)
                {
                    playerLives++;
                    playerAudioSource.PlayOneShot(extraLifeSound);
                    UI_Controller.instance.UpdatePlayerLives();
                    UI_Controller.instance.PopupText("Extra life gained");
                    extraLife = true;
                }
            }

            if(!extraLife)
                playerAudioSource.PlayOneShot(goldCollect);

            //------------------------------------

            Destroy(collision.gameObject);

            UI_Controller.instance.CollectCoin();
        }
        else if(collision.gameObject.CompareTag("UnlockSecondary"))
        {
            unlockedSecondaryMove = true;
            AreaLock_Controller.instance.unlockSecondaryNeededAreas();
            UI_Controller.instance.ArrowText.gameObject.SetActive(true);
            UI_Controller.instance.ArrowImage.gameObject.SetActive(true);
            UI_Controller.instance.ShootArrow();
            Destroy(collision.gameObject);
            StartCoroutine(unlockedNewAbility(() =>
            {
                // Trigger cutscene after animation finishes.
                // Game_Progress_Controller.instance.StartCH2();
            }));

            quests[1].isComplete = true;
            quests.Add(new Quest("Lonesome Road", "You have looted the catacombs in the sewers and now have proven your worth. Find and clear out dungeons on your way to Kharon's Hollow, the Holy Grail of crypts. You should start by travelling to \"The Bleak\", a forest northwest of town", false));
            UI_Controller.instance.questIndex = Player_Controller.instance.quests.Count - 1;
            UI_Controller.instance.ActiveQuest();
        }
        else if(collision.gameObject.CompareTag("UnlockUlt"))
        {
            unlockedUltMove = true;
            AreaLock_Controller.instance.unlockUltimateNeededAreas();
            Destroy(collision.gameObject);
            StartCoroutine(unlockedNewAbility(() =>
            {
                // Trigger cutscene after animation finishes.
                // Game_Progress_Controller.instance.StartCH3();
            }));

            quests[2].isComplete = true;
            quests.Add(new Quest("Heavy is The Crown", "The dead king's time has come. Head to Kharon's Hollow... and defile it. Steal what ought to be yours", false));
            UI_Controller.instance.questIndex = Player_Controller.instance.quests.Count - 1;
            UI_Controller.instance.ActiveQuest();
        }
        else if(collision.gameObject.CompareTag("Win"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(unlockedNewAbility(() =>
            {
                // Trigger cutscene after animation finishes.
                Game_Progress_Controller.instance.StartOutro();
            }));
        }
    }
}

[System.Serializable]
public class Quest
{
    public int questID;
    public string questTitle;
    public string questDescription;
    public bool isComplete;

    public float maxHealthIncrease;
    public int maxPotionsIncrease;
    public float swordDamageIncrease;
    public float goldReward;
    public int healthPoitionReward;

    public Quest(string questTitle, string questDescription, bool isComplete)
    {
        this.questTitle = questTitle;
        this.questDescription = questDescription;
        this.isComplete = isComplete;
    }

    public override string ToString()
    {
        return $"{questTitle}, {questDescription}, ({(isComplete ? "Complete" : "Incomplete")})";
    }
}
