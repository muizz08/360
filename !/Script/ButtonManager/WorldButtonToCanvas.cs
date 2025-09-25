using UnityEngine;
using UnityEngine.UI;

public class WorldToCanvasButton : MonoBehaviour
{
    public Transform worldTargetNext;
    public Transform worldTargetPopup;
    public RectTransform buttonNext;
    public RectTransform buttonPopup;
    public Camera mainCamera;

    void Update()
    {
        if (worldTargetNext == null || buttonNext == null) return;
        if (worldTargetPopup == null || buttonPopup == null) return;

        
        Vector3 screenPosNext = mainCamera.WorldToScreenPoint(worldTargetNext.position);
        Vector3 screenPosPopup = mainCamera.WorldToScreenPoint(worldTargetPopup.position);

        
        if (screenPosNext.z > 0)
        {
            buttonNext.gameObject.SetActive(true);
            buttonNext.position = screenPosNext;
        }
        else
        {
            buttonNext.gameObject.SetActive(false);
        }

        
        if (screenPosPopup.z > 0)
        {
            buttonPopup.gameObject.SetActive(true);
            buttonPopup.position = screenPosPopup;
        }
        else
        {
            buttonPopup.gameObject.SetActive(false);
        }
    }
}
