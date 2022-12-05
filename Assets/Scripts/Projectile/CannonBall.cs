using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] protected float lifeTime;

    void Start()
    {
        // Destruimos el objeto una vez cumplido el tiempo de vida.
        Destroy(gameObject, lifeTime);
    }
}
