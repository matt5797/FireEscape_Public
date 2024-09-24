using FireEscape.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FireEscape.VR;

namespace FireEscape.UI
{
    public class ThresholdIndicator : MonoBehaviour
    {
        [SerializeField]
        private SelectorWrapperThreshold selectorWrapperThreshold;

        [SerializeField]
        private Image guideImage; // Reference to the guide image

        [SerializeField]
        private Animator animator; // Reference to the Animator component

        private Coroutine selectionTimerCoroutine;

        private void Awake()
        {
            // Initially turn off the guideImage
            guideImage.enabled = false;
        }

        private void OnEnable()
        {
            //selectorWrapperThreshold.OnSelectProgress.AddListener(UpdateGuideImageFill);
            selectorWrapperThreshold.OnSelectStart.AddListener(OnSelectStart);
            selectorWrapperThreshold.OnSelectThresholdReached.AddListener(OnThresholdReached);
            selectorWrapperThreshold.OnSelectThresholdFailed.AddListener(OnThresholdFailed);
        }

        private void OnDisable()
        {
            //selectorWrapperThreshold.OnSelectProgress.RemoveListener(UpdateGuideImageFill);
            selectorWrapperThreshold.OnSelectStart.RemoveListener(OnSelectStart);
            selectorWrapperThreshold.OnSelectThresholdReached.RemoveListener(OnThresholdReached);
            selectorWrapperThreshold.OnSelectThresholdFailed.RemoveListener(OnThresholdFailed);
        }

        /*private void UpdateGuideImageFill(float normalizedProgress)
        {
            // If the image is not enabled and the progress starts, enable it
            if (!guideImage.enabled && normalizedProgress > 0)
            {
                guideImage.enabled = true;
                animator.SetTrigger("WhenSelected"); // Triggering the selection animation
            }

            guideImage.fillAmount = normalizedProgress; // Update the fill amount
        }*/

        private void OnSelectStart(float totalTime)
        {
            guideImage.enabled = true; // Enable the guideImage
            animator.SetTrigger("WhenSelected"); // Triggering the selection animation

            // Reset the fill amount
            guideImage.fillAmount = 0;

            // Start the coroutine
            selectionTimerCoroutine = StartCoroutine(SelectionProgress(totalTime));
        }

        private void OnThresholdReached()
        {
            animator.SetTrigger("OnThresholdReached"); // Triggering the success animation
            ResetIndicator();

            // Stop the coroutine
            if (selectionTimerCoroutine != null)
            {
                StopCoroutine(selectionTimerCoroutine);
            }
        }

        private void OnThresholdFailed()
        {
            animator.SetTrigger("OnThresholdFailed"); // Triggering the failure animation
            ResetIndicator();

            // Stop the coroutine
            if (selectionTimerCoroutine != null)
            {
                StopCoroutine(selectionTimerCoroutine);
            }
        }

        private void ResetIndicator()
        {
            guideImage.fillAmount = 0;  // Reset fill amount
            guideImage.enabled = false; // Hide the guideImage again
        }

        private IEnumerator SelectionProgress(float totlaTime)
        {
            float elapsedTime = 0f;

            while (elapsedTime < totlaTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float normalizedProgress = Mathf.Clamp01(elapsedTime / totlaTime);
                guideImage.fillAmount = normalizedProgress; // Update the fill amount
                yield return null;
            }
        }
    }
}