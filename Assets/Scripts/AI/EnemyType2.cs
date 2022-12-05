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
    [SerializeField] protected float attackRange; // Indica la distancia a la que el enemigo ataca.
    [SerializeField] protected float chaseRange; // Indica el radio de distancia en la que el jugador es perseguido.
    [SerializeField] protected float attackCooldown = 5f; //Indica el tiempo de espera entre disparo y disparo.
    [SerializeField] protected int baseDamage = 5; // Daño base del ataque.

    [Header("Cannon Settings")]
    [SerializeField] protected GameObject cannonBallPrefab;
    [SerializeField] protected Transform leftCannonSpawnPoint;
    [SerializeField] protected Transform rightCannonSpawnPoint;

    // Component References
    protected ProjectileController projectileCtrl;

    // Private fields
    private float attackTimer = 0;
    private Vector3 landPoint = Vector3.zero;

    private bool isFleeing; // Indica si el enemigo está huyendo del combate.

    protected override void Initialize()
    {
        // Llamamos al método de la clase base para heredar su comportamiento.
        base.Initialize();

        // Cargamos las referencias a los componentes propios..
        projectileCtrl = GetComponent<ProjectileController>();
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

        // Actualizamos el cooldown de ataque.
        attackTimer -= Time.deltaTime;

        // TODO: testing - quitar código de prueba.
        if (Input.GetKeyDown(KeyCode.Space))
            healthCtrl.TakeDamage(5);
    }

    void Attack()
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
                projectileCtrl.Fire(cannonBallPrefab, landPoint, cannonSpawnPoint, baseDamage);

                // Reiniciamos el cooldown.
                attackTimer = attackCooldown;
            }
        }
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
