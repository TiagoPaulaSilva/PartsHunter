using System;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using PartsHunter.Models;
using PartsHunter.Repositories.Interfaces;

namespace PartsHunter.Repositories
{
    public class FirebaseRepositorie : IFirebaseRepositorie
    {
        private IFirebaseConfig _config;
        private IFirebaseClient _client;

        public dynamic JSON_Firebase { get; set; }

        public FirebaseRepositorie()
        {

        }

        public void Login(string basePath, string authSecret)
        {
            _config = new FirebaseConfig {
                BasePath = basePath,
                AuthSecret = authSecret
            };

            Autheticate();
        }

        public bool Autheticate()
        {
            _client = new FireSharp.FirebaseClient(_config);

            if (_client == null || _config == null)
            {
                return false;
            }
            return true;
        }

        public void Push_New_Component(string category, string description, string drawer)
        {
            Component component = new Component {
                Description = description,
                Drawer = drawer
            };

            PushResponse response = _client.Push(category, component);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JsonConvert.SerializeObject(response).ToString();
            }
        }

        public void Load_Firebase_Database()
        {
            _ = GetComponent(String.Empty);
        }

        public void DeleteComponent(string drawer)
        {
            string todo = String.Empty;


            foreach (dynamic key in JSON_Firebase.Keys)
            {
                if (key != "void" && key != "HardwareDevice")
                {
                    foreach (dynamic key2 in JSON_Firebase[key].Keys)
                    {
                        if (JSON_Firebase[key][key2]["Drawer"] == drawer)
                        {
                            todo = key + "/" + key2;
                        }
                    }
                }
            }


            FirebaseResponse response = _client.Delete(todo);
            _ = JsonConvert.SerializeObject(response).ToString();
        }

        public string GetAddressComponent(string drawer)
        {
            string r = String.Empty;

            foreach (dynamic key in JSON_Firebase.Keys)
            {
                if (key != "void" && key != "HardwareDevice")
                {
                    foreach (dynamic key2 in JSON_Firebase[key].Keys)
                    {
                        if (JSON_Firebase[key][key2]["Drawer"] == drawer)
                        {
                            r = key + "/" + key2;
                        }
                    }
                }
            }

            return r;
        }

        public void UpdateComponent(string drawer, string description)
        {
            string address = GetAddressComponent(drawer);

            var todo = new
            {
                Description = description,
                Drawer = drawer
            };


            FirebaseResponse response = _client.Update(address, todo);
            _ = JsonConvert.SerializeObject(response).ToString();
        }

        public void SetHardwareDevice(string command_setup)
        {
            try
            {
                string address = "/";

                FirebaseResponse response = _client.Update(address,
                    new
                    {
                        HardwareDevice = command_setup
                    });

                string retorno = JsonConvert.SerializeObject(response).ToString();
            }
            catch
            {

            }
        }

        public async Task<dynamic> GetComponent(string input)
        {
            FirebaseResponse response = await _client.GetAsync(input);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string result = response.Body;
                JSON_Firebase = new JavaScriptSerializer().DeserializeObject(result);

                if (JSON_Firebase != null)
                {
                    return JSON_Firebase;
                }
            }

            return new JavaScriptSerializer().DeserializeObject("{void:0}");
        }
    }
}
