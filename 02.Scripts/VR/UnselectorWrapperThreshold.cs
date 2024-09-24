using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace FireEscape.VR
{
    public class UnselectorWrapperThreshold : MonoBehaviour
    {
        [SerializeField]
        private SelectorUnityEventWrapper selector;

        [SerializeField]
        private float unselectThresholdTime = 2.0f; // Time in seconds

        public UnityEvent<float> OnUnselectStart; // Event to notify the start of unselection
        public UnityEvent OnUnselectThresholdReached;
        public UnityEvent OnUnselectThresholdFailed;

        private bool isSelected;
        private Coroutine unselectionTimerCoroutine;

        private void OnEnable()
        {
            selector.WhenSelected.AddListener(OnSelected);
            selector.WhenUnselected.AddListener(OnUnselected);
        }

        private void OnDisable()
        {
            selector.WhenSelected.RemoveListener(OnSelected);
            selector.WhenUnselected.RemoveListener(OnUnselected);
        }

        private void OnSelected()
        {
            isSelected = true;

            if (unselectionTimerCoroutine != null)
            {
                StopCoroutine(unselectionTimerCoroutine);
                OnUnselectThresholdFailed?.Invoke();
            }
        }

        private void OnUnselected()
        {
            isSelected = false;
            OnUnselectStart?.Invoke(unselectThresholdTime); // Notify the start of unselection

            if (unselectionTimerCoroutine != null)
            {
                StopCoroutine(unselectionTimerCoroutine);
            }

            unselectionTimerCoroutine = StartCoroutine(UnselectionTimer());
        }

        private IEnumerator UnselectionTimer()
        {
            float elapsedTime = 0f;

            while (elapsedTime < unselectThresholdTime && !isSelected)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!isSelected)
            {
                OnUnselectThresholdReached?.Invoke();
            }
        }
    }
}
