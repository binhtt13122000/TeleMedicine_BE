using TeleMedicine_BE.Utils;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using static TeleMedicine_BE.Utils.Constants;

namespace TeleMedicine_BE.ExternalService
{
    public interface IUploadFileService
    {
        Task<string> UploadFile(IFormFile file, string bucket, string directory);
    }
    public class UploadFileService : IUploadFileService
    {
        private readonly IConfiguration _configuration;

        public UploadFileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFile(IFormFile file, string bucket, string directory)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(AppSetting.FirebaseApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AppSetting.FirebaseAuthEmail, AppSetting.FirebaseAuthPassword);
            var task = new FirebaseStorage(
                AppSetting.FirebaseBucket,
                new FirebaseStorageOptions()
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                });
            string fileExtension = Path.GetExtension(file.FileName);
            Guid guid = Guid.NewGuid();
            string fileName = guid.ToString() + "." + fileExtension;
            return await task.Child(bucket)
                .Child(directory)
                .Child(fileName)
                .PutAsync(file.OpenReadStream());
        }
    }
}
