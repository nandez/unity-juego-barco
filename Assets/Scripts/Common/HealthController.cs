using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [Tooltip("Indica los puntos de vida iniciales del enemigo.")]
    [SerializeField] protected int maxHitpoints;
    public int GetMaxHitpoints() => maxHitpoints;

    protected int hitPoints; // Indica los puntos de vida actuales.
    public int GetHitpoints() => hitPoints;

    [Tooltip("Indica si el objeto debe ser destruido al morir.")]
    [SerializeField] protected bool destroyOnDeath = false;

    [Header("Events")]
    public UnityAction<int, int> OnHealthUpdated;
    public UnityAction OnDeath;

    void Start()
    {
        hitPoints = maxHitpoints;

        // Llamamos a la función de actualización de vida con un delay de 0.5 segundos para permitir
        // a los scripts que se suscriban al evento de actualización de vida, obtener los valores.
        Invoke(nameof(UpdateHealth), 0.25f);
    }

    protected virtual void UpdateHealth()
    {
        OnHealthUpdated?.Invoke(hitPoints, maxHitpoints);
    }

    public virtual void TakeDamage(int damage, GameObject attacker)
    {
        hitPoints -= damage;
        damage = Mathf.Clamp(damage, 0, maxHitpoints);

        UpdateHealth();

        if (hitPoints <= 0)
        {
            OnDeath?.Invoke();

            // Verificamos si debemos destruir el objeto al morir, o si se contempla
            // en otro script.
            if (destroyOnDeath)
                Destroy(gameObject);
        }
    }
}
