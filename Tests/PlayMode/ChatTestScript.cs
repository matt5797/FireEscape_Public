using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FireEscape.Chat;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using YamlDotNet.Serialization;
using FireEscape.Action;

public class ChatTestScript
{
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("ChatTestScene", LoadSceneMode.Single);
    }

    [TearDown]
    public void Teardown()
    {
        
    }

    // Example of Singleton Test
    [UnityTest]
    public IEnumerator ChatManagerSingletonTest()
    {
        var firstChatManager = ChatManager.Instance;
        var secondChatManager = ChatManager.Instance;

        Assert.That(firstChatManager, Is.EqualTo(secondChatManager));

        yield return null;
    }

    // Example of Message Sending Test
    [UnityTest]
    public IEnumerator MessageSendingTest()
    {
        var chatManager = ChatManager.Instance;

        string testMessage = "Test Message";

        // find ChatUI Script from Scene
        ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();

        chatUI.chatInput.text = testMessage;
        chatUI.sendButton.onClick.Invoke();

        // wait for message to be sent
        yield return null;

        Assert.That(chatManager.inputMessages[chatManager.inputMessages.Count - 1].message, Is.EqualTo(testMessage));

        chatUI.doneButton.onClick.Invoke();

        // wait for message to be done
        yield return null;

        Assert.That(chatManager.messagesLog[chatManager.messagesLog.Count - 1].message, Is.EqualTo(testMessage));
        Assert.IsTrue(chatManager.inputMessages.Count == 0);

        yield return null;
    }

    // Example of Message Display Test
    [UnityTest]
    public IEnumerator MessageDisplayTest()
    {
        var chatManager = ChatManager.Instance;

        string testMessage = "Test Message";

        // find ChatUI Script from Scene
        ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();

        chatUI.chatInput.text = testMessage;
        chatUI.sendButton.onClick.Invoke();

        MessageUI[] messageUIs = chatUI.messageContent.GetComponentsInChildren<MessageUI>();

        // Assuming lastMessageText is a Text component displaying the last sent message
        Assert.AreEqual(testMessage, messageUIs[messageUIs.Length - 1].messageText.text);

        yield return null;
    }

    // Example of Message Grouping Test
    [UnityTest]
    public IEnumerator MessageGroupingTest()
    {
        ChatManager chatManager = ChatManager.Instance;

        // find ChatUI Script from Scene
        ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();

        // Simulate sending multiple messages
        chatUI.chatInput.text = "Test Message 1";
        chatUI.sendButton.onClick.Invoke();
        chatUI.chatInput.text = "Test Message 2";
        chatUI.sendButton.onClick.Invoke();

        chatManager.OnPlayerSendDone();

        Assert.IsTrue(chatManager.messageGroups[chatManager.messageGroups.Count - 1].Messages.Contains("Test Message 1"));
        Assert.IsTrue(chatManager.messageGroups[chatManager.messageGroups.Count - 1].Messages.Contains("Test Message 2"));

        yield return null;
    }

    // Server Response Test
    [UnityTest]
    public IEnumerator ServerResponseTest()
    {
        var chatManager = ChatManager.Instance;
        string mockServerResponse = @"ActionsToExecute:
    - ActionID: 0
      ActionName: DoNothing
      Message: are you talking to me?
    - ActionID: 0
      ActionName: DoNothing
      Message: I feel very good"; // Replace with a valid YAML string.

        chatManager.OnChildMessageReceived(mockServerResponse);

        Assert.IsTrue(chatManager.messageGroups.Count > 0);
        Assert.IsTrue(chatManager.messageGroups[0].Messages.Count > 1);

        yield return null;
    }

    // Emotion and Object Serialization Test
    [UnityTest]
    public IEnumerator EmotionAndObjectSerializationTest()
    {
        var chatManager = ChatManager.Instance;

        PlayerMessageGroup playerMessageGroup = new PlayerMessageGroup();

        playerMessageGroup.ChildEmotion = new ChildEmotion(3);
        playerMessageGroup.SurroundingObjects = new List<SurroundingObject>();
        playerMessageGroup.SurroundingObjects.Add(new SurroundingObject("ball", "this is ball"));
        playerMessageGroup.SurroundingObjects.Add(new SurroundingObject("chair", "this is chair"));

        string serializedYAML = playerMessageGroup.SerializeToYAML();
        var deserializer = new DeserializerBuilder().Build();
        var yamlObject = deserializer.Deserialize<PlayerMessageGroup>(serializedYAML);

        Assert.AreEqual(3, yamlObject.ChildEmotion.AnxietyLevel);
        Assert.IsTrue(yamlObject.SurroundingObjects[0].ObjectType.Equals("ball"));
        Assert.IsTrue(yamlObject.SurroundingObjects[1].ObjectType.Equals("chair"));

        yield return null;
    }

    // Action Execution Test
    [UnityTest]
    public IEnumerator ActionExecutionTest()
    {
        var chatManager = ChatManager.Instance;
        string mockServerResponse = @"ActionsToExecute:
    - ActionID: 0
      ActionName: DoNothing
      Message: are you talking to me?"; // Replace with a valid YAML string.

        chatManager.OnChildMessageReceived(mockServerResponse);
        Assert.IsTrue(AIActionManager.Instance.actionQueue.Count > 0);
        AIActionManager.Instance.StartActionExecution();
        // You will need to verify actions based on what actions you're testing
        Assert.IsTrue(AIActionManager.Instance.actionQueue.Count == 0);

        yield return null;
    }

    // UI Button Activation Test
    [UnityTest]
    public IEnumerator UIButtonActivationTest()
    {
        var chatManager = ChatManager.Instance;
        var wasSendButtonClicked = false;

        // find ChatUI Script from Scene
        ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();

        // You might need a mock for the actual button click, this is a simplification
        chatUI.sendButton.onClick.AddListener(() => wasSendButtonClicked = true);

        // Simulate sending multiple messages
        chatUI.chatInput.text = "Test Message 1";
        chatUI.sendButton.onClick.Invoke();

        Assert.IsTrue(wasSendButtonClicked);

        yield return null;
    }

    // UI Input Field Test
    [UnityTest]
    public IEnumerator UIInputFieldTest()
    {
        var chatManager = ChatManager.Instance;

        // find ChatUI Script from Scene
        ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();

        chatUI.chatInput.text = "Test Input";

        Assert.AreEqual("Test Input", chatUI.chatInput.text);

        yield return null;
    }

    // Chat Message Prefab Instantiation Test
    [UnityTest]
    public IEnumerator ChatMessagePrefabInstantiationTest()
    {
        var chatManager = ChatManager.Instance;

        // find ChatUI Script from Scene
        ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();

        // Send a player message and a server response
        chatUI.chatInput.text = "Test Message";
        chatUI.sendButton.onClick.Invoke();

        string mockServerResponse = @"ActionsToExecute:
    - ActionID: 0
      ActionName: DoNothing
      Message: are you talking to me?"; // Replace with a valid YAML string.
        chatManager.OnChildMessageReceived(mockServerResponse);

        AIActionManager.Instance.StartActionExecution();

        yield return new WaitWhile(() => AIActionManager.Instance.IsActionRunning);

        // Assuming the messageContent object contains all the instantiated message prefabs
        Assert.IsTrue(chatUI.messageContent.GetComponentsInChildren<MessageUI>().Length > 1);

        yield return null;
    }

    // Scrolling Test
    [UnityTest]
    public IEnumerator ScrollingTest()
    {
        var chatManager = ChatManager.Instance;

        // find ChatUI Script from Scene
        ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();

        // This assumes you have more messages to send than the capacity of the chat window
        for (int i = 0; i < 100; i++)
        {
            chatUI.chatInput.text = $"Test Message {i}";
            chatUI.sendButton.onClick.Invoke();
        }

        yield return new WaitForSeconds(0.1f);  // Allow time for scrolling to occur

        // Verify scroll position, the exact method will depend on how you've implemented scrolling
        Assert.AreEqual(0, chatUI.GetComponentInChildren<ScrollRect>().normalizedPosition.y);

        yield return null;
    }

    // Multiple Message Ordering Test
    [UnityTest]
    public IEnumerator MultipleMessageOrderingTest()
    {
        var chatManager = ChatManager.Instance;

        // find ChatUI Script from Scene
        ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();

        string mockResponse1 = @"ActionsToExecute:
    - ActionID: 0
      ActionName: DoNothing
      Message: Test response 1";

        string mockResponse2 = @"ActionsToExecute:
    - ActionID: 0
      ActionName: DoNothing
      Message: Test response 2";

        // Send multiple player messages and server responses, each with a unique string
        chatUI.chatInput.text = "Test Message 1";
        chatUI.sendButton.onClick.Invoke();
        chatManager.OnChildMessageReceived(mockResponse1);
        AIActionManager.Instance.StartActionExecution();

        chatUI.chatInput.text = "Test Message 2";
        chatUI.sendButton.onClick.Invoke();
        chatManager.OnChildMessageReceived(mockResponse2);
        AIActionManager.Instance.StartActionExecution();

        yield return new WaitWhile(() => AIActionManager.Instance.IsActionRunning);

        // Verify message order in UI. The exact method will depend on how you've implemented your UI
        // For example, you might retrieve all message prefabs and check their order
        var messagePrefabs = chatUI.messageContent.GetComponentsInChildren<MessageUI>();
        Assert.AreEqual("Test Message 1", messagePrefabs[0].messageText.text);
        Assert.AreEqual("Test response 1", messagePrefabs[1].messageText.text);
        Assert.AreEqual("Test Message 2", messagePrefabs[2].messageText.text);
        Assert.AreEqual("Test response 2", messagePrefabs[3].messageText.text);

        yield return null;
    }

    // Child Emotion Level Test
    [UnityTest]
    public IEnumerator ChildEmotionLevelTest()
    {
        var chatManager = ChatManager.Instance;

        // Trigger an action that should change child's emotion level
        // chatManager.TriggerAction( ... );

        // Verify that emotion level has changed as expected
        // Assert.AreEqual(expectedEmotionLevel, chatManager.childEmotionLevel);

        yield return null;
    }
}
