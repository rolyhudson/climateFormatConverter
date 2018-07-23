using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weaReader
{
    class WEAReader
    {
        
        string[] weaFiles;
        List<LocationData> cd = new List<LocationData>();
        //string[] hourlydatafields = { "dbTemp", "rHumid", "wDir", "wSpeed", "diffuseHorizRad", "normDirectRad", "skyCover", "liquidPrecipDepth" };
        //string[] monthlydatafields = { "RAINFALL", "HUMIDITY_0900", "HUMIDITY_1500", "TEMPERATURE", "diffuseHorizRad", "TEMP_AVEMAX", "TEMP_AVEMIN",
        //    "TEMP_STDDEV","DAYLIGHTHOURS", "IRRADIATION", "DEGREEHOURS_HEAT", "DEGREEHOURS_COOL", "DEGREEHOURS_SOLAR", "WINDSPEED_0900", "WINDSPEED_1500", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27"};
        //note 28 possible montly averages in a wea

        public WEAReader(string[] files)
        {
            this.weaFiles = files;
            foreach (string file in files)
            {
                
                if(StringTools.getExt(file)==".wea")readWEAFile(file);
                //otherwise ignore
            }
            
        }
        public List<LocationData> getData()
        {
            return this.cd;
        }
        
        private void nameClean(ref LocationData ld)
        {
            string[] cities = { "Barrancabermeja", "Barranquilla", "Bogota", "Bucaramanga", "Buenaventura", "Cali", "Cartagena", "Cucuta","Nevia", "Gaviotas", "Girardot", "Ibague", "Ipiales", "Leticia", "Medellin", "Pereira", "Quibdo", "SanAndresIsla", "SantaMarta", "Turbo", "Valledupar", "Villavicencio", "LONDON_GATWICK", "Los Angeles", "Vancouver Int'l", "Shearwater" };
            for(int i=0;i<cities.Length;i++)
            {
                if (ld.name.Contains(cities[i]))
                    {
                    ld.name = cities[i];
                }
            }
        }
        private void readWEAFile(string fileName)
        {
           
            // FORMAT AT http://wiki.naturalfrequency.com/wiki/WeatherTool/File_Format
            LocationData ld = new LocationData();
            //fillFieldLists(ld);
            ld.defineWEAHourlyFields();
            ld.defineWEAMonthlyFields();
            int hour = 0;
            int day = 0;
            int wData = 0;
            int month = 0;
            using (BinaryReader b = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                var identifier = b.ReadBytes(16);
                var myString = System.Text.Encoding.UTF8.GetString(identifier);
                ld.name =StringTools.CleanInput( new string(b.ReadChars(32)));
                nameClean(ref ld);
                ld.where = StringTools.CleanInput(new string(b.ReadChars(32)));
                ld.where = "COL";
                //hourly data
                while (day < 365)
                {
                    while (hour < 24)
                    {
                        List<double> hourlyvalues = new List<double>();
                        for (int i = 0; i < 8; i++)
                        {
                            //read the 8 hourly values
                            hourlyvalues.Add((double)BitConverter.ToInt16(b.ReadBytes(2), 0));
                        }
                        for (int i = 0; i < hourlyvalues.Count; i++)
                        {
                            if (i == 0) hourlyvalues[i] = hourlyvalues[i] / 10.0;
                            if (i == 2) hourlyvalues[i] = hourlyvalues[i]/10 * 0.277778;//convert km/h to m/s
                            if (i == 6) hourlyvalues[i] = Math.Round(hourlyvalues[i] / 10.0);

                        }
                        ld.hourlyData.Add(hourlyvalues);
                        hour++;
                    }
                    hour = 0;
                    day++;
                }
                while (wData < 28)
                {
                    List<double> monthlyvalues = new List<double>();
                    while (month < 12)
                    {
                        monthlyvalues.Add((double)BitConverter.ToInt32(b.ReadBytes(4), 0));
                        month++;
                    }
                    
                    ld.monthlyData.Add(monthlyvalues);
                    month = 0;
                    wData++;
                }

                ld.longitude = b.ReadSingle();
                ld.timezone = b.ReadSingle();
                ld.latitude = b.ReadSingle();
                ld.altitude = b.ReadSingle();
                ld.day = BitConverter.ToInt32(b.ReadBytes(4), 0);
                ld.sky = BitConverter.ToInt32(b.ReadBytes(4), 0);
                if (b.BaseStream.Position == b.BaseStream.Length)
                {
                    //reach eof
                }
                else
                {
                    //more stuff to read somethings wrong!
                }
            }
            ld.cleanHourlyValues();
            ld.cleanMonthlyValues();
            this.cd.Add(ld);
        }
    }
}
