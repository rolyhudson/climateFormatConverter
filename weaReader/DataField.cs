using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weaReader
{
    class DataField
    {
        public string name;
        public string units;
        public double min;
        public double max;
        public double missingValue;
        public bool allZeros;
        public int missingData;
        public DataField(string n,string u,double lower,double upper,double missVal)
        {
            this.name = n;
            this.units = u;
            this.min = lower;
            this.max = upper;
            this.missingValue = missVal;
            this.allZeros = false;
            this.missingData = 0;
        }

    }
    
}
