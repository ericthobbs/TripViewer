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

namespace LeafSpy.DataParser.ClassMaps
{
    internal class CsvToTripHistoryMap : ClassMap<TripHistory>
    {
        public CsvToTripHistoryMap()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.Time).Name("Time");
            Map(m => m.OdoInMiles).Name("odo mi");
            Map(m => m.TripDistanceInMiles).Name("dist mi");
            Map(m => m.ElevationDeltaFeet).Name("elv ft");
            Map(m => m.EnergyInKwhUsed).Name("Energy");
            Map(m => m.Gids).Name("Gids");
            Map(m => m.SGids).Name("SGids");
            Map(m => m.EGids).Name("EGids");
            Map(m => m.Ahr).Name("AHr");
            Map(m => m.SOH).Name("SOH");
            Map(m => m.Hx).Name("Hx");
            Map(m => m.SHVolt).Name("SHVolt");
            Map(m => m.EHVolt).Name("EHVolt");
            Map(m => m.Drive).Name("Drive");
            Map(m => m.Regen).Name("Regen");
            Map(m => m.Charge).Name("Charge");
            Map(m => m.L1L2Count).Name("L1/L2");
            Map(m => m.QCCount).Name("QC");
        }
    }
}