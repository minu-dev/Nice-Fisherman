using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeGalleryNamespace;

public class NativeManager : MonoBehaviour
{
    public Tensorflow tf;
	public PostManager postManager;
	public ProfileManager profileManager;
	public ToastManager toastManager;

    public void PickImage(int maxSize)
    {
	    NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
	    {
		    if(path != null)
		    {
	    		Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
		    	if(texture == null)
				{
					toastManager.doToast("업로드를 실패했습니다.");
					return;
				}
                tf.FromGallery(texture);
		    }
	    });
    }

    public void PickImage_Post(int maxSize)
    {
	    NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
	    {
		    if(path != null)
		    {
	    		Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
		    	if(texture == null)
				{
					toastManager.doToast("업로드를 실패했습니다.");
					return;
				}
				RenderTexture renderTex = RenderTexture.GetTemporary(
                							texture.width,
                							texture.height,
                							0,
                							RenderTextureFormat.Default,
                							RenderTextureReadWrite.Linear);

    			Graphics.Blit(texture, renderTex);
    			RenderTexture previous = RenderTexture.active;
    			RenderTexture.active = renderTex;
    			Texture2D readableTex = new Texture2D(texture.width, texture.height);
    			readableTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
    			readableTex.Apply();
    			RenderTexture.active = previous;
    			RenderTexture.ReleaseTemporary(renderTex);
                postManager.ReadGallery(readableTex);
		    }
	    });
    }

    public void PickImage_Profile(int maxSize)
    {
	    NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
	    {
		    if(path != null)
		    {
	    		Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
		    	if(texture == null)
				{
					toastManager.doToast("업로드를 실패했습니다.");
					return;
				}
				RenderTexture renderTex = RenderTexture.GetTemporary(
                							texture.width,
                							texture.height,
                							0,
                							RenderTextureFormat.Default,
                							RenderTextureReadWrite.Linear);

    			Graphics.Blit(texture, renderTex);
    			RenderTexture previous = RenderTexture.active;
    			RenderTexture.active = renderTex;
    			Texture2D readableTex = new Texture2D(texture.width, texture.height);
    			readableTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
    			readableTex.Apply();
    			RenderTexture.active = previous;
    			RenderTexture.ReleaseTemporary(renderTex);
				profileManager.Submit(readableTex);
		    }
	    });
    }
}