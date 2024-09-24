using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FireEscape.VR
{
    public class OVRVirtualKeyboardInputHandler : MonoBehaviour
    {
        private const float RAY_MAX_DISTANCE = 100.0f;
        private const float THUMBSTICK_DEADZONE = 0.2f;
        private const float COLLISION_BOUNDS_ADDED_BLEED_PERCENT = 0.1f;
        private const float LINEPOINTER_THINNING_THRESHOLD = 0.015f;

        private static float ApplyDeadzone(float value)
        {
            if (value > THUMBSTICK_DEADZONE)
                return (value - THUMBSTICK_DEADZONE) / (1.0f - THUMBSTICK_DEADZONE);
            else if (value < -THUMBSTICK_DEADZONE)
                return (value + THUMBSTICK_DEADZONE) / (1.0f - THUMBSTICK_DEADZONE);
            return 0.0f;
        }

        public float AnalogStickX => ApplyDeadzone(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x +
                                                   OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x);

        public float AnalogStickY => ApplyDeadzone(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y +
                                                   OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y);

        public Vector3 InputRayPosition => inputModule.rayTransform.position;

        public OVRVirtualKeyboard OVRVirtualKeyboard;

        [SerializeField]
        private OVRRaycaster raycaster;

        [SerializeField]
        private OVRInputModule inputModule;

        private void Update()
        {
            UpdateInteractionAnchor();
        }

        private void UpdateInteractionAnchor()
        {
            OVRInput.Controller activeController = OVRInput.Controller.None;

            var leftControllerExists = OVRVirtualKeyboard.leftControllerRootTransform != null;
            var leftControllerActive = leftControllerExists && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
            activeController = (leftControllerActive) ? OVRInput.Controller.LTouch : activeController;

            var rightControllerExists = OVRVirtualKeyboard.rightControllerRootTransform != null;
            var rightControllerActive = rightControllerExists && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
            activeController = (rightControllerActive) ? OVRInput.Controller.RTouch : activeController;

            var handLeftExists = OVRVirtualKeyboard.handLeft != null;
            var handLeftIsActive =
                handLeftExists && OVRVirtualKeyboard.handLeft.GetFingerIsPinching(OVRHand.HandFinger.Index);
            activeController = (handLeftIsActive) ? OVRInput.Controller.LHand : activeController;

            var handRightExists = OVRVirtualKeyboard.handRight != null;
            var handRightIsActive =
                handRightExists && OVRVirtualKeyboard.handRight.GetFingerIsPinching(OVRHand.HandFinger.Index);
            activeController = (handRightIsActive) ? OVRInput.Controller.RHand : activeController;

            if (activeController == OVRInput.Controller.None)
            {
                return;
            }

            // Set transforms for Unity UI interaction
            var dominantHandIsLeft =
                (activeController == OVRInput.Controller.LHand || activeController == OVRInput.Controller.LTouch);
            raycaster.pointer = (dominantHandIsLeft)
                ? OVRVirtualKeyboard.handLeft.gameObject
                : OVRVirtualKeyboard.handRight.gameObject;
            inputModule.rayTransform = activeController switch
            {
                OVRInput.Controller.LHand => OVRVirtualKeyboard.handLeft.PointerPose,
                OVRInput.Controller.LTouch => OVRVirtualKeyboard.handLeft.transform,
                OVRInput.Controller.RHand => OVRVirtualKeyboard.handRight.PointerPose,
                _ => OVRVirtualKeyboard.handRight.transform
            };
        }
    }
}