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
                // STEP 1: Validate the XML using the XSD
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, xsdURL);
                settings.ValidationType = ValidationType.Schema;

                string errors = "";

                settings.ValidationEventHandler += (sender, args) =>
                {
                    errors += args.Message + "\n";
                };

                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read()) ;
                }

                if (!string.IsNullOrEmpty(errors))
                {
                    return "False";
                }

                // STEP 2: Load the validated XML
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlUrl);

                // STEP 3: Convert @Rating → _Rating
                XmlNodeList hotels = doc.SelectNodes("//Hotel");
                foreach (XmlNode hotel in hotels)
                {
                    XmlAttribute ratingAttr = hotel.Attributes["Rating"];
                    if (ratingAttr != null)
                    {
                        XmlElement ratingElem = doc.CreateElement("_Rating");
                        ratingElem.InnerText = ratingAttr.Value;
                        hotel.AppendChild(ratingElem);
                        hotel.Attributes.Remove(ratingAttr);
                    }
                }

                // STEP 4: Convert @NearestAirport → _NearestAirport
                XmlNodeList addresses = doc.SelectNodes("//Address");
                foreach (XmlNode addr in addresses)
                {
                    XmlAttribute airportAttr = addr.Attributes["NearestAirport"];
                    if (airportAttr != null)
                    {
                        XmlElement airportElem = doc.CreateElement("_NearestAirport");
                        airportElem.InnerText = airportAttr.Value;
                        addr.AppendChild(airportElem);
                        addr.Attributes.Remove(airportAttr);
                    }
                }

                // STEP 5: Serialize to JSON
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
                return jsonText;
            }
            catch
            {
                return "False"; // Catch any exceptions or malformed input
            }
        }



    }
}
