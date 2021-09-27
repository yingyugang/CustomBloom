using System;
using UnityEngine;

namespace BlueNoah.ImageEffect
{
    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class BloomCustom : MonoBehaviour
    {
        [Range(1,16)]
        public int iterations = 1;

        public Shader shader;
        [NonSerialized]
        Material bloomMat;
        const int BoxDownPass = 0;
        const int BoxUpPass = 1;
        const int ApplyBloomPass = 2;
        RenderTexture[] textures = new RenderTexture[16];

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (bloomMat == null)
            {
                bloomMat = new Material(shader);
                bloomMat.hideFlags = HideFlags.HideAndDontSave;
            }
            int width = source.width;
            int height = source.height;
            var format = source.format;
            //0代表不对深度缓存做任何修改
            RenderTexture currentDestination = textures[0] = RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(source, currentDestination, bloomMat, BoxDownPass);
            RenderTexture currentSource = currentDestination;
            int i = 1;
            for (; i < iterations; i++)
            {
                width /= 2;
                height /= 2;
                if (height < 2)
                {
                    break;
                }
                currentDestination = textures[i] =  RenderTexture.GetTemporary(width, height, 0, format);
                Graphics.Blit(currentSource, currentDestination,bloomMat,BoxDownPass);
               // RenderTexture.ReleaseTemporary(currentSource);
                currentSource = currentDestination;
            }
            for (i -= 2; i >= 0; i--)
            {
                currentDestination = textures[i];
                textures[i] = null;
                bloomMat.SetTexture("_SourceTex", source);
	            Graphics.Blit(currentSource, destination, bloomMat, ApplyBloomPass);
	            RenderTexture.ReleaseTemporary(currentSource);
                currentSource = currentDestination;
            }
            Graphics.Blit(currentSource, destination, bloomMat, ApplyBloomPass);
            RenderTexture.ReleaseTemporary(currentSource);
        }
    }
}