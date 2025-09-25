using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Label lokal untuk cocokkan dengan Strapi")]
    public string localLabel;  

    private ButtonData data;
    private Button button;
    public Camera targetCamera;   // assign Main Camera
    public float rotateSpeed = 50f;

    private bool rotateLeft;
    private bool rotateRight;

    void Update()
    {
        if (targetCamera == null) return;

        if (rotateLeft)
        {
            targetCamera.transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime, Space.World);
        }

        if (rotateRight)
        {
            targetCamera.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }
    }
    public void StartRotateLeft() => rotateLeft = true;
    public void StopRotateLeft() => rotateLeft = false;

    public void StartRotateRight() => rotateRight = true;
    public void StopRotateRight() => rotateRight = false;


    public void Init(ButtonData data)
    {
        this.data = data;

        Debug.Log($"[DEBUG] Init ButtonManager | Label={data.Label}");

        float posX = data.Position != null ? data.Position.PositionX : 0f;
        float posY = data.Position != null ? data.Position.PositionY : 0f;
        float dist = data.Position != null ? data.Position.Distance : 0f;

        // kalau ada RectTransform (UI)
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            Debug.Log($"[DEBUG] Current RectTransform before set: {rect.anchoredPosition}");
            rect.anchoredPosition = new Vector2(posX, posY);
            Debug.Log($"[DEBUG] RectTransform after set: {rect.anchoredPosition}");
        }
        else
        {
            Debug.Log($"[DEBUG] Current Transform before set: {transform.localPosition}");
            transform.localPosition = new Vector3(posX, posY, dist);
            Debug.Log($"[DEBUG] Transform after set: {transform.localPosition}");
        }

        // pastikan ada komponen Button
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (data == null)
        {
            Debug.LogWarning("⚠ Data button belum diinisialisasi");
            return;
        }

        Debug.Log($"▶ {data.Label} ditekan → Action: {data.ActionType}");

        string actionType = data.ActionType != null ? data.ActionType.Replace(";", "").ToLower() : "";

        switch (actionType)
        {
            case "room":
                if (data.Target_Room != null)
                    Debug.Log($"➡ Pindah ke Room: {data.Target_Room.Name} (ID {data.Target_Room.id})");
                else
                    Debug.LogWarning("⚠ Target_Room tidak ada");
                break;

            case "url":
                if (!string.IsNullOrEmpty(data.ActionData))
                    Application.OpenURL(data.ActionData);
                break;

            case "audio":
                Debug.Log($"🎵 Play audio: {data.ActionData}");
                break;

            case "info":
                Debug.Log("ℹ Tampilkan info popup");
                break;

            default:
                Debug.LogWarning("⚠ ActionType tidak dikenal: " + data.ActionType);
                break;
        }
    }

}
