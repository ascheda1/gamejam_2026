using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LetterboxCamera : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    private int lastWidth;
    private int lastHeight;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        ApplyLetterbox();
        SaveResolution();
    }

    void Update()
    {
        // Detect resolution or fullscreen change
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            ApplyLetterbox();
            SaveResolution();
        }
    }

    void ApplyLetterbox()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;

            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
            cam.rect = rect;
        }
    }

    void SaveResolution()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }
}