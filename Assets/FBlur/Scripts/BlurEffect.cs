using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
public class BlurEffect : MonoBehaviour
{
    public Camera BlurCamera;
    public Material BlurMaterial;

    [Header("Options")]
    [Range(0, 50)] public float Blur = 10;
    [Range(1, 20)] public int Downsample = 2;
    [Range(0, 20)] public int Iterations = 8;

    private RenderTexture UICamRT;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (BlurCamera != null && BlurMaterial != null) {
            if (BlurCamera.targetTexture == null) {
                UICamRT = RenderTexture.GetTemporary(source.width, source.height, 24, source.format);
                BlurCamera.targetTexture = UICamRT;
                BlurMaterial.SetTexture("_UIMask", UICamRT);
            }
            RenderTexture _uiblur = RenderTexture.GetTemporary(source.width/Downsample, source.height/Downsample, 24,
                source.format);
            BlurMaterial.SetFloat("_Blur", Blur/1000);

            Graphics.Blit(source, _uiblur, BlurMaterial, 0);

            for (var i = 0; i < Iterations; i++) {
                RenderTexture IterationBlur = RenderTexture.GetTemporary(source.width, source.height, 0);
                Graphics.Blit(_uiblur, IterationBlur, BlurMaterial, 0);
                Graphics.Blit(IterationBlur, _uiblur);
                RenderTexture.ReleaseTemporary(IterationBlur);
            }

            BlurMaterial.SetTexture("_UIBlured", _uiblur);
            Graphics.Blit(source, destination, BlurMaterial, 1);
            DestroyImmediate(_uiblur);
        }
    }
}