using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este tipo de enemigo se encuentra en constante búsqueda del
/// jugador e intentará hundirlo a toda costa.
/// </summary>
public class EnemyType1 : EnemyBaseController
{
    [SerializeField] protected float attackRange; // Indica la distancia a la que el enemigo ataca.


    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // Calculamos la distancia entre el enemigo y el jugador.
            var distance = Vector3.Distance(transform.position, player.position);

            // Si la distancia es menor a la distancia de ataque, atacamos.
            if (distance < attackRange)
            {
                Attack();
            }
            else
            {
                // Si no, nos movemos hacia el jugador.
                ChasePlayer();
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

    void OnDrawGizmos()
    {
        // Dibujamos el rango de ataque..
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
