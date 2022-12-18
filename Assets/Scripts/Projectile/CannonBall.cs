using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] protected float lifeTime;
    [SerializeField] protected int damage;

    public GameObject Owner { get; protected set; }

    void Start()
    {
        // Destruimos el objeto una vez cumplido el tiempo de vida.
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision coll)
    {
        // Si el objeto colisionado tiene el componente HealthController, le aplicamos daño.
        if (coll.gameObject.TryGetComponent<HealthController>(out var healthCtrl))
            healthCtrl.TakeDamage(damage, Owner);


        // Destruimos el objeto.
        Destroy(gameObject);
    }

    public static CannonBall Instantiate(CannonBall prefab, Vector3 position, Quaternion rotation, GameObject owner)
    {
        var cannonBall = Instantiate(prefab, position, rotation).GetComponent<CannonBall>();
        cannonBall.Owner = owner;
        return cannonBall;
    }
}
