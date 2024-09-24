using UnityEngine;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using FireEscape.Network;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UnityEngine.TestTools;

public class NetworkTestScript : MonoBehaviour
{
    private NetworkManager networkManager;

    public float maxWaitTime = 5f;

    // Set up before each test
    [SetUp]
    public void SetUp()
    {
        GameObject gameObject = new GameObject();
        networkManager = gameObject.AddComponent<NetworkManager>();
        networkManager.autoConnect = false;
        networkManager.autoReconnect = false;
    }

    [TearDown]
    public void TearDownChatManager()
    {
        GameObject.DestroyImmediate(GameObject.FindObjectOfType<NetworkManager>().gameObject);
    }

    // Test that the Connect method sets isConnected to true
    [UnityTest]
    public IEnumerator Connect_WhenCalled_SetsIsConnectedToTrue()
    {
        networkManager.Connect();

        float elapsedTime = 0f;
        while (!networkManager.isConnected && elapsedTime<maxWaitTime)
        {
            yield return null;
            elapsedTime += Time.fixedDeltaTime;
        }

        Assert.True(networkManager.isConnected);
    }

    // Test that the Disconnect method sets isConnected to false
    [UnityTest]
    public IEnumerator Disconnect_WhenCalled_SetsIsConnectedToFalse()
    {
        networkManager.Connect();

        float elapsedTime = 0f;
        while (!networkManager.isConnected && elapsedTime < maxWaitTime)
        {
            yield return null;
            elapsedTime += Time.fixedDeltaTime;
        }

        networkManager.Disconnect();

        elapsedTime = 0f;
        while (networkManager.isConnected && elapsedTime < maxWaitTime)
        {
            yield return null;
            elapsedTime += Time.fixedDeltaTime;
        }

        Assert.False(networkManager.isConnected);
    }

    [UnityTest]
    public IEnumerator MessageSendTest()
    {
        networkManager.Connect();

        float elapsedTime = 0f;
        while (!networkManager.isConnected && elapsedTime < maxWaitTime)
        {
            yield return null;
            elapsedTime += Time.fixedDeltaTime;
        }

        string data = "# A list of conversations you've had before this one.\r\nMessagesHistory:\r\n  - Message:\r\n      Sender: Child\r\n      ActionName: Talk\r\n      Content: I am Junwoo\r\n  - Message:\r\n      Sender: Player\r\n      ActionName: Chat\r\n      Content: Hello\r\n  - Message:\r\n      Sender: Child\r\n      ActionName: Talk\r\n      Content: Nice to meet you\r\n# A group of messages and information from the player.\r\nplayerMessageGroup:\r\n  # A description of the current situation.\r\n  CurrentState: Standing in a room\r\n  # A description of the surrounding objects. This is for reference only.\r\n  SurroundingObjects:\r\n    - Object:\r\n        Type: chair\r\n        Content: A wooden chair with a cushioned seat\r\n    - Object:\r\n        Type: bed\r\n        Content: A king-size bed with blue sheets\r\n  # A list of actions Chid can execute now.\r\n  # Child can select actions within actionWeight 5 and execute them sequentially.\r\n  # In the case of talk with actionName Talk, messages to be talked to should be written in order in messages.\r\n  # The number of Talk actions and the number of messages must be the same.\r\n  PossibleActions:\r\n    - ActionID: action01\r\n      ActionName: Talk\r\n      ActionDescription: Engage in conversation\r\n      ActionEffect: A child speaks a message. The message refers to the messages recorded in messages one by one from the top. To say multiple messages, you need to execute that number of actions.\r\n      ActionWeight: 1\r\n    - ActionID: action02\r\n      ActionName: Increase anxiety\r\n      ActionDescription: Engage in activities that increase anxiety\r\n      ActionEffect: Induces a state of heightened worry, nervousness, or unease. It can be caused by various stressors and may have negative effects on mental and physical well-being.\r\n      ActionWeight: 1\r\n    - ActionID: action03\r\n      ActionName: Decrease anxiety\r\n      ActionDescription: Engage in activities that decrease anxiety\r\n      ActionEffect: Promotes a sense of calm and relaxation, reducing feelings of tension, worry, and unease. It can involve techniques such as deep breathing, mindfulness, or engaging in soothing activities.\r\n      ActionWeight: 1\r\n    - ActionID: action04\r\n      ActionName: Nod head\r\n      ActionDescription: Nod the head in agreement\r\n      ActionEffect: Indicates agreement or understanding in a non-verbal manner. It can show active listening and affirm the speaker's words, promoting effective communication and rapport.\r\n      ActionWeight: 1\r\n    - ActionID: action05\r\n      ActionName: Open door\r\n      ActionDescription: Open a door\r\n      ActionEffect: Enables access to another space or area. Opening a door allows movement, facilitates transitions, and can symbolize an invitation or opportunity for exploration.\r\n      ActionWeight: 2\r\n  # The current emotional state of the Child. Expressed on a scale of 0-5.\r\n  Child's Emotion:\r\n    AnxietyLevel: 2\r\n  # New messages from the player. A maximum of three messages can be sent.\r\n  Messages:\r\n    - Message: How are you doing, Junwoo?";
        string action = "AI_Response_2";
        ChatRequestData sendData = new ChatRequestData(action, data, "[]");
        string jsonData = JsonUtility.ToJson(sendData);

        Assert.AreEqual(0, networkManager.history.Count);

        _ = networkManager.SendWebSocketMessage(jsonData);

        // Wait for onMessageReceive Invoke
        elapsedTime = 0f;
        while (networkManager.history.Count == 0 && elapsedTime<30)
        {
            yield return null;
            elapsedTime += Time.fixedDeltaTime;
        }
        yield return null;

        Assert.AreEqual(1, networkManager.history.Count);
    }
}
