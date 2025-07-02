

using System.ComponentModel;

/**
 * MIT License
 * 
 * Copyright (c) 2025 Eric Hobbs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
namespace LeafSpy.DataParser.Tests
{
    [TestClass]
    public sealed class LeafSpyParserValueTests
    {
        [DataTestMethod]
        [DataRow("34 29.07306", "-117 23.9699", 34.484551, -117.39949833333333)]
        [DataRow(          "0",            "0",         0,                   0)]
        [DataRow(         null,             "",         0,                   0)]
        [DataRow("34 29.07306",            " ", 34.484551,                   0)]
        [DataRow( "-33 51.684",   "151 12.123",  -33.8614,           151.20205)]
        public void DecodeLatLong_FromDDM_ToDD(string lsLat, string lsLong, double expectedLat, double expectedLong)
        {
            var lat = new GPSCoord(lsLat);
            var @long = new GPSCoord(lsLong);

            Assert.AreEqual(expectedLat, lat.ToDecimalDegrees(), double.Epsilon);
            Assert.AreEqual(expectedLong, @long.ToDecimalDegrees(), double.Epsilon);
        }

        [DataTestMethod] //61037F, 610377, 37027F, 00067F
        [DataRow(      "", 0, GpsStatusFlags.None)]                        //Invalid data (CSV Error)
        [DataRow(     "0", 0, GpsStatusFlags.None)]                        //Invalid data (CSV Error)
        [DataRow(   "37F", 3, GpsStatusFlags.All)]                         //Example data from LeafSpy Help
        [DataRow("0C067F", 6, GpsStatusFlags.All)]                         //Seen in real data
        [DataRow("21047F", 4, GpsStatusFlags.All)]                         //Seen in real data
        [DataRow("270477", 4, GpsStatusFlags.All & ~GpsStatusFlags.GpsOn)] //Seen in real data
        public void DecodeGpsStatus_FromHexString(string gpsStatus, int expectedAccuracy, GpsStatusFlags expectedFlags)
        {
            var status = new GPSStatus(gpsStatus);
            Assert.AreEqual(expectedAccuracy, status.GetAccuracy());
            Assert.AreEqual(expectedFlags, status.GetFlags());
        }

        [DataTestMethod]
        [DataRow(TemperatureUnit.FAHRENHEIT, "32", TemperatureUnit.CELSIUS,     0)] // 32 F to 0 C
        [DataRow(TemperatureUnit.CELSIUS,     "0", TemperatureUnit.FAHRENHEIT, 32)] // 0 C to 32 F
        [DataRow(TemperatureUnit.CELSIUS,     "0", TemperatureUnit.CELSIUS,     0)] // 0 C to 0 C
        [DataRow(TemperatureUnit.FAHRENHEIT, null, TemperatureUnit.CELSIUS,     0)] // null/invalid F to C
        [DataRow(TemperatureUnit.FAHRENHEIT,   "", TemperatureUnit.CELSIUS,     0)] // invalid F to C
        public void TemperatureValue_ConvertToTests(TemperatureUnit sourceUnit, string rawValue, TemperatureUnit outputUnit, float expectedTemp)
        {
            var temp = new TemperatureValue(sourceUnit, rawValue);

            Assert.AreEqual(expectedTemp,temp.ConvertTo(outputUnit), 0.1f);
        }

        [DataTestMethod]
        [DataRow(AirPressureUnit.BAR, "2.0", AirPressureUnit.PSI, 29.00f )] // 2 bar to PSI
        [DataRow(AirPressureUnit.BAR, "1.5", AirPressureUnit.KPA, 150.0f )] // 1.5 bar to kPa
        [DataRow(AirPressureUnit.PSI,  "32", AirPressureUnit.BAR, 2.20f  )] // 32 PSI to bar
        [DataRow(AirPressureUnit.PSI,  "35", AirPressureUnit.KPA, 241.31f)] // 35 PSI to kPa
        [DataRow(AirPressureUnit.KPA, "220", AirPressureUnit.PSI, 31.90f )] // 220 kPa to PSI
        [DataRow(AirPressureUnit.KPA, "300", AirPressureUnit.BAR, 3.0f   )] // 300 kPa to bar
        [DataRow(AirPressureUnit.PSI,  "36", AirPressureUnit.PSI, 36.0f  )] // Same unit
        [DataRow(AirPressureUnit.PSI,   "0", AirPressureUnit.BAR, 0.0f   )] // zero value
        [DataRow(AirPressureUnit.PSI,    "", AirPressureUnit.BAR, 0.0f   )] // blank value
        [DataRow(AirPressureUnit.PSI,  null, AirPressureUnit.BAR, 0.0f   )] // null value
        public void PressureValue_ConvertToTests(AirPressureUnit sourceUnit, string rawValue, AirPressureUnit outputUnit, float expectedValue)
        {
            var pressure = new PressureValue(sourceUnit, rawValue);
            Assert.AreEqual(expectedValue, pressure.ConvertTo(outputUnit), 0.1f);
        }

        [DataTestMethod]
        [DataRow(DistanceUnit.FEET, "1000", DistanceUnit.FEET,  1000f   )]
        [DataRow(DistanceUnit.FEET, "1000", DistanceUnit.METER, 304.8f  )]
        [DataRow(DistanceUnit.METER, "500", DistanceUnit.METER, 500f    )]
        [DataRow(DistanceUnit.METER, "500", DistanceUnit.FEET,  1640.42f)]
        [DataRow(DistanceUnit.METER,    "", DistanceUnit.FEET,  0f      )]
        [DataRow(DistanceUnit.FEET,   null, DistanceUnit.METER, 0f      )]
        public void AltitudeValue_ConvertToTests(DistanceUnit sourceUnit, string rawValue, DistanceUnit outputUnit, float expectedValue)
        {
            var alt = new AltitudeValue(sourceUnit, rawValue);
            Assert.AreEqual(expectedValue, alt.ConvertTo(outputUnit), 0.1f);
        }

        [DataTestMethod]
        [DataRow(DistanceUnit.FEET,  "25", DistanceUnit.FEET,  25.0f )]
        [DataRow(DistanceUnit.FEET,  "25", DistanceUnit.METER, 40.23f)]
        [DataRow(DistanceUnit.METER,  "9", DistanceUnit.METER, 9f    )]
        [DataRow(DistanceUnit.METER,   "", DistanceUnit.METER, 0f    )]
        [DataRow(DistanceUnit.METER, null, DistanceUnit.METER, 0f    )]
        public void SpeedValue_ConvertToTests(DistanceUnit sourceUnit, string rawValue, DistanceUnit outputUnit, float expectedValue)
        {
            var alt = new SpeedValue(sourceUnit, rawValue);
            Assert.AreEqual(expectedValue, alt.ConvertTo(outputUnit), 0.1f);
        }

        [DataTestMethod]
        [DataRow(DistanceUnit.FEET, "25", (DistanceUnit)4, 25.0f)]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void SpeedValue_ConverTo_InvalidValue_Tests(DistanceUnit sourceUnit, string rawValue, DistanceUnit outputUnit, float expectedValue)
        {
            var alt = new SpeedValue(sourceUnit, rawValue);
            Assert.AreEqual(expectedValue, alt.ConvertTo(outputUnit), 0.1f);
        }
    }
}
