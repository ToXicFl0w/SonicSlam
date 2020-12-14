using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class moveTest : MonoBehaviour
{

    public float speed = 2.5f;
    float startX;
    public float distance = 5;
    float addX;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        addX = Mathf.PingPong(Time.time * speed, distance);
        transform.position = new Vector3(startX + addX, transform.position.y, transform.position.z);
    }
}
