using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HealthController))]
public class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]

    [Tooltip("Indica la velocidad de movimiento del enemigo..")]
    [SerializeField] protected float speed;

    [Tooltip("Indica la velocidad de rotación del enemigo.")]
    [SerializeField] protected float turnSpeed;

    [Tooltip("Indica los puntos de recompensa que otorga el enemigo al ser destruido..")]
    [SerializeField] protected int rewardPoints;


    [Header("Events")]
    public UnityAction<EnemyDestroyedEventArgs> OnEnemyDestroyed; // Evento que se invoca cuando el enemigo es destruido.

    [Header("References")]
    [SerializeField] protected BarController healthBarCtrl; // Referencia al controlador de la barra de vida.

    [Header("Obstacle Sensors")]
    [SerializeField] protected GameObject rightSensor;
    [SerializeField] protected GameObject leftSensor;
    [SerializeField] protected GameObject forwardSensor;
    [SerializeField] protected LayerMask obstacleLayers;

    // Component References
    protected HealthController healthCtrl; // Referencia al controlador de vida.

    // Protected Fields
    protected GameObject player; // Referencia al jugador.

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
        // Buscamos al jugador en la escena; si no lo encontramos, tiramos un error..
        player = SceneUtils.FindPlayer(true);

        // Inicializamos las referencias a los componentes que controla los puntos de vida
        // y asignamos los handlers para los eventos.
        healthCtrl = GetComponent<HealthController>();
        healthCtrl.OnDeath += OnEnemyDeathHandler;
        healthCtrl.OnHealthUpdated += OnEnemyHealthUpdatedHandler;
    }

    protected virtual void OnEnemyDeathHandler()
    {
        // Cuando el enemigo muere, invocamos el evento OnEnemyDestroyed indicando
        // los puntos de recompensa como parámetro.
        OnEnemyDestroyed?.Invoke(new EnemyDestroyedEventArgs()
        {
            EnemyType = this.GetType(),
            RewardPoints = rewardPoints
        });

        // TODO: animación de "hundimiento" (transform.Translate(Vector3.down * Time.deltaTime * 2f)
        // sonido de explosión??
        Destroy(gameObject);
    }

    protected virtual void OnEnemyHealthUpdatedHandler(int currentHitPoints, int maxHitPoints)
    {
        // Cuando el enemigo recibe daño, actualizamos la barra de vida.
        healthBarCtrl?.UpdateValue(currentHitPoints, maxHitPoints);
    }

    protected virtual void Move(Vector3 direction, float speedFactor)
    {
        Vector3 destination = direction;
        Vector3 forwardView = forwardSensor.transform.position - transform.position;
        Vector3 rightObstaclePoint = Vector3.zero;
        Vector3 leftObstaclePoint = Vector3.zero;
        var right = false;
        var left = false;

        Debug.DrawRay(transform.position, destination, Color.red, 0);
        Debug.DrawRay(rightSensor.transform.position - forwardView.normalized, (forwardView.normalized + (rightSensor.transform.position - transform.position) / 2) * 7);
        if (Physics.Raycast(rightSensor.transform.position - forwardView.normalized, forwardView.normalized + (rightSensor.transform.position - transform.position) / 2, out var rightHitInfo, 7, obstacleLayers))
        {
            rightObstaclePoint = rightHitInfo.point;
            right = true;
        }

        Debug.DrawRay(leftSensor.transform.position - forwardView.normalized, (forwardView.normalized + (leftSensor.transform.position - transform.position) / 2) * 7);
        if (Physics.Raycast(leftSensor.transform.position - forwardView.normalized, forwardView.normalized + (leftSensor.transform.position - transform.position) / 2, out var leftHitInfo, 7, obstacleLayers))
        {
            leftObstaclePoint = leftHitInfo.point;
            left = true;
        }

        if (right && left)
        {
            var distanceToRightObstacle = (rightObstaclePoint - transform.position).magnitude;
            var distanceToLeftObstacle = (leftObstaclePoint - transform.position).magnitude;

            if (distanceToRightObstacle < distanceToLeftObstacle)
                destination = rightObstaclePoint.normalized + (leftSensor.transform.position - transform.position).normalized * 3;

            if (distanceToRightObstacle > distanceToLeftObstacle)
                destination = leftObstaclePoint.normalized + (rightSensor.transform.position - transform.position).normalized * 3;
        }
        else if (right && !left)
        {
            destination = rightObstaclePoint.normalized + (leftSensor.transform.position - transform.position).normalized * 3;
        }
        else if (!right && left)
        {
            destination = leftObstaclePoint.normalized + (rightSensor.transform.position - transform.position).normalized * 3;
        }
        else
        {
            destination = direction;
        }

        // Rotamos y movemos al enemigo
        var rotation = Quaternion.LookRotation(destination); // crea una variable rotation y le asigna la rotacion de Sentido
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed * speedFactor * Time.deltaTime); // transforma la rotacion del jugador suavemente
        transform.position += transform.forward * speed * speedFactor * Time.deltaTime;
    }
}
