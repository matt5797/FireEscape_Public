using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Events;
using System.Collections.Concurrent;

namespace FireEscape.Network
{
    [System.Serializable]
    public struct ChatRequestData
    {
        public string action;
        public string situation;
        public string actions;

        public ChatRequestData(string action, string situation, string actions)
        {
            this.action = action;
            this.situation = situation;
            this.actions = actions;
        }
    }

    [System.Serializable]
    public class ServerResponse
    {
        public string type;
        public string data;
    }

    [System.Serializable]
    public class ChatResponse
    {
        public string type;
        public ChatResponseData data;
    }

    [Serializable]
    public class ChatResponseData : ISerializationCallbackReceiver
    {
        public string name;
        public int instanceID;
        public string arguments;
        public Arguments argumentsObject;

        public void OnAfterDeserialize()
        {
            Debug.Log($"1111111111111111111111 {name}");
            instanceID = int.Parse(name.Split("___")[1]);
            name = name.Split("___")[0];
            Debug.Log($"22222222222222 {arguments}");
            argumentsObject = JsonUtility.FromJson<Arguments>(arguments);
            Debug.Log($"33333333333333 {argumentsObject}");
        }

        public void OnBeforeSerialize()
        {

        }
    }

    [Serializable]
    public class Arguments
    {
        public string childMessage;
        public bool isMessageAfterAction;
    }

    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance;

        private ClientWebSocket websocket;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        private bool connected = false;
        public bool isConnected { get => connected; private set => connected = value; }

        public List<string> history = new List<string>();

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private ConcurrentQueue<UnityAction> mainThreadActions = new ConcurrentQueue<UnityAction>();

        [Header("Settings")]
        public bool autoConnect = true;

        [Tooltip("Automatically reconnect when disconnected")]
        public bool autoReconnect = true;
        
        [Tooltip("Time to wait before trying to reconnect")]
        public float reconnectDelay = 5f;


        [Header("Network Settings")]
        [Tooltip("WebSocket URL")]
        public string url = "wss://0eamuo8da4.execute-api.ap-northeast-2.amazonaws.com/dev";

        [Header("Events")]
        public UnityEvent onConnected = new UnityEvent();
        public UnityEvent onDisconnected = new UnityEvent();
        public UnityEvent<ChatResponseData> onChatReceive = new UnityEvent<ChatResponseData>();
        public UnityEvent<string> onHintReceive = new UnityEvent<string>();

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void Start()
        {
            if (autoConnect)
                Connect();
        }

        private void Update()
        {
            // Execute all actions stored in the queue
            while (mainThreadActions.TryDequeue(out var action))
            {
                action.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            cancellationToken.Cancel();
            Task.Run(CloseWebSocket);
        }

        public void OnDestroy()
        {
            cancellationToken.Cancel();
            Task.Run(CloseWebSocket);
        }

        public async void Connect()
        {
            if (!connected)
            {
                await ConnectWebSocket();
                if (connected) // If connection is successful
                {
                    history.Clear();
                    Task.Run(ReceiveWebSocketMessage);
                    onConnected?.Invoke();
                }
            }
        }

        public void SendAIResponce(string message, string actions)
        {
            ChatRequestData sendData = new ChatRequestData("response", message, actions);
            string jsonData = JsonUtility.ToJson(sendData);

            Task.Run(() => SendWebSocketMessage(jsonData));
        }

        public void Disconnect()
        {
            if (connected)
                Task.Run(CloseWebSocket);
        }

        private async Task ConnectWebSocket()
        {
            await semaphoreSlim.WaitAsync();

            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                Debug.Log("WebSocket is already open.");
                semaphoreSlim.Release();
                return;
            }

            websocket = new ClientWebSocket();
            Uri endpointUri = new Uri(url);

            try
            {
                await websocket.ConnectAsync(endpointUri, cancellationToken.Token);
                Debug.Log("WebSocket connected.");
                isConnected = true;
            }
            catch (Exception ex)
            {
                isConnected = false;
                Debug.LogException(ex);

                if (autoReconnect)
                {
                    await Task.Delay(TimeSpan.FromSeconds(reconnectDelay));
                    await ConnectWebSocket();
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task ReconnectOrCloseWebSocket()
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                if (websocket != null &&
                    (websocket.State == WebSocketState.Open
                     || websocket.State == WebSocketState.CloseReceived
                     || websocket.State == WebSocketState.Aborted))  // Reflecting Improvement 4
                {
                    await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken.Token);
                    isConnected = false;

                    if (autoReconnect)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(reconnectDelay));
                        await ConnectWebSocket();  // Reflecting Improvement 3
                    }
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task ReceiveWebSocketMessage()
        {
            byte[] buffer = new byte[1024]; // Increase buffer size to 1024 bytes (1 KB)
            WebSocketReceiveResult result;

            while (websocket.State == WebSocketState.Open)
            {
                var segments = new List<ArraySegment<byte>>();
                do
                {
                    result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken.Token);
                    segments.Add(new ArraySegment<byte>(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                byte[] messageBytes = new byte[segments.Sum(e => e.Count)];
                int offset = 0;
                foreach (ArraySegment<byte> segment in segments)
                {
                    Buffer.BlockCopy(segment.Array, 0, messageBytes, offset, segment.Count);
                    offset += segment.Count;
                }

                string message = Encoding.UTF8.GetString(messageBytes);

                ServerResponse messageObj = JsonUtility.FromJson<ServerResponse>(message);

                if (messageObj.type == "chat")
                {
                    ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(message);

                    //Debug.Log(chatResponse.type + ", "+ chatResponse.data.name + ", " + chatResponse.data.arguments.message);

                    mainThreadActions.Enqueue(() => {
                        onChatReceive?.Invoke(chatResponse.data);
                    });
                }
                else if (messageObj.type == "hint")
                {
                    /*mainThreadActions.Enqueue(() => {
                        onHintReceive?.Invoke(messageObj.data);
                    });*/
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    history.Add(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ReconnectOrCloseWebSocket();
                }
            }
        }

        public async Task SendWebSocketMessage(string message)
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                if (websocket == null || websocket.State != WebSocketState.Open)
                {
                    Debug.Log("WebSocket is not open.");
                    return;
                }

                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await websocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken.Token);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task CloseWebSocket()
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                if (websocket != null && websocket.State == WebSocketState.Open)
                {
                    await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken.Token);
                    isConnected = false;
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

    }
}