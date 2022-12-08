using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Al contrario que los barcos de Tipo 2, este comportamiento funciona a la inversa; cuando el
/// jugador entra en un rango predefinido, el barco enemigo intentará huir del jugador.
/// </summary>
public class EnemyType3 : BaseEnemy
{
    [SerializeField] protected float fleeRange; // Indica la distancia a la que el enemigo ataca.


    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // Calculamos la distancia entre el enemigo y el jugador.
            var distance = Vector3.Distance(transform.position, player.transform.position);

            // Si la distancia es menor a la distancia de ataque, atacamos.
            if (distance < fleeRange)
            {
                FleeFromPlayer();
            }
        }
    }

    void FleeFromPlayer()
    {
        // Calculamos el vector dirección entre el enemigo y el jugador.
        var direction = (player.transform.position - transform.position) * -1;

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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, fleeRange);
    }
}
