using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileController : MonoBehaviour
{
    public LayerMask ignoreLayer;
    public bool showTrajectory = true;

    public int archLineCount = 50;
    public float archCalcInterval = 0.2f;
    public float archHeightLimit = 0;
    [Range(0, 90)]
    public float throwAngle = 45f;
    public float shootRange = 200f;

    public float startLineWidth = 0.1f;
    public float endLineWidth = 0.1f;
    public Color startColor = Color.blue;
    public Color endColor = Color.blue;
    [Range(0, 1)] public float startAlpha = 0.7f;
    [Range(0, 1)] public float endAlpha = 0.1f;


    private GameObject shootPoint;
    private float spdVec;
    private List<GameObject> archObj = new List<GameObject>();
    private Vector3 shootVec;
    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = endLineWidth;
        line.endWidth = startLineWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(endColor, 0.0f), new GradientColorKey(startColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(endAlpha, 0.0f), new GradientAlphaKey(startAlpha, 1.0f) }
        );
        line.colorGradient = gradient;
    }

    public void Fire(GameObject objPrefab, Vector3 hitPos, Transform firePoint)
    {
        shootPoint = firePoint.gameObject;
        CheckVector(hitPos);
        GameObject obj = Instantiate(objPrefab, shootPoint.transform.position, Quaternion.identity);
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        Vector3 force = shootVec * rig.mass;
        rig.AddForce(force, ForceMode.Impulse);
    }

    private void CheckVector(Vector3 hitPos)
    {

        spdVec = CalculateVectorFromAngle(hitPos, throwAngle);
        if (spdVec <= 0.0f)
        {
            Debug.Log("impossible hit point");
            return;
        }

        shootVec = ConvertVectorToVector3(spdVec, throwAngle, hitPos);
        if (showTrajectory)
        {
            DisplayTrajectory(hitPos);
        }
        else
        {
            ClearTrajectory();
        }

    }

    private float CalculateVectorFromAngle(Vector3 pos, float angle)
    {
        Vector2 shootPos = new Vector2(shootPoint.transform.position.x,
            shootPoint.transform.position.z);
        Vector2 hitPos = new Vector2(pos.x, pos.z);
        float x = Vector2.Distance(shootPos, hitPos);
        float g = Physics.gravity.y;
        float y0 = shootPoint.transform.position.y;
        float y = pos.y;
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);

        float v0Sq = g * x * x / (2 * cos * cos * (y - y0 - x * tan));
        if (v0Sq <= 0.0f)
        {
            return 0.0f;
        }
        return Mathf.Sqrt(v0Sq);
    }

    private Vector3 ConvertVectorToVector3(float spdVec, float angle, Vector3 pos)
    {
        Vector3 shootPos = shootPoint.transform.position;
        Vector3 hitPos = pos;
        shootPos.y = 0f;
        hitPos.y = 0f;

        Vector3 dir = (hitPos - shootPos).normalized;
        Quaternion Rot3D = Quaternion.FromToRotation(Vector3.right, dir);
        Vector3 vec = spdVec * Vector3.right;
        vec = Rot3D * Quaternion.AngleAxis(angle, Vector3.forward) * vec;

        return vec;
    }

    private void DisplayTrajectory(Vector3 hitPos)
    {
        foreach (GameObject obj in archObj)
            Destroy(obj, 0f);

        archObj.Clear();

        float x;
        float y = shootPoint.transform.position.y;
        float y0 = y;
        float g = Physics.gravity.y;
        float rad = throwAngle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        float time;

        List<Vector3> archVerts = new List<Vector3>();
        Vector3 shootPos3 = shootPoint.transform.position;
        hitPos.y = shootPos3.y;
        Vector3 dir = (hitPos - shootPos3).normalized;
        float spd = spdVec;
        Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
        RaycastHit hit;

        for (int i = 0; y > 0; i++)
        {
            time = archCalcInterval * i;
            x = spd * cos * time;
            y = spd * sin * time + y0 + g * time * time / 2;
            archVerts.Add(new Vector3(x, y, 0));
            archVerts[i] = yawRot * archVerts[i];
            archVerts[i] = new Vector3(archVerts[i].x + shootPos3.x, archVerts[i].y, archVerts[i].z + shootPos3.z);

            if (i > 0)
            {
                if (Physics.Linecast(archVerts[i - 1], archVerts[i], out hit, ignoreLayer))
                {
                    archVerts[i] = hit.point;
                    break;
                }
            }
        }
        int lineLength = archLineCount;
        archVerts.Reverse();

        if (archVerts.Count < lineLength)
            lineLength = archVerts.Count;

        line.startWidth = endLineWidth;
        line.endWidth = startLineWidth;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(endColor, 0.0f), new GradientColorKey(startColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(endAlpha, 0.0f), new GradientAlphaKey(startAlpha, 1.0f) }
        );
        line.colorGradient = gradient;
        line.positionCount = archVerts.Count - (archVerts.Count - lineLength);
        line.SetPositions(archVerts.ToArray());
        line.useWorldSpace = true;
    }

    private void ClearTrajectory()
    {
        line.positionCount = 0;
        foreach (GameObject obj in archObj)
            Destroy(obj);
    }
}
