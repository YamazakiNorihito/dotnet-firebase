using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System.IO;
using System.Collections;
using dotnet_firebase.model;

namespace dotnet_firebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        #region Create Db
        [HttpPost]
        [Route("create/autoid")]
        public JsonResult Auto()
        {
            var database = CreateConnection();

            CollectionReference coll = database.Collection("Add_Document");
            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                {"firstname", "aaaaa" },
                { "lastname","bbbbb"},
                {"phone nuber", 000011112222}
            };

            coll.AddAsync(data1);

            object response = null;

            response = new
            {
                msg = "成功しました"
            };

            return new JsonResult(response);

        }

        [HttpPost]
        [Route("create/customid")]
        public JsonResult Custom()
        {
            var database = CreateConnection();

            DocumentReference coll = database.Collection("Add_Document_Custom").Document("MyDoc");
            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                {"firstname", "aaaaa" },
                { "lastname","bbbbb"},
                {"phone nuber", 000011112222}
            };

            coll.SetAsync(data1);

            object response = null;

            response = new
            {
                msg = "成功しました"
            };

            return new JsonResult(response);

        }

        [HttpPost]
        [Route("create/array")]
        public JsonResult Array()
        {
            var database = CreateConnection();

            DocumentReference coll = database.Collection("Add_Array").Document("MyDoc");
            Dictionary<string, object> data1 = new Dictionary<string, object>();


            ArrayList arr = new ArrayList();

            arr.Add(123);
            arr.Add("name");
            arr.Add(true);

            data1.Add("Array", arr);

            coll.SetAsync(data1);

            object response = null;

            response = new
            {
                msg = "成功しました"
            };

            return new JsonResult(response);
        }


        #endregion

        #region Get
        [HttpGet]
        [Route("get/test")]
        public async Task<JsonResult> Get()
        {
            var database = CreateConnection();

            DocumentReference docref = database.Collection("emplist").Document("Websystem");

            DocumentSnapshot snap = await docref.GetSnapshotAsync();

            var result_data = new Dictionary<string, object>();
            if(snap.Exists)
            {
                Dictionary<string, object> data_dic = snap.ToDictionary();

                foreach(var item in data_dic)
                {
                    result_data.Add(item.Key, item.Value);
                }
            }

            var response = new
            {
                msg = "成功しました",
                result_data
            };

            return new JsonResult(response);
        }


        [HttpGet]
        [Route("get/city")]
        public async Task<JsonResult> city()
        {
            var database = CreateConnection();

            DocumentReference docref = database.Collection("city").Document("港区");

            DocumentSnapshot snap = await docref.GetSnapshotAsync();

            City cityDatas = null;
            if (snap.Exists)
            {
                cityDatas = snap.ConvertTo<City>();
            }

            var response = new
            {
                msg = "成功しました",
                cityDatas
            };

            return new JsonResult(response);
        }

        [HttpGet]
        [Route("get/alldocument")]
        public async Task<JsonResult> alldocument([FromQuery(Name = "nameOfCollection")] string nameOfCollection)
        {
            var database = CreateConnection();

            CollectionReference document = database.Collection(nameOfCollection);

            QuerySnapshot snap = await document.GetSnapshotAsync();



            List<City> resDatas = new List<City>();
            foreach(var docsnap in snap.Documents)
            {
                City city = docsnap.ConvertTo<City>();

                if(docsnap.Exists)
                {
                    resDatas.Add(city);
                }
            }



            var response = new
            {
                msg = "成功しました",
                resDatas
            };

            return new JsonResult(response);
        }
        #endregion

        #region update
        [HttpPut]
        [Route("update/field_set")]
        public async Task<JsonResult> update_field_set()
        {
            var database = CreateConnection();

            DocumentReference docref = database.Collection("testing1").Document("docs1");

            Dictionary<string, object> datas = new Dictionary<string, object>()
            {
                {"name" , "NICKEY"},
                {"url" , @"https://www.youtube.com/channel/UCj8dMCunPNRonWCff-ijeAQ"},
            };

            DocumentSnapshot snap = await docref.GetSnapshotAsync();

            if (snap.Exists)
            {
                await docref.SetAsync(datas);
            }

            var response = new
            {
                msg = "成功しました",
                datas
            };

            return new JsonResult(response);
        }

        [HttpPut]
        [Route("update/field")]
        public async Task<JsonResult> update_field()
        {
            var database = CreateConnection();

            DocumentReference docref = database.Collection("testing1").Document("docs1");

            Dictionary<string, object> datas = new Dictionary<string, object>()
            {
                {"name" , "NICKEY channel"},
            };

            DocumentSnapshot snap = await docref.GetSnapshotAsync();

            if (snap.Exists)
            {
                await docref.UpdateAsync(datas);
            }

            var response = new
            {
                msg = "成功しました",
                datas
            };

            return new JsonResult(response);
        }
        #endregion

        #region delete
        [HttpDelete]
        [Route("delete/coll")]
        public async Task<JsonResult> delete_coll()
        {
            var database = CreateConnection();

            DocumentReference docref = database.Collection("test2").Document("docs1");

            var result  = await docref.DeleteAsync();

            var response = new
            {
                msg = "成功しました",
                result
            };

            return new JsonResult(response);
        }

        [HttpDelete]
        [Route("delete/field")]
        public async Task<JsonResult> delete_field()
        {
            var database = CreateConnection();

            DocumentReference docref = database.Collection("test2").Document("docs2");

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"age", FieldValue.Delete }
            };


            var result = await docref.UpdateAsync(data);

            var response = new
            {
                msg = "成功しました",
                result
            };

            return new JsonResult(response);
        }

        [HttpDelete]
        [Route("delete/all")]
        public async Task<JsonResult> delete_all()
        {
            var database = CreateConnection();

            CollectionReference coll = database.Collection("test2");

            QuerySnapshot snap = await coll.Limit(3).GetSnapshotAsync();

            IReadOnlyList<DocumentSnapshot> docs = snap.Documents;

            List<object> result = new List<object>();
            while(docs.Count > 0)
            {
                foreach(DocumentSnapshot doc in docs)
                {
                    var d  = await doc.Reference.DeleteAsync();
                    result.Add(d);
                }

                snap = await coll.Limit(3).GetSnapshotAsync();
                docs = snap.Documents;
            }

            var response = new
            {
                msg = "成功しました",
                result
            };

            return new JsonResult(response);
        }
        #endregion

        private FirestoreDb CreateConnection()
        {

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dotnetfirebase-5415f-firebase-adminsdk-b2hyr-ff743743d0.json");

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            FirestoreDb db = FirestoreDb.Create("dotnetfirebase-5415f");


            return db;
        }


    }
}
