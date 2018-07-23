using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weaReader
{
    class LocationData
    {


        public string name;
        public string where;//country
        public string region;// State Province Region
        public string wmo;//some kind of code for enrgy plus
        public string source;//data source 
        
	    public float longitude;
        public float timezone;
        public float latitude;
        public float altitude;
        public int day;
        public int year;
        public int sky;
        public List<DataField> monthlyDataFields = new List<DataField>();
        public List<List<double>> monthlyData = new List<List<double>>();

        public List<DataField> hourlyDataFields = new List<DataField>();
        public List<List<double>> hourlyData = new List<List<double>>();

        public void cleanMonthlyValues()
        {
            
            for (int i = 0; i < this.monthlyData.Count; i++)
            {
                double total = 0;
                // array represents one field of monthly averages
                for (int j = 0; j < this.monthlyData[i].Count; j++)
                {
                    total += this.monthlyData[i][j];
                    if (this.monthlyData[i][j] == this.monthlyDataFields[i].missingValue) this.monthlyDataFields[i].missingData++;
                }
                if (total > 0) this.monthlyDataFields[i].allZeros = false;
                else this.monthlyDataFields[i].allZeros = true;

            }
        }
        public void cleanHourlyValues()
        {
            cleanHourlyZeroValues();
            cleanWithMinValues();
        }
        private void cleanHourlyZeroValues()
        {

            double[] fieldInUse = new double[this.hourlyDataFields.Count];
           
            
            //get the fields with zero
            for(int i=0;i< this.hourlyData.Count;i++)
            {
                for(int j=0;j< this.hourlyData[i].Count;j++)
                {
                    fieldInUse[j] += this.hourlyData[i][j];
                }
            }
            for(int i=0;i<fieldInUse.Length;i++)
            {
                if(fieldInUse[i]==0)
                {
                    this.hourlyDataFields[i].allZeros = true;
                }
                else
                {
                    this.hourlyDataFields[i].allZeros = false;
                }
            }
           
        }
        private void cleanWithMinValues()
        {
            for (int i = 0; i < this.hourlyData.Count; i++)
            {
                for (int j = 0; j < this.hourlyData[i].Count; j++)
                {
                    if (this.hourlyData[i][j] == this.hourlyDataFields[j].missingValue)
                    {
                        this.hourlyDataFields[j].missingData++;
                    }
                }
            }
        }
        
        public void defineWEAHourlyFields()
        {
            this.hourlyDataFields.Add(new DataField("Dry Bulb Temperature", "C", -15, 40, 99.9));
            this.hourlyDataFields.Add(new DataField("Relative Humidity", "%", 0, 100, 999));
            this.hourlyDataFields.Add(new DataField("Wind Speed", "m/s", 0, 40, 999));
            this.hourlyDataFields.Add(new DataField("Wind Direction", "degrees", 0, 360, 9999));
            
            this.hourlyDataFields.Add(new DataField("Diffuse Horizontal Radiation", "Wh", 0, 1000, 9999));
            this.hourlyDataFields.Add(new DataField("Direct Normal Radiation", "Wh", 0, 1000, 9999));
            this.hourlyDataFields.Add(new DataField("Total Sky Cover", "/10ths", 0, 10, 99));
            this.hourlyDataFields.Add(new DataField("Liquid Precipitation Depth", "mm", 0, 1000, 999));

        }
        public void defineWEAMonthlyFields()
        {
            //28 spaces in the definintion see http://wiki.naturalfrequency.com/wiki/WeatherTool/File_Format
            //max min and missing values have been estimated
            this.monthlyDataFields.Add(new DataField("Liquid Precipitation Depth", "mm", 0, 1000, 999));
            this.monthlyDataFields.Add(new DataField("Relative Humidity 0900", "%", 0, 110, 999));
            this.monthlyDataFields.Add(new DataField("Relative Humidity 1500", "%", 0, 110, 999));
            this.monthlyDataFields.Add(new DataField("Dry Bulb Temperature", "C", -15, 40, 99.9));
            this.monthlyDataFields.Add(new DataField("Diffuse Horizontal Radiation", "Wh", 0, 120000, 9999));
            this.monthlyDataFields.Add(new DataField("Dry Bulb Temperature Max", "C", -15, 40, 99.9));
            this.monthlyDataFields.Add(new DataField("Dry Bulb Temperature Min", "C", -15, 40, 99.9));
            this.monthlyDataFields.Add(new DataField("Dry Bulb Temperature Std dev", "C", -15, 40, 99.9));
            this.monthlyDataFields.Add(new DataField("Daylight hours", "hrs", 0, 24, 99.9));
            this.monthlyDataFields.Add(new DataField("Irradiation", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("Degree hours heat", "hrs", 0, 240, 99));
            this.monthlyDataFields.Add(new DataField("Degree hours cool", "hrs", 0, 240, 99));
            this.monthlyDataFields.Add(new DataField("Degree hours solar", "hrs", 0, 240, 99));
            this.monthlyDataFields.Add(new DataField("Wind Speed 0900", "m/s", 0, 40, 999));
            this.monthlyDataFields.Add(new DataField("Wind Speed 1500", "m/s", 0, 40, 999));
            this.monthlyDataFields.Add(new DataField("16", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("17", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("18", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("19", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("20", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("21", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("22", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("23", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("24", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("25", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("26", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("27", "", 0, 10000, 9999));
            this.monthlyDataFields.Add(new DataField("28", "", 0, 10000, 9999));
        }
        public void defineEPWFields()
        {
            //dbTemp;//field Dry Bulb Temperature\units C\minimum > -70\maximum < 70\missing 99.9
            this.hourlyDataFields.Add(new DataField("Dry Bulb Temperature", "C", -15, 40, 99.9));
            //dpTemp; //field Dew Point Temperature\units C\minimum > -70\maximum < 70\missing 99.9
            this.hourlyDataFields.Add(new DataField("Dew Point Temperature", "C", -15, 40, 99.9));
            //rHumid; //field Relative Humidity\missing 999.\minimum 0\maximum 110
            this.hourlyDataFields.Add(new DataField("Relative Humidity", "%", 0, 110, 999));
            //stationPress; // Atmospheric Station Pressure
            this.hourlyDataFields.Add(new DataField("Atmospheric Station Pressure", "Pa", 50000, 120000, 999999));
            //ETHorizRad; //Extraterrestrial Horizontal Radiation\units Wh / m2\missing 9999.\minimum 0
            this.hourlyDataFields.Add(new DataField("Extraterrestrial Horizontal Radiation", "Wh/m2", 0, 1500, 9999));
            //ETDirectNormalRad;//Extraterrestrial Direct Normal Radiation\units Wh / m2\missing 9999.\minimum 0
            this.hourlyDataFields.Add(new DataField("Extraterrestrial Direct Normal Radiation", "Wh/m2", 0, 1500, 9999));
            //horizIRRad;//Horizontal Infrared Radiation Intensity\units Wh / m2\missing 9999.\minimum 0
            this.hourlyDataFields.Add(new DataField("Horizontal Infrared Radiation Intensity", "Wh/m2", 0, 1000, 9999));
            //globalHorizRad;//Global Horizontal Radiation \units Wh / m2\missing 9999.\minimum 0
            this.hourlyDataFields.Add(new DataField("Global Horizontal Radiation", "Wh/m2", 0, 1000, 9999));
            //normDirectRad;//field Direct Normal Radiation \units Wh / m2\missing 9999.\minimum 0
            this.hourlyDataFields.Add(new DataField("Direct Normal Radiation", "Wh/m2", 0, 1000, 9999));
            //diffuseHorizRad;// field Diffuse Horizontal Radiation\units Wh / m2\missing 9999.\minimum 0
            this.hourlyDataFields.Add(new DataField("Diffuse Horizontal Radiation", "Wh/m2", 0, 1000, 9999));
            //horizIRIntensity //\field Horizontal Infrared Radiation Intensity\units Wh/m2\missing 9999.
            this.hourlyDataFields.Add(new DataField("Horizontal Infrared Radiation Intensity", "Wh/m2", 0, 100000, 9999));
            //globalHorizIllum;//Global Horizontal Illuminance\units lux\missing 999999.\note will be missing if > = 999900\minimum 0
            this.hourlyDataFields.Add(new DataField("Global Horizontal Illuminance", "lux", 0, 100000, 999900));
            //directNormalIllum;//field Direct Normal Illuminance\units lux\missing 999999.\note will be missing if > = 999900\minimum 0
            this.hourlyDataFields.Add(new DataField("Direct Normal Illuminance", "lux", 0, 100000, 999900));
            //diffHorizIllum;//field Diffuse Horizontal Illuminance\units lux\missing 999999.\note will be missing if > = 999900\minimum 0
            this.hourlyDataFields.Add(new DataField("Diffuse Horizontal Illuminance", "lux", 0,50000, 999900));
            //zenithLum;//field Zenith Luminance\units Cd / m2\missing 9999.\note will be missing if > = 9999\minimum 0
            this.hourlyDataFields.Add(new DataField("Zenith Luminance", "Cd/m2", 0, 1000, 9999));
            //wSpeed;// field Wind Direction\units degrees\missing 999.\minimum 0\maximum 360
            this.hourlyDataFields.Add(new DataField("Wind Direction", "degrees", 0, 360, 9999));
            //wDir;// field Wind Speed\units m / s\missing 999.\minimum 0\maximum 40
            this.hourlyDataFields.Add(new DataField("Wind Speed", "m/s", 0, 40, 999));
            //skyCover;// field Total Sky Cover\missing 99\minimum 0\maximum 10
            this.hourlyDataFields.Add(new DataField("Total Sky Cover", "/10ths", 0, 10, 99));
            //opaqueSkyCover;//field Opaque Sky Cover (used if Horizontal IR Intensity missing)\missing 99\minimum 0\maximum 10
            this.hourlyDataFields.Add(new DataField("Opaque Sky Cover", "/100ths", 0, 100, 99));
            //visibility;//field Visibility\units km\missing 9999
            this.hourlyDataFields.Add(new DataField("Visibility", "m", 0, 100000, 9999));
            //ceilingHt;//field Ceiling Height\units m\missing 99999
            this.hourlyDataFields.Add(new DataField("Ceiling Height", "m", 0, 10000, 99999));
            //presWeaObs;//field Present Weather Observation
            this.hourlyDataFields.Add(new DataField("Present Weather Observation", "", 0, 10000, 99999));
            //prsWeaCodes;//field Present Weather Codes
            this.hourlyDataFields.Add(new DataField("Present Weather Codes", "", 0, 10000, 99999));
            //precipWater;//field Precipitable Water\units mm\missing 999
            this.hourlyDataFields.Add(new DataField("Precipitable Water", "mm", 0, 1000, 999));
            //aeroOpticalDepth;//field Aerosol Optical Depth\units thousandths\missing .999
            this.hourlyDataFields.Add(new DataField("Aerosol Optical Depth", "1/1000ths", 0, 1000, 01));
            //snowDepth;//field Snow Depth\units cm\missing 999
            this.hourlyDataFields.Add(new DataField("Snow Depth", "cm", 0, 1000, 88));
            //daysSinceSnow;//field Days Since Last Snowfall\missing 99
            this.hourlyDataFields.Add(new DataField("Days Since Last Snowfall", "days", 0, 100, 99));
            //albedo;//field Albedo\missing 999
            this.hourlyDataFields.Add(new DataField("Albedo", "", 0, 100, 999));
            //liquidPrecipDepth;// field Liquid Precipitation Depth\units mm\missing 999
            this.hourlyDataFields.Add(new DataField("Liquid Precipitation Depth", "mm", 0, 1000, 999));
            //liquidPrecipQuantity;//field Liquid Precipitation Quantityunits hr\missing 99
            this.hourlyDataFields.Add(new DataField("Liquid Precipitation Quantity", "hr", 0, 100, 99));
        }
    }
}
