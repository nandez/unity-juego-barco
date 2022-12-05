using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    [Tooltip("Indica los puntos de vida iniciales del enemigo.")]
    [SerializeField] protected int maxHitpoints;

    [Tooltip("Indica la velocidad de movimiento del enemigo..")]
    [SerializeField] protected float speed;

    [Tooltip("Indica la velocidad de rotación del enemigo.")]
    [SerializeField] protected float turnSpeed;

    [Tooltip("Indica los puntos de recompensa que otorga el enemigo al ser destruido..")]
    [SerializeField] protected int rewardPoints;

    public UnityAction<int> OnDeath;

    protected HealthBarController healthBarCtrl; // Referencia al controlador de la barra de vida.
    protected GameObject player; // Referencia al jugador.
    protected int hitPoints; // Indica la vida actual del enemigo.

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Metodo que inicializa las variables del enemigo.
    /// Puede sobreescribirse en las clases derivadas.
    /// </summary>
    protected virtual void Initialize()
    {
        // Buscamos al jugador en la escena; si no lo encontramos, tiramos un error..
        player = GameObject.FindObjectOfType<PlayerMovementController>()?.gameObject;
        if (player == null)
            throw new System.Exception($"No se encontró al jugador en la escena.");

        // Inicializamos las referencias a los componentes.
        healthBarCtrl = GetComponentInChildren<HealthBarController>();

        // Inicializamos los puntos de vida.
        hitPoints = maxHitpoints;
        healthBarCtrl.UpdateHealthBar(hitPoints, maxHitpoints);
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        damage = Mathf.Clamp(damage, 0, maxHitpoints);

        if(hitPoints <= 0)
        {
            OnDeath?.Invoke(rewardPoints);
            Destroy(gameObject);
        }
        else
        {
            healthBarCtrl.UpdateHealthBar(hitPoints, maxHitpoints);
        }
    }
}
