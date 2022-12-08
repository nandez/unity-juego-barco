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
public class EnemyType2 : BaseEnemy
{
    [Header("Attack Settings")]
    [SerializeField] protected float attackRange = 15f; //Indica la distancia a la que el enemigo ataca.
    [SerializeField] protected float chaseRange = 25f; // Indica el radio de distancia en la que el jugador es perseguido.
    [SerializeField] protected float minDistanceToPlayer = 6f; // Indica el radio de distancia mínimo al que puede aproximarse al jugador.


    [Header("Cannon Settings")]
    [SerializeField] protected CannonController cannonCtrl; // TODO: podria ser una lista dependiendo de si tiene múltiples cañones..
    [SerializeField] protected List<CannonBall> cannonBallPrefabs; // TODO: representa el tipo de proyectil que dispara el cañón.. podría variar si tiene un powerup.


    // Private fields
    private Vector3 target; // Indica la posición de disparo del proyectil..
    private CannonBall currentCannonBall; // Indica el prefab del proyectil que se está usando para disparar..
    private bool isFleeing; // Indica si el enemigo está huyendo del combate.



    protected override void Initialize()
    {
        // Llamamos al método de la clase base para heredar su comportamiento.
        base.Initialize();

        // Inicializamos el prefab del proyectil que se usará para disparar..
        if (cannonBallPrefabs?.Count > 0)
            currentCannonBall = cannonBallPrefabs[0];
    }

    // Update is called once per frame
    void Update()
    {
        // Determinamos si el barco se encuentra huyendo.
        isFleeing = healthCtrl.GetHitpoints() <= healthCtrl.GetMaxHitpoints() * 0.25f;

        if (player != null)
        {
            // Calculamos la distancia entre el enemigo y el jugador.
            var distance = Vector3.Distance(transform.position, player.transform.position);

            // Si la distancia es menor a la distancia de ataque y no estamos huyendo atacamos.
            if (!isFleeing && distance < attackRange)
            {
                Attack(distance);
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

    void Attack(float distance)
    {
        if (distance > minDistanceToPlayer)
        {
            // Mientras la distancia entre el jugador y el enemigo sea mayor que minDistanceToPlayer,
            // reducimos la velocidad de movimiento y de rotación, y nos movemos en dirección al jugador.
            var newDirection = (player.transform.position - transform.position);
            var newRotation = Quaternion.LookRotation(newDirection);

            // El factor de reducción de velocidad lo calculamos en base a la distancia desde el jugador
            var reductionSpeedFactor = Mathf.Lerp(0f, 1f, (newDirection.magnitude - minDistanceToPlayer) / (attackRange - minDistanceToPlayer));

            // Si la distancia es menor que 0.001f, no nos movemos y evitamos
            // valores extremadamente pequeños que pueden ocasioanr cosas raras...
            if (reductionSpeedFactor < 0.001f)
                reductionSpeedFactor = 0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * reductionSpeedFactor * Time.deltaTime);
            transform.position += newDirection.normalized * speed * reductionSpeedFactor * Time.deltaTime;
        }

        // Determinamos el punto donde queremos que el proyectil impacte.
        var spreadArea = Random.insideUnitCircle;
        target = player.transform.position + (player.transform.forward * Random.Range(0, 3f)) + new Vector3(spreadArea.x, player.transform.position.y, spreadArea.y);

        // Llamamos al método Fire del cañon para disparar el proyectil cuando esté listo.
        cannonCtrl.SetTarget(currentCannonBall, target);
    }

    void ChasePlayer()
    {
        // Calculamos el vector dirección entre el enemigo y el jugador.
        var direction = player.transform.position - transform.position;

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
        var direction = (player.transform.position - transform.position) * -1;

        // Rotamos en dirección opuesta jugador
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
