using UnityEngine;

public class PathSetter : MonoBehaviour
{
    [SerializeField] GameObject[] paths;
    [SerializeField] Texture2D[] pathTextures;

    private void Start()
    {
        SetPathTextures();
    }

    public void SetPathTextures()
    {
        for (int i = 0; i < paths.Length; i++)
            if (i <= pathTextures.Length)
                if (paths[i].TryGetComponent(out Renderer renderer))
                    if (pathTextures[i] != null)
                        renderer.material.mainTexture = pathTextures[i];
    }
}
