using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(LineRenderer))]
public class CannonController : MonoBehaviour
{
    public LayerMask ignoreLayer;

    [Header("Trajectory Settings")]
    [SerializeField] protected bool showTrajectory = true; // Indica si se debe mostrar o no la trayectoria.
    [SerializeField] protected int archLineCount = 50; // Indica la cantidad de puntos que se dibujarán en la trayectoria.
    [SerializeField] protected float archCalcInterval = 0.25f; // Indica el intérvalo entre cada punto de la trayectoria.


    [Header("Cannon Settings")]
    [SerializeField] protected Transform cannonFirePoint;
    [SerializeField] protected Transform cannonBody;
    [SerializeField] protected Transform cannonPivot;
    [Range(0, 90)][SerializeField] protected float fireAngle = 35f; // Indica el ángulo de disparo
    [SerializeField] protected float rotationSpeed = 5f; // Indica la velocidad de rotación del cañon.
    [SerializeField] protected float damageMultiplier = 1f; // Indica el multiplicador de daño del cañon.

    [Header("CoolDown Settings")]
    [SerializeField] protected bool showCooldownBar = true; // Indica si mostrar o no la barra de cooldown
    [SerializeField] protected BarController cooldownBarCtrl; // Referencia al componente BarController.
    [SerializeField] protected float attackCooldown = 5f; //Indica el tiempo de espera entre disparo y disparo.

    // Public properties..
    public bool CanFire() => attackTimer <= 0;

    // Component References
    private LineRenderer line; // Referencia al componente LineRenderer.

    // Private Fields..
    private Vector3 fireDirection;
    private float velocity;
    private List<GameObject> archObj = new List<GameObject>();
    private float attackTimer = 0;

    void Start()
    {
        line = GetComponent<LineRenderer>();

        // TODO: rotar el cuerpo del cañon en el ángulo de disparo.
    }

    void Update()
    {
        // Actualizamos el cooldown de ataque.
        attackTimer -= Time.deltaTime;

        // Si tenemos habilitada la visualización de la barra de cooldown, actualizamos su valor.
        if (showCooldownBar)
            cooldownBarCtrl.UpdateValue(attackTimer, attackCooldown);
    }

    public void SetTarget(CannonBall projectilePrefab, Vector3 target, GameObject owner)
    {
        // Rotamos el pivote del cañon hacia el objetivo.
        var rotation = Quaternion.LookRotation(target - cannonPivot.position, Vector3.up);
        cannonPivot.rotation = Quaternion.Lerp(cannonPivot.rotation, rotation, rotationSpeed * Time.deltaTime);

        if (CanFire() && CheckVector(target))
        {
            DisplayTrajectory(target);

            // Instanciamos el proyectil y le aplicamos una fuerza de impulso
            var projectile = CannonBall.Instantiate(projectilePrefab, cannonFirePoint.position, Quaternion.identity, owner);
            projectile.SetDamageMultiplier(damageMultiplier);

            // Deshabilitamos la colisión entre el proyectil y su propietario.
            Physics.IgnoreCollision(projectile.GetComponent<Collider>(), owner.GetComponent<Collider>(), true);

            // Le aplicamos una fuerza de impulso al proyectil.
            var projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.AddForce(fireDirection * projectileRb.mass, ForceMode.Impulse);

            // Si tenemos habilitada la visualización de la trayectoria, limpiamos la trayectoria con delay..
            if (showTrajectory)
                Invoke(nameof(CleanTrajectory), 2f);

            // Reiniciamos el cooldown.
            attackTimer = attackCooldown;
        }
    }

    private bool CheckVector(Vector3 target)
    {
        velocity = CalculateVectorFromAngle(target, fireAngle);

        if (velocity <= 0.0f)
            return false;

        fireDirection = ConvertVectorToVector3(velocity, fireAngle, target);

        return true;
    }

    private float CalculateVectorFromAngle(Vector3 target, float angle)
    {
        var shootPos = new Vector2(cannonFirePoint.position.x, cannonFirePoint.position.z);
        var hitPos = new Vector2(target.x, target.z);

        float x = Vector2.Distance(shootPos, hitPos);
        float g = Physics.gravity.y;
        float y0 = cannonFirePoint.position.y;
        float y = target.y;
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);

        float v0Sq = g * x * x / (2 * cos * cos * (y - y0 - x * tan));
        return v0Sq <= 0f ? 0f : Mathf.Sqrt(v0Sq);
    }

    private Vector3 ConvertVectorToVector3(float velocity, float angle, Vector3 target)
    {
        var shootPos = new Vector3(cannonFirePoint.position.x, 0f, cannonFirePoint.position.z);
        var hitPos = new Vector3(target.x, 0f, target.z);

        var dir = (hitPos - shootPos).normalized;
        var rot = Quaternion.FromToRotation(Vector3.right, dir);
        var vec = rot * Quaternion.AngleAxis(angle, Vector3.forward) * (velocity * Vector3.right);

        return vec;
    }

    private void DisplayTrajectory(Vector3 hitPos)
    {
        CleanTrajectory();

        if (showTrajectory)
        {
            float x;
            float y = cannonFirePoint.position.y;
            float y0 = y;
            float g = Physics.gravity.y;
            float rad = fireAngle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            float time;

            hitPos.y = y;

            var dir = (hitPos - cannonFirePoint.position).normalized;
            Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);
            RaycastHit hit;

            var archPoints = new List<Vector3>();
            for (int i = 0; y > 0; i++)
            {
                time = archCalcInterval * i;
                x = velocity * cos * time;
                y = velocity * sin * time + y0 + g * time * time / 2;

                var newPoint = rot * new Vector3(x, y, 0);
                newPoint = new Vector3(newPoint.x + cannonFirePoint.position.x, newPoint.y, newPoint.z + cannonFirePoint.position.z);
                archPoints.Add(newPoint);

                if (i > 0)
                {
                    if (Physics.Linecast(archPoints[i - 1], archPoints[i], out hit, ignoreLayer))
                    {
                        archPoints[i] = hit.point;
                        break;
                    }
                }
            }
            int lineLength = archLineCount;
            archPoints.Reverse();

            if (archPoints.Count < lineLength)
                lineLength = archPoints.Count;

            line.positionCount = archPoints.Count - (archPoints.Count - lineLength);
            line.SetPositions(archPoints.ToArray());
            line.useWorldSpace = true;
        }
    }

    private void CleanTrajectory()
    {
        line.positionCount = 0;
        foreach (GameObject obj in archObj)
            Destroy(obj);

        archObj.Clear();
    }
}
