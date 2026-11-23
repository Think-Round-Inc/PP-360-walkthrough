using UnityEngine;

public sealed class TextureSetter : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField] Texture2D[] textures;
    [SerializeField] Color textureMaterialColor = Color.white;

    private void Start() => SetPathColors();

    public void SetPathColors()
    {
        for (int i = 0; i < objects.Length; i++)
            if (i < textures.Length)
                if (objects[i].TryGetComponent(out Renderer renderer) && textures[i] != null)
                {
                    renderer.material.color = textureMaterialColor;
                    renderer.material.SetTexture("_MainTex", textures[i]);
                }
    }
}
