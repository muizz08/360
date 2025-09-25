//using UnityEngine;

//public class TapToTouchObject : MonoBehaviour
//{
//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//            if (Physics.Raycast(ray, out hit, 1000.0f))
//            {
//                // cek apakah objek yang di-klik punya ButtonManager
//                ButtonManager btn = hit.collider.GetComponent<ButtonManager>();
//                if (btn != null)
//                {
//                    btn.OnClick(); // panggil event tombol sesuai data dari Strapi
//                }

//                // kalau ada popup khusus
//                PopupManager popup = hit.collider.GetComponent<PopupManager>();
//                if (popup != null)
//                {
//                    popup.TogglePopup();
//                }
//            }
//        }
//    }
//}
