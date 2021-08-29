using dotnet_firebase.model;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_firebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private static string ApiKey = "";
        private static string Bucket = "dotnetfirebase-5415f.appspot.com";
        private static string AuthEmail = "";
        private static string AuthPassword = "";

        [HttpPost]
        public async Task<JsonResult> FilesUpload(List<IFormFile> files)
        {
            object response = null;

            /**/

            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new System.Threading.CancellationTokenSource();

            /**/
            long size = files.Sum(f => f.Length);
            var datas = new List<string[]>();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var task = new FirebaseStorage(
                            Bucket,
                            new FirebaseStorageOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                                ThrowOnCancel = true
                            }
                        )
                        .Child("images")
                        .Child(formFile.Name)
                        .PutAsync(formFile.OpenReadStream(), cancellation.Token);

                    var data = new string[3];

                    data[0] = formFile.FileName;
                    data[1] = formFile.Name;
                    data[1] = await task;

                    datas.Add(data);
                    //var filePath = Path.GetTempFileName();

                    /*
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    */
                }
            }

            response = new
            {
                datas,
                size
            };

            return new JsonResult(response);
        }

    }
}
