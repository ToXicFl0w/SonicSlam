using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CooldownManager;

public class GameTimer : MonoBehaviour
{
    #region Variables/etc.
    public AudioClip[] bellSounds;
    private AudioSource dingBell;
    public AudioClip EndOfGameSound;

    public GameObject player1;
    public GameObject player2;
    private PlayerScript script1;
    private PlayerScript script2;
    private float hp1;
    private float hp2;

    private float second = 1.0f;

    [Header("Game Info"), SerializeField]
    private int gameLength = 120; // Game Length in seconds
    [SerializeField]
    private int currentTime = 0; // Current Game time in seconds.
    #endregion
    void Start()
    {
        dingBell = gameObject.GetComponent<AudioSource>();
        addTime();
        script1 = player1.GetComponent<PlayerScript>();
        script2 = player2.GetComponent<PlayerScript>();
    }
    void Update()
    {
        bellDing();
        endOfTimer();
    }
    private void addTime()
    {
        Cooldown(second,() => addTime());
        currentTime += 1;
    }
    private void bellDing()
    {
        if ((currentTime == 30 || currentTime == 60 || currentTime == 90) && !dingBell.isPlaying)
        {
            dingBell.clip = bellSounds[Random.Range(0, bellSounds.Length)];
            dingBell.Play();
            Cooldown(0.5f, () => dingBell.Stop());
        }
        if ((gameLength - currentTime == 10) && !dingBell.isPlaying)
        {
            dingBell.clip = EndOfGameSound;
            dingBell.Play();
            dingBell.loop = true;
        }
        if ((gameLength - currentTime == 0))
        {
            dingBell.Stop();
        }
    }
    private void endOfTimer()
    {
        if(gameLength - currentTime <= 0)
        {
            hp1 = script1.playerHP;
            hp2 = script2.playerHP;
            if (hp1 > hp2) script2.playerHP = 0;
            if (hp1 < hp2) script1.playerHP = 0;
            if (hp1 == hp2) print("ITS A TIE!!! WHAT?!");
        }
    }
}
