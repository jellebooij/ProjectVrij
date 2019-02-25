using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour {

    public Material fadeMaterial;
    int fadeDirection;
    public float alpha;
    float fadeSpeed;

    private void Start()
    {
        fadeDirection = -1;
        fadeSpeed = 0.4f;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, fadeMaterial);
    }

    private void Update()
    {
        
        alpha += fadeSpeed * Time.deltaTime * fadeDirection;
        alpha = Mathf.Clamp01(alpha);

        fadeMaterial.SetFloat("_FadeAmount", alpha);

    }

    public void FadeIn(float fadeSpeed)
    {
        fadeDirection = -1;
        this.fadeSpeed = fadeSpeed; 
    }

    public void FadeOut(float fadeSpeed)
    {
        fadeDirection = 1;
        this.fadeSpeed = fadeSpeed;
    }


}
