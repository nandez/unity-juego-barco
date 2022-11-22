using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour
{
    [SerializeField] protected int maxHitPoints; // Indica los puntos de vida iniciales del enemigo.
    [SerializeField] protected float speed; // Indica la velocidad de movimiento del enemigo.
    [SerializeField] protected float turnSpeed; // Indica la velocidad de rotación del enemigo.
    [SerializeField] protected string playerTag; // Indica el tag que se le asigna al jugador.


    protected Transform player; // Referencia al jugador.
    [SerializeField] protected int hitPoints; // Indica la vida actual del enemigo.

    void Start()
    {
        Initialize();
    }

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
