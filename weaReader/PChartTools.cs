using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weaReader
{
    class PChartTools
    {
        static public double getStandardPressure(double altitude/* meters */)   // Returns result in Pascals
        {
            // Below 51 km: Practical Meteorology by Roland Stull, pg 12
            // Above 51 km: http://www.braeunig.us/space/atmmodel.htm
            // Validation data: https://www.avs.org/AVS/files/c7/c7edaedb-95b2-438f-adfb-36de54f87b9e.pdf

            altitude = altitude / 1000.0;  // Convert m to km
            double geopot_height = getGeopotential(altitude);

            double t = getStandardTemperature(geopot_height);

            if (geopot_height <= 11)
                return 101325 * Math.Pow(288.15 / t, -5.255877);
            else if (geopot_height <= 20)
                return 22632.06 * Math.Exp(-0.1577 * (geopot_height - 11));
            else if (geopot_height <= 32)
                return 5474.889 * Math.Pow(216.65 / t, 34.16319);
            else if (geopot_height <= 47)
                return 868.0187 * Math.Pow(228.65 / t, 12.2011);
            else if (geopot_height <= 51)
                return 110.9063 * Math.Exp(-0.1262 * (geopot_height - 47));
            else if (geopot_height <= 71)
                return 66.93887 * Math.Pow(270.65 / t, -12.2011);
            else if (geopot_height <= 84.85)
                return 3.956420 * Math.Pow(214.65 / t, -17.0816);

            //throw std::out_of_range("altitude must be less than 86 km.");
            return -1;
        }

        // geopot_height = earth_radius * altitude / (earth_radius + altitude) /// All in km
        // Temperature is in Kelvin = 273.15 + Celsius
        static double getStandardTemperature(double geopot_height)
        {
            // Standard atmospheric pressure
            // Below 51 km: Practical Meteorology by Roland Stull, pg 12
            // Above 51 km: http://www.braeunig.us/space/atmmodel.htm

            if (geopot_height <= 11)          // Troposphere
                return 288.15 - (6.5 * geopot_height);
            else if (geopot_height <= 20)     // Stratosphere starts
                return 216.65;
            else if (geopot_height <= 32)
                return 196.65 + geopot_height;
            else if (geopot_height <= 47)
                return 228.65 + 2.8 * (geopot_height - 32);
            else if (geopot_height <= 51)     // Mesosphere starts
                return 270.65;
            else if (geopot_height <= 71)
                return 270.65 - 2.8 * (geopot_height - 51);
            else if (geopot_height <= 84.85)
                return 214.65 - 2 * (geopot_height - 71);
            // Thermosphere has high kinetic temperature (500 C to 2000 C) but temperature
            // as measured by a thermometer would be very low because of almost vacuum.

            //throw std::out_of_range("geopot_height must be less than 84.85 km.")
            return -1;
        }

        static double getGeopotential(double altitude_km)
        {
            double EARTH_RADIUS = 6356.766; // km

            return EARTH_RADIUS * altitude_km / (EARTH_RADIUS + altitude_km);
        }
    }
}
