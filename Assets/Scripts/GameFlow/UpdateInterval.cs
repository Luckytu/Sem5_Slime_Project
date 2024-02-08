using System.Collections;
using GameFlow.Interpolators;
using Global;
using UnityEngine;
using UnityEngine.Events;

namespace GameFlow
{
    public class UpdateInterval : Interval
    {
        [SerializeField] private UnityEvent<float>[] onUpdate;
        [SerializeField] private Interpolator[] interpolators;
        
        protected override IEnumerator interval()
        {
            while (remaining > 0)
            {
                if (GameState.state != GameState.Simulation || paused)
                {
                    yield return null;
                    continue;
                }
                
                yield return new WaitWhile(() => paused);

                float t = 1 - Mathf.Clamp01(remaining / duration);
                for (int i = 0; i < onUpdate.Length; i++)
                {
                    float it = interpolators[i].interpolate(t);
                    onUpdate[i].Invoke(it);
                }
                
                remaining -= Time.deltaTime;
                yield return null;
            }
            
            onFinished.Invoke();

            if (loopThis)
            {
                startInterval();
                yield break;
            }

            if (next != null)
            {
                next.startInterval();
            }
        }
    }
}