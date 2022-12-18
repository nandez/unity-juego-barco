using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroyedEventArgs
{
    public int RewardPoints { get; set; }
    public Type EnemyType { get; set; }
}
