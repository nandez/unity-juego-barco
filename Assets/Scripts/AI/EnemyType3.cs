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
        var direction = transform.position - player.transform.position;
        direction.y = 0;

        Move(direction, 1f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, fleeRange);
    }
}
