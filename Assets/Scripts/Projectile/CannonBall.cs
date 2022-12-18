using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] protected float lifeTime;
    [SerializeField] protected int damage;
    [SerializeField] protected string waterTag;
    [SerializeField] protected LayerMask waterLayer;

    public GameObject Owner { get; protected set; }

    void Start()
    {
        // Destruimos el objeto una vez cumplido el tiempo de vida.
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision col)
    {
        // Si el objeto colisionado tiene el componente HealthController, le aplicamos daño.
        if (col.gameObject.TryGetComponent<HealthController>(out var healthCtrl))
            healthCtrl.TakeDamage(damage, Owner);


        // TODO: Puede ser interesante que la bala de cañon explote al impactar.
        Destroy(gameObject);

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(waterTag))
        {
            // Si colisiona con el agua, le quitamos la velocidad para
            // que no siga avanzando con la misma trayectoria, en su lugar dejamos que la gravedad
            // simule que la bala de cañon se hunde en el mar.
            GetComponent<Rigidbody>().velocity = Vector3.zero;

        }
    }

    public void SetDamageMultiplier(float multiplier)
    {
        damage *= (int)multiplier;
    }

    public static CannonBall Instantiate(CannonBall prefab, Vector3 position, Quaternion rotation, GameObject owner)
    {
        var cannonBall = Instantiate(prefab, position, rotation).GetComponent<CannonBall>();
        cannonBall.Owner = owner;
        return cannonBall;
    }
}
