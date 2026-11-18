using System.Collections;
using UnityEngine;
using System.IO;

public class Standalone360Capture : MonoBehaviour
{
    [Header("Camera Control")]
    public KeyCode toggleCameraKey = KeyCode.C;
    public bool startInWalkthroughMode = false;
    
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 2f;
    public float cameraHeight = 0.04f;
    
    [Header("360° Capture Settings")]
    public int cubemapSize = 1024;
    public string saveFolder = "360Screenshots";
    public KeyCode captureKey = KeyCode.Space;
    
    [Header("UI")]
    public GameObject captureUI;
    public UnityEngine.UI.Text statusText;
    
    private Camera walkthroughCamera;
    private Camera[] originalCameras;
    private float verticalRotation = 0f;
    private bool isCapturing = false;
    private bool isWalkthroughActive = false;
    private bool originalCursorState;
    private CursorLockMode originalCursorLockMode;
    
    void Start()
    {
        // Get the walkthrough camera (should be child of this GameObject)
        walkthroughCamera = GetComponentInChildren<Camera>();
        
        if (walkthroughCamera == null)
        {
            Debug.LogError("❌ No camera found as child of " + gameObject.name + ". Please add a Camera as a child GameObject.");
            return;
        }
        else
        {
            Debug.Log("✅ Walkthrough camera found: " + walkthroughCamera.name);
        }
        
        // Set initial position
        transform.position = new Vector3(transform.position.x, cameraHeight, transform.position.z);
        
        // Make sure walkthrough camera is positioned correctly
        walkthroughCamera.transform.localPosition = Vector3.zero;
        walkthroughCamera.transform.localRotation = Quaternion.identity;
        
        // Store original camera states
        originalCameras = FindObjectsOfType<Camera>();
        Debug.Log($"📷 Found {originalCameras.Length} total cameras in scene");
        
        // Create save directory
        string savePath = Path.Combine(Application.persistentDataPath, saveFolder);
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        Debug.Log($"💾 Save directory: {savePath}");
        
        // Initialize based on startInWalkthroughMode
        if (startInWalkthroughMode)
        {
            ActivateWalkthroughMode();
        }
        else
        {
            DeactivateWalkthroughMode();
        }
        
        // Initialize UI
        UpdateStatusText();
    }
    
    void Update()
    {
        // Toggle between walkthrough and original camera system
        if (Input.GetKeyDown(toggleCameraKey))
        {
            ToggleCameraMode();
        }
        
        // Only handle walkthrough controls when in walkthrough mode
        if (isWalkthroughActive)
        {
            HandleMovement();
            HandleMouseLook();
            HandleCapture();
        }
        
        HandleUI();
    }
    
    void ToggleCameraMode()
    {
        if (isWalkthroughActive)
        {
            DeactivateWalkthroughMode();
        }
        else
        {
            ActivateWalkthroughMode();
        }
        
        UpdateStatusText();
    }
    
    void ActivateWalkthroughMode()
    {
        isWalkthroughActive = true;
        
        // Store original cursor state
        originalCursorState = Cursor.visible;
        originalCursorLockMode = Cursor.lockState;
        
        // Disable all other cameras
        foreach (Camera cam in originalCameras)
        {
            if (cam != walkthroughCamera && cam != null)
            {
                cam.enabled = false;
            }
        }
        
        // Enable walkthrough camera
        walkthroughCamera.enabled = true;
        
        // Lock cursor for walkthrough
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Walkthrough mode activated. Use WASD to move, mouse to look, " + captureKey + " to capture 360°");
    }
    
    void DeactivateWalkthroughMode()
    {
        isWalkthroughActive = false;
        
        // Restore original cursor state
        Cursor.visible = originalCursorState;
        Cursor.lockState = originalCursorLockMode;
        
        // Disable walkthrough camera
        walkthroughCamera.enabled = false;
        
        // Re-enable original cameras
        foreach (Camera cam in originalCameras)
        {
            if (cam != walkthroughCamera && cam != null)
            {
                cam.enabled = true;
            }
        }
        
        Debug.Log("Returned to original camera system");
    }
    
    void HandleMovement()
    {
        // Get input (X and Z movement - horizontal plane)
        float horizontal = Input.GetAxis("Horizontal"); // A/D keys (X axis)
        float vertical = Input.GetAxis("Vertical");     // W/S keys (Z axis)
        
        // Calculate movement direction (X and Z plane, Y locked)
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction.y = 0f; // Ensure no Y movement - stay at fixed height
        
        // Determine speed (run when holding Shift)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // Calculate new position
        Vector3 newPosition = transform.position + direction * currentSpeed * Time.deltaTime;
        
        // Lock Y at camera height
        newPosition.y = cameraHeight;
        
        // Apply movement
        transform.position = newPosition;
    }
    
    void HandleMouseLook()
    {
        if (isCapturing) return; // Don't rotate during capture
        
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate camera vertically
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        walkthroughCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
    
    void HandleCapture()
    {
        if (Input.GetKeyDown(captureKey) && !isCapturing)
        {
            StartCoroutine(Capture360Image());
        }
        
        // ESC to return to original camera system
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeactivateWalkthroughMode();
            UpdateStatusText();
        }
    }
    
    void HandleUI()
    {
        if (captureUI != null)
        {
            captureUI.SetActive(isCapturing);
        }
    }
    
    void UpdateStatusText()
    {
        if (statusText != null)
        {
            if (isWalkthroughActive)
            {
                statusText.text = $"WALKTHROUGH MODE | {captureKey} = Capture 360° | WASD = Move | Mouse = Look | ESC = Exit";
            }
            else
            {
                statusText.text = $"Press {toggleCameraKey} to enter walkthrough mode for 360° capture";
            }
        }
    }
    
    IEnumerator Capture360Image()
    {
        isCapturing = true;
        
        if (statusText != null)
            statusText.text = "Capturing 360° image...";
        
        // Create a temporary camera for capturing
        GameObject tempCameraGO = new GameObject("360Camera");
        tempCameraGO.transform.position = walkthroughCamera.transform.position;
        tempCameraGO.transform.rotation = walkthroughCamera.transform.rotation;
        
        Camera tempCamera = tempCameraGO.AddComponent<Camera>();
        tempCamera.fieldOfView = 90f;
        tempCamera.aspect = 1f;
        tempCamera.nearClipPlane = walkthroughCamera.nearClipPlane;
        tempCamera.farClipPlane = walkthroughCamera.farClipPlane;
        
        // Create cubemap
        Cubemap cubemap = new Cubemap(cubemapSize, TextureFormat.RGB24, false);
        
        // Render 6 faces of the cubemap
        tempCamera.RenderToCubemap(cubemap);
        
        // Convert cubemap to equirectangular format
        Texture2D equirectangular = ConvertCubemapToEquirectangular(cubemap);
        
        // Save the image
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = $"360_capture_{timestamp}.png";
        string savePath = Path.Combine(Application.persistentDataPath, saveFolder, filename);
        
        byte[] imageData = equirectangular.EncodeToPNG();
        File.WriteAllBytes(savePath, imageData);
        
        // Clean up
        DestroyImmediate(tempCameraGO);
        DestroyImmediate(cubemap);
        DestroyImmediate(equirectangular);
        
        if (statusText != null)
        {
            statusText.text = $"360° image saved! | Press {captureKey} to capture another";
            StartCoroutine(ResetStatusText());
        }
        
        Debug.Log($"360° image saved to: {savePath}");
        
        isCapturing = false;
        yield return null;
    }
    
    IEnumerator ResetStatusText()
    {
        yield return new WaitForSeconds(3f);
        UpdateStatusText();
    }
    
    Texture2D ConvertCubemapToEquirectangular(Cubemap cubemap)
    {
        int width = cubemapSize * 4;
        int height = cubemapSize * 2;
        Texture2D equirectangular = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        Color[] pixels = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Convert pixel coordinates to spherical coordinates
                //float u = (float)x / width; 
                //float v = (float)y / height;

                float u = ((x + 0.5f) / width);          // 0..1
                float v = 1f - ((y + 0.5f) / height);    // <-- flip V so top-of-image is +Y (north pole)

                float theta = u * 2f * Mathf.PI; // Longitude
                float phi = v * Mathf.PI; // Latitude
                
                // Convert spherical to cartesian coordinates
                Vector3 direction = new Vector3(
                    Mathf.Sin(phi) * Mathf.Sin(theta),
                    Mathf.Cos(phi),
                    Mathf.Sin(phi) * Mathf.Cos(theta)
                );
                
                // Sample the cubemap
                Color color = SampleCubemap(cubemap, direction);
                pixels[y * width + x] = color;
            }
        }
        
        equirectangular.SetPixels(pixels);
        equirectangular.Apply();
        
        return equirectangular;
    }
    
    Color SampleCubemap(Cubemap cubemap, Vector3 direction)
    {
        // Determine which face of the cubemap to sample
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);
        float absZ = Mathf.Abs(direction.z);
        
        CubemapFace face;
        Vector2 uv;
        
        if (absX >= absY && absX >= absZ)
        {
            if (direction.x > 0)
            {
                face = CubemapFace.PositiveX;
                uv = new Vector2(-direction.z / absX, -direction.y / absX);
            }
            else
            {
                face = CubemapFace.NegativeX;
                uv = new Vector2(direction.z / absX, -direction.y / absX);
            }
        }
        else if (absY >= absZ)
        {
            if (direction.y > 0)
            {
                face = CubemapFace.PositiveY;
                uv = new Vector2(direction.x / absY, direction.z / absY);
            }
            else
            {
                face = CubemapFace.NegativeY;
                uv = new Vector2(direction.x / absY, -direction.z / absY);
            }
        }
        else
        {
            if (direction.z > 0)
            {
                face = CubemapFace.PositiveZ;
                uv = new Vector2(direction.x / absZ, -direction.y / absZ);
            }
            else
            {
                face = CubemapFace.NegativeZ;
                uv = new Vector2(-direction.x / absZ, -direction.y / absZ);
            }
        }
        
        // Convert UV coordinates to pixel coordinates
        uv = (uv + Vector2.one) * 0.5f;
        int pixelX = Mathf.FloorToInt(uv.x * cubemapSize);
        int pixelY = Mathf.FloorToInt(uv.y * cubemapSize);
        
        pixelX = Mathf.Clamp(pixelX, 0, cubemapSize - 1);
        pixelY = Mathf.Clamp(pixelY, 0, cubemapSize - 1);
        
        return cubemap.GetPixel(face, pixelX, pixelY);
    }
}