using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManagement : MonoBehaviour
{
    private AudioSource boxSound;
    public AudioClip[] soundsList;

    private Rigidbody rb;

    private Vector3 oldBoxPos;
    private Vector3 newBoxPos;


    // Start is called before the first frame update
    void Start()
    {
        boxSound = gameObject.GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody>();
        oldBoxPos = rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        playAudio();
    }

    private void playAudio()
    {
        oldBoxPos = newBoxPos;
        newBoxPos = rb.position;

        if(oldBoxPos != newBoxPos)
        {
            if (!boxSound.isPlaying)
            {
                boxSound.clip = soundsList[Random.Range(0, soundsList.Length)];
                boxSound.Play();
            }
        }

    }
}
