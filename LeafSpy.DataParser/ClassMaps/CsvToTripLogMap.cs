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
using CsvHelper.Configuration;
using LeafSpy.DataParser.TypeConverters;
using LeafSpy.DataParser.ValueTypes;

namespace LeafSpy.DataParser.ClassMaps
{
    internal class CsvToTripLogMap : ClassMap<TripLog>
    {
        public CsvToTripLogMap(LeafspyImportConfiguration config)
        {
            Map(m => m.DateTime).Name("Date/Time");                 //Date/Time

            Map(m => m.GpsPhoneCoordinates)
                .Convert(args =>
                {
                    return new GPSCoordinates(
                        new GPSCoord(args.Row.GetField("Lat") ?? ""),
                        new GPSCoord(args.Row.GetField("Long") ?? "")
                        );
                }).Name("Lat, Long");                               //Lat, Long

            Map(m => m.GpsPhoneElevation).Name("Elv")
                .TypeConverter(new AltitudeValueConverter(config.DistanceUnit));              //Elv
            Map(m => m.GpsPhoneSpeed).Name("Speed")
                .TypeConverter(new SpeedValueConverter(config.DistanceUnit));                //Speed
            Map(m => m.Gids).Name("Gids")
                .TypeConverter<GidConverter>();                     //Gids
            Map(m => m.StateOfChargePercent).Name("SOC")
                .TypeConverter<FloatConverter10K>();                //SOC
            Map(m => m.AHr).Name("AHr")
                .TypeConverter<FloatConverter10K>();                //AHr
            Map(m => m.PackVolts).Name("Pack Volts");               //Pack Volts
            Map(m => m.PackAmps).Name("Pack Amps");                 //Pack Amps
            Map(m => m.MaxCPmV).Name("Max CP mV")
                .TypeConverter<FloatConverter1K>();                 //Max CP mV
            Map(m => m.MinCPmV).Name("Min CP mV")
                .TypeConverter<FloatConverter1K>();                 //Min CP mV
            Map(m => m.AvgCPmV).Name("Avg CP mV")
                .TypeConverter<FloatConverter1K>();                 //Avg CP mV
            Map(m => m.CPmVDiff).Name("CP mV Diff");                //CP mV Diff
            Map(m => m.JudgmentValue).Name("Judgment Value");       //Judgment Value
            Map(m => m.PackT1F).Name("Pack T1 F")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.FAHRENHEIT));             //Pack T1 F
            Map(m => m.PackT1C).Name("Pack T1 C")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.CELSIUS));             //Pack T1 C
            Map(m => m.PackT2F).Name("Pack T2 F")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.FAHRENHEIT));             //Pack T2 F
            Map(m => m.PackT2C).Name("Pack T2 C")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.CELSIUS));             //Pack T2 C
            Map(m => m.PackT3F).Name("Pack T3 F")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.FAHRENHEIT));             //Pack T3 F
            Map(m => m.PackT3C).Name("Pack T3 C")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.CELSIUS));             //Pack T3 C
            Map(m => m.PackT4F).Name("Pack T4 F")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.FAHRENHEIT));             //Pack T4 F
            Map(m => m.PackT4C).Name("Pack T4 C")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.CELSIUS));             //Pack T4 C
            MapSequentialCellProperty("CP", 96);                    //CP1 - CP96
            Map(m => m.Bat12vAmps).Name("12v Bat Amps")
                .TypeConverter<FloatOrNoneConverter>();             //12v Bat Amps
            Map(m => m.VIN).Name("VIN");                            //VIN
            Map(m => m.Hx).Name("Hx");                              //Hx
            Map(m => m.Bat12vVolts).Name("12v Bat Volts")
                .TypeConverter<FloatOrNoneConverter>();             //12v Bat Volts
            Map(m => m.Odokm).Name("Odo(km)");                      //Odo(km)
            Map(m => m.QCCount).Name("QC");                         //QC
            Map(m => m.L1L2Count).Name("L1/L2");                    //L1/L2
            Map(m => m.TPFL).Name("TP-FL")
                .TypeConverter(new PressureValueConverter(AirPressureUnit.PSI));   //TP-FL
            Map(m => m.TPFR).Name("TP-FR")
                .TypeConverter(new PressureValueConverter(AirPressureUnit.PSI));   //TP-FR
            Map(m => m.TPRR).Name("TP-RR")
                .TypeConverter(new PressureValueConverter(AirPressureUnit.PSI));   //TP-RR
            Map(m => m.TPRL).Name("TP-RL")
                .TypeConverter(new PressureValueConverter(AirPressureUnit.PSI));   //TP-RL
            Map(m => m.Ambient).Name("Ambient")
                .TypeConverter(new TemperatureValueConverter(TemperatureUnit.FAHRENHEIT)); //Ambient
            Map(m => m.SOH).Name("SOH");                            //SOH
            Map(m => m.RegenWh).Name("RegenWh");                    //RegenWh
            Map(m => m.PhoneBatteryLevelPercent).Name("BLevel");    //BLevel
            Map(m => m.EpochTime).Name("epoch time")
                .TypeConverter<EpochConverter>();                   //epoch time
            Map(m => m.MotorPwrw).Name("Motor Pwr(w)");             //Motor Pwr(w)
            Map(m => m.AuxPwr).Name("Aux Pwr(100w)")
                .TypeConverter<MultiplyBy100Converter>();           //Aux Pwr(100w)
            Map(m => m.ACPwr).Name("A/C Pwr(250w)")
                .TypeConverter<MultiplyBy250Converter>();           //A/C Pwr(250w)
            Map(m => m.ACComp01MPa).Name("A/C Comp(0.1MPa)");       //A/C Comp(0.1MPa)
            Map(m => m.EstPwrAC).Name("Est Pwr A/C(50w)")
                .TypeConverter<MultiplyBy50Converter>();            //Est Pwr A/C(50w)
            Map(m => m.EstPwrHtr).Name("Est Pwr Htr(250w)")
                .TypeConverter<MultiplyBy250Converter>();           //Est Pwr Htr(250w)
            Map(m => m.PlugState).Name("Plug State")
                .TypeConverter<EnumConverter<PlugState>>();         //Plug State
            Map(m => m.ChargeMode).Name("Charge Mode")
                .TypeConverter<EnumConverter<ChargeMode>>();        //Charge Mode
            Map(m => m.OBCOutPwr).Name("OBC Out Pwr");              //OBC Out Pwr
            Map(m => m.Gear).Name("Gear")
                .TypeConverter<EnumConverter<GearPosition>>();      //Gear
            Map(m => m.HVolt1).Name("HVolt1");                      //HVolt1
            Map(m => m.HVolt2).Name("HVolt2");                      //HVolt2
            Map(m => m.GPSStatus).Name("GPS Status")
                .TypeConverter<GPSStatusConverter>();               //GPS Status
            Map(m => m.PowerSW).Name("Power SW")
                .TypeConverter<EnumConverter<ReadState>>();         //Power SW
            Map(m => m.BMS).Name("BMS")
                .TypeConverter<EnumConverter<ReadState>>();         //BMS
            Map(m => m.OBC).Name("OBC")
                .TypeConverter<EnumConverter<ReadState>>();         //OBC
            Map(m => m.Debug).Name("Debug");                        //Debug
            Map(m => m.MotorTemp).Name("Motor Temp")
                .TypeConverter<TempOffsetConverter>();              //Motor Temp
            Map(m => m.Inverter2Temp).Name("Inverter 2 Temp")
                .TypeConverter<TempOffsetConverter>();              //Inverter 2 Temp
            Map(m => m.Inverter4Temp).Name("Inverter 4 Temp")
                .TypeConverter<TempOffsetConverter>();              //Inverter 4 Temp
            Map(m => m.Speed1).Name(" Speed1");                      //Speed1 (There is a space at the start)
            Map(m => m.Speed2).Name(" Speed2");                      //Speed2 (There is a space at the start)
            Map(m => m.WiperStatus).Name("Wiper Status")
                .TypeConverter<HexEnumConverter<FrontWiperStatus>>();  //Wiper Status
            Map(m => m.TorqueNm).Name("Torque Nm");                 //Torque Nm
            Map(m => m.RPM).Name("RPM");                            //RPM
        }

        public void MapSequentialCellProperty(string prefix, int count)
        {
            for (int i = 0; i <= count; i++)
            {
                foreach (var prop in ClassType.GetProperties())
                {
                    var propName = $"{prefix}{i}";
                    if (prop.Name == propName)
                    {
                        Map(ClassType, prop).Name(propName).TypeConverter<FloatConverter1K>();
                    }
                }
            }
        }
    }
}