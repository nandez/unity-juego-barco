using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour
{
    [Tooltip("Indica los puntos de vida iniciales del enemigo.")]
    [SerializeField] protected int maxHitPoints;

    [Tooltip("Indica la velocidad de movimiento del enemigo..")]
    [SerializeField] protected float speed;

    [Tooltip("Indica la velocidad de rotación del enemigo.")]
    [SerializeField] protected float turnSpeed;

    [Tooltip("Indica el tag que se le asigna al jugador.")]
    [SerializeField] protected string playerTag;


    protected Transform player; // Referencia al jugador.
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
        // Buscamos al jugador por su tag.
        player = GameObject.FindGameObjectWithTag(playerTag).transform;

        // Si no lo encontramos, tiramos un error..
        if (player == null)
            throw new System.Exception($"Falta asignar el tag {playerTag} al jugador!");

        // Inicializamos los puntos de vida.
        hitPoints = maxHitPoints;
    }
}
