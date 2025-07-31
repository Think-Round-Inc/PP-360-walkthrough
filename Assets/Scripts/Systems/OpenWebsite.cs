using UnityEngine;

public sealed class OpenWebsite : MonoBehaviour
{
    public string websiteURL = "https://www.example.com";

    public void OpenExternalWebsite() => Application.OpenURL(websiteURL);
}
