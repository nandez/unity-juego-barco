using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{
    [SerializeField] protected float impulse;
    [SerializeField] protected float lifeTime;

    void Start()
    {
        // Invocamos al destroy del objeto pasado el tiempo de vida.
        Destroy(gameObject, lifeTime);
    }

    /* NOTE: comentado para utilizar RigidBody.
    void Update()
    {
        if (direction != null)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }*/

    public void SetDirection(Vector3 direction)
    {
        //this.direction = direction;
        GetComponent<Rigidbody>().AddForce(direction * impulse, ForceMode.Impulse);
    }
}
