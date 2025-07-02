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
namespace LeafSpy.DataParser
{
    public class TripHistory
    {
        public DateOnly? Date { get; set; }              //Date
        public TimeOnly Time { get; set; }              //Time
        public float OdoInMiles { get; set; }           //odo mi
        public TimeSpan TripDuration { get; set; }      //Duration
        public float TripDistanceInMiles { get; set; }  //dist mi
        public int ElevationDeltaFeet { get; set; }     //elv ft
        public int EnergyInKwhUsed { get; set; }        //Energy
        public int Gids { get; set; }                   //Gids
        public int SGids { get; set; }                  //SGids
        public int EGids { get; set; }                  //EGids
        public float Ahr { get; set; }                  //AHr
        public string? SOH { get; set; }                //SOH
        public float Hx { get; set; }                   //Hx
        public float SHVolt { get; set; }               //SHVolt
        public float EHVolt { get; set; }               //EHVolt
        public int Drive { get; set; }                  //Drive
        public int Regen { get; set; }                  //Regen
        public int Charge { get; set; }                 //Charge
        public int L1L2Count { get; set; }              //L1/L2
        public int QCCount { get; set; }                //QC
    }

}