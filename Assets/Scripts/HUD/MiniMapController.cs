using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [SerializeField] protected bool rotateWithPlayer = false;

    protected GameObject player;

    void Start()
    {
        player = SceneUtils.FindPlayer();
    }

    void LateUpdate()
    {
        // Actualizamos la posición de la cámara de la mini-map.
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

            // Actualizamos la rotación de la cámara de la mini-map.
            if (rotateWithPlayer)
                transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
        }
    }
}
