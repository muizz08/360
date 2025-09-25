using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PositionManager : MonoBehaviour
{
    public List<ButtonManager> buttons; // drag & drop button yang ada di scene
    public float sphereRadius = 10f;    // radius sphere 360, samain sama sphere environment
    private List<Vector3> debugPoints = new List<Vector3>();
    public MaterialChanger materialChanger; // assign di Inspector


    public void SetupRooms(List<RoomData> rooms)
    {
        debugPoints.Clear();
        Debug.Log($"{rooms.Count} room ditemukan");

        foreach (var room in rooms)
        {
            if (room.Image != null && room.Image.Count > 0)
            {
                string fullUrl = "http://localhost:1337" + room.Image[0].url; // ambil gambar pertama
                Debug.Log($"✅ Room {room.Name} pakai image: {fullUrl}");
                StartCoroutine(LoadAndApplyTexture(fullUrl, materialChanger));
            }
            else
            {
                Debug.LogWarning($"⚠ Room {room.Name} tidak punya Image!");
            }

            foreach (var buttonData in room.Button)
            {
                Debug.Log($"🔘 Button {buttonData.Label} type {buttonData.ActionType}");
            }
        }

    }

    IEnumerator LoadAndApplyTexture(string url, MaterialChanger materialChanger)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Gagal download texture: " + uwr.error);
            }
            else
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
                Debug.Log("✅ Berhasil download: " + url);
                materialChanger.AddTextureAndShow(tex);
            }
        }
    }




    /// <summary>
    /// Konversi koordinat normalisasi (0-1) ke posisi sphere 360
    /// </summary>
    Vector3 ConvertToSpherePosition(float nx, float ny, float radius)
    {
        // Longitude: 0 kiri → 1 kanan → -180..180
        float longitude = nx * 360f - 180f;

        float latitude = (1f - ny) * 180f - 90f;

        float lonRad = longitude * Mathf.Deg2Rad;
        float latRad = latitude * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float z = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);
        float y = radius * Mathf.Sin(latRad);

        return new Vector3(x, y, z);
    }


    // === Debug Visual di Scene View ===
    private void OnDrawGizmos()
    {
        if (debugPoints == null) return;

        Gizmos.color = Color.red;
        foreach (var p in debugPoints)
        {
            Gizmos.DrawSphere(p, 0.2f); // titik merah kecil
        }
    }
}
