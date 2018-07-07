using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Configuration;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Escaner_Twain
{

    class StorageQuickstart
    {

        public static void Main(string[] args)
        {
            // Your Google Cloud Platform project ID.
            string projectId = "escaner-twain";
            // The name for the new bucket.
            

            try
            {
                /**                
                string jsonPath = ConfigurationManager.AppSettings["jsonPath"].ToString();

                //ApiLibrary authApiLibrary = new ApiLibrary();
                //authApiLibrary.AuthExplicit(projectId, jsonPath);

                var credential = GoogleCredential.FromFile(jsonPath);

                // Instantiates a client.
                StorageClient storageClient = StorageClient.Create(credential);

                // Creates the new bucket.
                storageClient.CreateBucket(projectId, bucketName);
                Console.WriteLine($"Bucket {bucketName} created.");
                */

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmEscaner());

            }
            catch (Google.GoogleApiException e)
            when (e.Error.Code == 409)
            {
                // The bucket already exists.  That's fine.
                Console.WriteLine(e.Error.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}