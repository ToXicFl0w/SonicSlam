using JetBrains.Annotations;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using static CooldownManager;

public class PlayerScript : MonoBehaviour
{
    #region Variables/etc.
    public AudioClip[] footstepsList;
    private AudioSource soundPlayer;
    public AudioClip gruntSound;
    public AudioClip powerupSound;
    public AudioClip powerdownSound;
    private SceneManager sceneManager;
    private string currentScene;
    private Rigidbody rb => GetComponent<Rigidbody>();
    public GameObject otherPlayer;
    public GameObject canvas;
    private EnemyScript otherScript;
    public float playerHP = 100.0f;

    public bool getInput = true;
    public bool isStunned = false;
    public bool isAttacking = false;
    public bool canAttack = true;
    public bool opponentDied = false;

    private float startSpeed;
    private float inputH;
    private float inputV;
    private float mouseSens = 2.0f;
    private float rotateYaw = 0.0f;
    private float drag = 0.9f;
    private bool makeFootsteps = true;

    [Header("Player Info"), SerializeField]
    private float playerSpeed = 6.0f;
    [SerializeField]
    private float speedIncrease = 2.0f;
    [SerializeField]
    private float launchDuration = 0.7f;
    [SerializeField]
    private float maxLaunchSpeed = 50.0f;
    [SerializeField]
    private float mouseHoldTime;
    [SerializeField]
    private float stunDuration = 0.5f;
    [SerializeField]
    private float attackCooldownOnHit = 0.75f;
    [SerializeField]
    private float hitStunDur = 0.5f;
    [SerializeField]
    private float powerUpLength = 7.5f;
    [SerializeField]
    private float speedUpgradeSpeed = 24.0f;
    [SerializeField]
    public bool isBlocking = false;
    public bool canActuallyMove = true;

    private Vector3 oldPlayerPos;
    private Vector3 newPlayerPos;

    private Transform playerStart;
    #endregion
    private void Start() {
        soundPlayer = gameObject.GetComponent<AudioSource>();
        startSpeed = playerSpeed;
        oldPlayerPos = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
        playerStart = gameObject.transform;
        otherScript = otherPlayer.GetComponent<EnemyScript>();
        opponentDied = false;
    }
    void Update()
    {
        currentScene = SceneManager.GetActiveScene().name;
        playerMove();
        checkDeath();
    }
    public void hitPlayer()
    {
        playerHP -= 10.0f;
        isStunned = true;
        Cooldown(stunDuration, () => isStunned = false);
        print(gameObject.name + " got hit! HP IS NOW: " + playerHP);
    }
    private void playerMove()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (canvas.activeSelf)
            {
                canvas.SetActive(false);
                print("DISABLE CANVAS!");
            }
            if (!canvas.gameObject.activeSelf)
            {
                canvas.SetActive(true);
                print("DISABLE CANVAS!");
            }
        }
        if (!isStunned)
        {
            oldPlayerPos = newPlayerPos;

            if (Input.GetMouseButton(0))
            {
                mouseHoldTime += Time.deltaTime;
                playerSpeed *= 0.3f;
            }
            if (Input.GetMouseButton(1))
            {
                isBlocking = true;
                playerSpeed *= 0.2f;
            }

            Vector3 additiveForce = Vector3.zero;

            if (inputV != 0 && inputH != 0)
            {
                playerSpeed *= 0.5f;
            }

            if (Input.GetMouseButtonUp(0) && canAttack)
            {
                mouseHoldTime = Mathf.Max(mouseHoldTime - 0.1f, 0);
                float forceSpeed = 1.0f;
                forceSpeed += mouseHoldTime * speedIncrease * 100.0f;
                forceSpeed = Mathf.Min(forceSpeed, maxLaunchSpeed);
                additiveForce = transform.forward * forceSpeed;
                print("Apply force: " + additiveForce + " mouseHoldTime: " + mouseHoldTime);

                mouseHoldTime = 0.0f;
                getInput = false;
                Cooldown(launchDuration, () => getInput = true);
                isAttacking = true;
                Cooldown(launchDuration + 0.1f, () => isAttacking = false);
            }

            if (getInput && !isStunned)
            {
                inputV = Input.GetAxis("Vertical");
                inputH = Input.GetAxis("Horizontal");
            }

            Vector3 force = (transform.forward * inputV + transform.right * inputH) * playerSpeed;
            rb.AddForce(force + additiveForce);
            rb.velocity *= drag;

            playerSpeed = startSpeed;

            rotateYaw += mouseSens * Input.GetAxis("Mouse X");
            transform.eulerAngles = new Vector3(0, rotateYaw, 0);
            isBlocking = false;

            newPlayerPos = transform.position;

            if (newPlayerPos != oldPlayerPos)
            {
                if (!soundPlayer.isPlaying && makeFootsteps)
                {
                    print("INSERT FOOTSTEP SOUND");
                    soundPlayer.clip = footstepsList[Random.Range(0, footstepsList.Length)];
                    soundPlayer.Play();
                }
            }
        }
    }
    private void checkDeath()
    {
        if (playerHP <= 0)
        {
            reloadScene();
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("DefeatScreen");
        }
        if (opponentDied)
        {
            reloadScene();
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("ScreenVictory");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy" && isAttacking == true)
        {
            Rigidbody otherRB = otherPlayer.GetComponent<Rigidbody>();
            if (otherScript.isBlocking == false)
            {
                otherScript.hitPlayer();
                isAttacking = false;
                canAttack = false;
                Cooldown(attackCooldownOnHit, () => canAttack = true);
                print(otherRB.velocity + "   -   " + otherRB.angularVelocity);

                if(otherScript.enemyHP <= 0)
                {
                    opponentDied = true;
                }
            }
            else if(otherScript.isBlocking == true)
            {
                otherRB.drag = 100000;
                rb.drag = 100000;
                Cooldown(hitStunDur, () => otherRB.drag = 0.0f);
                Cooldown(hitStunDur, () => rb.drag = 0.0f);
            }
        }
        if(collision.gameObject.tag == "Enemy" && isAttacking == false && !soundPlayer.isPlaying)
        {
            soundPlayer.clip = gruntSound;
            soundPlayer.Play();
            Cooldown(1.0f, () => soundPlayer.Stop());
        }
        if(collision.gameObject.tag == "Deadly")
        {
            playerHP = 0;
        }
        if(collision.gameObject.tag == "SpeedUpgrade")
        {
            startSpeed = speedUpgradeSpeed;
            playerSpeed = speedUpgradeSpeed;
            Cooldown(powerUpLength, () => startSpeed = 16.0f);
            soundPlayer.clip = powerupSound;
            soundPlayer.Play();
            Destroy(collision.gameObject);
            Cooldown(powerUpLength, () => soundPlayer.clip = powerdownSound);
            Cooldown(powerUpLength, () => soundPlayer.Play());
        }
        if(collision.gameObject.tag == "SilenceUpgrade")
        {
            makeFootsteps = false;
            Cooldown(powerUpLength, () => makeFootsteps = true);
            soundPlayer.clip = powerupSound;
            soundPlayer.Play();
            Destroy(collision.gameObject);
            Cooldown(powerUpLength, () => soundPlayer.clip = powerdownSound);
            Cooldown(powerUpLength, () => soundPlayer.Play());
        }
    }
    private void reloadScene()
    {
        this.gameObject.transform.position = playerStart.position;
        this.gameObject.transform.rotation = playerStart.rotation;
        playerHP = 100;
    }
}