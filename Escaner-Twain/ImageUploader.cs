// Copyright 2015 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace GoogleCloudSamples.Services
{
    public class ImageUploader
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public ImageUploader(string bucketName)
        {
            try
            {
                string jsonPath = ConfigurationManager.AppSettings["jsonPath"].ToString();
                var credential = GoogleCredential.FromFile(jsonPath);

                _bucketName = bucketName;
                // [START storageclient]
                _storageClient = StorageClient.Create(credential);
                // [END storageclient]
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // [START uploadimage]
        public async Task<String> UploadImageAsync(string localPath, string objectName = null)
        {
            var imageAcl = PredefinedObjectAcl.PublicRead;

            var f = File.OpenRead(localPath);

            objectName = objectName ?? Path.GetFileName(localPath);
            var imageObject = await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: objectName, // id.ToString(),
                contentType: "image/jpeg", // image.ContentType,
                source: f, //image.InputStream,
                options: new UploadObjectOptions { PredefinedAcl = imageAcl}
            );

            return imageObject.MediaLink;
        }
        // [END uploadimage]

        public async Task DeleteUploadedImage(long id)
        {
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, id.ToString());
            }
            catch (Google.GoogleApiException exception)
            {
                // A 404 error is ok.  The image is not stored in cloud storage.
                if (exception.Error.Code != 404)
                    throw;
            }
        }
    }
}