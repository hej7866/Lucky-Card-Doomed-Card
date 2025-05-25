using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Photon.Pun;

public class BulletEffect : MonoBehaviour
{
    [Header("탄환 프리팹")]
    public GameObject bulletPrefab;

    [Header("총알 박스")]
    [SerializeField] private RectTransform bulletContainer;

    /// <summary>
    /// start에서 end로 탄환 발사
    /// </summary>
    public void FireBullet(RectTransform start, RectTransform end, int bulletCount, float delay = 0.1f)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float currentDelay = i * delay;

            DOVirtual.DelayedCall(currentDelay, () =>
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletContainer);
                bullet.transform.SetSiblingIndex(0); // 가장 아래로 이동

                RectTransform bulletRt = bullet.GetComponent<RectTransform>();

                Vector2 startPos = start.anchoredPosition;
                Vector2 endPos = end.anchoredPosition;

                bulletRt.anchoredPosition = startPos;

                Vector2 dir = endPos - startPos;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                bulletRt.rotation = Quaternion.Euler(0, 0, angle);

                bulletRt.DOAnchorPos(endPos, 0.6f).SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        end.DOShakeAnchorPos(0.2f, 10f, 8);
                        Destroy(bullet);
                    });
            });
        }
    }

}
