using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System;
using FireEscape.Network;
using FireEscape.Chat;
using MoreMountains.TopDownEngine;

namespace FireEscape.Action
{
    public class AIActionManager : MonoBehaviour
    {
        public static AIActionManager Instance { get; private set; }
        public List<AIAction> actions = new List<AIAction>();
        public Queue<AIAction> actionQueue = new Queue<AIAction>();
        public AIAction currentAction;

        public bool IsActionRunning { get; private set; }
        public bool IsWaitingAction { get; set; }

        public UnityEvent actionCompletedEvent;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void RegisterAction(AIAction action)
        {
            actions.Add(action);
        }

        public void DeregisterAction(AIAction action)
        {
            actions.Remove(action);
        }

        public List<AIAction> GetAvailableActions()
        {
            return actions.Where(action => action.CanExecute()).ToList();
        }

        public AIAction GetActionByID(int actionID)
        {
            return actions.Where(action => action.ActionID == actionID).FirstOrDefault();
        }

        public AIAction GetActionByInstanceID(int instanceID)
        {
            return actions.Where(action => action.GetInstanceID() == instanceID).FirstOrDefault();
        }

        public void EnqueueAction(AIAction action)
        {
            if (action.CanExecute())
            {
                actionQueue.Enqueue(action);
            }
        }

        public void StartActionExecution()
        {
            StartCoroutine(ExecuteActionsCoroutine());
        }

        private IEnumerator ExecuteActionsCoroutine()
        {
            IsActionRunning = true;
            LevelManager.Instance.Players[0].GetComponent<Character>()._animator.SetTrigger("StartWatching");
            LevelManager.Instance.Players[0].GetComponent<Character>()._animator.SetBool("IsWatching", true);

            Debug.Log("시작!");

            if (IsWaitingAction)
            {
                yield return new WaitUntil(() => !IsWaitingAction);
            }
            
            Debug.Log("끝!");

            LevelManager.Instance.Players[0].GetComponent<Character>()._animator.SetBool("IsWatching", false);

            yield return new WaitForSeconds(4);

            Debug.Log("찐끝!");

            while (actionQueue.Count > 0)
            {
                currentAction = actionQueue.Dequeue();

                if (currentAction.CanExecute())
                {
                    currentAction.OnEnterAction();

                    if (!currentAction.isMessageAfterAction && currentAction.message != null)
                    {
                        ChatManager.Instance.OnChildMessageReceived(currentAction.message);
                        yield return new WaitForSeconds(3);
                    }

                    yield return StartCoroutine(currentAction.Execute());

                    if (currentAction.isMessageAfterAction && currentAction.message != null)
                    {
                        ChatManager.Instance.OnChildMessageReceived(currentAction.message);
                        yield return new WaitForSeconds(3);
                    }

                    currentAction.OnExitAction();

                    if (currentAction.ActionCancelled)
                    {
                        // Handle cancellation logic here if necessary
                    }
                }
            }

            IsActionRunning = false;
            actionCompletedEvent.Invoke();
        }

        public void CancelCurrentAction()
        {
            if (currentAction != null && currentAction.ActionInProgress)
            {
                currentAction.Cancel();
                currentAction.ActionCancelled = true;
            }
        }

        public void CancelAllActions()
        {
            actionQueue.Clear();

            if (currentAction != null && currentAction.ActionInProgress)
            {
                currentAction.Cancel();
                currentAction.ActionCancelled = true;
            }
        }

        public void ClearActionQueue()
        {
            actionQueue.Clear();
        }

        public void OnChatRecive(ChatResponseData responseData)
        {
            //GetActionByInstanceID
            if (responseData != null)
            {
                AIAction action = GetActionByInstanceID(responseData.instanceID);
                if (action != null)
                {
                    action.message = responseData.argumentsObject.childMessage;
                    action.isMessageAfterAction = responseData.argumentsObject.isMessageAfterAction;
                    EnqueueAction(action);
                    //StartActionExecution();
                }
                IsWaitingAction = false;
            }
        }
    }
}
