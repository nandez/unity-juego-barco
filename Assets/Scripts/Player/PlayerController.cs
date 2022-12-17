using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HealthController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] protected BarController healthBarCtrl;

    // Component References
    protected HealthController healthCtrl;

    //public UnityAction OnPlayerDeath; // Evento que se invoca cuando el jugador muere.
    //public UnityAction OnPlayerHealthUpdate; // Evento que se invoca cuando el jugador muere.

    void Start()
    {
        // Buscamos los componentes HealthController y HealthBarController y asignamos los handlers para los eventos.
        healthCtrl = GetComponent<HealthController>();
        healthCtrl.OnHealthUpdated += OnHealthUpdatedHandler;
        healthCtrl.OnDeath += OnDeathHandler;
    }

    private void OnDeathHandler()
    {
        // TODO: animación de muerte, sonido de muerte, etc.

        // Cuando el jugador muere, invocamos el evento OnPlayerDeath.
        //OnPlayerDeath?.Invoke();

        // Destruimos el objeto..
        Destroy(gameObject, 0.2f);
    }

    private void OnHealthUpdatedHandler(int currentHitPoints, int maxHitPoints)
    {
        // Actualizamos la barra de vida que está encima del jugador.
        healthBarCtrl?.UpdateValue(currentHitPoints, maxHitPoints);
    }
}
