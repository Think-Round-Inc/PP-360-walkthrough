using UnityEngine;

public class EarthMatSetter : MonoBehaviour
{

    public Renderer[] objectRenderers;
    public Vector2[] textureOffsets;

    private Material[] materials;

    private void Start()
    {
        // Initialize the materials array with the materials from the object renderers
        materials = new Material[objectRenderers.Length];
        for (int i = 0; i < objectRenderers.Length; i++)
        {
            materials[i] = objectRenderers[i].material;
        }
    }

    private void Update()
    {
        // Set the texture offsets in the shaders
        for (int i = 0; i < objectRenderers.Length; i++)
        {
            materials[i].SetVector("_TextureOffset", new Vector4(textureOffsets[i].x, textureOffsets[i].y, 0f, 0f));
        }
    }
}

