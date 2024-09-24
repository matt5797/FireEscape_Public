using FireEscape.Object;
using System.Collections;
using UnityEngine;

namespace FireEscape.Action
{
    [System.Serializable]
    public abstract class AIAction : MonoBehaviour
    {
        [HideInInspector]
        public ActionableLevelObject ActionableLevelObject;

        public int ActionID;

        [HideInInspector]
        public string name;
        [HideInInspector]
        public string description;
        [HideInInspector]
        public Parameters parameters = new Parameters();

        [HideInInspector]
        public string message;
        [HideInInspector]
        public bool isMessageAfterAction;

        public bool ActionInProgress { get; set; }
        public bool ActionCancelled { get; set; }

        public AIAction()
        {
            Initialization();
        }

        private void Start()
        {
            Register();

            ActionableLevelObject = GetComponentInParent<ActionableLevelObject>();
        }

        private void OnDestroy()
        {
            Deregister();
        }

        protected abstract void Initialization();

        public abstract bool CanExecute();

        public abstract IEnumerator Execute();

        public abstract void Cancel();

        public virtual void OnEnterAction()
        {
            ActionInProgress = true;
            ActionCancelled = false;
        }

        public virtual void OnExitAction()
        {
            ActionInProgress = false;
        }

        public void Register()
        {
            AIActionManager.Instance.RegisterAction(this);
        }

        public void Deregister()
        {
            AIActionManager.Instance.DeregisterAction(this);
        }
    }
}
