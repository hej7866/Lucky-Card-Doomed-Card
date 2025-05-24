using UnityEngine;
using DG.Tweening;
using TMPro;

public class Effect : MonoBehaviour
{
    [Header("데미지 텍스트")]
    public GameObject damageTextPrefab;   // TextMeshProUGUI 프리팹
    public Transform uiCanvas;            // World space 또는 overlay Canvas

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// UI 흔들림 연출 (패널 전체)
    /// </summary>
    public void PlayHitShake()
    {
        rect.DOShakeAnchorPos(0.3f, 10f, 10); // UI용 흔들기
    }

    /// <summary>
    /// 데미지 떠오르기 텍스트
    /// </summary>
    public void ShowDamageText(int damage)
    {
        if (damageTextPrefab == null || uiCanvas == null) return;

        GameObject dmg = Instantiate(damageTextPrefab, rect.position, Quaternion.identity, uiCanvas);
        var text = dmg.GetComponent<TextMeshProUGUI>();
        text.text = $"-{damage}";

        RectTransform dmgRt = dmg.GetComponent<RectTransform>();
        dmgRt.anchoredPosition = rect.anchoredPosition;

        dmgRt.DOAnchorPosY(dmgRt.anchoredPosition.y + 50f, 0.8f).SetEase(Ease.OutCubic);
        text.DOFade(0, 0.8f).OnComplete(() => Destroy(dmg));
    }

    public void PlayHitEffect(int damage)
    {
        PlayHitShake();
        ShowDamageText(damage);
    }
}
