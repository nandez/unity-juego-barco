using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    protected HealthController healthCtrl;
    protected HealthBarController healthBarCtrl;


    [Tooltip("Evento que se dispara cuando muere el jugador..")]
    public UnityAction OnPlayerDeath;
    void Start()
    {
        // Buscamos los componentes HealthController y HealthBarController y asignamos los handlers para los eventos.
        healthBarCtrl = GetComponentInChildren<HealthBarController>();
        healthCtrl = GetComponent<HealthController>();
        healthCtrl.OnHealthUpdated += OnHealthUpdatedHandler;
        healthCtrl.OnDeath += OnDeathHandler;
    }


    private void OnDeathHandler()
    {
        // TODO: animación de muerte, sonido de muerte, etc.

        // Cuando el jugador muere, invocamos el evento OnPlayerDeath.
        OnPlayerDeath?.Invoke();

        // Destruimos el objeto..
        Destroy(gameObject, 0.2f);
    }

    private void OnHealthUpdatedHandler(int currentHitPoints, int maxHitPoints)
    {
        // Actualizamos la barra de vida.
        healthBarCtrl.UpdateHealthBar(currentHitPoints, maxHitPoints);
    }
}
