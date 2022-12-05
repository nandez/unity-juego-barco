using System;
using UnityEngine;

public static class SceneUtils
{
    public static GameObject FindPlayer(bool throwError = false)
    {
        var player = GameObject.FindObjectOfType<PlayerController>()?.gameObject;

        if (player == null && throwError)
            throw new Exception($"No se encontró al jugador en la escena.");

        return player;
    }
}
