/*
*   Name: PaintingData.cs
*   Description: Stores painting image, title, description, and audio. Sets the exhibit screen,
*               and loads an image from a URL when the screen is close.
*/

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

    [SerializeField, Min(0.1f), Tooltip("Seconds before retrying a failed image download while the painting is nearby.")]
    private float retryDelay = 5f;

    private bool isLoaded;
    private UnityWebRequest pendingRequest;
    private float nextRetry;

    public void InitializePaintingData()
    {
        var sprite = paintingData?.paintingImage;
        ApplyTexture(sprite != null ? sprite.texture : null);
    }

    public void EnsureImageLoaded()
    {
        if (!isLoaded && Time.unscaledTime >= nextRetry)
            LoadImageBasedOnProximity();
    }

    public void LoadImageBasedOnProximity()
    {
        if (pendingRequest == null && !string.IsNullOrEmpty(paintingData?.imageURL))
            StartCoroutine(LoadImage(paintingData.imageURL));
    }

    private IEnumerator LoadImage(string url)
    {
        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            pendingRequest = request;
            try
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                    ApplyTexture(DownloadHandlerTexture.GetContent(request));
                else
                    Debug.LogWarning(request.error, this);
                nextRetry = Time.unscaledTime + retryDelay;
            }
            finally
            {
                pendingRequest = null;
            }
        }
    }

    private void OnDisable()
    {
        if (pendingRequest == null) return;
        pendingRequest.Abort();
        StopAllCoroutines();
        pendingRequest?.Dispose();
        pendingRequest = null;
    }

    private void ApplyTexture(Texture texture)
    {
        if (!TryGetComponent<Renderer>(out var screenRenderer)) return;
        var material = screenRenderer.material;
        material.mainTexture = texture;
        material.color = texture != null ? paintingData.paintingTextureColor : SectionColorHolder.EmptyScreenColor;
        isLoaded = texture != null;
    }
}
