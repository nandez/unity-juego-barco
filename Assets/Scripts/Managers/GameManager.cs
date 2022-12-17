using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected PlayerController player;
    [SerializeField] protected MapManager mapManager;


    [Header("HUD")]
    [SerializeField] protected BarController healthBarCtrl;
    [SerializeField] protected TextMeshProUGUI timerText;
    [SerializeField] protected TextMeshProUGUI scoreText;

    private BaseEnemy[] enemies;

    private int score = 0;
    private float timer = 0f;

    void Start()
    {
        // Asignamos un evento para ejecutar cuando se crea un enemigo.
        mapManager.OnEnemySpawned += OnEnemySpawned;

        // Asignamos los evento para ejecutar cuando el jugador muere y cuando su vida cambia.
        var playerHealthCtrl = player.GetComponent<HealthController>();
        playerHealthCtrl.OnDeath += GameOver;
        playerHealthCtrl.OnHealthUpdated += (int currentHp, int maxHp) => healthBarCtrl.UpdateValue(currentHp, maxHp);
    }

    void Update()
    {
        UpdateTimer();
    }

    private void OnEnemySpawned(BaseEnemy enemy)
    {
        // Cuando el enemigo muere, le asignamos un evento que suma
        // la cantidad de puntos de recompensa y actualiza el HUD.
        enemy.OnEnemyDestroyed += (int rewardPoints) =>
        {
            score += rewardPoints;
            scoreText.text = $"Puntos: {score}";
        };
    }

    private void GameOver()
    {
        // TODO: codigo de prueba.. reemplazar por un sistema de eventos.
        Debug.Log("Game Over");
        Time.timeScale = 0;
    }

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
        int min = Mathf.FloorToInt(timer / 60);
        int seg = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{00:00}:{01:00}", min, seg);
    }
}
