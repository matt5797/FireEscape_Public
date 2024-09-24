using System.Collections;
using UnityEngine;

namespace FireEscape.Action
{
    public class SampleAIAction : AIAction
    {
        protected override void Initialization()
        {
            ActionID = 9990;
            name = "Sample AI Action";
            description = "This is a sample AI action.";
            parameters = new Parameters
            {
                type = "object",
                properties = new Properties
                {
                    childMessage = new Message()
                }
            };
        }

        public override bool CanExecute()
        {
            // Sample criteria to determine if the action can be executed
            return true;
        }

        public override IEnumerator Execute()
        {
            // Example execution code (waits for 2 seconds)
            Debug.Log("Sample AI Action started");
            yield return new WaitForSeconds(2f);
            Debug.Log("Sample AI Action completed");
        }

        public override void Cancel()
        {
            // Example cancellation code
            Debug.Log("Sample AI Action cancelled");
        }
    }
}
