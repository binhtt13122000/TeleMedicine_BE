using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ExternalService
{
    public interface IFirestoreService
    {
        public Task<Dictionary<string, object>> Get(string collection, string key);

        public Task<bool> Create(string collection, string key, Dictionary<string, object> data);
        public Task<bool> Delete(string collection, string key);
        public Task<bool> Update(string collection, string key, Dictionary<string, object> data);

    }
    public class FirestoreService : IFirestoreService
    {
        FirestoreDb firestoreDb;
        public FirestoreService()
        {
            //Environment.SetEnvironmentVariable("GOOLE_APPLICATION_CREDENTIALS", "C:\Users\binht\source\repos\TeleMedicine_BE\TeleMedicine_BE\Keys\");
            firestoreDb = FirestoreDb.Create("telemedicine-fc0ee");

        }

        public async Task<bool> Create(string collection, string key, Dictionary<string, object> data)
        {
            DocumentReference documentReference = firestoreDb.Collection(collection).Document(key);
            await documentReference.CreateAsync(data);
            return true;
        }

        public async Task<bool> Delete(string collection, string key)
        {
            DocumentReference documentReference = firestoreDb.Collection(collection).Document(key);
            await documentReference.DeleteAsync();
            return true;
        }

        public async Task<Dictionary<string, object>> Get(string collection, string key)
        {
            DocumentReference documentReference = firestoreDb.Collection(collection).Document(key);
            DocumentSnapshot snapshot = await documentReference.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ToDictionary();
            }

            return null;
        }

        public async Task<bool> Update(string collection, string key, Dictionary<string, object> data)
        {
            DocumentReference documentReference = firestoreDb.Collection(collection).Document(key);
            await documentReference.UpdateAsync(data);
            return true;   
        }
    }
}
