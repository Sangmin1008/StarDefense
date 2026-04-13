using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ProgressTweener
{
    private float progressRatio;
    private AnimationCurve runningCurve;
    
    private CancellationTokenSource cts; 

    public ProgressTweener()
    {
    }
    
    public ProgressTweener SetCurve(AnimationCurve curve)
    {
        runningCurve = curve;
        return this;
    }

    public ProgressTweener Play(UnityAction<float> onUpdate, float duration, UnityAction onComplete = null, bool ignoreTimeScale = false)
    {
        Stop();

        cts = new CancellationTokenSource();
        
        TweenAsync(onUpdate, duration, ignoreTimeScale, onComplete, cts.Token).Forget();
        
        return this;
    }

    public void Stop()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
    }

    private async UniTaskVoid TweenAsync(UnityAction<float> onUpdate, float duration, bool ignoreTimeScale, UnityAction onComplete, CancellationToken token)
    {
        try
        {
            float time = progressRatio * duration;
            while (time < duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                time += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                
                float t = Mathf.Clamp01(time / duration);
                progressRatio = runningCurve != null ? runningCurve.Evaluate(t) : t;
                
                onUpdate?.Invoke(progressRatio);
            }
            
            progressRatio = 1f;
            onUpdate?.Invoke(progressRatio); 
            runningCurve = null;
            progressRatio = 0f;
            onComplete?.Invoke(); 
        }
        catch (OperationCanceledException)
        {
#if UNITY_EDITOR
            Debug.Log("TweenAsync 종료");
#endif
        }
    }
}