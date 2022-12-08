using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] protected float lifeTime;
    [SerializeField] protected int damage;

    void Start()
    {
        // Destruimos el objeto una vez cumplido el tiempo de vida.
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision coll)
    {
        // Si el objeto colisionado tiene el componente HealthController, le aplicamos daño.
        if (coll.gameObject.TryGetComponent<HealthController>(out var healthCtrl))
            healthCtrl.TakeDamage(damage);


        // Destruimos el objeto.
        Destroy(gameObject);
    }
}
