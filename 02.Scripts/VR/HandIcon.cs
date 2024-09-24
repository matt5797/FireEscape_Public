using FireEscape.Chat;
using FireEscape.VR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIcon : MonoBehaviour
{
    public List<HandIconCircle> circles;
    public float speed = 1f;
    private float lastSpeed;

    public float loadingSpeed = 5f;
    public float activateSpeed = 3f;
    public float deactivateSpeed = 1f;

    public ParticleSystem orbParticleSystem;
    private ParticleSystem.MainModule mainModule;

    public Color idleColor = Color.white;
    public Color recordingColor = Color.red;
    public Color transcriptingColor = Color.blue;
    public Color loadingColor = Color.yellow;

    public VoiceToChat voiceToChat;

    [SerializeField]
    private SelectorWrapperThreshold recordSelectorThreshold;
    [SerializeField]
    private SelectorWrapperThreshold transcriptSelectorThreshold;

    private Coroutine selectionTimerCoroutine;

    private void Awake()
    {
        if (orbParticleSystem != null)
        {
            mainModule = orbParticleSystem.main;
        }
    }

    private void Start()
    {
        foreach (HandIconCircle circle in circles)
        {
            circle.speed = speed;
        }

        lastSpeed = speed;
    }

    private void OnEnable()
    {
        recordSelectorThreshold.OnSelectStart.AddListener(OnRecordSelectStart);
        transcriptSelectorThreshold.OnSelectStart.AddListener(OnTranscriptSelectStart);
    }

    private void OnDisable()
    {
        recordSelectorThreshold.OnSelectStart.RemoveListener(OnRecordSelectStart);
        transcriptSelectorThreshold.OnSelectStart.AddListener(OnTranscriptSelectStart);
    }

    public void Update()
    {
        if (lastSpeed != speed)
        {
            lastSpeed = speed;
            foreach (HandIconCircle circle in circles)
            {
                circle.speed = speed;
            }
        }
    }

    public void OnIdle()
    {
        if (orbParticleSystem != null)
        {
            mainModule.startColor = idleColor;
        }
        speed = deactivateSpeed;
    }

    public void OnLoading()
    {
        if (orbParticleSystem != null)
        {
            mainModule.startColor = loadingColor;
        }
        speed = loadingSpeed;
    }

    public void OnStartRecording()
    {
        if (orbParticleSystem != null)
        {
            mainModule.startColor = recordingColor;
        }
        speed = activateSpeed;
    }

    public void OnStartTranscripting()
    {
        if (orbParticleSystem != null)
        {
            mainModule.startColor = transcriptingColor;
        }
        speed = activateSpeed;
    }

    private void OnRecordSelectStart(float totalTime)
    {
        // Start the coroutine
        //selectionTimerCoroutine = StartCoroutine(SelectionProgress(totalTime));

        if (voiceToChat.isRecording || voiceToChat.isTranscribing)
            return;

        OnLoading();
    }

    private void OnTranscriptSelectStart(float totalTime)
    {
        // Start the coroutine
        //selectionTimerCoroutine = StartCoroutine(SelectionProgress(totalTime));

        if (!voiceToChat.isRecording) 
            return;

        OnLoading();
    }

}
