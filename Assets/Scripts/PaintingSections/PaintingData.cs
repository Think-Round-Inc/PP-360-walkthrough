using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public sealed class PaintingDataHolder
{
    public Sprite paintingImage;
    public string paintingName;
    [TextArea(0, 10)] public string extraPaintingInfo;
    public AudioClip paintingClip;
    [HideInInspector] public Color paintingTextureColor = Color.white;
    public string imageURL = "https://images.squarespace-cdn.com/content/v1/522e01f0e4b074119b24a9d8/1605033706665-NKRRW9VIOKVRSHSDVV59/4%2BFamilies%2BoThe+families+from+left+to+right%3A+Spong-Fernandez%2C+Sacks%2C+Pestrong%2C+Attiaf%2BFIP.2+%281%29.jpg?format=2500w";
}

[System.Serializable]
public sealed class PaintingData : MonoBehaviour
{
    public PaintingDataHolder paintingData;

    // new flag
    private bool isLoaded;

    public void InitializePaintingData()
    {
        if (TryGetComponent<Renderer>(out var screenRenderer))
        {
            if (paintingData.paintingImage != null)
            {
                screenRenderer.material.color = paintingData.paintingTextureColor;
                screenRenderer.material.mainTexture = paintingData.paintingImage.texture;   // :contentReference[oaicite:1]{index=1}
                isLoaded = true;
            }
            else
            {
            // show the section’s fallback color when no image is available
                screenRenderer.material.mainTexture = null;
                screenRenderer.material.color = SectionColorHolder.EmptyScreenColor;       // :contentReference[oaicite:2]{index=2}
            }
        }
    }

    public void LoadImageBasedOnProximity()
    {
        if (!string.IsNullOrEmpty(paintingData.imageURL))
            StartCoroutine(LoadImage(paintingData.imageURL));                              // :contentReference[oaicite:3]{index=3}
    }

    IEnumerator LoadImage(string link)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(link);
        yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            if (TryGetComponent<Renderer>(out var screenRenderer))
            {
                if (myTexture != null)
                {
                    screenRenderer.material.color = paintingData.paintingTextureColor;
                    screenRenderer.material.mainTexture = myTexture;                       // :contentReference[oaicite:4]{index=4}
                    isLoaded = true;  // <-- set the flag here, inside a method
                }
                else
                {
                    screenRenderer.material.mainTexture = null;
                    screenRenderer.material.color = SectionColorHolder.EmptyScreenColor;   // :contentReference[oaicite:5]{index=5}
                }
            }
        }
    }

    private void Update()
    {
        // load once when we are the closest, never clear after
        if (gameObject.tag == "ClosestHotspot" && !isLoaded)
            LoadImageBasedOnProximity();
    }
}
