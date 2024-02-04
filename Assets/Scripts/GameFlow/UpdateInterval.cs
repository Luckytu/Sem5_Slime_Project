using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameFlow
{
    public class UpdateInterval : Interval
    {
        [SerializeField] private UnityEvent<float> onUpdate;
        
        protected override IEnumerator interval()
        {
            while (remaining > 0)
            {
                yield return new WaitWhile(() => paused);
                onUpdate.Invoke(Mathf.Clamp01(1 - (remaining / duration)));
                remaining -= Time.deltaTime;
                yield return null;
            }
            
            onFinished.Invoke();
            if (next != null)
            {
                next.startInterval();
            }
        }
    }
}