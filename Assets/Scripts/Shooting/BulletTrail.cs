using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{

    public float speed = 10f;

    private void Start()
    {
        Destroy(gameObject, 3);
    }
    void Update()
    {
        Vector3 move = new Vector3(0, 0, speed * Time.deltaTime);
        transform.Translate(move);
    }
}
