using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] protected Image foregroundImg;
    [SerializeField] protected float updateRate = 0.5f;

    public void UpdateHealthBar(float amount, float maxAmount)
    {
        if (maxAmount <= 0f)
            maxAmount = 1f;

        foregroundImg.fillAmount = Mathf.Lerp(0f, 1f, amount / maxAmount);
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
