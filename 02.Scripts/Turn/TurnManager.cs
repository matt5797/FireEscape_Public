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
            //싱글톤 패턴
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // 1. TurnManager는 currentTurn을 1로, currentPhase를 SearchPhase로 초기화합니다.
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

            //검색 단계에서 행동 단계로 또는 그 반대로 게임을 전환
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
            //현재 턴 번호를 반환
            return currentTurn;
        }

        public Phase GetCurrentPhase()
        {
            //현재 게임 단계를 반환
            return currentPhase;
        }
        
        public void Register(IAffectedByTurn Obj)
        {
            //목록에 오브젝트 등록
            AffectedObjects.Add(Obj);
        }

        public void Unregister(IAffectedByTurn Obj)
        {
            //목록에서 객체를 제거
            AffectedObjects.Remove(Obj);
        }

        public void ExecuteTurnMethods()
        {
            //모든객체의 턴관련 함수 호출
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

            // TimeScale을 0.1로 설정
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

            // TimeScale을 1로 설정
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