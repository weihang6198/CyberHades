using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostEffectController : MonoBehaviour
{

    public Shader PostShader;
    Material PostEffectMaterial;

    public Color ScreenTint;
    RenderTexture PostRendererTexture;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (PostEffectMaterial == null)
        {
            PostEffectMaterial = new Material(PostShader);
        }

        // *** Use a temporary texture (PostRendererTexture) for the intermediate result ***
        // (This block is correct for acquiring the temporary texture)
        PostRendererTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        PostEffectMaterial.SetColor("_ScreenTint", ScreenTint);

        // 1. Apply effect: source -> PostRendererTexture
        Graphics.Blit(source, PostRendererTexture, PostEffectMaterial, 0);

        // 2. Set the global texture (This is why you're using an intermediate texture)
        Shader.SetGlobalTexture("_GlobalRenderTexture", PostRendererTexture);

        // 3. Final Blit: PostRendererTexture -> destination
        Graphics.Blit(PostRendererTexture, destination);

        // 4. Release the temporary texture
        RenderTexture.ReleaseTemporary(PostRendererTexture);

        // *** You should also set PostRendererTexture to null to avoid leaking memory ***
        PostRendererTexture = null;
    }
}
