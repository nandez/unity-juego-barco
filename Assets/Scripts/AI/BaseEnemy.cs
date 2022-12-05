using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Tooltip("Indica los puntos de vida iniciales del enemigo.")]
    [SerializeField] protected int hitPoints;

    [Tooltip("Indica la velocidad de movimiento del enemigo..")]
    [SerializeField] protected float speed;

    [Tooltip("Indica la velocidad de rotación del enemigo.")]
    [SerializeField] protected float turnSpeed;


    protected GameObject player; // Referencia al jugador.
    protected int currentHitPoints; // Indica la vida actual del enemigo.

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
        // Buscamos al jugador por su tag.
        player = GameObject.FindObjectOfType<PlayerMovementController>()?.gameObject;

        // Si no lo encontramos, tiramos un error..
        if (player == null)
            throw new System.Exception($"No se encontró al jugador en la escena.");

        // Inicializamos los puntos de vida.
        currentHitPoints = hitPoints;
    }
}
