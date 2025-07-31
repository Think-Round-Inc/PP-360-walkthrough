using UnityEngine;

public class ColorSetter : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField] Color[] colors = { Color.white };

    private void Start() => SetPathColors();

    public void SetPathColors()
    {
        for (int i = 0; i < objects.Length; i++)
            if (i < colors.Length)
                if (objects[i].TryGetComponent(out Renderer renderer))
                    renderer.material.SetColor("_Color", colors[i]);
    }
}
