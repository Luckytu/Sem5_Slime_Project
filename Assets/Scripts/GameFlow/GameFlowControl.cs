using UnityEngine;
using UnityEngine.Events;

namespace GameFlow
{
    public abstract class GameFlowControl : MonoBehaviour
    {
        [SerializeField] protected UnityEvent onExecutionStart;
        [SerializeField] protected UnityEvent onExecutionFinished;
        
        public void execute()
        {
            onExecutionStart.Invoke();
            
            executeGameFlow();
        }
        
        protected abstract void executeGameFlow();
    }
}