using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MINSIProjekt
{
    public class City
    {
        public string id;
        public string name;
        public Dictionary<string, string> roads = new Dictionary<string, string>();

        public City(string givenid, string givenname)
        {
            id = givenid;
            name = givenname;
        }

        public static void ReadJSONFile(string jsonFileIn, List<City> list)
        {
            dynamic jsonfile = JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn));
            for (int i = 0; i < 18; i++) {
                City city = new City(jsonfile[i]["id"].ToString(), jsonfile[i]["name"].ToString());
                for(int j = 0; j< 17; j++)
                {
                    city.roads.Add(jsonfile[i]["roads"][j]["Key"].ToString(), jsonfile[i]["roads"][j]["Value"].ToString());
                }
                list.Add(city);
            }
        }
    } 
}
