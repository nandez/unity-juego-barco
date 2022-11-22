using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este tipo de barco reacciona y entra en modo combate cuando la distancia entre el
/// barco y el jugador sea menor a un rango predefinido.
///
/// Mientras que el jugador permanezca en dicho rango, el enemigo intentará hundirlo.
/// Si el jugador decide huir del combate, el enemigo lo perseguirá hasta que el mismo
/// esté fuera del rango predefinido.
///
/// Si su HP se reduce al 25%, este tipo de barco emprenderá la huida del combate
/// </summary>
public class EnemyType2 : EnemyBaseController
{
    [SerializeField] protected float attackRange; // Indica la distancia a la que el enemigo ataca.
    [SerializeField] protected float chaseRange; // Indica el radio de distancia en la que el jugador es perseguido.

    protected bool isFleeing; // Indica si el enemigo está huyendo del combate.

    // Update is called once per frame
    void Update()
    {
        // Determinamos si el barco se encuentra huyendo.
        isFleeing = hitPoints <= maxHitPoints * 0.25f;

        if (player != null)
        {
            // Calculamos la distancia entre el enemigo y el jugador.
            var distance = Vector3.Distance(transform.position, player.position);

            // Si la distancia es menor a la distancia de ataque y no estamos huyendo atacamos.
            if (!isFleeing && distance < attackRange)
            {
                Attack();
            }
            else if (!isFleeing && distance < chaseRange)
            {
                // Si no estamos huyendo, nos movemos hacia el jugador.
                ChasePlayer();
            }
            else if (isFleeing)
            {
                // En este caso, estamos huyendo del combate.
                FleeFromPlayer();
            }
        }
    }

    void Attack()
    {
        // TODO: implementar el ataque...
    }

    void ChasePlayer()
    {
        // Calculamos el vector dirección entre el enemigo y el jugador.
        var direction = player.position - transform.position;

        // Rotamos en dirección al jugador
        var rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);

        // Movemos al enemigo en esa dirección.
        transform.position += direction.normalized * speed * Time.deltaTime;

        // TODO: tener en cuenta que pueden existir obstaculos en el camino;
        // proyectar con un raycast si existe colisión con algun obstaculo
        // antes de realizar el movimiento.
    }

    void FleeFromPlayer()
    {
        // Calculamos el vector dirección entre el enemigo y el jugador.
        var direction = (player.position - transform.position) * -1;

        // Rotamos en dirección al jugador
        var rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);

        // Movemos al enemigo en esa dirección.
        transform.position += direction.normalized * speed * Time.deltaTime;

        // TODO: tener en cuenta que pueden existir obstaculos en el camino;
        // proyectar con un raycast si existe colisión con algun obstaculo
        // antes de realizar el movimiento.
    }

    void OnDrawGizmos()
    {
        // Dibujamos el rango de ataque..
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Dibujamos el rango de persecución.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
