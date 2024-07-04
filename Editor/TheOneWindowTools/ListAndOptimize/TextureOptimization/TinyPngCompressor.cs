using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class TinyPngCompressor
{
    private const string ApiKey = "6PMpnKmQD03fSjVW2SvWQLVKBz4K0f4g"; // Replace with your actual API key
    private const string ApiUrl = "https://api.tinify.com/shrink";

    // Method to start the compression process for a list of textures
    public static async UniTaskVoid CompressTextures(List<Texture2D> textures)
    {
        foreach (var texture in textures)
        {
            var imagePath = AssetDatabase.GetAssetPath(texture); // Get the path of the texture
            if (System.IO.Path.GetExtension(imagePath).ToLower() != ".png")
            {
                Debug.Log("Skipping non-PNG file: " + imagePath);
                continue; // Skip non-PNG files
            }

            var imageBytes = ImageToByteArray(texture); // Convert texture to byte array

            var           request       = new UnityWebRequest(ApiUrl, "POST");
            UploadHandler uploadHandler = new UploadHandlerRaw(imageBytes);
            uploadHandler.contentType = "application/octet-stream";
            request.uploadHandler     = uploadHandler;

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + ApiKey));
            request.SetRequestHeader("Authorization", "Basic " + auth);

            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Extract the URL from the response
                var jsonResponse = request.downloadHandler.text;
                var url          = ExtractUrlFromResponse(jsonResponse);

                // Download the compressed image
                DownloadCompressedImage(url, imagePath);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    private static byte[] ImageToByteArray(Texture2D texture)
    {
        if (texture.isReadable)
        {
            return texture.EncodeToPNG();
        }

        // Create a temporary readable Texture2D to encode
        var tmp = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(texture, tmp);
        var previous = RenderTexture.active;
        RenderTexture.active = tmp;
        var myTexture2D = new Texture2D(texture.width, texture.height);
        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        var png = myTexture2D.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(myTexture2D); // Clean up temporary texture
        return png;
    }

    private static async UniTaskVoid DownloadCompressedImage(string url, string savePath)
    {
        var request = UnityWebRequest.Get(url);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var imageBytes = request.downloadHandler.data;
            File.WriteAllBytes(savePath, imageBytes); // Save the compressed image
            Debug.Log("Compressed image saved to " + savePath);
        }
        else
        {
            Debug.LogError("Failed to download compressed image: " + request.error);
        }
    }

    private static string ExtractUrlFromResponse(string jsonResponse)
    {
        // Simple JSON parsing to extract the URL. Consider using a JSON library for more complex scenarios.
        var urlStart = jsonResponse.IndexOf("\"url\":\"") + 7;
        var urlEnd   = jsonResponse.IndexOf("\"", urlStart);
        var url      = jsonResponse.Substring(urlStart, urlEnd - urlStart);
        return url.Replace("\\/", "/"); // Unescape the URL
    }
}