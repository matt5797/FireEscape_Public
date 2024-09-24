using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceIcon : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartRecording()
    {
        animator.SetBool("IsRecording", true);
    }

    public void StopRecording()
    {
        animator.SetBool("IsRecording", false);
    }

    public void StartTranscribing()
    {
        animator.SetBool("IsTranscribing", true);
    }

    public void StopTranscribing()
    {
        animator.SetBool("IsTranscribing", false);
    }
}
