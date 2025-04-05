using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;

/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 **/

namespace ConsoleApp1
{
    public class Program
    {
        // Hosted URLs (update as needed)
        public static string xmlURL = "https://malmar10.github.io/Hotels-xml/Hotels.xml";
        public static string xmlErrorURL = "https://malmar10.github.io/Hotels-xml/HotelsErrors.xml";
        public static string xsdURL = "https://malmar10.github.io/Hotels-xml/Hotels.xsd";

        public static void Main(string[] args)
        {
            Console.WriteLine("=== Validating Hotels.xml (Valid XML) ===");
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result == "No Error" ? "No errors are found." : result);

            Console.WriteLine("\n=== Validating HotelsErrors.xml (Invalid XML) ===");
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result == "No Error" ? "No errors are found." : result);

            Console.WriteLine("\n=== Converting Hotels.xml to JSON ===");
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1 — Validate XML against XSD
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, xsdUrl);
                settings.ValidationType = ValidationType.Schema;

                string validationErrors = "";

                settings.ValidationEventHandler += (sender, args) =>
                {
                    validationErrors += $"{args.Message}\n";
                };

                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read()) ;
                }

                return string.IsNullOrWhiteSpace(validationErrors) ? "No Error" : validationErrors.Trim();
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        // Q2.2 — Convert valid XML to JSON
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                // Try to load XML — if it's malformed, this will throw
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlUrl);

                // Process attributes into elements as required by spec
                XmlNodeList hotelList = doc.SelectNodes("//Hotel");
                foreach (XmlNode hotel in hotelList)
                {
                    XmlAttribute rating = hotel.Attributes["Rating"];
                    if (rating != null)
                    {
                        XmlElement ratingElem = doc.CreateElement("_Rating");
                        ratingElem.InnerText = rating.Value;
                        hotel.AppendChild(ratingElem);
                        hotel.Attributes.Remove(rating);
                    }
                }

                XmlNodeList addressList = doc.SelectNodes("//Address");
                foreach (XmlNode addr in addressList)
                {
                    XmlAttribute airport = addr.Attributes["NearestAirport"];
                    if (airport != null)
                    {
                        XmlElement airportElem = doc.CreateElement("_NearestAirport");
                        airportElem.InnerText = airport.Value;
                        addr.AppendChild(airportElem);
                        addr.Attributes.Remove(airport);
                    }
                }

                // Convert XML to JSON
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);
                return jsonText;
            }
            catch
            {
                // If it's malformed or unreadable XML, return "False"
                return "False";
            }
        }

    }
}
