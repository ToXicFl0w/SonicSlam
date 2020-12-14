using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallImpact : MonoBehaviour
{
    private AudioSource hitSound;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        hitSound = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hit a wall at XYZ:: " + rb.position);
        if (other.gameObject.tag == "Player")
        {
            if (!hitSound.isPlaying) hitSound.Play();
        }
    }
}
