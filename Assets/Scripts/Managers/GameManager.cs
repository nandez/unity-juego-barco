using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected PlayerController player;
    [SerializeField] protected MapManager mapManager;


    [Header("HUD")]
    [SerializeField] protected BarController healthBarCtrl;
    [SerializeField] protected TextMeshProUGUI timerText;
    [SerializeField] protected TextMeshProUGUI scoreText;
    [SerializeField] protected TextMeshProUGUI scoreMenuTimeText;
    [SerializeField] protected TextMeshProUGUI scoreMenuEnemy1Text;
    [SerializeField] protected TextMeshProUGUI scoreMenuEnemy2Text;
    [SerializeField] protected TextMeshProUGUI scoreMenuEnemy3Text;

    // Events
    public UnityEvent OnGameOver;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;

    private bool gamePaused;
    private bool gameOver;
    private BaseEnemy[] enemies;

    private int scorePoints = 0;
    private Dictionary<Type, int> enemiesDestroyed = new Dictionary<Type, int>()
    {
        { typeof(EnemyType1), 0}, // Lleva el conteo de los enemigos de tipo "EnemyType1"
        { typeof(EnemyType2), 0}, // Lleva el conteo de los enemigos de tipo "EnemyType2"
        { typeof(EnemyType3), 0}, // Lleva el conteo de los enemigos de tipo "EnemyType3"
    };

    private float timer = 0f;

    void Start()
    {
        // Asignamos un evento para ejecutar cuando se crea un enemigo.
        mapManager.OnEnemySpawned += OnEnemySpawned;

        // Asignamos los evento para ejecutar cuando el jugador muere y cuando su vida cambia.
        var playerHealthCtrl = player.GetComponent<HealthController>();
        playerHealthCtrl.OnDeath += GameOver;
        playerHealthCtrl.OnHealthUpdated += (int currentHp, int maxHp) => healthBarCtrl.UpdateValue(currentHp, maxHp);

        // Seteamos el estado del juego
        gamePaused = false;
        gameOver = false;
        Time.timeScale = 1;
    }

    void Update()
    {
        if (!gamePaused)
            UpdateTimer();

        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            if (gamePaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0;
            gamePaused = true;
            OnGamePaused?.Invoke();
        }
    }

    public void ResumeGame()
    {
        if (gamePaused)
        {
            Time.timeScale = 1;
            gamePaused = false;
            OnGameResumed?.Invoke();
        }
    }

    public void RestartGame()
    {
        // Cargamos la escena actual.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuScene");
    }

    private void OnEnemySpawned(BaseEnemy enemy)
    {
        // Cuando el enemigo muere, le asignamos un evento que suma
        // la cantidad de puntos de recompensa y actualiza el HUD.
        enemy.OnEnemyDestroyed += (EnemyDestroyedEventArgs args) =>
        {
            scorePoints += args.RewardPoints;
            scoreText.text = $"Puntos: {scorePoints}";

            // Buscamos en el diccionario, el registro que corresponde al tipo de enemigo
            if (enemiesDestroyed.ContainsKey(args.EnemyType))
                enemiesDestroyed[args.EnemyType] += 1;
        };
    }

    private void GameOver()
    {
        gameOver = true;
        scoreMenuTimeText.text = $"Tiempo de Juego: {timerText.text}";
        scoreMenuEnemy1Text.text = $"Enemigos Tipo 1 -- {enemiesDestroyed[typeof(EnemyType1)]}";
        scoreMenuEnemy2Text.text = $"Enemigos Tipo 2 -- {enemiesDestroyed[typeof(EnemyType2)]}";
        scoreMenuEnemy3Text.text = $"Enemigos Tipo 3 -- {enemiesDestroyed[typeof(EnemyType3)]}";

        Time.timeScale = 0;
        OnGameOver?.Invoke();
    }

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
        int min = Mathf.FloorToInt(timer / 60);
        int seg = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{00:00}:{01:00}", min, seg);
    }
}
