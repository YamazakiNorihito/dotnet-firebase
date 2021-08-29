using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_firebase.model
{
    [FirestoreData]
    public class City
    {
        [FirestoreProperty]
        public int population { get; set; }
        [FirestoreProperty]
        public int area { get; set; }
    }
}
