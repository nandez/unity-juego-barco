using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]

    [Tooltip("Indica la velocidad de movimiento del enemigo..")]
    [SerializeField] protected float speed;

    [Tooltip("Indica la velocidad de rotación del enemigo.")]
    [SerializeField] protected float turnSpeed;

    [Tooltip("Indica los puntos de recompensa que otorga el enemigo al ser destruido..")]
    [SerializeField] protected int rewardPoints;


    [Header("Events")]
    public UnityAction<int> OnEnemyDestroyed; // Evento que se invoca cuando el enemigo es destruido.

    protected HealthController healthCtrl; // Referencia al controlador de vida.
    protected HealthBarController healthBarCtrl; // Referencia al controlador de la barra de vida.
    protected GameObject player; // Referencia al jugador.


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
        player = SceneUtils.FindPlayer(true);

        // Inicializamos las referencias a los componentes que controla los puntos de vida
        // y asignamos los handlers para los eventos.
        healthBarCtrl = GetComponentInChildren<HealthBarController>();
        healthCtrl = GetComponent<HealthController>();
        healthCtrl.OnDeath += OnEnemyDeathHandler;
        healthCtrl.OnHealthUpdated += OnEnemyHealthUpdatedHandler;
    }

    protected virtual void OnEnemyDeathHandler()
    {
        // Cuando el enemigo muere, invocamos el evento OnEnemyDestroyed indicando
        // los puntos de recompensa como parámetro.
        OnEnemyDestroyed?.Invoke(rewardPoints);

        // TODO: animación de "hundimiento" (transform.Translate(Vector3.down * Time.deltaTime * 2f)
        // sonido de explosión??
        Destroy(gameObject);
    }

    protected virtual void OnEnemyHealthUpdatedHandler(int currentHitPoints, int maxHitPoints)
    {
        // Cuando el enemigo recibe daño, actualizamos la barra de vida.
        healthBarCtrl.UpdateHealthBar(currentHitPoints, maxHitPoints);
    }
}
