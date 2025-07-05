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
using LeafSpy.DataParser.ValueTypes;
using System.ComponentModel;
using System.Reflection;

namespace LeafSpy.DataParser
{
    public class TripLog
    {
        [Category("General")]
        [ReadOnly(true)]
        public required DateTime DateTime { get; set; }

        [Category("Position")]
        [ReadOnly(true)]
        [DisplayName("GPS Position")]
        public required GPSCoordinates GpsPhoneCoordinates { get; set; }

        [Category("Position")]
        [ReadOnly(true)]
        [DisplayName("GPS Elevation")]
        public required AltitudeValue GpsPhoneElevation { get; set; }

        [Category("Position")]
        [ReadOnly(true)]
        [DisplayName("GPS Speed")]
        public required SpeedValue GpsPhoneSpeed { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public required GidUnit Gids { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        [DisplayName("State of Charge (%)")]
        public float StateOfChargePercent { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float AHr { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float PackVolts { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float PackAmps { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float MaxCPmV { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float MinCPmV { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float AvgCPmV { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float CPmVDiff { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public int JudgmentValue { get; set; }


        [Category("Battery Temp")]
        [DisplayName("Pack Temp 1 (F)"), Description("Center Rear of HV Pack")]
        [ReadOnly(true)]
        public required TemperatureValue PackT1F { get; set; }

        [Category("Battery Temp")]
        [DisplayName("Pack Temp 1 (C)"), Description("Center Rear of HV Pack")]
        [ReadOnly(true)]
        public required TemperatureValue PackT1C { get; set; }

        [Category("Battery Temp")]
        [DisplayName("Pack Temp 2 (F)"), Description("Front Right of HV Pack")]
        [ReadOnly(true)]
        public required TemperatureValue PackT2F { get; set; }

        [Category("Battery Temp")]
        [DisplayName("Pack Temp 2 (C)"), Description("Front Right of HV Pack")]
        [ReadOnly(true)]
        public required TemperatureValue PackT2C { get; set; }

        [Category("Battery Temp")]
        [DisplayName("Pack Temp 3 (F)"), Description("Left Center of HV Pack")]
        [ReadOnly(true)]
        public TemperatureValue? PackT3F { get; set; }

        [Category("Battery Temp")]
        [DisplayName("Pack Temp 3 (C)"), Description("Left Center of HV Pack")]
        [ReadOnly(true)]
        public TemperatureValue? PackT3C { get; set; }

        [Category("Battery Temp")]
        [DisplayName("Pack Temp 4 (F)"), Description("Right Center of HV Pack")]
        [ReadOnly(true)]
        public required TemperatureValue PackT4F { get; set; }

        [Category("Battery Temp")]
        [DisplayName("Pack Temp 4 (C)"), Description("Right Center of HV Pack")]
        [ReadOnly(true)]
        public required TemperatureValue PackT4C { get; set; }

        #region CellPairs 1-96

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 01")]
        [ReadOnly(true)]
        public float CP1 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 02")]
        [ReadOnly(true)]
        public float CP2 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 03")]
        [ReadOnly(true)]
        public float CP3 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 04")]
        [ReadOnly(true)]
        public float CP4 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 05")]
        [ReadOnly(true)]
        public float CP5 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 06")]
        [ReadOnly(true)]
        public float CP6 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 07")]
        [ReadOnly(true)]
        public float CP7 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 08")]
        [ReadOnly(true)]
        public float CP8 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 09")]
        [ReadOnly(true)]
        public float CP9 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 10")]
        [ReadOnly(true)]
        public float CP10 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 11")]
        [ReadOnly(true)]
        public float CP11 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 12")]
        [ReadOnly(true)]
        public float CP12 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 13")]
        [ReadOnly(true)]
        public float CP13 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 14")]
        [ReadOnly(true)]
        public float CP14 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 15")]
        [ReadOnly(true)]
        public float CP15 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 16")]
        [ReadOnly(true)]
        public float CP16 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 17")]
        [ReadOnly(true)]
        public float CP17 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 18")]
        [ReadOnly(true)]
        public float CP18 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 19")]
        [ReadOnly(true)]
        public float CP19 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 20")]
        [ReadOnly(true)]
        public float CP20 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 21")]
        [ReadOnly(true)]
        public float CP21 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 22")]
        [ReadOnly(true)]
        public float CP22 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 23")]
        [ReadOnly(true)]
        public float CP23 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 24")]
        [ReadOnly(true)]
        public float CP24 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 25")]
        [ReadOnly(true)]
        public float CP25 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 26")]
        [ReadOnly(true)]
        public float CP26 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 27")]
        [ReadOnly(true)]
        public float CP27 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 28")]
        [ReadOnly(true)]
        public float CP28 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 29")]
        [ReadOnly(true)]
        public float CP29 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 30")]
        [ReadOnly(true)]
        public float CP30 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 31")]
        [ReadOnly(true)]
        public float CP31 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 32")]
        [ReadOnly(true)]
        public float CP32 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 33")]
        [ReadOnly(true)]
        public float CP33 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 34")]
        [ReadOnly(true)]
        public float CP34 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 35")]
        [ReadOnly(true)]
        public float CP35 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 36")]
        [ReadOnly(true)]
        public float CP36 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 37")]
        [ReadOnly(true)]
        public float CP37 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 38")]
        [ReadOnly(true)]
        public float CP38 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 39")]
        [ReadOnly(true)]
        public float CP39 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 40")]
        [ReadOnly(true)]
        public float CP40 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 41")]
        [ReadOnly(true)]
        public float CP41 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 42")]
        [ReadOnly(true)]
        public float CP42 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 43")]
        [ReadOnly(true)]
        public float CP43 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 44")]
        [ReadOnly(true)]
        public float CP44 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 45")]
        [ReadOnly(true)]
        public float CP45 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 46")]
        [ReadOnly(true)]
        public float CP46 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 47")]
        [ReadOnly(true)]
        public float CP47 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 48")]
        [ReadOnly(true)]
        public float CP48 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 49")]
        [ReadOnly(true)]
        public float CP49 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 50")]
        [ReadOnly(true)]
        public float CP50 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 51")]
        [ReadOnly(true)]
        public float CP51 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 52")]
        [ReadOnly(true)]
        public float CP52 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 53")]
        [ReadOnly(true)]
        public float CP53 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 54")]
        [ReadOnly(true)]
        public float CP54 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 55")]
        [ReadOnly(true)]
        public float CP55 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 56")]
        [ReadOnly(true)]
        public float CP56 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 57")]
        [ReadOnly(true)]
        public float CP57 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 58")]
        [ReadOnly(true)]
        public float CP58 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 59")]
        [ReadOnly(true)]
        public float CP59 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 60")]
        [ReadOnly(true)]
        public float CP60 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 61")]
        [ReadOnly(true)]
        public float CP61 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 62")]
        [ReadOnly(true)]
        public float CP62 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 63")]
        [ReadOnly(true)]
        public float CP63 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 64")]
        [ReadOnly(true)]
        public float CP64 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 65")]
        [ReadOnly(true)]
        public float CP65 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 66")]
        [ReadOnly(true)]
        public float CP66 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 67")]
        [ReadOnly(true)]
        public float CP67 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 68")]
        [ReadOnly(true)]
        public float CP68 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 69")]
        [ReadOnly(true)]
        public float CP69 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 70")]
        [ReadOnly(true)]
        public float CP70 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 71")]
        [ReadOnly(true)]
        public float CP71 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 72")]
        [ReadOnly(true)]
        public float CP72 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 73")]
        [ReadOnly(true)]
        public float CP73 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 74")]
        [ReadOnly(true)]
        public float CP74 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 75")]
        [ReadOnly(true)]
        public float CP75 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 76")]
        [ReadOnly(true)]
        public float CP76 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 77")]
        [ReadOnly(true)]
        public float CP77 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 78")]
        [ReadOnly(true)]
        public float CP78 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 79")]
        [ReadOnly(true)]
        public float CP79 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 80")]
        [ReadOnly(true)]
        public float CP80 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 81")]
        [ReadOnly(true)]
        public float CP81 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 82")]
        [ReadOnly(true)]
        public float CP82 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 83")]
        [ReadOnly(true)]
        public float CP83 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 84")]
        [ReadOnly(true)]
        public float CP84 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 85")]
        [ReadOnly(true)]
        public float CP85 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 86")]
        [ReadOnly(true)]
        public float CP86 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 87")]
        [ReadOnly(true)]
        public float CP87 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 88")]
        [ReadOnly(true)]
        public float CP88 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 89")]
        [ReadOnly(true)]
        public float CP89 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 90")]
        [ReadOnly(true)]
        public float CP90 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 91")]
        [ReadOnly(true)]
        public float CP91 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 92")]
        [ReadOnly(true)]
        public float CP92 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 93")]
        [ReadOnly(true)]
        public float CP93 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 94")]
        [ReadOnly(true)]
        public float CP94 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 95")]
        [ReadOnly(true)]
        public float CP95 { get; set; }

        [Category("HV Battery Cell Pairs")]
        [DisplayName("Cell Pair 96")]
        [ReadOnly(true)]
        public float CP96 { get; set; }

        #endregion 1-96

        [Category("12V Battery")]
        [ReadOnly(true)]
        public float Bat12vAmps { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public required string VIN { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float Hx { get; set; }

        [Category("12V Battery")]
        [ReadOnly(true)]
        public float Bat12vVolts { get; set; }

        [Category("General")]
        [DisplayName("Odometer (km)")]
        [ReadOnly(true)]
        public float Odokm { get; set; }

        [Category("Charging")]
        [DisplayName("DCFC Counts")]
        [ReadOnly(true)]
        public int QCCount { get; set; }

        [Category("Charging")]
        [DisplayName("J1172 Counts")]
        [ReadOnly(true)]
        public int L1L2Count { get; set; }

        [Category("Tire Pressure")]
        [DisplayName("Front Left")]
        [ReadOnly(true)]
        public required PressureValue TPFL { get; set; }

        [Category("Tire Pressure")]
        [DisplayName("Front Right")]
        [ReadOnly(true)]
        public required PressureValue TPFR { get; set; }

        [Category("Tire Pressure")]
        [DisplayName("Rear Right")]
        [ReadOnly(true)]
        public required PressureValue TPRR { get; set; }

        [Category("Tire Pressure")]
        [DisplayName("Rear Left")]
        [ReadOnly(true)]
        public required PressureValue TPRL { get; set; }

        [Category("General")]
        [ReadOnly(true)]
        public required TemperatureValue Ambient { get; set; }

        [Category("Battery")]
        [DisplayName("State of Health (%)")]
        [ReadOnly(true)]
        public float SOH { get; set; }

        [Category("General")]
        [ReadOnly(true)]
        public int RegenWh { get; set; }

        [Category("General")]
        [DisplayName("Phone Battery (%)")]
        [ReadOnly(true)]
        public int PhoneBatteryLevelPercent { get; set; }

        [Category("Debug")]
        [ReadOnly(true)]
        public required UnixEpoch EpochTime { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public int MotorPwrw { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public int AuxPwr100w { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public int ACPwr250w { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public string? ACComp01MPa { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public int EstPwrAC50w { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public int EstPwrHtr250w { get; set; }

        [Category("Charging")]
        [ReadOnly(true)]
        public PlugState PlugState { get; set; }

        [Category("Charging")]
        [ReadOnly(true)]
        public ChargeMode ChargeMode { get; set; }

        [Category("Charging")]
        [ReadOnly(true)]
        public string? OBCOutPwr { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public GearPosition Gear { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float HVolt1 { get; set; }

        [Category("Battery")]
        [ReadOnly(true)]
        public float HVolt2 { get; set; }

        [Category("Position")]
        [ReadOnly(true)]
        public required GPSStatus GPSStatus { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public ReadState PowerSW { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public ReadState BMS { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public ReadState OBC { get; set; }

        [Category("Debug")]
        [ReadOnly(true)]
        public string? Debug { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public float MotorTemp { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public float Inverter2Temp { get; set; }

        [Category("Power")]
        [ReadOnly(true)]
        public float Inverter4Temp { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public string? Speed1 { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public string? Speed2 { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public FrontWiperStatus WiperStatus { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public float TorqueNm { get; set; }

        [Category("Vehicle")]
        [ReadOnly(true)]
        public int RPM { get; set; }


        public IEnumerable<Tuple<int, float>> GetAllCellPairs()
        {
            var properties =  GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                {
                    Property = p,
                    HasIndex = int.TryParse(p.Name.AsSpan(2), out int index) && p.Name.StartsWith("CP"),
                    Index = int.TryParse(p.Name.AsSpan(2), out int idx) ? idx : -1
                })
                .Where(x => x.HasIndex && x.Index >= 1 && x.Index <= 96 && x.Property.PropertyType == typeof(float))
                .OrderBy(x => x.Index)
                .Select(x => (x.Index, x.Property));

            foreach (var (index, prop) in properties)
            {
                yield return new Tuple<int, float>(index, (float)(prop.GetValue(this) ?? 0));
            }
        }
    }
}
