// InvertEffect.cs
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class InvertEffect : MonoBehaviour
{
    [SerializeField] private Shader invertShader;

    [Range(0, 1)]
    public float intensity = 0;
    public float fadeDuration = 0.5f;

    private Material material;
    private Coroutine fadeRoutine;

    void OnEnable()
    {
        // 优先使用序列化字段，其次使用查找
        if (invertShader == null)
        {
            Debug.LogWarning("使用Shader.Find()获取InvertColors shader，建议拖动赋值");
            invertShader = Shader.Find("Custom/InvertColors");
        }

        if (invertShader == null || !invertShader.isSupported)
        {
            Debug.LogError("Shader无效或不被支持");
            enabled = false;
            return;
        }

        material = new Material(invertShader);
        UpdateMaterialIntensity();
    }

    void OnDisable()
    {
        if (material != null)
            DestroyImmediate(material);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (intensity <= 0)
        {
            Graphics.Blit(src, dest);
            return;
        }

        Graphics.Blit(src, dest, material);
    }

    public void ToggleEffect(bool enable)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeEffect(enable ? 1f : 0f));
    }

    private IEnumerator FadeEffect(float targetIntensity)
    {
        float startIntensity = intensity;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / fadeDuration);
            UpdateMaterialIntensity();
            elapsed += Time.deltaTime;
            yield return null;
        }

        intensity = targetIntensity;
        UpdateMaterialIntensity();
    }

    private void UpdateMaterialIntensity()
    {
        if (material != null)
            material.SetFloat("_Intensity", intensity);
    }
}