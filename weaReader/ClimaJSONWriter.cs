using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace weaReader
{
    class ClimaJSONWriter
    {
        LocationData ld = new LocationData();
        string directory;
        JsonWriter climajson;
        StreamWriter writer;
        string filename;
        int minMonthly = 5;
        int minHourly = 1000;
        public ClimaJSONWriter(LocationData locData,string d)
        {
            this.ld = locData;
            this.directory = d;
            writeClima();
        }
        private void writeClima()
        {
            Directory.CreateDirectory(this.directory);
            // if there is a name or a region
            if (this.ld.name != "" || this.ld.where != "" || this.ld.region != "")
            {
                if (this.ld.where != "" && this.ld.name == "")
                {
                    //flag a warning that file over write is possibe
                    MessageBox.Show("No place name is supplied. File names will be formatted: country_longitude_latitude.json", "Important Message");
                    this.filename = this.ld.where + "_" + this.ld.longitude.ToString() + "_" + this.ld.latitude.ToString() + ".json";
                }
                this.filename = this.ld.where + "_" + this.ld.name + ".json";
            }
            else
            {
                this.filename = this.ld.longitude.ToString() + "_" + this.ld.latitude.ToString() + ".json";
            }

            this.writer = new StreamWriter(this.directory +"\\"+ this.filename);
            this.climajson = new JsonTextWriter(this.writer);
            this.climajson.Formatting = Formatting.None;
            writeJSON();
            
            this.climajson.Close();
            this.writer.Close();
        }
        private void writeHourlyDataField(int a)
        {
            this.climajson.WriteStartObject();
            this.climajson.WritePropertyName("name");
            this.climajson.WriteValue(ld.hourlyDataFields[a].name);
            this.climajson.WritePropertyName("units");
            this.climajson.WriteValue(ld.hourlyDataFields[a].units);
            this.climajson.WritePropertyName("min");
            this.climajson.WriteValue(ld.hourlyDataFields[a].min);
            this.climajson.WritePropertyName("max");
            this.climajson.WriteValue(ld.hourlyDataFields[a].max);
            this.climajson.WriteEndObject();
        }
        private void writeMonthlyDataField(int a)
        {
            this.climajson.WriteStartObject();
            this.climajson.WritePropertyName("name");
            this.climajson.WriteValue(ld.monthlyDataFields[a].name);
            this.climajson.WritePropertyName("units");
            this.climajson.WriteValue(ld.monthlyDataFields[a].units);
            this.climajson.WritePropertyName("min");
            this.climajson.WriteValue(ld.monthlyDataFields[a].min);
            this.climajson.WritePropertyName("max");
            this.climajson.WriteValue(ld.monthlyDataFields[a].max);
            this.climajson.WriteEndObject();
        }
        private void writeJSON()
        {
            this.climajson.WriteStartObject();
            if (ld.name != "")
            {
                this.climajson.WritePropertyName("name");
                this.climajson.WriteValue(this.ld.name);
            }
            //location is mandatory
            this.climajson.WritePropertyName("location");
            this.climajson.WriteStartObject();
            this.climajson.WritePropertyName("longitude");
            this.climajson.WriteValue(this.ld.longitude);

            this.climajson.WritePropertyName("latitude");
            this.climajson.WriteValue(this.ld.latitude);
            this.climajson.WriteEndObject();
            if (ld.altitude >= 0)
            {
                this.climajson.WritePropertyName("alt");
                this.climajson.WriteValue(this.ld.altitude);
            }
            if(ld.year>=0)
            {
                this.climajson.WritePropertyName("year");
                this.climajson.WriteValue(this.ld.year);
            }
            if(ld.monthlyDataFields.Count>0)
            {
                this.climajson.WritePropertyName("monthlyDataFields");
                this.climajson.WriteStartArray();
                 for(int i=0;i< ld.monthlyDataFields.Count;i++)
                {
                    if (ld.monthlyDataFields[i].missingData < minMonthly && !ld.monthlyDataFields[i].allZeros)
                    {
                        //write the datafield object
                        writeMonthlyDataField(i);
                    }
                }
                this.climajson.WriteEndArray();
            }
            if (ld.monthlyData.Count > 0)
            {
                this.climajson.WritePropertyName("monthlyData");
                this.climajson.WriteStartArray();
                for (int i = 0; i < ld.monthlyData.Count; i++)
                {
                    if (ld.monthlyDataFields[i].missingData < minMonthly && !ld.monthlyDataFields[i].allZeros)
                    {
                        this.climajson.WriteStartArray();
                        for (int j = 0; j < ld.monthlyData[i].Count; j++)
                        {
                            this.climajson.WriteValue(ld.monthlyData[i][j]);
                        }
                        this.climajson.WriteEndArray();
                    }
                    
                }
                this.climajson.WriteEndArray();
            }
            if (ld.hourlyDataFields.Count > 0)
            {
                this.climajson.WritePropertyName("hourlyDataFields");
                this.climajson.WriteStartArray();
                for (int i = 0; i < ld.hourlyDataFields.Count; i++)
                {
                    if (ld.hourlyDataFields[i].missingData < minHourly && !ld.hourlyDataFields[i].allZeros)
                    {
                        writeHourlyDataField(i);
                    }

                }
                this.climajson.WriteEndArray();
            }
            if (ld.hourlyData.Count > 0)
            {
                this.climajson.WritePropertyName("hourlyData");
                this.climajson.WriteStartArray();
                for (int i = 0; i < ld.hourlyData.Count; i++)
                {
                    this.climajson.WriteStartArray();
                    for (int j = 0; j < ld.hourlyData[i].Count; j++)
                    {
                        if (ld.hourlyDataFields[j].missingData < minHourly && !ld.hourlyDataFields[j].allZeros) this.climajson.WriteValue(ld.hourlyData[i][j]);
                    }
                    this.climajson.WriteEndArray();
                }
                this.climajson.WriteEndArray();
            }
            this.climajson.WriteEndObject();
        }

    }
}
