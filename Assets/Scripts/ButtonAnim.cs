using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonAnim : MonoBehaviour
{
    [SerializeField] private RectTransform playButton;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutBack;

    [Header("Pulse Animation")]
    [SerializeField] private float pulseScale = 1.1f; 
    [SerializeField] private float pulseDuration = 0.6f; 
    [SerializeField] private Ease pulseEaseType = Ease.InOutSine;

    private Button button;

    private void Start()
    {
        button = playButton.GetComponent<Button>();
        button.interactable = false;

        if (button != null)
        {
            playButton.localScale = Vector3.zero;


            playButton.DOScale(Vector3.one, animationDuration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    button.interactable = true;
                    StartPulseAnimation();
                });
        }
    }

    private void StartPulseAnimation()
    {
        if (button != null)
        {
            playButton.DOScale(Vector3.one * pulseScale, pulseDuration)
                .SetEase(pulseEaseType)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
