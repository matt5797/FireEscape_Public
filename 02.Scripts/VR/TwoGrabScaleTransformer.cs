using System.Collections.Generic;
using UnityEngine;
using System;
using Oculus.Interaction;

namespace FireEscape.VR
{
    /// <summary>
    /// A Transformer that allows the target to be scaled using two grab points,
    /// while keeping position and rotation unchanged.
    /// </summary>
    public class TwoGrabScaleTransformer : MonoBehaviour, ITransformer
    {
        private float _initialDistance;
        private float _initialScale = 1.0f;
        private float _activeScale = 1.0f;

        [Serializable]
        public class TwoGrabScaleConstraints
        {
            [Tooltip("If true then the constraints are relative to the initial scale of the object " +
                     "if false, constraints are absolute with respect to the object's x-axis scale.")]
            public bool ConstraintsAreRelative;
            public FloatConstraint MinScale;
            public FloatConstraint MaxScale;
        }

        [SerializeField]
        private TwoGrabScaleConstraints _constraints;

        public TwoGrabScaleConstraints Constraints
        {
            get
            {
                return _constraints;
            }

            set
            {
                _constraints = value;
            }
        }

        private IGrabbable _grabbable;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
        }

        public void BeginTransform()
        {
            var grabA = _grabbable.GrabPoints[0];
            var grabB = _grabbable.GrabPoints[1];

            Vector3 diff = grabB.position - grabA.position;
            _initialDistance = diff.magnitude;

            if (!_constraints.ConstraintsAreRelative)
            {
                _activeScale = _grabbable.Transform.localScale.x;
            }
            _initialScale = _activeScale;
        }

        public void UpdateTransform()
        {
            var grabA = _grabbable.GrabPoints[0];
            var grabB = _grabbable.GrabPoints[1];

            Vector3 targetVector = grabB.position - grabA.position;
            float activeDistance = targetVector.magnitude;
            if (Mathf.Abs(activeDistance) < 0.0001f) activeDistance = 0.0001f;

            float scalePercentage = activeDistance / _initialDistance;

            _activeScale = _initialScale * scalePercentage;

            if (_constraints.MinScale.Constrain)
            {
                _activeScale = Mathf.Max(_constraints.MinScale.Value, _activeScale);
            }
            if (_constraints.MaxScale.Constrain)
            {
                _activeScale = Mathf.Min(_constraints.MaxScale.Value, _activeScale);
            }

            var targetTransform = _grabbable.Transform;
            targetTransform.localScale = new Vector3(_activeScale, _activeScale, _activeScale);
        }

        public void EndTransform() { }
    }
}
