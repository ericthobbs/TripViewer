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
using System.Globalization;
using LeafSpy.DataParser;
using LeafSpy.DataParser.Parsers;
using LeafSpy.DataParser.ValueTypes;


namespace LeafSpyKMLExporter
{
    public class LeafSpyKmlExporter
    {
        private LeafSpyKmlExporter() { 
        }

        /// <summary>
        /// Exports a LeafSpy trip to KML format with a tour of the trip for demonstration purposes.
        /// This is a work in progress.
        /// </summary>
        /// <param name="sourceFilename">source csv log file</param>
        /// <param name="outputFilename">output kml file name</param>
        /// <param name="placename">Trip name</param>
        public static void ExportToKml(LeafspyImportConfiguration cfg, string sourceFilename, string outputFilename, string placename)
        {
            using var tripParser = new LeafSpySingleTripParser(cfg);
            tripParser.Open(sourceFilename);

            var track = new SharpKml.Dom.GX.Track();

            var playlist = new SharpKml.Dom.GX.Playlist();

            // Add extended data for the track
            var extendedData = new SharpKml.Dom.ExtendedData();
            var speedArray = new SharpKml.Dom.GX.SimpleArrayData { Name = "speed" };

            GPSCoordinates? previousCoord = null;

            foreach (var record in tripParser.Read())
            {
                var lat = record.GpsPhoneCoordinates.Latitude.ToDecimalDegrees();
                var lon = record.GpsPhoneCoordinates.Longitude.ToDecimalDegrees();

                if (lat == 0 && lon == 0)
                    continue;

                track.AddWhen(record.EpochTime.ToDateTime().UtcDateTime);
                track.AddCoordinate(record.GpsPhoneCoordinates.ToKmlVector());
                speedArray.AddValue(record.GpsPhoneSpeed.RawValue);

                double heading = 0;
                if (previousCoord != null)
                {
                    var prevLat = previousCoord.Latitude.ToDecimalDegrees();
                    var prevLon = previousCoord.Longitude.ToDecimalDegrees();
                    heading = CalculateHeading(prevLat, prevLon, lat, lon);
                }
                previousCoord = record.GpsPhoneCoordinates;

                var lookAt = new SharpKml.Dom.LookAt
                {
                    Latitude = lat,
                    Longitude = lon,
                    Altitude = 0,
                    Heading = heading,
                    Tilt = 60,
                    Range = 300,
                    AltitudeMode = SharpKml.Dom.AltitudeMode.RelativeToGround
                };

                var flyTo = new SharpKml.Dom.GX.FlyTo
                {
                    Duration = 1,
                    View = lookAt
                };

                playlist.AddTourPrimitive(flyTo);
            }

            var tour = new SharpKml.Dom.GX.Tour
            {
                Name = "Trip Tour",
                Playlist = playlist
            };

            var document = new SharpKml.Dom.Document()
            {
                Name = $"Trip {placename}",
            };
            document.AddFeature(tour);

            var lineStyle = new SharpKml.Dom.LineStyle()
            {
                Color = SharpKml.Base.Color32.Parse("FFFF0000"), // Blue in ARGB format
                Width = 4
            };

            var style = new SharpKml.Dom.Style()
            {
                Id = "myLineStyle",
                Line = lineStyle
            };

            document.AddStyle(style);

            DateTime parsedDate = DateTime.ParseExact(placename, "yyMMdd", null);

            var schema = new SharpKml.Dom.Schema()
            {
                Id = "trackSchema",
                Name = "trackSchema",
            };
            schema.AddField(new SharpKml.Dom.SimpleField
            {
                Name = "speed",
                FieldType = "float",
                DisplayName = "Speed (mph)"
            });
            document.AddSchema(schema);

            var schemaData = new SharpKml.Dom.SchemaData();
            schemaData.AddArray(speedArray);
            extendedData.AddSchemaData(schemaData);

            track.ExtendedData = extendedData;

            var placemark = new SharpKml.Dom.Placemark()
            {
                Name = parsedDate.ToString("MMM-dd-yyyy").ToUpper(),
                Geometry = track,
                StyleUrl = new Uri("#myLineStyle", UriKind.Relative)
            };

            document.AddFeature(placemark);

            var kml = new SharpKml.Dom.Kml
            {
                Feature = document
            };
            kml.AddNamespacePrefix("gx", "http://www.google.com/kml/ext/2.2");

            var file = SharpKml.Engine.KmlFile.Create(kml, false);

            using var stream = File.Create(outputFilename);
            file.Save(stream);
        }

        private static double CalculateHeading(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = Extensions.ToRadians(lon2 - lon1);
            lat1 = Extensions.ToRadians(lat1);
            lat2 = Extensions.ToRadians(lat2);

            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) -
                    Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            var heading = Math.Atan2(y, x);
            return (Extensions.ToDegrees(heading) + 360) % 360;
        }
    }
}
