using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este tipo de enemigo se encuentra en constante búsqueda del
/// jugador e intentará hundirlo a toda costa.
/// </summary>
public class EnemyType1 : EnemyBaseController
{
    [Header("Attack Settings")]
    [Tooltip("Indica la distancia a la que el enemigo ataca.")]
    [SerializeField] protected float attackRange = 5f;

    [Tooltip("Indica el tiempo de espera entre disparo y disparo.")]
    [SerializeField] protected float attackCooldown = 5f;

    [Tooltip("Indica el tag asignado al objeto que representa el lado izquierdo del jugador.")]
    [SerializeField] protected string playerLeftSideTag = "PlayerLeftSide";

    [Tooltip("Indica el tag asignado al objeto que representa el lado derecho del jugador.")]
    [SerializeField] protected string playerRightSideTag = "PlayerRightSide";

    [Tooltip("Representa el prefab de la bala de cañon.")]
    [SerializeField] protected GameObject cannonBallPrefab;

    private Transform playerRightSide;
    private Transform playerLeftSide;
    private float attackTimer = 0;

    protected override void Initialize()
    {
        // Llamamos al método de la clase base para heredar su comportamiento.
        base.Initialize();

        // Buscamos los puntos de referencia del jugador.
        playerLeftSide = GameObject.FindGameObjectWithTag(playerLeftSideTag).transform;
        playerRightSide = GameObject.FindGameObjectWithTag(playerRightSideTag).transform;
    }

    private void Update()
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

        // Actualizamos el cooldown de ataque.
        attackTimer -= Time.deltaTime;
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

        // Determinamos a que lado del jugador nos posicionamos en base al cuadrante en el que nos encontramos.
        var targetPosition = dirAngle >= 0 && dirAngle < 180
            ? playerRightSide.position
            : playerLeftSide.position;

        // Si aún no hemos llegado al punto de ataque, nos movemos hacia el mismo.
        if (Vector3.Distance(transform.position, targetPosition) > 0.25f)
        {
            // Calculamos la dirección hacia el nuevo objetivo y rotamos en su dirección
            var newDirection = targetPosition - transform.position;
            var newRotation = Quaternion.LookRotation(newDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * 2f * Time.deltaTime);
            transform.position += newDirection.normalized * speed * 2f * Time.deltaTime;
        }
        else
        {
            // Si el cooldown nos habilita, disparamos.
            if (attackTimer <= 0)
            {
                // Calculamos la dirección de disparo e instanciamos una bala de cañon.
                var fireDir = (player.transform.position - transform.position).normalized;
                var cannonBall = Instantiate(cannonBallPrefab, transform.position, Quaternion.identity);
                var cannonBallCtrl = cannonBall.GetComponent<CannonBallController>();
                cannonBallCtrl.SetDirection(fireDir);

                // Reiniciamos el cooldown.
                attackTimer = attackCooldown;
            }
        }
        /*var value = player.InverseTransformPoint(transform.position);
        Debug.Log($"Value: {value}");


        var right = value.x > 0;

        var angle = Mathf.Deg2Rad * (right ? attackPointAngle : -attackPointAngle);
        var x = player.position.x;
        if (value.x > 0)
            x += (attackRange * Mathf.Cos(angle));
        else
            x -= (attackRange * Mathf.Cos(angle));


        var z = player.position.z + (attackRange * Mathf.Sin(angle));
        if (value.z > 0)
            z += (attackRange * Mathf.Sin(angle));
        else
            z -= (attackRange * Mathf.Sin(angle));


        var target = new Vector3(x, player.position.y, z);
        //Debug.Log($"Target: {target}");
        transform.position = target;*/


        // TODO: implementar el ataque...
        // Calculamos el punto al que debemos llegar para atacar.
        // aumentamos la velocidad de movimiento para acercarnos más rápido.
        // movemos el bote hacia el punto.
        // atacamos con cooldown
    }

    private void ChasePlayer()
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

    private void OnDrawGizmos()
    {
        // Dibujamos el rango de ataque..
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
