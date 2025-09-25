using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class StrapiSceneLoader : MonoBehaviour
{
    private const string API_URL = "http://localhost:1337/api/rooms?populate[Image]=true&populate[Button][populate][Position]=true";

    public PositionManager positionManager;

    private void Start()
    {
        StartCoroutine(LoadRoomsFromStrapi());
    }

    private IEnumerator LoadRoomsFromStrapi()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(API_URL))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Gagal ambil data Strapi: " + req.error);
                yield break;
            }

            Debug.Log("✅ Response: " + req.downloadHandler.text);

            // ✅ ganti ke Newtonsoft
            RoomResponse response = JsonConvert.DeserializeObject<RoomResponse>(req.downloadHandler.text);

            if (response != null && response.data != null)
            {
                Debug.Log($"📦 {response.data.Count} room ditemukan");
                positionManager.SetupRooms(response.data);
            }
            else
            {
                Debug.LogWarning("⚠ Tidak ada data room");
            }
        }
    }


}
