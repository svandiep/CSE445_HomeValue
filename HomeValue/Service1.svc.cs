using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace HomeValue
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        string ZillowKey = " "; //Zillow API key

        public int ValueByZip(string zip)
        {
            string value = null;
            string url = "http://api.zippopotam.us/us/" + zip;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); // get City from zip code
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader sreader = new StreamReader(dataStream);
            string responsereader = sreader.ReadToEnd();
            response.Close();
            zipObject zipobject = JsonConvert.DeserializeObject<zipObject>(responsereader);

            string url2 = "http://www.zillow.com/webservice/GetRegionChildren.htm?zws-id=" + ZillowKey;
            string url3 = "&state=" + zipobject.places[0].stateabbreviation + "&city=" + zipobject.places[0].placename + "&childtype=zipcode";
            string urlz = string.Concat(url2, url3);
            //Console.WriteLine(urlz);
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(urlz); // get City from zip code
            WebResponse response2 = request2.GetResponse();
            Stream dataStream2 = response2.GetResponseStream();
            StreamReader sreader2 = new StreamReader(dataStream2);
            XDocument xmlDoc = new XDocument();
            xmlDoc = XDocument.Parse(sreader2.ReadToEnd()); //Get home value data from XML response
            response2.Close();
            //Console.WriteLine(xmlDoc);            
            foreach (XElement element in xmlDoc.Descendants("name"))
            {
                if (element.Value == zip)
                {
                    XNode next = element.NextNode;
                    XElement zindex = (next as XElement);
                    value = zindex.Value;
                    //Console.WriteLine(element.Value);
                    //Console.WriteLine(value);
                }
            }

            return Convert.ToInt32(value); //Zip code returns average for single zip code
        }

        public int CityAverage(string city, string state)
        {
            bool found = false;
            int value = 1;
            int count = 0;
            string url2 = "http://www.zillow.com/webservice/GetRegionChildren.htm?zws-id=" + ZillowKey;
            string url3 = "&state=" + state + "&city=" + city + "&childtype=zipcode";
            string urlz = string.Concat(url2, url3);
            //Console.WriteLine(urlz);
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(urlz); // get City from zip code
            WebResponse response2 = request2.GetResponse();
            Stream dataStream2 = response2.GetResponseStream();
            StreamReader sreader2 = new StreamReader(dataStream2);
            XDocument xmlDoc = new XDocument();
            xmlDoc = XDocument.Parse(sreader2.ReadToEnd());
            response2.Close();
            //Console.WriteLine(xmlDoc);            
            foreach (XElement element in xmlDoc.Descendants("zindex"))//find all values for averaging 
            {
                found = true;
                count++;
                //Console.WriteLine(count);
                int temp = Convert.ToInt32(element.Value);
                value = value + temp;
                //Console.WriteLine(value);
            }
            if (found) //if values were found calculate average
            {
                value = value / count;
            }

            return value;
        }

        public class zipObject
        {

            [JsonProperty(PropertyName = "post code")]
            public string postcode { get; set; }

            public string country { get; set; }

            [JsonProperty(PropertyName = "country abbreviation")]
            public string countryabbreviation { get; set; }
            public List<Places> places { get; set; }
        }

        public class Places
        {
            [JsonProperty(PropertyName = "place name")]
            public string placename { get; set; }

            public decimal longitude { get; set; }
            public string state { get; set; }

            [JsonProperty(PropertyName = "state abbreviation")]
            public string stateabbreviation { get; set; }

            public decimal latitude { get; set; }
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}
