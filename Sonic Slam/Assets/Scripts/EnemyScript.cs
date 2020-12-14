using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CooldownManager;

public class EnemyScript : MonoBehaviour
{
    #region Variables/etc.
    public AudioClip[] footstepsList;
    public GameObject otherPlayer;
    private AudioSource soundPlayer;
    private PlayerScript otherScript;
    private Rigidbody rb;

    public bool isAttacking;
    public bool canAttack;
    public bool isBlocking;
    public bool isStunned;

    private Vector3 oldPlayerPos;
    private Vector3 newPlayerPos;
    public Transform enemyStart;
    
    private float moveSpeed = 16.0f;
    private float minDist = 10.0f;
    public int enemyHP = 100;

    [Header("Enemy Info"), SerializeField]
    private float attackCooldownOnHit = 0.75f;
    [SerializeField]
    private float hitStunDur = 0.5f;
    [SerializeField]
    private float stunDuration = 0.5f;
    #endregion
    private void Start()
    {
        otherScript = otherPlayer.GetComponent<PlayerScript>();
        oldPlayerPos = transform.position;
        enemyStart = gameObject.transform;
        soundPlayer = gameObject.GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody>();
        enemyStart = transform;
    }
    private void Update()
    {
        enemyMove();
    }
    void enemyMove()
    {
        oldPlayerPos = newPlayerPos;
        transform.LookAt(otherPlayer.transform);
        if (Vector3.Distance(transform.position, otherPlayer.transform.position) >= minDist)
        {
            //transform.position += transform.forward * moveSpeed * Time.deltaTime;
            rb.AddForce(transform.forward * moveSpeed * Time.deltaTime);
        }
        newPlayerPos = transform.position;
        if (newPlayerPos != oldPlayerPos)
        {
            if (!soundPlayer.isPlaying)
            {
                soundPlayer.clip = footstepsList[Random.Range(0, footstepsList.Length)];
                soundPlayer.Play();
            }
        }
    }
    public void hitPlayer()
    {
        enemyHP -= 10;
        isStunned = true;
        Cooldown(stunDuration, () => isStunned = false);
        print(this.gameObject.name + " got hit! HP IS NOW: " + enemyHP);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && isAttacking == true)
        {
            Rigidbody otherRB = otherPlayer.GetComponent<Rigidbody>();
            if (otherScript.isBlocking == false)
            {
                otherScript.hitPlayer();
                isAttacking = false;
                canAttack = false;
                Cooldown(attackCooldownOnHit, () => canAttack = true);
                print(otherRB.velocity + "   -   " + otherRB.angularVelocity);
            }
            else if (otherScript.isBlocking == true)
            {
                otherRB.drag = 100000;
                rb.drag = 100000;
                Cooldown(hitStunDur, () => otherRB.drag = 0.0f);
                Cooldown(hitStunDur, () => rb.drag = 0.0f);
            }
        }
        if(collision.gameObject.tag == "Deadly")
        {
            print("ENEMY DIED! PLAYER WINS!!");
            enemyHP = 0;
            otherScript.opponentDied = true;
            print(otherScript.opponentDied);
            transform.position = enemyStart.position;
            transform.rotation = enemyStart.rotation;
        }
    }
}
