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

    private string previousTag = "Screen";
    private string currentTag = "Screen";

    public void InitializePaintingData()
    {
        if (TryGetComponent<Renderer>(out var screenRenderer))
        {
            if (paintingData.paintingImage != null)
            {
                screenRenderer.material.color = paintingData.paintingTextureColor;
                screenRenderer.material.mainTexture = paintingData.paintingImage.texture;
            }
            else
                screenRenderer.material.color = SectionColorHolder.EmptyScreenColor;
        }
    }

    /*private void FixedUpdate()
    {
        LoadImageBasedOnProximity();
    }*/

    public void LoadImageBasedOnProximity()
    {
        if (!string.IsNullOrEmpty(paintingData.imageURL))
        {
            StartCoroutine(LoadImage(paintingData.imageURL));
        }
    }

    IEnumerator LoadImage(string link)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(link);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
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
                    screenRenderer.material.mainTexture = myTexture;
                }
                else
                    screenRenderer.material.color = SectionColorHolder.EmptyScreenColor;
            }
        }
    }

    private void Update()
    {
        currentTag = gameObject.tag;

        if (currentTag == "ClosestHotspot" && previousTag == "Screen") LoadImageBasedOnProximity();

        if (currentTag == "Screen" && previousTag == "ClosestHotspot") InitializePaintingData();

        previousTag = currentTag;
    }
}