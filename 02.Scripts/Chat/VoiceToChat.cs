using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LeastSquares.Undertone;
using System.Collections.Generic;
using UnityEngine.Events;
using Oculus.Interaction;
using System.Collections.Concurrent;
using FireEscape.Network;
using System.Threading;

namespace FireEscape.Chat
{
    /// <summary>
    /// Class responsible for managing the recording button and transcribing audio using a SpeechEngine.
    /// </summary>
    public class VoiceToChat : MonoBehaviour
    {
        [SerializeField] public SpeechEngine Engine;
        [SerializeField] public TMP_Text transcriptionText;
        [SerializeField] public int MaxRecordingTime = 100;

        private CrossPlatformAudioClip _clip;
        [HideInInspector]
        public bool isRecording;
        [HideInInspector]
        public bool isTranscribing;

        private bool _isPlaying;

        private float _recordingInitTime;
        private float _recordingStartTime;
        private float _recordingEndTime;

        private Task TranscribeTask;

        public UnityEvent onStartRecording = new UnityEvent();
        public UnityEvent onStopRecording = new UnityEvent();
        public UnityEvent onTranscriptionStart = new UnityEvent();
        public UnityEvent onTranscriptionDone = new UnityEvent();
        public UnityEvent<string> onTranscription = new UnityEvent<string>();

        private ConcurrentQueue<UnityAction> mainThreadActions = new ConcurrentQueue<UnityAction>();

        public Papago papago;

        private void Start()
        {
            Engine.NoContext = true;
            //StartRecording();

            _recordingInitTime = Time.realtimeSinceStartup;
            _clip = CrossPlatformMicrophone.Start(null, false, 1800, SpeechEngine.SampleFrequency);
        }

        private void Update()
        {
            // Execute all actions stored in the queue
            while (mainThreadActions.TryDequeue(out var action))
            {
                action.Invoke();
            }

            if (isTranscribing && TranscribeTask.IsCompleted)
            {
                TranscribeTask = null;
                isRecording = false;
                isTranscribing = false;
                onTranscriptionDone?.Invoke();
            }

#if UNITY_EDITOR
            // if Space is pressed, stop recording
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space))
            {
                OnEndPose();
            }
            // if Space is pressed, start recording
            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space))
            {
                OnStartPose();
            }
#endif
        }

        public void WhenRelease(PointerEvent pointerEvent)
        {
            //OnClicked();
        }

        public void OnStartPose()
        {
            if (!isRecording)
            {
                //Debug.Log("Start recording");
                StartRecording();
                isRecording = true;
            }
        }

        public void OnEndPose()
        {
            if (isRecording && !isTranscribing)
            {
                Debug.Log("OnEndPose");
                isTranscribing = true;
                //Task.Run(async () => await OnEndPoseAsync());
                StopRecording();
            }
        }

        /*public async Task OnEndPoseAsync()
        {
            Debug.Log("Stop recording");
            List<string> transcriptions = StopRecording();
        }*/

        /// <summary>
        /// Starts recording audio from the microphone.
        /// </summary>
        private void StartRecording()
        {
            //_clip = CrossPlatformMicrophone.Start(null, false, MaxRecordingTime, SpeechEngine.SampleFrequency);
            _recordingStartTime = Time.realtimeSinceStartup;

            onStartRecording.Invoke();
        }

        /// <summary>
        /// Stops recording audio and transcribes it using a SpeechEngine.
        /// </summary>
        /// <returns>A string with the transcribed audio segments and their respective timestamps.</returns>
        private void StopRecording()
        {
            onStopRecording?.Invoke();
            onTranscriptionStart?.Invoke();

            _recordingEndTime = Time.realtimeSinceStartup;

            float recordingTime = _recordingEndTime - _recordingStartTime;
            Debug.Log("Recording time: " + recordingTime);

            int offset = (int)(_recordingStartTime - _recordingInitTime) * SpeechEngine.SampleFrequency;
            int length = (int)(recordingTime * SpeechEngine.SampleFrequency);

            TranscribeTask = Engine.TranscribeClip(_clip, offset, length, TranscribeCallback);

            //TranscribeTask = Engine.TranscribeClip(_clip, 0, CrossPlatformMicrophone.GetPosition(null), TranscribeCallback);
            //CrossPlatformMicrophone.End(null);
        }

        private void TranscribeCallback(SpeechSegment NewSegment)
        {
            string result = papago.Translate(NewSegment.text);

            mainThreadActions.Enqueue(() => {
                //onTranscription.Invoke(NewSegment.text);
                onTranscription.Invoke(result);
            });
        }
    }
}
