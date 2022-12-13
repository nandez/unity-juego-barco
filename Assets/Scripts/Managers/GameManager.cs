using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private BaseEnemy[] enemies;
    private PlayerController player;
    private MapManager mapManager;

    private int score = 0;
    public int GetScore() => score;

    void Awake()
    {
        mapManager = FindObjectOfType<MapManager>();
        mapManager.OnEnemySpawned += OnEnemySpawned;

        player = FindObjectOfType<PlayerController>();
        player.OnPlayerDeath += OnPlayerDeathHandler;
    }


    private void OnEnemySpawned(BaseEnemy enemy)
    {
        enemy.OnEnemyDestroyed += OnEnemyDestroyedHandler;
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
