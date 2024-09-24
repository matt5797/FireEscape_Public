using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FireEscape.Chat;
using TMPro;
using YamlDotNet.Serialization;
using FireEscape.Action;

public class ChatTestScript
{
    [SetUp]
    public void SetupChatManager()
    {
        AIActionManager aiActionManager = new GameObject("AIActionManager").AddComponent<AIActionManager>();
        ChatManager chatManager = new GameObject("ChatManager").AddComponent<ChatManager>();

        aiActionManager.Awake();
        chatManager.Awake();
    }
    
    [TearDown]
    public void TearDownChatManager()
    {
        GameObject.DestroyImmediate(GameObject.FindObjectOfType<AIActionManager>().gameObject);
        GameObject.DestroyImmediate(GameObject.FindObjectOfType<ChatManager>().gameObject);
    }

    [Test]
    public void MessageCreationTest()
    {
        Message message = new Message(Message.Sender.Player, "Test message");

        Assert.AreEqual(Message.Sender.Player, message.sender);
        Assert.AreEqual("Test message", message.message);
    }

    [Test]
    public void SingletonInstanceTestForChatManager()
    {
        new GameObject("test1").AddComponent<ChatManager>();
        new GameObject("test2").AddComponent<ChatManager>();
        ChatManager chatManager1 = ChatManager.Instance;
        ChatManager chatManager2 = ChatManager.Instance;

        Assert.AreEqual(chatManager1, chatManager2);
    }

    [Test]
    public void PlayerMessageInputTest()
    {
        Message newMessage = new Message(Message.Sender.Player, "New Message");
        ChatManager.Instance.inputMessages.Add(newMessage);

        Assert.AreEqual(newMessage, ChatManager.Instance.inputMessages[ChatManager.Instance.inputMessages.Count - 1]);
    }

    [UnityTest]
    public IEnumerator PlayerSendDoneFunctionalityTest()
    {
        Message newMessage = new Message(Message.Sender.Player, "New Message");
        ChatManager.Instance.inputMessages.Add(newMessage);

        ChatManager.Instance.OnPlayerSendDone();

        yield return null;

        Assert.IsEmpty(ChatManager.Instance.inputMessages);
        Assert.IsInstanceOf(typeof(PlayerMessageGroup), ChatManager.Instance.messageGroups[ChatManager.Instance.messageGroups.Count - 1]);
    }

    [UnityTest]
    public IEnumerator ServerResponseHandlingTest()
    {
        DoNothing doNothing = new GameObject("DoNothing").AddComponent<DoNothing>();
        doNothing.ActionID = 0;
        doNothing.ActionName = "Do Nothing";

        string yamlResponse = @"ActionsToExecute:
    - ActionID: 0
      ActionName: Talk
      Message: are you talking to me?
    - ActionID: 0
      ActionName: Talk
      Message: I feel very good"; // Replace with a valid YAML string.

        ChatManager.Instance.OnServerResponseReceived(yamlResponse);

        yield return null;

        Assert.IsInstanceOf(typeof(ChildMessageGroup), ChatManager.Instance.messageGroups[ChatManager.Instance.messageGroups.Count - 1]);
    }

    // Test 6
    [Test]
    public void ChatUIMessageSendingTest()
    {
        // Arrange
        string expectedMessage = "Test message";
        ChatUI chatUI = new GameObject().AddComponent<ChatUI>();
        chatUI.chatInput = new GameObject().AddComponent<TMP_InputField>();
        chatUI.chatInput.text = expectedMessage;
        chatUI.playerMessagePrefab = new GameObject();
        MessageUI message = chatUI.playerMessagePrefab.AddComponent<MessageUI>();
        message.messageText = chatUI.playerMessagePrefab.AddComponent<TextMeshProUGUI>();

        // Act
        chatUI.OnSendButtonPressed();

        // Assert
        string lastMessage = ChatManager.Instance.inputMessages[ChatManager.Instance.inputMessages.Count - 1].message;
        Assert.AreEqual(expectedMessage, lastMessage);
    }

    // Test 7
    [Test]
    public void PlayerMessageGroupSerializationTest()
    {
        // Arrange
        PlayerMessageGroup playerMessageGroup = new PlayerMessageGroup();
        playerMessageGroup.CurrentState = "Current State Test";
        playerMessageGroup.ChildEmotion = new ChildEmotion(3);
        playerMessageGroup.SurroundingObjects = new List<SurroundingObject>() { new SurroundingObject("Type1", "Content1") };
        playerMessageGroup.PossibleActions = new List<PossibleAction>() { new PossibleAction(0, "ActionName1", "ActionDescription1", "ActionEffect1", 1) };

        // Act
        string yamlData = playerMessageGroup.SerializeToYAML();

        // Assert
        // Check if the yamlData contains all the key values. The actual yaml string may vary depending on the library used.
        Assert.IsTrue(yamlData.Contains(playerMessageGroup.CurrentState));
        Assert.IsTrue(yamlData.Contains(playerMessageGroup.ChildEmotion.AnxietyLevel.ToString()));
        // ... and so on for all properties
    }

    // Test 8
    [Test]
    public void ChildMessageGroupDeserializationTest()
    {
        // Arrange
        string yamlData = @"ActionsToExecute:
    - ActionID: 0
      ActionName: DoNothing
      Message: are you talking to me?
    - ActionID: 0
      ActionName: DoNothing
      Message: I feel very good";

        // Act
        ChildMessageGroup childMessageGroup = new ChildMessageGroup();
        childMessageGroup.DeserializeFromYAML(yamlData);

        // Assert
        Assert.IsNotNull(childMessageGroup.ActionsToExecute);
        Assert.AreEqual(2, childMessageGroup.ActionsToExecute.Count);

        // Assert the properties of each action
        Assert.AreEqual(0, childMessageGroup.ActionsToExecute[0].ActionID);
        Assert.AreEqual("DoNothing", childMessageGroup.ActionsToExecute[0].ActionName);
        Assert.AreEqual("are you talking to me?", childMessageGroup.ActionsToExecute[0].Message);

        Assert.AreEqual(0, childMessageGroup.ActionsToExecute[1].ActionID);
        Assert.AreEqual("DoNothing", childMessageGroup.ActionsToExecute[1].ActionName);
        Assert.AreEqual("I feel very good", childMessageGroup.ActionsToExecute[1].Message);
    }

    // Test 9
    [Test]
    public void ChildEmotionAnxietyLevelTest()
    {
        // Arrange
        int expectedAnxietyLevel = 5;

        // Act
        ChildEmotion childEmotion = new ChildEmotion(expectedAnxietyLevel);

        // Assert
        Assert.AreEqual(expectedAnxietyLevel, childEmotion.AnxietyLevel);
    }

    // Test 10
    [Test]
    public void MessageGroupMessageHandlingTest()
    {
        // Arrange
        string expectedMessage = "Test message";
        PlayerMessageGroup playerMessageGroup = new PlayerMessageGroup();
        playerMessageGroup.Messages.Add(expectedMessage);

        // Act
        string actualMessage = playerMessageGroup.Messages[playerMessageGroup.Messages.Count - 1];

        // Assert
        Assert.AreEqual(expectedMessage, actualMessage);
    }

    // Test 12
    [Test]
    public void MessageUIInitializationTest()
    {
        // Arrange
        MessageUI messageUI = new GameObject().AddComponent<MessageUI>();
        Message testMessage = new Message(Message.Sender.Player, "Test Message");
        messageUI.messageText = new GameObject().AddComponent<TextMeshProUGUI>();

        // Act
        messageUI.Initialize(testMessage);

        // Assert
        Assert.AreEqual(testMessage.message, messageUI.messageText.text);
    }

    // Test 13
    [Test]
    public void SurroundingObjectInitializationTest()
    {
        // Arrange
        string expectedObjectType = "Type1";
        string expectedContent = "Content1";

        // Act
        SurroundingObject surroundingObject = new SurroundingObject(expectedObjectType, expectedContent);

        // Assert
        Assert.AreEqual(expectedObjectType, surroundingObject.ObjectType);
        Assert.AreEqual(expectedContent, surroundingObject.Content);
    }

    // Test 14
    [Test]
    public void ActionToExecuteInitializationTest()
    {
        // Arrange
        int expectedActionID = 0;
        string expectedActionName = "ActionName1";
        string expectedMessage = "Message1";

        // Act
        ActionToExecute actionToExecute = new ActionToExecute(expectedActionID, expectedActionName, expectedMessage);

        // Assert
        Assert.AreEqual(expectedActionID, actionToExecute.ActionID);
        Assert.AreEqual(expectedActionName, actionToExecute.ActionName);
        Assert.AreEqual(expectedMessage, actionToExecute.Message);
    }
}
