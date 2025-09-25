using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public float animationDuration = 0.3f;
    public float scaleFactor = 1.1f;

    private Tween currentTween;
    private Vector3 initialScale;

    void Awake()
    {
        initialScale = popupPanel.transform.localScale;
        popupPanel.transform.localScale = Vector3.zero;
        popupPanel.SetActive(false);
        Debug.Log("PopupManager: Awake - Panel diinisialisasi dan disembunyikan.");
    }

    void Update()
    {
        if (popupPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                if (!IsPointerOverUI(Input.mousePosition))
                {
                    Debug.Log("PopupManager: Klik terdeteksi di luar UI. Menutup panel.");
                    HideWithAnimation();
                }
                else
                {
                    Debug.Log("PopupManager: Klik terdeteksi di atas UI. Tidak melakukan apa-apa.");
                }
            }
        }
    }

    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        bool isOverUI = results.Count > 0;
        Debug.Log("PopupManager: IsPointerOverUI() dipanggil. Hasil: " + isOverUI + " (Elemen UI yang terdeteksi: " + results.Count + ")");
        return isOverUI;
    }

    public void TogglePopup()
    {
        Debug.Log("PopupManager: TogglePopup() dipanggil.");
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
            Debug.Log("PopupManager: Tween yang sedang berjalan dihentikan.");
        }

        if (popupPanel.activeSelf)
        {
            Debug.Log("PopupManager: Panel aktif, memanggil HideWithAnimation().");
            HideWithAnimation();
        }
        else
        {
            Debug.Log("PopupManager: Panel tidak aktif, memanggil ShowWithAnimation().");
            ShowWithAnimation();
        }
    }

    public void ShowWithAnimation()
    {
        Debug.Log("PopupManager: ShowWithAnimation() dipanggil. Mengaktifkan panel.");
        popupPanel.SetActive(true);

        currentTween = popupPanel.transform.DOScale(initialScale * scaleFactor, animationDuration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                Debug.Log("PopupManager: Animasi Show pertama selesai. Memulai animasi kedua.");
                popupPanel.transform.DOScale(initialScale, animationDuration / 2).SetEase(Ease.InQuad);
            });
    }

    public void HideWithAnimation()
    {
        Debug.Log("PopupManager: HideWithAnimation() dipanggil. Menyembunyikan panel.");
        currentTween = popupPanel.transform.DOScale(0f, animationDuration).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                Debug.Log("PopupManager: Animasi Hide selesai. Menonaktifkan panel.");
                popupPanel.SetActive(false);
            });
    }
}