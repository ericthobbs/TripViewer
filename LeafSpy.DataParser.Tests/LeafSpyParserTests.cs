using LeafSpy.DataParser.Parsers;

namespace LeafSpy.DataParser.Tests
{
    [TestClass]
    [UsesVerify]
    public sealed partial class LeafSpyParserTests
    {
        private readonly LeafspyImportConfiguration _config = new() { DistanceUnit = DistanceUnit.FEET, CsvDelimiter = "," };

        [TestMethod]
        public Task Run() => VerifyChecks.Run();

        [DataTestMethod]
        [DataRow(@"..\\..\\..\\..\\examples\Log.csv")]
        public async Task LeafSpyParser_SingleTrip_ExampleLog_Valid(string filename)
        {
            var fullPath = System.IO.Path.GetFullPath(filename);
            var parser = new LeafSpySingleTripParser(_config);
            parser.Open(fullPath);

            var records = parser.Read();

            var verifySettings = new VerifySettings();
            verifySettings.UseTextForParameters(System.IO.Path.GetFileNameWithoutExtension(filename));

            await Verify(records, verifySettings);
        }
    }
}
