﻿using System;
using System.IO;

using System.Net;
using Microsoft.Win32;

namespace Renderly.Utils
{
    /// <summary>
    /// This is an implementatin of IRenderlyAssetManager that interacts with the native filesystem
    /// and network to retrieve/store assets.
    /// </summary>
    public class RenderlyNativeAssetManager : IRenderlyAssetManager
    {
        public Stream Get(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Null or blank string passed to Get.");
            }

            try
            {
                var uri = new Uri(path);
                var wc = new WebClient();
                return wc.OpenRead(uri);
            }
            catch (WebException e)
            {
                // most likely couldn't retrieve the URI due to timeouts or 404 or something.
                throw new IOException("Problem encountered retrieving path.", e);
            }
        }

        public bool Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public void Fetch(string fetchPath, string savePath)
        {
            var uri = new Uri(fetchPath);
            var wc = new WebClient();
            wc.DownloadFile(fetchPath, savePath);
        }

        public string FetchToRandomFilename(string fetchUri, string outputDirectory)
        {
            if (fetchUri == null)
            {
                throw new ArgumentNullException("fetchUri can not be null");
            }

            if (outputDirectory == null)
            {
                throw new ArgumentNullException("outputDirectory can not be null");
            }

            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new ArgumentException("outputDirectory can not be empty or null");
            }

            Uri outputUri = new Uri(outputDirectory);
            Uri uri = new Uri(fetchUri);
            CreateFolder(outputDirectory);
            

            string randomName = Guid.NewGuid().ToString("N");

            if (uri.IsFile || uri.IsUnc)
            {
                randomName = string.Format("{0}{1}", randomName, Path.GetExtension(uri.AbsoluteUri));
                File.Copy(uri.LocalPath, Path.Combine(outputUri.LocalPath, randomName));
            }
            else
            {
                // If it gets here, I'm assuming that you're not going to be giving me non file/non-HTTP URIs,
                // since I'm going to use a download now.
                // HttpWebRequest is used because it gives us access to the
                // ContentType in the response, where WebClient does not without an extension method.
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                using (var response = request.GetResponse())
                {
                    var contentType = response.ContentType;
                    randomName = string.Format("{0}{1}", randomName, GetRegisteredExtension(contentType));
                    Save(response.GetResponseStream(), Path.Combine(outputDirectory, randomName));
                }
            }

            return randomName;
            
        }

        public void Save(Stream file, string savePath)
        {
            using(var ostream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(ostream);
            }
        }

        public void Save(string contents, string savePath)
        {
            File.WriteAllText(savePath, contents);
        }

        public void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        private string GetRegisteredExtension(string type)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + type, false);
            var value = key != null ? key.GetValue("Extension", null) : null;
            return value != null ? value.ToString() : "";
        }
    }
}
