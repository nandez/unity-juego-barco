using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField] protected Vector2 mapBottomLeft;
    [SerializeField] protected Vector2 mapTopRight;
    [SerializeField] protected float minDistanceBetweenElements = 20f;
    [SerializeField] protected Transform mapContainer;
    [SerializeField] protected LayerMask obstaclesLayerMask;

    [Header("Island Settings")]
    [SerializeField] protected GameObject islandPrefab;
    [SerializeField] protected float islandSeaLevel = 0;
    [SerializeField] protected int maxIslesOnLevel = 15;


    [Header("Enemies Settings")]
    [SerializeField] protected GameObject enemy1Prefab;
    [SerializeField][Range(0, 1)] protected float enemy1SpawnRate;
    [Space]
    [SerializeField] protected GameObject enemy2Prefab;
    [SerializeField][Range(0, 1)] protected float enemy2SpawnRate;
    [Space]
    [SerializeField] protected GameObject enemy3Prefab;
    [SerializeField][Range(0, 1)] protected float enemy3SpawnRate;
    [Space]

    [SerializeField] protected float enemySpanDelay = 5f;
    [SerializeField] protected int maxEnemiesOnLevel = 10;
    [SerializeField] protected float enemySeaLevel = 0.5f; // Indica el nivel del mar donde instanciar los barcos.

    void Start()
    {
        // Instanciamos los enemigos..
        StartCoroutine(nameof(SpawnEnemies));

        // Instanciamos las islas..
        SpawnIsles();
    }

    protected IEnumerator SpawnEnemies()
    {
        while (true)
        {
            //yield return new WaitForSeconds(enemySpanDelay);

            // Calculamos cuantos enemigos tenemos en el nivel.
            var enemyCount = FindObjectsOfType<BaseEnemy>()?.Length ?? 0;


            // Si no superamos el máximo permitido, instanciamos un nuevo enemigo.
            if (enemyCount < maxEnemiesOnLevel)
            {
                // Obtenemos un enemigo según el spawnRate
                var enemyRate = Random.Range(0f, 1f);

                var enemyPrefab = enemyRate >= 0 && enemyRate <= enemy1SpawnRate ? enemy1Prefab
                    : enemyRate > enemy1SpawnRate && enemyRate <= enemy2SpawnRate ? enemy2Prefab
                    : enemy3Prefab;

                var done = false;

                do
                {
                    // Generamos una posición random en el mapa
                    var pos = GetRandomPosition();

                    // Verificamos si existe algun obstaculo cerca del punto generado en un radio
                    // definido por "minDistanceBetweenElements".
                    if (Physics.OverlapSphere(pos, minDistanceBetweenElements, obstaclesLayerMask).Length > 0)
                        continue;

                    var enemy = Instantiate(enemyPrefab, new Vector3(pos.x, enemySeaLevel, pos.y), Quaternion.identity);
                    done = true;

                } while (!done);
            }

            yield return new WaitForSeconds(enemySpanDelay);
        }
    }

    protected void SpawnIsles()
    {
        for (int i = maxIslesOnLevel; i > 0; i--)
        {
            var done = false;
            do
            {
                // Generamos una posición random en el mapa
                var pos = GetRandomPosition();

                // Verificamos si existe algun obstaculo cerca del punto generado en un radio
                // definido por "minDistanceBetweenElements".
                if (Physics.OverlapSphere(pos, minDistanceBetweenElements, obstaclesLayerMask).Length > 0)
                    continue;

                var isle = Instantiate(islandPrefab, new Vector3(pos.x, islandSeaLevel, pos.y), Quaternion.identity);
                done = true;

            } while (!done);
        }
    }

    protected Vector2 GetRandomPosition()
    {
        var x = Random.Range(mapBottomLeft.x, mapTopRight.x);
        var z = Random.Range(mapBottomLeft.y, mapTopRight.y);

        return new Vector2(x, z);
    }
}
