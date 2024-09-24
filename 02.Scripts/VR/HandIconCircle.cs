using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIconCircle : MonoBehaviour
{
    public Vector3 angle;
    [HideInInspector]
    public float speed = 1f;
    private float currentSpeed;

    public float lerpingSpeed = 3f;

    public void LateUpdate()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, speed, lerpingSpeed * Time.unscaledDeltaTime);
        transform.Rotate(angle * currentSpeed * Time.unscaledDeltaTime);
    }
}
