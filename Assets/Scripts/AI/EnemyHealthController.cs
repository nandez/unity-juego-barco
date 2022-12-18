using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : HealthController
{
    [SerializeField] protected string playerTag;

    public override void TakeDamage(int damage, GameObject attacker)
    {
        // Cuando el enemigo recibe daño, verificamos el atacante para
        // asegurarnos que solo pueda recibir daño del jugador.
        if (attacker.CompareTag(playerTag))
            base.TakeDamage(damage, attacker);

    }
}
