using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MaterialChanger : MonoBehaviour
{
    public Renderer targetRenderer;
    public List<Texture2D> textures = new List<Texture2D>(); // Ubah menjadi new List<Texture2D>()
    public float transitionDuration = 1.0f;
    public Shader crossFadeShader;

    public int currentIndex { get; private set; } = 0;
    private Material fadeMaterialInstance;
    private Tween currentTween;

    // Ubah Start() menjadi InitializeMaterial() dan panggil dari StrapiSceneLoader
    public void InitializeMaterial() // Method baru untuk inisialisasi
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (targetRenderer != null && textures.Count > 0)
        {
            if (crossFadeShader == null)
            {
                Debug.LogError("CrossFade Shader is not assigned!");
                return;
            }

            // Pastikan MaterialChanger hanya diinisialisasi sekali
            if (fadeMaterialInstance == null)
            {
                fadeMaterialInstance = new Material(crossFadeShader);
                targetRenderer.material = fadeMaterialInstance; // Assign material ke renderer
            }

            fadeMaterialInstance.SetTexture("_Texture_A", textures[0]); // Set tekstur awal
            fadeMaterialInstance.SetTexture("_Texture_B", textures[0]);
            fadeMaterialInstance.SetFloat("_Blend", 0);
            currentIndex = 0; // Atur currentIndex ke 0
        }
        else if (textures.Count == 0)
        {
            Debug.LogWarning("Tidak ada tekstur yang dimuat ke MaterialChanger.");
        }
    }

    void Start() // Kosongkan Start() karena inisialisasi akan dilakukan oleh StrapiSceneLoader
    {
        // Debug.Log("MaterialChanger Start() called, waiting for textures from Strapi.");
        // Inisialisasi akan dipanggil oleh StrapiSceneLoader setelah tekstur siap.
    }


    public void ChangeToNext()
    {
        if (targetRenderer == null || textures.Count <= 1) return;

        int nextIndex = currentIndex + 1;
        if (nextIndex >= textures.Count)
        {
            nextIndex = 0;
        }

        UpdateVisuals(nextIndex);
    }

    public void ChangeToPrevious()
    {
        if (targetRenderer == null || textures.Count <= 1) return;

        int prevIndex = currentIndex - 1;
        if (prevIndex < 0)
        {
            prevIndex = textures.Count - 1;
        }

        UpdateVisuals(prevIndex);
    }

    private void UpdateVisuals(int newIndex)
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        fadeMaterialInstance.SetTexture("_Texture_A", textures[currentIndex]);
        fadeMaterialInstance.SetTexture("_Texture_B", textures[newIndex]);
        fadeMaterialInstance.SetFloat("_Blend", 0);

        currentTween = DOTween.To(() => fadeMaterialInstance.GetFloat("_Blend"), x => fadeMaterialInstance.SetFloat("_Blend", x), 1, transitionDuration)
            .OnComplete(() =>
            {
                fadeMaterialInstance.SetTexture("_Texture_A", textures[newIndex]);
                fadeMaterialInstance.SetTexture("_Texture_B", textures[newIndex]);
                fadeMaterialInstance.SetFloat("_Blend", 0);
                currentIndex = newIndex;
                currentTween = null;
            });
    }
    public void AddTextureAndShow(Texture2D newTexture)
    {
        if (newTexture == null) return;

        textures.Add(newTexture);

        if (fadeMaterialInstance == null)
        {
            InitializeMaterial(); // auto init kalau belum
        }

        // langsung transisi ke tekstur baru
        int newIndex = textures.Count - 1;
        UpdateVisuals(newIndex);
    }

}