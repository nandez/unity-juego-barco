using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileController : MonoBehaviour
{
    public LayerMask ignoreLayer;

    [Tooltip("Indica si se debe mostrar o no la trayectoria.")]
    public bool showTrajectory = true;

    [Tooltip("Indica la cantidad de puntos que se dibujarán en la trayectoria.")]
    [SerializeField] protected int archLineCount = 50;

    [Tooltip("Indica el intérvalo entre cada punto de la trayectoria.")]
    [SerializeField] protected float archCalcInterval = 0.25f;

    [Tooltip("Indica angulo de disparo.")]
    [Range(0, 90)][SerializeField] protected float throwAngle = 35f;

    private LineRenderer line; // Referencia al componente LineRenderer.
    private GameObject shootPoint;
    private Vector3 shootDirection;
    private float velocity;
    private List<GameObject> archObj = new List<GameObject>();


    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void Fire(GameObject objPrefab, Vector3 hitPos, Transform firePoint, int damage)
    {
        shootPoint = firePoint.gameObject;
        if (CheckVector(hitPos))
        {
            var obj = Instantiate(objPrefab, shootPoint.transform.position, Quaternion.identity);
            obj.GetComponent<CannonBall>().damage = damage;

            var rb = obj.GetComponent<Rigidbody>();
            rb.AddForce(shootDirection * rb.mass, ForceMode.Impulse);

            Invoke(nameof(CleanTrajectory), 2f);
        }
    }

    private bool CheckVector(Vector3 hitPos)
    {
        velocity = CalculateVectorFromAngle(hitPos, throwAngle);

        if (velocity <= 0.0f)
            return false;

        shootDirection = ConvertVectorToVector3(velocity, throwAngle, hitPos);

        DisplayTrajectory(hitPos);

        return true;
    }

    private float CalculateVectorFromAngle(Vector3 pos, float angle)
    {
        var shootPos = new Vector2(shootPoint.transform.position.x, shootPoint.transform.position.z);
        var hitPos = new Vector2(pos.x, pos.z);

        float x = Vector2.Distance(shootPos, hitPos);
        float g = Physics.gravity.y;
        float y0 = shootPoint.transform.position.y;
        float y = pos.y;
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);

        float v0Sq = g * x * x / (2 * cos * cos * (y - y0 - x * tan));
        return v0Sq <= 0f ? 0f : Mathf.Sqrt(v0Sq);
    }

    private Vector3 ConvertVectorToVector3(float spdVec, float angle, Vector3 pos)
    {
        var shootPos = new Vector3(shootPoint.transform.position.x, 0f, shootPoint.transform.position.z);
        var hitPos = new Vector3(pos.x, 0f, pos.z);

        var dir = (hitPos - shootPos).normalized;
        var rot = Quaternion.FromToRotation(Vector3.right, dir);
        var vec = rot * Quaternion.AngleAxis(angle, Vector3.forward) * (spdVec * Vector3.right);

        return vec;
    }

    private void DisplayTrajectory(Vector3 hitPos)
    {
        CleanTrajectory();

        if (showTrajectory)
        {
            float x;
            float y = shootPoint.transform.position.y;
            float y0 = y;
            float g = Physics.gravity.y;
            float rad = throwAngle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            float time;

            hitPos.y = y;

            var dir = (hitPos - shootPoint.transform.position).normalized;
            Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);
            RaycastHit hit;

            var archPoints = new List<Vector3>();
            for (int i = 0; y > 0; i++)
            {
                time = archCalcInterval * i;
                x = velocity * cos * time;
                y = velocity * sin * time + y0 + g * time * time / 2;

                var newPoint = rot * new Vector3(x, y, 0);
                newPoint = new Vector3(newPoint.x + shootPoint.transform.position.x, newPoint.y, newPoint.z + shootPoint.transform.position.z);
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
