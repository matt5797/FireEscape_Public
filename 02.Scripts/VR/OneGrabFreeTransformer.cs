using Oculus.Interaction;
using UnityEngine;

namespace FireEscape.VR
{
    /// <summary>
    /// A Transformer that moves the target in a 1-1 fashion with the GrabPoint.
    /// Updates transform the target in such a way as to maintain the target's
    /// local positional and rotational offsets from the GrabPoint, with optional
    /// constraints on position and rotation axes.
    /// </summary>
    public class OneGrabFreeTransformer : MonoBehaviour, ITransformer
    {
        private IGrabbable _grabbable;
        private Pose _grabDeltaInLocalSpace;

        [Header("Constraints")]
        public Vector3 positionConstraints = Vector3.zero; // 1 to fix an axis, 0 to allow transformations.
        public Vector3 rotationConstraints = Vector3.zero; // 1 to fix an axis, 0 to allow transformations.

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
        }

        public void BeginTransform()
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;
            _grabDeltaInLocalSpace = new Pose(targetTransform.InverseTransformVector(grabPoint.position - targetTransform.position),
                                            Quaternion.Inverse(grabPoint.rotation) * targetTransform.rotation);
        }

        public void UpdateTransform()
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;

            targetTransform.rotation = grabPoint.rotation * _grabDeltaInLocalSpace.rotation;
            targetTransform.position = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);

            // Apply rotation constraints
            /*Quaternion newRotation = grabPoint.rotation * _grabDeltaInLocalSpace.rotation;
            Vector3 newEulerAngles = newRotation.eulerAngles;
            Vector3 currentEulerAngles = targetTransform.rotation.eulerAngles;
            newEulerAngles = Vector3.Scale(newEulerAngles, Vector3.one - rotationConstraints) + Vector3.Scale(currentEulerAngles, rotationConstraints);

            targetTransform.rotation = Quaternion.Euler(newEulerAngles);

            // Apply position constraints
            Vector3 newPosition = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);
            *//*Vector3 currentPosition = targetTransform.position;
            newPosition = Vector3.Scale(newPosition, Vector3.one - positionConstraints) + Vector3.Scale(currentPosition, positionConstraints);*//*

            targetTransform.position = newPosition;*/
        }

        public void EndTransform() { }
    }
}
