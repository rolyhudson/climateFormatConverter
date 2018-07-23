using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace weaReader
{
    class EPWReader
    {
        string[] epwFiles;
        List<LocationData> cd = new List<LocationData>();
        List<Country> countries = new List<Country>();
        string directory = "C:\\Users\\r.hudson\\Documents\\WORK\\piloto\\webDev\\data\\json\\allEPW\\";
        //double[] missingValues = {99.9,99.9,999.0,999999.0,9999.0,9999.0,9999.0, 9999.0, 9999.0, 9999.0,999999.0, 999999.0, 999999.0,9999.0,999.0,999.0,99.0,99.0,9999.0,99999.0,-1,-1,999.0,0.999,999.0,99.0,999.0,999.0,99.0};
        //string[] fields = { "Dry bulb temp", "Dew point temp", "Relative humidity", "Station pressure", "Extraterrestrial horizontal radiation",
        //    "Extraterrestrial direct normal Radiation","Horizontal infrared intensity", "Global horizontal radiation ", "Normal direct radiation",
        //    "Diffuse horizontal radiation", "Global horizontal illuminance", "Direct normal illuminance ", "Diffuse horizontal illumianance","Zenith luminance", "Wind direction",
        //    "Wind speed",  "Sky cover", "Opaque sky cover", "Visibility","Ceiling height", "Present weather observation", "Present weather codes", "Precipitable water",
        //    "Aerosol optical depth", "Snow depth", "Days since last snowfall", "Albedo", "Liquid precipitation depth", "Liquid precipitation quantity"};
        //string[] monthlylydatafields = { };
        //all possible fields from epw are:
        //dbTemp;//field Dry Bulb Temperature\units C\minimum > -70\maximum < 70\missing 99.9
        //dpTemp; //field Dew Point Temperature\units C\minimum > -70\maximum < 70\missing 99.9
        //rHumid; //field Relative Humidity\missing 999.\minimum 0\maximum 110
        //stationPress; // Atmospheric Station Pressure
        //ETHorizRad; //Extraterrestrial Horizontal Radiation\units Wh / m2\missing 9999.\minimum 0
        //ETDirectNormalRad;//Extraterrestrial Direct Normal Radiation\units Wh / m2\missing 9999.\minimum 0
        //horizIRRad;//Horizontal Infrared Radiation Intensity\units Wh / m2\missing 9999.\minimum 0
        //globalHorizRad;//Global Horizontal Radiation \units Wh / m2\missing 9999.\minimum 0
        //normDirectRad;//field Direct Normal Radiation \units Wh / m2\missing 9999.\minimum 0
        //diffuseHorizRad;// field Diffuse Horizontal Radiation\units Wh / m2\missing 9999.\minimum 0
        //horizIRIntensity //\field Horizontal Infrared Radiation Intensity\units Wh/m2\missing 9999.
        //globalHorizIllum;//Global Horizontal Illuminance\units lux\missing 999999.\note will be missing if > = 999900\minimum 0
        //directNormalIllum;//field Direct Normal Illuminance\units lux\missing 999999.\note will be missing if > = 999900\minimum 0
        //diffHorizIllum;//field Diffuse Horizontal Illuminance\units lux\missing 999999.\note will be missing if > = 999900\minimum 0
        //zenithLum;//field Zenith Luminance\units Cd / m2\missing 9999.\note will be missing if > = 9999\minimum 0
        //wSpeed;// field Wind Direction\units degrees\missing 999.\minimum 0\maximum 360
        //wDir;// field Wind Speed\units m / s\missing 999.\minimum 0\maximum 40
        //skyCover;// field Total Sky Cover\missing 99\minimum 0\maximum 10
        //opaqueSkyCover;//field Opaque Sky Cover (used if Horizontal IR Intensity missing)\missing 99\minimum 0\maximum 10
        //visibility;//field Visibility\units km\missing 9999
        //ceilingHt;//field Ceiling Height\units m\missing 99999
        //presWeaObs;//field Present Weather Observation
        //prsWeaCodes;//field Present Weather Codes
        //precipWater;//field Precipitable Water\units mm\missing 999
        //aeroOpticalDepth;//field Aerosol Optical Depth\units thousandths\missing .999
        //snowDepth;//field Snow Depth\units cm\missing 999
        //daysSinceSnow;//field Days Since Last Snowfall\missing 99
        //albedo;//field Albedo\missing 999
        //liquidPrecipDepth;// field Liquid Precipitation Depth\units mm\missing 999
        //liquidPrecipQuantity;//field Liquid Precipitation Quantityunits hr\missing 99
        public List<DataField> hourlyDataFields = new List<DataField>();

        public EPWReader(string[] files)//from a folder
        {
            this.epwFiles = files;
            foreach (string file in files)
            {

                if (StringTools.getExt(file) == ".epw")
                {
                    
                    StreamReader epw = new StreamReader(file);
                    readEPWFile(epw);
                }
                //otherwise ignore
            }

        }
        public EPWReader(List<string> urls)//from online where data is the epw url
        {
            int count = 0;
            foreach (string file in urls)
            {
           
                    WebClient wb = new WebClient();

                    Stream data = wb.OpenRead(file);
                    StreamReader epw = new StreamReader(data);
                    readEPWFile(epw);
                    
        
                count++;
            }
            writeCountryData();
            //writeCountryDataJSON();

        }
        private void writeCountryData()
        {
            StreamWriter sr = new StreamWriter(this.directory + "\\countrydata.js");
            sr.Write("var countries =[");
            for (int i = 0; i < this.countries.Count; i++)
            {
               if(i== this.countries.Count-1)sr.Write('"' + this.countries[i].name + '"');
               else sr.Write('"' + this.countries[i].name + '"' + ",");
            }
            sr.WriteLine("];");
            for (int i = 0; i < this.countries.Count; i++)
            {
                //var CAN = ["Vancouver Int'l","Shearwater"];
                sr.Write("var "+this.countries[i].name+"=[");
                for (int j = 0; j < this.countries[i].cities.Count; j++)
                {
                    if(j==this.countries[i].cities.Count-1) sr.Write('"' + this.countries[i].cities[j] + '"');
                    else sr.Write('"' + this.countries[i].cities[j] + '"' + ",");
                }
                sr.WriteLine("];");
                
            }
            sr.Close();
        }
        
        public List<LocationData> getData()
        {
            return this.cd;
        }
        private void readEPWFile(StreamReader epw)
        {
            LocationData ld = new LocationData();
            ld.defineEPWFields();
            
            string line = epw.ReadLine();
            string[] data;
            
            while(line!=null)
            {
                data = line.Split(',');
                switch(data[0])
                {
                    case "LOCATION":
                        getLocationData(data, ref ld);
                        break;
                    case "DATA PERIODS":
                        line = epw.ReadLine();//read first hour
                        while (line != null)
                        {
                            ld.hourlyData.Add(getHourlyValueList(line));
                            line = epw.ReadLine();
                        }
                       
                        break;

                }
                line = epw.ReadLine();
            }
            ld.cleanHourlyValues();
            ld.cleanMonthlyValues();
            string directory = this.directory+ld.where;
            ClimaJSONWriter climajson = new ClimaJSONWriter(ld, directory);
            recordPlace(ld.where, ld.name);
            //cd.Add(ld);
            epw.Close();
        }
        private void recordPlace(string where, string name)
        {
            bool gotCountry = false;
            for(int i=0;i<this.countries.Count;i++)
            {
                if (this.countries[i].name == where)
                {
                    gotCountry = true;
                    this.countries[i].cities.Add(name);
                    break;
                }
            }
            if(!gotCountry)
            {
                this.countries.Add(new Country(where, name));
            }
        }
        private List<double> getHourlyValueList(string line)
        {
            List<double> hourlyvalues = new List<double>();
            string[] data = line.Split(',');
            //value 6 is the first data field
            for (int i = 6; i < data.Length; i++)
            {
                hourlyvalues.Add(Convert.ToDouble(data[i]));
            }
                return hourlyvalues;
        }
        
        private void getLocationData(string[] data,ref LocationData ld)
        {
            for(int i=1;i<data.Length;i++)
            {
                switch(i)
                {
                    case 1:
                        //remove nonalphanumerric from name
                        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                        data[i] = rgx.Replace(data[i], "");
                        ld.name = data[i];
                        break;
                    case 2:
                        ld.region = data[i];
                        break;
                    case 3:
                        ld.where = data[i];
                        break;
                    case 4:
                        ld.source = data[i];
                        break;
                    case 5:
                        ld.wmo = data[i];
                        break;
                    case 6:
                        ld.latitude = float.Parse(data[i]);
                        break;
                    case 7:
                        ld.longitude = float.Parse(data[i]);
                        break;
                    case 8:
                        ld.timezone = float.Parse(data[i]);
                        break;
                    case 9:
                        ld.altitude = float.Parse(data[i]);
                        break;
                }
            }
        }
        
    }
    class Country : IComparable<Country>
    {
        public List<string> cities = new List<string>();
        public string name;
        public Country(string countryname, string cityname)
        {
            this.name = countryname;
            this.cities.Add(cityname);
        }
        public Country(string countryname)
        {
            this.name = countryname;
            
        }
        public void addCities(string[] c)
        {
            for(int i = 0;i< c.Length;i++)
            {
                this.cities.Add(c[i]);
            }
            this.cities.Sort();
        }
        public int CompareTo(Country other)
        {
            return name.CompareTo(other.name);
        }
    }
}
