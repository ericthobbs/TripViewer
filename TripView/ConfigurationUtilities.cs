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
using System.Text.RegularExpressions;

namespace TripView
{
    /// <summary>
    /// Configuration Helper Methods
    /// </summary>
    internal static class ConfigurationUtilities
    {
        /// <summary>
        /// Converts a color string into a Skia SKColor
        /// </summary>
        /// <param name="colordesc">the ARGB hex color code string</param>
        /// <param name="fallback">Fallback color in case the color code is invalid.</param>
        /// <returns>the color value from the provided string or the fallback color if conversion fails.</returns>
        public static SkiaSharp.SKColor GetColorFromString(string? colordesc, SkiaSharp.SKColor fallback)
        {
            if (colordesc == null)
            {
                return fallback;
            }
            try
            {
                var pointColor = fallback;

                if (colordesc.StartsWith('#'))
                {
                    if (!SkiaSharp.SKColor.TryParse(colordesc, out pointColor))
                    {
                        pointColor = fallback;
                    }
                }
                else if(colordesc.StartsWith("rgb(") || colordesc.StartsWith("hsv(") || colordesc.StartsWith("hsl("))
                {
                    //TODO: Refactor this
                    var regex = new Regex(@"^(?<model>rgba|hsl|hsv)\s*\(\s*(?<c1>\d+(?:\.\d+)?)\s*,\s*(?<c2>\d+(?:\.\d+)?)\s*,\s*(?<c3>\d+(?:\.\d+)?)(?:\s*,\s*(?<alpha>\d+(?:\.\d+)?))?\s*\)$", RegexOptions.IgnoreCase);

                    var match = regex.Match(colordesc);

                    if (match.Success && match.Groups["model"].Success)
                    {
                        byte alpha = 255;
                        if (match.Groups["alpha"].Success)
                        {
                            float parsedAlphaValue = 1.0f;
                            if (float.TryParse(match.Groups["alpha"].Value, out parsedAlphaValue))
                            {
                                if (parsedAlphaValue <= 1.0f && (match.Groups["model"].Value.ToLower() == "hsl" || match.Groups["model"].Value.ToLower() == "hsv"))
                                {
                                    alpha = (byte)(parsedAlphaValue * 255);
                                }
                                else
                                {
                                    alpha = Convert.ToByte(parsedAlphaValue);
                                }
                            }
                        }
                        switch (match.Groups["model"].Value)
                        {
                            case "rgb":
                                return new SkiaSharp.SKColor(
                                    Convert.ToByte(match.Groups["c1"].Value),
                                    Convert.ToByte(match.Groups["c2"].Value),
                                    Convert.ToByte(match.Groups["c3"].Value),
                                    alpha);
                            case "hsl":
                                return SkiaSharp.SKColor.FromHsl(
                                    float.Parse(match.Groups["c1"].Value),
                                    float.Parse(match.Groups["c2"].Value),
                                    float.Parse(match.Groups["c3"].Value),
                                    alpha);
                            case "hsv":
                                return SkiaSharp.SKColor.FromHsv(
                                    float.Parse(match.Groups["c1"].Value),
                                    float.Parse(match.Groups["c2"].Value),
                                    float.Parse(match.Groups["c3"].Value),
                                    alpha);
                            default:
                                break;
                        }
                    }
                }
                return pointColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to GetColorFromString ({colordesc}): {ex.Message}");
                return fallback;
            }
        }
    }
}
