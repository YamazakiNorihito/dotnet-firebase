using dotnet_firebase.model;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_firebase.Controllers
{
    /// <summary>
    /// FirebaseRealtimeDatabase
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RealtimeDbController : ControllerBase
    {

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "",
            BasePath = "https://dotnetfirebase-5415f-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        [HttpGet]
        public JsonResult Get([FromQuery(Name = "id")] string id)
        {
            object response_data = null ;
            try
            {
                client = new FireSharp.FirebaseClient(config);


                if(string.IsNullOrWhiteSpace(id))
                {// list
                    var list = new List<Employee>();

                    FirebaseResponse response = client.Get("employee");
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                    foreach (var item in data)
                    {
                        var json_prop = ((JProperty)item).Value.ToString();
                        var emp_data = JsonConvert.DeserializeObject<Employee>(json_prop);
                        list.Add(emp_data);
                    }

                    response_data = list;
                }
                else
                {// single

                    FirebaseResponse response = client.Get("employee/" + id);
                    response_data = JsonConvert.DeserializeObject<Employee>(response.Body);
                }

                ModelState.AddModelError(string.Empty, "added success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return new JsonResult(response_data);
        }

        [HttpPost]
        public JsonResult Insert([FromBody]Employee data)
        {
            try
            {
                AddEmpToFireBase(data);
                ModelState.AddModelError(string.Empty, "added success");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return new JsonResult(data);
        }

        private void AddEmpToFireBase(Employee data)
        {
            client = new FireSharp.FirebaseClient(config);
            PushResponse res = client.Push("employee/", data);
            data.id = res.Result.name;
            SetResponse setRes = client.Set("employee/" + data.id, data);

        }

        [HttpPut]
        public JsonResult Update([FromQuery(Name = "id")] string id, [FromBody] Employee data)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                SetResponse response = client.Set("employee/" + id, data);

                ModelState.AddModelError(string.Empty, "added success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return new JsonResult(data);
        }

        [HttpDelete]
        [Route("{id}")]
        public JsonResult Delete([FromRoute(Name = "id")] string id)
        {
            FirebaseResponse response = null;
            try
            {
                client = new FireSharp.FirebaseClient(config);
                response = client.Delete("employee/" + id);

                ModelState.AddModelError(string.Empty, "added success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return new JsonResult(response);
        }
    }
}
