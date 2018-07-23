using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace weaReader
{
    class EPWDownload
    {
        //string url = "https://raw.githubusercontent.com/NREL/EnergyPlus/develop/weather/master.geojson";
        JObject epwData;
        List<string> urls = new List<string>();
        public EPWDownload()
        {
           StreamReader sr = new StreamReader("C:\\Users\\r.hudson\\Documents\\WORK\\piloto\\webDev\\data\\epwmaster.json");
           string value = sr.ReadToEnd();
            using (JsonTextReader reader = new JsonTextReader(new StringReader(value)))
            {
            this.epwData = (JObject)JToken.ReadFrom(reader);
            }
            JArray allposts = (JArray)this.epwData["features"];
            getUrls(allposts);
            EPWReader epwreader = new EPWReader(this.urls);
            //reached Kirksville Regional Ap number 2048
        }
        private void getUrls(JArray allposts)
        {
            for (int t = 0; t < allposts.Count; t++)
            {
                JObject props = (JObject)this.epwData.SelectToken("features[" + t + "].properties");
                string href = (string)props.SelectToken("epw");
                string file = href.Substring(href.IndexOf("=") + 1, href.IndexOf(">") - href.IndexOf("=") - 1);
                this.urls.Add(file);

            }
        }
        static public void makeCountriesCitiesObj()
        {
            string directory = "C:\\Users\\r.hudson\\Documents\\WORK\\piloto\\webDev\\data\\json\\allEPW";
            StreamReader sr = new StreamReader( directory+ "\\countrydata.js");
            string line = sr.ReadLine();
            string tag;
            List<Country> countries = new List<Country>();
            List<string> names = new List<string>();
            int count = 0;
            while (line!=null)
            {
                tag = line.Substring(line.IndexOf(" ") + 1, line.IndexOf("=") - line.IndexOf(" ")-1);
                line = line.Substring(line.IndexOf("[")+1, line.IndexOf("]") - line.IndexOf("[")-1);
                line = line.Replace("\"", "");
                string[] parts = line.Split(',');
                switch (tag)
                {
                    case "countries":
                        
                        for(int i=0;i<parts.Length;i++)
                        {
                            if(parts[i]!="")
                            {
                                names.Add(parts[i]);
                            }
                            names.Sort();
                        }
                        break;
                            default:
                        
                        foreach(string n in names )
                        {
                            
                            Country c = new Country(n);
                            if(c.name==tag)
                            {
                                count += parts.Length;
                                c.addCities(parts);
                                countries.Add(c);
                                break;
                            }
                        }
                        break;
                }
                

                line = sr.ReadLine();
            }
            sr.Close();
            countries.Sort();
            writeCountryDataJSON(countries, directory);
        }
        static private void writeCountryDataJSON(List<Country> countries,string directory)
        {
            StreamWriter writer = new StreamWriter(directory + "\\countrydata.json");
            JsonWriter climajson = new JsonTextWriter(writer);
            climajson.Formatting = Formatting.Indented;
            climajson.WriteStartObject();
            
            climajson.WritePropertyName("countries");
            climajson.WriteStartArray();
            for (int i = 0; i < countries.Count; i++)
            {
                climajson.WriteStartObject();
                climajson.WritePropertyName("country");
                climajson.WriteValue(countries[i].name);
                climajson.WritePropertyName("cities");
                climajson.WriteStartArray();
                for (int j = 0; j < countries[i].cities.Count; j++)
                {
                    climajson.WriteValue(countries[i].cities[j]);
                }
                climajson.WriteEndArray();
                climajson.WriteEndObject();

            }
            climajson.WriteEndArray();
            climajson.WriteEndObject();
            climajson.Close();
        }
        private JObject readfromwebJson(string url)
        {
            WebClient client = new WebClient();
            JObject data = new JObject();
            // Download string.
            string value = client.DownloadString(url);
            StreamWriter sw = new StreamWriter("C:\\Users\\r.hudson\\Documents\\WORK\\piloto\\webDev\\data\\epwmaster.json");
            sw.Write(value);
            sw.Close();

                using (JsonTextReader reader = new JsonTextReader(new StringReader(value)))

                {
                    data = (JObject)JToken.ReadFrom(reader);

                }
            return data;
        }
    }
}
