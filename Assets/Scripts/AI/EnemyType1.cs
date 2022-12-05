using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este tipo de enemigo se encuentra en constante búsqueda del
/// jugador e intentará hundirlo a toda costa.
/// </summary>
[RequireComponent(typeof(ProjectileController))]
public class EnemyType1 : BaseEnemy
{
    [Header("Attack Settings")]
    [SerializeField] protected float attackRange = 5f; //Indica la distancia a la que el enemigo ataca.
    [SerializeField] protected float attackCooldown = 5f; //Indica el tiempo de espera entre disparo y disparo.

    [Header("Cannon Settings")]
    [SerializeField] protected GameObject cannonBallPrefab;
    [SerializeField] protected Transform leftCannonSpawnPoint;
    [SerializeField] protected Transform rightCannonSpawnPoint;

    private float attackTimer = 0;
    protected ProjectileController projectileCtrl;
    private Vector3 landPoint = Vector3.zero;

    protected override void Initialize()
    {
        // Llamamos al método de la clase base para heredar su comportamiento.
        base.Initialize();

        // Cargamos las referencias a los componentes propios..
        projectileCtrl = GetComponent<ProjectileController>();
    }

    private void Update()
    {
        if (player != null)
        {
            // Calculamos la distancia entre el enemigo y el jugador y verificamos
            // el rango de ataque para determinar si atacar o perseguir al jugador..
            var distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < attackRange)
                Attack();
            else
                ChasePlayer();
        }

        // Actualizamos el cooldown de ataque.
        attackTimer -= Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            hitPoints -= 5;
            healthBarCtrl.UpdateHealthBar(hitPoints, maxHitpoints);
        }
    }

    private void Attack()
    {
        /*
            Cuando atacamos al jugador, debemos posicionarnos hacia un costado del mismo para poder
            efectuar el disparo. Para ello, calculamos el ángulo entre el enemigo y el jugador y dependiendo del signo
            del mismo, podemos determinar desde que "cuadrante" nos estamos posicionando para atacar.

            Finalmente, calculamos la posición a la cual debemos rotar para poder efectuar el disparo en base
            a la siguiente tabla:

            |---------------------|-------------|----------------|
            | Origen              | Destino     | Rango Angulo   |
            |---------------------|-------------|----------------|
            | Cuadrante 1         | Derecha     |    0  a   90   |
            | Cuadrante 2         | Derecha     |   90  a  180   |
            | Cuadrante 3         | Izquierda   | -180  a  -90   |
            | Cuadrante 4         | Izquierda   |  -90  a    0   |
            |---------------------|-------------|----------------|
        */

        // Calculamos el ángulo (con signo) entre el enemigo y el jugador..
        var dirAngle = Vector3.SignedAngle(
            new Vector3(player.transform.forward.x, 0, player.transform.forward.z),
            (transform.position - player.transform.position),
            Vector3.up
        );

        // Determinamos a que lado del jugador nos posicionamos en base al cuadrante en el que nos encontramos,
        // utilizando el vector "right" del jugador para determinar el lado y asegurándonos de estar dentro del radio
        // de ataque.
        var targetPosition = dirAngle >= 0 && dirAngle < 180
            ? player.transform.position + (player.transform.right * attackRange * 0.9f)
            : player.transform.position + (-player.transform.right * attackRange * 0.9f);

        // Si aún no hemos llegado al punto de ataque, nos movemos hacia el mismo.
        if (Vector3.Distance(transform.position, targetPosition) > 0.25f)
        {
            // Calculamos la dirección hacia el nuevo objetivo y rotamos en su dirección
            var newDirection = targetPosition - transform.position;
            var newRotation = Quaternion.LookRotation(newDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * Time.deltaTime);
            transform.position += newDirection.normalized * speed * Time.deltaTime;
        }
        else
        {
            // Rotamos paulatinamente en la misma dirección en la que gira el jugador..
            transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, turnSpeed * Time.deltaTime);

            // Si el cooldown nos habilita, disparamos.
            if (attackTimer <= 0)
            {
                // Determinamos que cañon utilizar en base al cuadrante en el que nos encontramos.
                var cannonSpawnPoint = dirAngle >= 0 && dirAngle < 180
                    ? leftCannonSpawnPoint
                    : rightCannonSpawnPoint;

                // Calculamos la dirección de disparo y llamamos al controlador de proyectiles
                // para disparar la bala de cañon.
                landPoint = player.transform.position + (player.transform.forward * Random.Range(0, 2f));
                projectileCtrl.Fire(cannonBallPrefab, landPoint, cannonSpawnPoint);

                // Reiniciamos el cooldown.
                attackTimer = attackCooldown;
            }
        }
    }

    private void ChasePlayer()
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

    private void OnDrawGizmos()
    {
        // Dibujamos el rango de ataque..
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (landPoint != Vector3.zero)
            Gizmos.DrawCube(landPoint, Vector3.one * 0.25f);
    }
}
