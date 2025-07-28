using LeafSpy.DataParser.Parsers;

namespace LeafSpy.DataParser.Tests
{
    [TestClass]
    [UsesVerify]
    public sealed partial class LeafSpyParserTests
    {
        [TestMethod]
        public Task Run() => VerifyChecks.Run();

        [DataTestMethod]
        [DataRow(@"..\\..\\..\\..\\examples\Log.csv", DistanceUnit.MILES, DistanceUnit.FEET, ",")]
        public async Task LeafSpyParser_SingleTrip_ExampleLog_Valid(string filename, DistanceUnit gpsSpeedUnit, DistanceUnit gpsElevUnit, string csvDelim)
        {
            var fullPath = System.IO.Path.GetFullPath(filename);
            var parser = new LeafSpySingleTripParser(new LeafspyImportConfiguration() { GpsSpeedUnit = gpsSpeedUnit, GpsElevUnit = gpsElevUnit, CsvDelimiter = csvDelim });
            parser.Open(fullPath);

            var records = parser.Read();

            var verifySettings = new VerifySettings();
            verifySettings.UseTextForParameters(System.IO.Path.GetFileNameWithoutExtension(filename));

            await Verify(records, verifySettings);
        }
    }
}
