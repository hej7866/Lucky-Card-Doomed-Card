using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BulletEffect : MonoBehaviour
{
    [Header("탄환 프리팹")]
    public GameObject bulletPrefab;

    [Header("UI 캔버스")]
    public RectTransform uiCanvas;

    /// <summary>
    /// start에서 end로 탄환 발사
    /// </summary>
    public void FireBullet(RectTransform start, RectTransform end, int bulletCount = 1, float delay = 0.1f)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float currentDelay = i * delay;

            DOVirtual.DelayedCall(currentDelay, () =>
            {
                GameObject bullet = Instantiate(bulletPrefab, uiCanvas);
                RectTransform bulletRt = bullet.GetComponent<RectTransform>();

                // 위치 초기화
                bulletRt.anchoredPosition = start.anchoredPosition;

                // 이동
                bulletRt.DOAnchorPos(end.anchoredPosition, 0.6f).SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    // 피격 연출
                    end.DOShakeAnchorPos(0.2f, 10f, 8);
                    Destroy(bullet);
                });
            });
        }
    }
}
