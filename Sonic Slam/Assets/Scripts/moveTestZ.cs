using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class moveTestZ : MonoBehaviour
{

    public float speed = 2.5f;
    float startZ;
    public float distance = 5;
    float addZ;

    void Start()
    {
        startZ = transform.position.z;
    }

    void Update()
    {
        addZ = Mathf.PingPong(Time.time * speed, distance);
        transform.position = new Vector3(transform.position.x, transform.position.y, startZ + addZ );
    }
}
