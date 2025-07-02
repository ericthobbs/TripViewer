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
using LeafSpy.DataParser.TypeConverters;
using System.ComponentModel;
using System.Diagnostics;

namespace LeafSpy.DataParser
{
    public enum TemperatureUnit
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Fahrenheit")]
        FAHRENHEIT,
        [System.ComponentModel.DataAnnotations.Display(Name = "Celsius")]
        CELSIUS,
    }

    public enum AirPressureUnit
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Bar")]
        BAR,
        [System.ComponentModel.DataAnnotations.Display(Name = "PSI")]
        PSI,
        [System.ComponentModel.DataAnnotations.Display(Name = "kPa")]
        KPA
    }

    [DebuggerDisplay("Raw={RawValue}")]
    public class BaseValue(string raw)
    {
        public string RawValue { get; private set; } = raw;

        public override string ToString()
        {
            return $"{RawValue}";
        }
    }

    [DebuggerDisplay("{ToDecimalDegrees()}")]
    public class GPSCoord : BaseValue
    {
        public GPSCoord(string raw) : base(raw) { } //DDD mmm.mmmmm Degrees Decimal Minutes (DDM)

        public double ToDecimalDegrees()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            int sep = RawValue.IndexOf(' ');
            if (sep == -1)
                return 0;

            // Parse degrees (can be negative)
            int degrees = int.Parse(RawValue[..sep].Trim());

            // Parse minutes (trim space after the separator)
            double minutes = double.Parse(RawValue[(sep + 1)..].Trim());

            // Handle sign correctly
            double decimalDegrees = Math.Abs(degrees) + (minutes / 60.0);
            return degrees < 0 ? -decimalDegrees : decimalDegrees;
        }

        public double ToRadians()
        {
            return ToDecimalDegrees() * (Math.PI / 180.0);
        }

        /// <summary>
        /// Calculates the distance in meters to another GPS coordinate using the Haversine formula.
        /// </summary>
        /// <param name="other">GPS Coord to compare to</param>
        /// <returns>distance in meters between both coordinates</returns>
        public static double DistanceInMeters(GPSCoord lat, GPSCoord @long, GPSCoord other_lat, GPSCoord other_long)
        {
            double lat1 = lat.ToRadians();
            double lon1 = @long.ToRadians();
            double lat2 = other_lat.ToRadians();
            double lon2 = other_long.ToRadians();
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            const int RadiusOfEarthInMeters = 6371000; // Radius of the Earth in meters (Mean Earth Radius)
            return RadiusOfEarthInMeters * c; // Distance in meters
        }
    }

    public class GPSCoordinates
    {
        public GPSCoord Latitude { get; private set; }
        public GPSCoord Longitude { get; private set; }

        public GPSCoordinates(GPSCoord lat, GPSCoord @long)
        {
            Latitude = lat;
            Longitude = @long;
        }

        public double DistanceInMeters(GPSCoordinates other)
        {
            return GPSCoord.DistanceInMeters(Latitude, Longitude, other.Latitude, other.Longitude);
        }

        public bool IsZero()
        {
            var isZeroLat = Latitude.ToDecimalDegrees() == 0;
            var isZeroLong = Longitude.ToDecimalDegrees() == 0;

            return isZeroLat && isZeroLong;
        }

        public override string ToString()
        {
            return $"{Latitude.ToDecimalDegrees():F6}, {Longitude.ToDecimalDegrees():F6}";
        }
    }

    [DebuggerDisplay("{ToDateTime()}")]
    public class UnixEpoch : BaseValue
    {
        public UnixEpoch(string raw) : base(raw) { }    //12345678.123 (includes MS)      

        public long ToEpochSeconds()
        {
            return (long)Math.Truncate(double.Parse(RawValue));
        }

        public DateTimeOffset ToDateTime()
        {
            return DateTimeOffset.FromUnixTimeSeconds(ToEpochSeconds());
        }
        public override string ToString()
        {
            return ToDateTime().ToString();
        }
    }

    [Flags]
    public enum GpsStatusFlags
    {
        None            = 0x0,
        HardwareAvail   = 0x1,
        HardwareEnabled = 0x2,
        LoggingEnabled  = 0x4,
        GpsOn           = 0x8,
        AccuracyValid   = 0x10,
        AltitudeValid   = 0x20,
        SpeedValid      = 0x40,
        All             = HardwareAvail | HardwareEnabled | LoggingEnabled | GpsOn | AccuracyValid | AltitudeValid | SpeedValid
    }

    public class GPSStatus : BaseValue
    {
        public Byte[] RawData { get; private set; }
        public GPSStatus(string raw) : base(raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                RawData = [0, 0];
            }
            else
            {
                RawData = HexStringToByteArray(raw);
            }
        }

        public GpsStatusFlags GetFlags()
        {
            return (GpsStatusFlags)(RawData[^1]);
        }

        public int GetAccuracy()
        {
            if (!GetFlags().HasFlag(GpsStatusFlags.AccuracyValid))
                return 0;

            if (RawData.Length >= 3)
                return RawData[1];
            if (RawData.Length == 2)
                return RawData[0];
            return 0;
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return [];

            if (hex.Length % 2 != 0)
                hex = "0" + hex; // Pad with leading zero if needed

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return bytes;
        }

        public override string ToString()
        {
            return GetFlags().ToString();
        }
    }

    [DebuggerDisplay("{Value}, {Energy} kwh Remaining")]
    public class GidUnit : BaseValue
    {
        public int InternalWhMultiplier { get; set; } = 80;

        public GidUnit(string rawValue, int? whMultiplier = null) : base(rawValue)
        {
            if (whMultiplier.HasValue)
                InternalWhMultiplier = whMultiplier.Value;
        }

        public int Value { get  { return int.Parse(RawValue); } }
        public int Energy { get { return Value * InternalWhMultiplier; } }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DebuggerDisplay("{ToPsi()} PSI")]
    public class PressureValue : BaseValue
    {
        public AirPressureUnit SourcePressureUnit;

        public PressureValue(AirPressureUnit unit, string rawValue) : base(rawValue) 
        {
            SourcePressureUnit = unit;
        }

        public float ToPsi()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourcePressureUnit == AirPressureUnit.PSI)
                return float.Parse(RawValue);

            switch(SourcePressureUnit)
            {
                case AirPressureUnit.KPA:
                    return float.Parse(RawValue) / 6.89476f;
                case AirPressureUnit.BAR:
                    return float.Parse(RawValue) * 14.5038f;
                default:
                    throw new InvalidEnumArgumentException(nameof(SourcePressureUnit));
            }
        }

        public float ToBar()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourcePressureUnit == AirPressureUnit.BAR)
                return float.Parse(RawValue);

            switch(SourcePressureUnit)
            {
                case AirPressureUnit.PSI:
                    return float.Parse(RawValue) / 14.5038f;
                case AirPressureUnit.KPA:
                    return float.Parse(RawValue) / 100;
                default:
                    throw new InvalidEnumArgumentException(nameof(SourcePressureUnit));
            }
        }

        public float ToKpa()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourcePressureUnit == AirPressureUnit.KPA)
                return float.Parse(RawValue);

            switch (SourcePressureUnit)
            {
                case AirPressureUnit.PSI:
                    return float.Parse(RawValue) * 6.89476f;
                case AirPressureUnit.BAR:
                    return float.Parse(RawValue) * 100;
                default:
                    throw new InvalidEnumArgumentException(nameof(SourcePressureUnit));
            }
        }

        public float ConvertTo(AirPressureUnit unit)
        {
            if(string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourcePressureUnit == unit)
                return float.Parse(RawValue);

            switch(unit)
            {
                case AirPressureUnit.PSI:
                    return ToPsi();
                case AirPressureUnit.KPA:
                    return ToKpa();
                case AirPressureUnit.BAR:
                    return ToBar();
                default:
                    throw new InvalidEnumArgumentException(nameof(unit));
            }
        }

        public override string ToString()
        {
            return $"{float.Parse(RawValue):N2} {SourcePressureUnit}";
        }
    }

    public class SpeedValue : BaseValue
    {
        public DistanceUnit SourceSpeedUnit { get; private set; }
        public const float MphToKmh = 1.609344f;
        public SpeedValue(DistanceUnit unit, string rawValue) : base(rawValue)
        {
            SourceSpeedUnit = unit;
        }

        public float ToMilesPerHour()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceSpeedUnit == DistanceUnit.FEET)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.FEET);
        }

        public float ToKilometersPerHour()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceSpeedUnit == DistanceUnit.METER)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.METER);
        }

        public float ConvertTo(DistanceUnit unit)
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceSpeedUnit == unit)
                return float.Parse(RawValue);

            switch (unit)
            {
                case DistanceUnit.FEET:
                    return float.Parse(RawValue) / MphToKmh;
                case DistanceUnit.METER:
                    return float.Parse(RawValue) * MphToKmh;
                default:
                    throw new InvalidEnumArgumentException(nameof(unit));
            }
        }

        public override string ToString()
        {
            return $"{float.Parse(RawValue):N2} {SourceSpeedUnit}";
        }
    }

    public class AltitudeValue : BaseValue
    {
        public DistanceUnit SourceDistanceUnit { get; private set; }
        public const float FeetToMeters = 0.3048f;

        public AltitudeValue(DistanceUnit unit, string rawValue) : base(rawValue)
        {
            SourceDistanceUnit = unit;
        }

        public float ToMeters()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == DistanceUnit.METER)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.METER);
        }

        public float ToFeet()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == DistanceUnit.FEET)
                return float.Parse(RawValue);
            return ConvertTo(DistanceUnit.FEET);
        }

        public float ConvertTo(DistanceUnit unit)
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceDistanceUnit == unit)
                return float.Parse(RawValue);
            
            switch(unit)
            {
                case DistanceUnit.FEET:
                    return float.Parse(RawValue) / FeetToMeters;
                case DistanceUnit.METER:
                    return float.Parse(RawValue) * FeetToMeters;
                default:
                    throw new InvalidEnumArgumentException(nameof(unit));
            }
        }

        public override string ToString()
        {
            return $"{float.Parse(RawValue):N2} {SourceDistanceUnit}";
        }
    }

    public class TemperatureValue : BaseValue
    {
        public TemperatureUnit SourceTemperatureUnit { get; private set; }

        public TemperatureValue(TemperatureUnit unit, string rawValue) : base(rawValue)
        {
            SourceTemperatureUnit = unit;
        }

        public float ToCelsius()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceTemperatureUnit == TemperatureUnit.CELSIUS)
                return float.Parse(RawValue);
            return ConvertTo(TemperatureUnit.CELSIUS);
        }

        public float ToFahrenheit()
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceTemperatureUnit == TemperatureUnit.FAHRENHEIT)
                return float.Parse(RawValue);
            return ConvertTo(TemperatureUnit.FAHRENHEIT);
        }

        public float ConvertTo(TemperatureUnit unit)
        {
            if (string.IsNullOrWhiteSpace(RawValue))
                return 0;

            if (SourceTemperatureUnit == unit)
                return float.Parse(RawValue);

            switch(unit)
            {
                case TemperatureUnit.CELSIUS:
                    return (float.Parse(RawValue) - 32) / 1.8f;
                case TemperatureUnit.FAHRENHEIT:
                    return float.Parse(RawValue) * 1.8f + 32;
                default:
                    throw new InvalidEnumArgumentException(nameof(SourceTemperatureUnit));
            }
        }

        public override string ToString()
        {
            if (RawValue == "none")
                return base.ToString();
            return $"{float.Parse(RawValue):N2} {SourceTemperatureUnit}";
        }
    }

    public enum GearPosition
    {
        NotRead = 0,
        Park = 1,
        Reverse = 2,
        Neutral = 3,
        Drive = 4,
        BMode = 7,
    }

    public enum PlugState
    {
        NotPluggedIn = 0,
        PartiallyPluggedIn = 1,
        PluggedIn = 2,
    }

    public enum ChargeMode
    {
        NotCharging = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
    }

    public enum ReadState
    {
       NotRead = 0,
       Read = 1,
    }

    public enum FrontWiperStatus
    {
        None = 0,
        Stopped = 0x08,
        Intermittent = 0x10,
        Switch = 0x20,
        Low = 0x40,
        High = 0x80,
    }

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
        public string TorqueNm { get; set; } = string.Empty;

        [Category("Vehicle")]
        [ReadOnly(true)]
        public string RPM { get; set; } = string.Empty;
    }
}
