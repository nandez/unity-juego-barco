using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private BaseEnemy[] enemies;
    private PlayerController player;

    private int score = 0;

    void Start()
    {
        // TODO: Este código es de prueba y debería ser reemplazado por un sistema de eventos.
        // Mergear con la implementación de Thyago / Emiliano
        enemies = FindObjectsOfType<BaseEnemy>();

        if (enemies?.Length > 0)
        {
            foreach (var enemy in enemies)
                enemy.OnEnemyDestroyed += OnEnemyDestroyedHandler;
        }

        player = FindObjectOfType<PlayerController>();
        player.OnPlayerDeath += OnPlayerDeathHandler;
    }


    private void OnEnemyDestroyedHandler(int rewardPoints)
    {
        score += rewardPoints;
        Debug.Log("Score: " + score);
    }

    private void OnPlayerDeathHandler()
    {
        // TODO: codigo de prueba.. reemplazar por un sistema de eventos.
        Debug.Log("Game Over");
        Time.timeScale = 0;
    }
}
