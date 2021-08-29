using dotnet_firebase.model;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using System;

namespace dotnet_firebase.Controllers
{
    /// <summary>
    /// Authentication
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private string web_key = "AIzaSyBd4zJpga4hbEoiQU2vgBdgV_KoYP9wyr8";

        /*IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "KJrJN3BWKBio7ato9TwKKxb9rlCEeak3mGq1Cf5O",
            BasePath = "https://dotnetfirebase-5415f-default-rtdb.firebaseio.com/"
        };*/

        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> Insert([FromBody] AuthUser data)
        {
            object response = null;
            try
            {
                var config = new FirebaseConfig(web_key);
                var auth = new FirebaseAuthProvider(config);

                response = await auth.CreateUserWithEmailAndPasswordAsync(data.email, data.passwd, data.email, true);

                ModelState.AddModelError(string.Empty, "added success");
            }
            catch (Exception ex)
            {
                response = ex;
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return new JsonResult(response);
        }


        [HttpPost]
        [Route("Login")]
        public async System.Threading.Tasks.Task<JsonResult> Login([FromBody] AuthUser data)
        {
            object response = null;
            try
            {
                var config = new FirebaseConfig(web_key);
                var auth = new FirebaseAuthProvider(config);

                FirebaseAuthLink fire_res = await auth.SignInWithEmailAndPasswordAsync(data.email, data.passwd);

                if(string.IsNullOrWhiteSpace(fire_res.FirebaseToken))
                {
                    response = new
                    {
                        status = "ng"
                    };
                }
                else
                {
                    response = new
                    {
                        token = fire_res.FirebaseToken,
                        User = fire_res.User
                    };
                }
                ModelState.AddModelError(string.Empty, "added success");
            }
            catch (Exception ex)
            {
                response = ex;
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return new JsonResult(response);
        }


    }
}
