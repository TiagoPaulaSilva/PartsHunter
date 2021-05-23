using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartsHunter.Models;

namespace PartsHunter.Repositories.Interfaces
{
    public interface ILocalRepositorie
    {
        void SaveData(Led led);

        Led LoadData();
    }
}
