using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using FireEscape.Action;
using MoreMountains.TopDownEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

namespace FireEscape.Turn
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        public int currentTurn;
        public List<IAffectedByTurn> AffectedObjects = new List<IAffectedByTurn>();

        public enum Phase
        {
            SearchPhase,
            ActionPhase,
        }
        public Phase currentPhase;

        public bool IsPhaseRunning { get; private set; }

        public UnityEvent OnSearchPhaseStart;
        public UnityEvent OnSearchPhaseEnd;
        public UnityEvent OnActionPhaseStart;
        public UnityEvent OnActionPhaseEnd;

        private void Awake()
        {
            //�̱��� ����
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // 1. TurnManager�� currentTurn�� 1��, currentPhase�� SearchPhase�� �ʱ�ȭ�մϴ�.
            currentTurn = 1;
            currentPhase = Phase.SearchPhase;
        }

        private void Start()
        {
            StartCoroutine(OnSearchPhase());
        }

        public void AdvancePhase()
        {
            /*if (IsPhaseRunning)
            {
                return;
            }*/

            //�˻� �ܰ迡�� �ൿ �ܰ�� �Ǵ� �� �ݴ�� ������ ��ȯ
            if (currentPhase == Phase.SearchPhase)
            {
                currentPhase = Phase.ActionPhase;
                StartCoroutine(OnActionPhase());
            }
            else if (currentPhase == Phase.ActionPhase)
            {
                currentTurn += 1;
                currentPhase = Phase.SearchPhase;
                StartCoroutine(OnSearchPhase());
            }
        }

        public void SeachPhaseExit()
        {
            if (IsPhaseRunning)
            {
                return;
            }
            if (currentPhase == Phase.SearchPhase)
            {
                StartCoroutine(OnExitSearchPhase());
                return;
            }
        }

        public void ActionPhaseExit()
        {
            if (IsPhaseRunning)
            {
                return;
            }
            if (currentPhase == Phase.ActionPhase)
            {
                StartCoroutine(OnExitActionPhase());
                return;
            }
        }
        
        public int GetCurrentTurn()
        {
            //���� �� ��ȣ�� ��ȯ
            return currentTurn;
        }

        public Phase GetCurrentPhase()
        {
            //���� ���� �ܰ踦 ��ȯ
            return currentPhase;
        }
        
        public void Register(IAffectedByTurn Obj)
        {
            //��Ͽ� ������Ʈ ���
            AffectedObjects.Add(Obj);
        }

        public void Unregister(IAffectedByTurn Obj)
        {
            //��Ͽ��� ��ü�� ����
            AffectedObjects.Remove(Obj);
        }

        public void ExecuteTurnMethods()
        {
            //��簴ü�� �ϰ��� �Լ� ȣ��
            for (int i = 0; i < AffectedObjects.Count; i++)
            {
                AffectedObjects[i].StartTurn();
                AffectedObjects[i].DuringTurn();
                AffectedObjects[i].EndTurn();
            }
        }
                
        private IEnumerator OnSearchPhase()
        {
            IsPhaseRunning = true;

            OnSearchPhaseStart.Invoke();

            // TimeScale�� 0.1�� ����
            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0.1f, 0.1f, false, 0f, true);

            yield return null;

            IsPhaseRunning = false;
        }

        private IEnumerator OnExitSearchPhase()
        {
            OnSearchPhaseEnd.Invoke();

            yield return null;

            AdvancePhase();
        }

        private IEnumerator OnActionPhase()
        {
            IsPhaseRunning = true;

            OnActionPhaseStart.Invoke();

            // TimeScale�� 1�� ����
            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 1f, 0.1f, false, 0f, true);

            ExecuteTurnMethods();
            AIActionManager.Instance.StartActionExecution();

            bool isActionRunning = true;

            if (AIActionManager.Instance.actionQueue.Count == 0)
            {
                isActionRunning = false;
            }

            AIActionManager.Instance.actionCompletedEvent.AddListener(() =>
            {
                isActionRunning = false;
            });

            while (isActionRunning)
            {
                yield return null;
            }

            yield return null;

            IsPhaseRunning = false;
        }

        private IEnumerator OnExitActionPhase()
        {
            OnActionPhaseEnd.Invoke();

            yield return null;

            AdvancePhase();
        }
    }
}