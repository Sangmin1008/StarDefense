using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TouchToScreenAnimator : MonoBehaviour
{
    [SerializeField] private float scaleSpeed = 1.0f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float minScale = 0.8f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        
        AnimateScaleAsync(token).Forget();
    }

    private async UniTaskVoid AnimateScaleAsync(CancellationToken token)
    {
        Vector3 startScale = Vector3.one * minScale;
        Vector3 endScale = Vector3.one * maxScale;

        try
        {
            while (!token.IsCancellationRequested)
            {
                await ScaleOverTimeAsync(startScale, endScale, token);
                await ScaleOverTimeAsync(endScale, startScale, token);
            }
        }
        catch (OperationCanceledException)
        {
#if UNITY_EDITOR
            Debug.Log("AnimateScaleAsync 종료");
#endif
        }
    }

    private async UniTask ScaleOverTimeAsync(Vector3 from, Vector3 to, CancellationToken token)
    {
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            rectTransform.localScale = Vector3.Lerp(from, to, elapsed);
            elapsed += Time.unscaledDeltaTime * scaleSpeed;

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        rectTransform.localScale = to;
    }
}