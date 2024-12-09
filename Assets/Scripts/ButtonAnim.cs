using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonAnim : MonoBehaviour
{
    [SerializeField] private RectTransform playButton;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutBack;

    [Header("Pulse Animation")]
    [SerializeField] private float pulseScale = 1.1f; // Ukuran maksimum saat denyutan
    [SerializeField] private float pulseDuration = 0.6f; // Durasi denyutan
    [SerializeField] private Ease pulseEaseType = Ease.InOutSine;

    private Button button;

    private void Start()
    {
        button = playButton.GetComponent<Button>();
        button.interactable = false; // Nonaktifkan tombol saat animasi berjalan

        playButton.localScale = Vector3.zero;

        // Animasi pop-up
        playButton.DOScale(Vector3.one, animationDuration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                button.interactable = true; // Aktifkan tombol setelah animasi selesai
                StartPulseAnimation(); // Mulai animasi denyut
            });
    }

    private void StartPulseAnimation()
    {
        playButton.DOScale(Vector3.one * pulseScale, pulseDuration)
            .SetEase(pulseEaseType)
            .SetLoops(-1, LoopType.Yoyo); // Animasi loop denyut (Yoyo)
    }
}
