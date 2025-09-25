//using UnityEngine;
//using UnityEngine.Networking;
//using System.Collections;
//using System.Collections.Generic;
//using Newtonsoft.Json.Linq;
//using UnityEngine.UI;

//public class SceneManager : MonoBehaviour
//{
//    [Header("Strapi Settings")]
//    public string strapiApiUrl = "http://localhost:1337/api/proyek-360s?populate=*";

//    [Header("References")]
//    public MaterialChanger materialChanger;
//    public GameObject navButtonPrefab;
//    public Transform buttonParent;

//    private Dictionary<string, JToken> roomDataCache = new Dictionary<string, JToken>();
//    private string currentRoomName;

//    void Start()
//    {
//        StartCoroutine(FetchAllRoomData());
//    }

//    private IEnumerator FetchAllRoomData()
//    {
//        UnityWebRequest request = UnityWebRequest.Get(strapiApiUrl);
//        yield return request.SendWebRequest();

//        if (request.result != UnityWebRequest.Result.Success)
//        {
//            Debug.LogError("❌ Gagal memuat data dari Strapi: " + request.error);
//            yield break;
//        }

//        string json = request.downloadHandler.text;
//        JObject parsedData = JObject.Parse(json);
//        JArray rooms = (JArray)parsedData["data"];

//        foreach (var room in rooms)
//        {
//            string roomName = room["attributes"]["room_name"].ToString();
//            roomDataCache[roomName] = room["attributes"];
//            Debug.Log($"✅ Data ruangan '{roomName}' berhasil di-cache.");
//        }

//        // Mulai dengan ruangan pertama, misalnya "Lobby"
//        ChangeRoom("Lobby");
//    }

//    public void ChangeRoom(string roomName)
//    {
//        if (roomDataCache.ContainsKey(roomName))
//        {
//            currentRoomName = roomName;
//            JToken roomAttributes = roomDataCache[roomName];

//            JToken roomImageData = roomAttributes["room"]["data"];
//            string imageUrl = "http://localhost:1337" + roomImageData["attributes"]["url"].ToString();

//            JArray navButtons = (JArray)roomAttributes["buttons"];

//            StartCoroutine(LoadAndPlaceElements(imageUrl, navButtons));
//        }
//        else
//        {
//            Debug.LogError($"Ruangan '{roomName}' tidak ditemukan di cache.");
//        }
//    }

//    private IEnumerator LoadAndPlaceElements(string textureUrl, JArray navButtons)
//    {
//        // Hapus semua tombol yang ada di scene
//        foreach (Transform child in buttonParent)
//        {
//            Destroy(child.gameObject);
//        }

//        UnityWebRequest texRequest = UnityWebRequestTexture.GetTexture(textureUrl);
//        yield return texRequest.SendWebRequest();

//        if (texRequest.result == UnityWebRequest.Result.Success)
//        {
//            Texture2D tex = DownloadHandlerTexture.GetContent(texRequest);
//            materialChanger.ApplyTexture(tex);

//            if (navButtons != null)
//            {
//                foreach (var button in navButtons)
//                {
//                    // Buat tombol baru dari prefab
//                    GameObject newButton = Instantiate(navButtonPrefab, buttonParent);

//                    // Parsing data posisi dari JSON
//                    JToken position = button["position"];
//                    Vector3 newPos = new Vector3(
//                        (float)position["x"],
//                        (float)position["y"],
//                        (float)position["z"]
//                    );

//                    // Atur posisi tombol
//                    newButton.transform.position = newPos;

//                    // Ambil nama ruangan tujuan dari Strapi
//                    string targetRoomName = button["target_room"]["data"]["attributes"]["room_name"].ToString();

//                    // Tambahkan fungsionalitas klik
//                    Button btnComponent = newButton.GetComponent<Button>();
//                    if (btnComponent != null)
//                    {
//                        // Tambahkan listener yang memanggil ChangeRoom dengan nama ruangan target
//                        btnComponent.onClick.AddListener(() => ChangeRoom(targetRoomName));
//                    }
//                }
//            }
//        }
//        else
//        {
//            Debug.LogError("Gagal mengunduh tekstur: " + texRequest.error);
//        }
//    }
//}