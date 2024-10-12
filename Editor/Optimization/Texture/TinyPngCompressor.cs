namespace TheOne.Tool.Optimization.Texture
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class TinyPngCompressor
    {
        private const string ApiKey = "6PMpnKmQD03fSjVW2SvWQLVKBz4K0f4g"; // Replace with your actual API key
        private const string ApiUrl = "https://api.tinify.com/shrink";

        // Method to start the compression process for a list of textures
        public static async UniTaskVoid CompressTextures(List<string> filePaths)
        {
            var skippedCount     = 0;
            var compressedCount  = 0;
            var compressionTasks = new List<UniTask>();

            foreach (var filePath in filePaths)
            {
                if (!filePath.ToLower().EndsWith(".png"))
                {
                    skippedCount++;
                    Debug.Log("Skipping non-PNG file: " + filePath);
                    continue;
                }

                compressionTasks.Add(CompressTextureAsync(filePath, () => compressedCount++));
            }

            await UniTask.WhenAll(compressionTasks);

            Debug.Log($"Try to Compress: {filePaths.Count}, Compressed textures: {compressedCount}, Skipped textures: {skippedCount}");
        }

        private static async UniTask CompressTextureAsync(string filePath, Action onCompressed)
        {
            var imageBytes = ImageToByteArray(filePath);
            if (imageBytes == null)
            {
                Debug.LogError("Failed to read file: " + filePath);
                return;
            }

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
                onCompressed?.Invoke();
                var jsonResponse = request.downloadHandler.text;
                var url          = ExtractUrlFromResponse(jsonResponse);
                var savePath     = Path.ChangeExtension(filePath, ".png");
                await DownloadCompressedImage(url, savePath);
            }
            else
            {
                Debug.LogError("Error compressing " + filePath + ": " + request.error);
            }
        }

        private static byte[] ImageToByteArray(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            else
            {
                Debug.LogError("File does not exist: " + filePath);
                return null;
            }
        }

        private static async UniTask DownloadCompressedImage(string url, string savePath)
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
            var urlStart = jsonResponse.IndexOf("\"url\":\"", StringComparison.Ordinal) + 7;
            var urlEnd   = jsonResponse.IndexOf("\"", urlStart, StringComparison.Ordinal);
            var url      = jsonResponse.Substring(urlStart, urlEnd - urlStart);
            return url.Replace("\\/", "/"); // Unescape the URL
        }
    }
}