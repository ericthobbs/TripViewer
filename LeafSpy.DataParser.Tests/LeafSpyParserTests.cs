using LeafSpy.DataParser.Parsers;

namespace LeafSpy.DataParser.Tests
{
    [TestClass]
    public sealed class LeafSpyParserTests
    {
        private readonly LeafspyImportConfiguration _config = new() { DistanceUnit = DistanceUnit.FEET, CsvDelimiter = "," };

        [DataTestMethod]
        [DataRow(@"..\\..\\..\\..\\examples\Log.csv", 964, 1, "1932", "68", 7612)]
        public void LeafSpyParser_SingleTrip_ExampleLog_Valid(string filename, int expectedRecordCount, int recordNumber, string expectedRawElevation, string expectedRawSpeed, int expectedRpm)
        {
            var fullPath = System.IO.Path.GetFullPath(filename);
            var parser = new LeafSpySingleTripParser(_config);
            parser.Open(fullPath);

            var records = parser.Read().ToList();
            var theRecord = records.Skip(recordNumber - 1).First();

            Assert.AreEqual(expectedRecordCount, records.Count);
            Assert.IsNotNull(theRecord);

            Assert.AreEqual(expectedRawElevation, theRecord.GpsPhoneElevation.RawValue);
            Assert.AreEqual(expectedRawSpeed, theRecord.GpsPhoneSpeed.RawValue);
            Assert.AreEqual(expectedRpm, theRecord.RPM);
        }
    }
}
