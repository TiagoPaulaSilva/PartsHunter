using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using PartsHunter.Models;
using PartsHunter.Repositories.Interfaces;

namespace PartsHunter.Repositories
{
    public class LocalRepositorie: ILocalRepositorie
    {
        public void SaveData(Led led)
        {
            Hashtable variables = new Hashtable
            {
                { "Color", led.Color },
                { "Time", led.Time },
                { "Brightness", led.Brightness }
            };

            FileStream fs = new FileStream("DataFile.dat", FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(fs, variables);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public Led LoadData()
        {
            if (File.Exists("DataFile.dat"))
            {
                FileStream fs = new FileStream("DataFile.dat", FileMode.OpenOrCreate);
                Hashtable variables = new Hashtable();

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    variables = (Hashtable)formatter.Deserialize(fs);
                }
                catch
                {

                }
                finally
                {
                    fs.Close();
                }

                return new Led() {
                    Color = (Color)variables["Color"],
                    Time = (int)variables["Time"],
                    Brightness = (int)variables["Brightness"]
                };
            }
            else
            {
                SaveData(new Led() {
                    Color = Color.FromArgb(0, 255, 0),
                    Time = 100,
                    Brightness = 128
                });
                return LoadData();
            }
        }
    }
}
