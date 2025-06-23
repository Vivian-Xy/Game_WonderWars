using UnityEngine;
using DG.Tweening;

/// <summary>
/// Plays a little pop-in animation when a card becomes active.
/// Attach to your ProgressCard prefab.
/// </summary>
public class CardAnimator : MonoBehaviour
{
    void OnEnable()
    {
        // Reset scale to zero
        transform.localScale = Vector3.zero;
        // Tween up to full size over 0.3 seconds with an Overshoot
        transform.DOScale(1f, 0.3f)
                 .SetEase(Ease.OutBack);
    }
}