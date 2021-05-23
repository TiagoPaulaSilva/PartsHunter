using System.Threading.Tasks;

namespace PartsHunter.Repositories.Interfaces
{
    public interface IFirebaseRepositorie
    {
        dynamic JSON_Firebase { get; set; }

        bool Autheticate();

        void Login(string basePath, string authSecret);

        void Push_New_Component(string category, string description, string drawer);

        void Load_Firebase_Database();

        void DeleteComponent(string drawer);

        string GetAddressComponent(string drawer);

        void UpdateComponent(string drawer, string description);

        void SetHardwareDevice(string command_setup);

        Task<dynamic> GetComponent(string input);
    }
}
