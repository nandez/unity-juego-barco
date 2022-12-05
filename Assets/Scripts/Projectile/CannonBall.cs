using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] protected float lifeTime;

    public int damage;

    void Start()
    {
        // Destruimos el objeto una vez cumplido el tiempo de vida.
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision coll)
    {
        // Si el objeto colisionado tiene el componente IDamageable, le aplicamos daño.
        if (coll.gameObject.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(damage);

        // Destruimos el objeto.
        Destroy(gameObject);
    }
}
