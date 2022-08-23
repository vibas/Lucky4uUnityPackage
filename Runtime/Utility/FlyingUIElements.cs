using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FlyingUIElements : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] float distance;
    CanvasGroup canvasGroup => GetComponent<CanvasGroup>();
    private void OnEnable()
    {
        canvasGroup.alpha = 1;
        FlyUpAndFade();
    }

    void FlyUpAndFade()
    {
        transform.DOLocalMoveY(distance, duration).OnComplete(Reset);
        canvasGroup.DOFade(0, duration);
    }

    private void Reset()
    {
        transform.DOLocalMoveY(0, 0.1f);
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
}
