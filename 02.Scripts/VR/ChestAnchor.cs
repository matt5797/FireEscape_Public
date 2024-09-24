using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireEscape.VR;

namespace FireEscape.VR
{
    public class ChestAnchor : MonoBehaviour
    {
        [SerializeField]
        private Transform eyeCenterTransform; // The Transform representing the center of the eyes
        [SerializeField]
        private float yOffset = 0.3f; // Offset from eye center to chest, you may want to tweak this
        [SerializeField]
        private float zOffset = 0f; // Offset from eye center to chest, you may want to tweak this
        [SerializeField]
        private float smoothSpeed = 3.0f; // Smoothing speed, you can adjust to your liking

        private void Update()
        {
            // Calculate the scaled offsets
            float scaledYOffset = yOffset * eyeCenterTransform.lossyScale.y;
            float scaledZOffset = zOffset * eyeCenterTransform.lossyScale.z;

            // Calculate the target chest position based on eye position and your specified offsets
            Vector3 targetPosition = eyeCenterTransform.position - eyeCenterTransform.up * scaledYOffset - eyeCenterTransform.forward * scaledZOffset;

            // Ensure that only the Y rotation of the eyes is applied
            Quaternion targetRotation = Quaternion.Euler(0, eyeCenterTransform.eulerAngles.y, 0);

            // Smoothly move to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.unscaledDeltaTime);

            // Smoothly rotate to the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.unscaledDeltaTime);
        }
    }
}
