using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireEscape.VR
{
    public class FootAnchor : MonoBehaviour
    {
        [SerializeField]
        private Transform eyeCenterTransform; // The Transform representing the center of the eyes
        [SerializeField]
        private float smoothSpeed = 3.0f; // Smoothing speed, you can adjust to your liking

        private void Update()
        {
            // Calculate the target chest position based on eye position and your specified offsets
            Vector3 targetPosition = new Vector3(eyeCenterTransform.position.x, 0, eyeCenterTransform.position.z);

            // Ensure that only the Y rotation of the eyes is applied
            Quaternion targetRotation = Quaternion.Euler(0, eyeCenterTransform.eulerAngles.y, 0);

            // Smoothly move to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.unscaledDeltaTime);

            // Smoothly rotate to the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.unscaledDeltaTime);
        }
    }
}
