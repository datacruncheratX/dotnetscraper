using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace CrawlerForDIGitEx
{
    internal class DTZListins
    {
        public String street_address { get; set; }
        public String street { get; set; }
        public String street_number { get; set; }
        public String street2 { get; set; }
        public String id { get; set; }
        public String mls_id { get; set; }
        public String short_description { get; set; }
        public String City { get; set; }
        public String Price { get; set; }
        public String stype_freq { get; set; }
        public String beds { get; set; }
        public String baths { get; set; }
        public String sqft { get; set; }
        public String lat_pos { get; set; }
        public String lon_pos { get; set; }
        public String thumb { get; set; }
        public String formattedprice { get; set; }
        public String formattedSqft { get; set; }
        public String proplink { get; set; }
    }

    static class DTZVancouver
    {
        static HtmlWeb hWeb = new HtmlWeb();

        public static void DtzVancouverData(List<RealEstateListings> lstrel)
        {
            String jsonResult = String.Empty;
            List<DTZListins> lstdzList = new List<DTZListins>();

            //String sPathForDownloadedData = @"E:\Work\DIGit Excavating\DownloadedData";


            String[] sSearchTerms = new String[] { "Clear Height", "Loading Doors", " Portion", "Taxes", "Availability", "Zoning" };

           // Char cSpace = (Char)32;

            String sDetailLink = String.Empty,
                   sStreetAddress = String.Empty,
                   sCity = String.Empty,
                   sTotalSF = String.Empty,
                   sNetRent = String.Empty,
                   sImageLink = String.Empty;



            String sTAnother = String.Empty;



            String sDTZLink = "http://www.dtzvancouver.com/index.php?option=com_iproperty&amp;view=advsearch&amp;task=ajaxSearch&amp;ptype=2,8,3,4,5,2,8,3,4,5&amp;price_high=1000000000&amp;price_low=0&amp;sqft_high=10000000&amp;sqft_low=0&amp;beds_high=20&amp;beds_low=0&amp;baths_high=20&amp;baths_low=0&amp;search=&amp;city=Vancouver&amp;stype=&amp;limit=50&amp;limitstart=0&amp;format=raw";
            //var noType;
            String sTotalCount = String.Empty;


            HtmlDocument hDoc;
            try
            {
                hDoc = hWeb.Load(sDTZLink);
            }
            catch (HtmlWebException)
            {
                return;
            }
            catch (System.Net.WebException)
            {
                return;
            }
            catch (Exception)
            {
                return;
            }

            jsonResult = hDoc.DocumentNode.InnerText;

            sTotalCount = Regex.Match(jsonResult, @",""(totalcount)"":""(\d+)""").Value.Split(':').Last().Replace('"', ' ').Trim();
            Int32 z = Int32.Parse(sTotalCount);

            for (int i = 0; i <= z; i += 50)
            {
                sDTZLink = String.Format("http://www.dtzvancouver.com/index.php?option=com_iproperty&amp;view=advsearch&amp;task=ajaxSearch&amp;ptype=2,8,3,4,5,2,8,3,4,5&amp;price_high=1000000000&amp;price_low=0&amp;sqft_high=10000000&amp;sqft_low=0&amp;beds_high=20&amp;beds_low=0&amp;baths_high=20&amp;baths_low=0&amp;search=&amp;city=Vancouver&amp;stype=&amp;limit=50&amp;limitstart={0}&amp;format=raw", i);

                hDoc = LoadDocument(sDTZLink);

                if (hDoc == null) continue;

                jsonResult = hDoc.DocumentNode.InnerText;

                jsonResult = Regex.Replace(jsonResult, @",""(totalcount)"":""(\d+)""", "");

                lstdzList.AddRange(JsonConvert.DeserializeObject<List<DTZListins>>(jsonResult));
            }

            //Console.WriteLine("The total number of records at DTZVancouver is:{0}",lstdzList.Count.ToString());
            RealEstateListings oRel = new RealEstateListings();
            String sDTZprop = @"http://www.dtzvancouver.com";
            String sPivotTableHTML = String.Empty,
                sTableData = String.Empty;

            Int32 x = 0;
            foreach (DTZListins lstItem in lstdzList)
            {
                x++;
                Console.WriteLine("Processing DTZ Vancouver website record number:{0}", x);
                oRel.ListingCompany = "DTZ Vancouver Real Estate Ltd.";
                oRel.Street = lstItem.street_address;
                oRel.City = lstItem.City;
                oRel.NetRent = lstItem.formattedprice;
                oRel.TotalSF = lstItem.formattedSqft;
                oRel.WebLink = sDTZprop + lstItem.proplink;
                oRel.WebSiteRecordId = lstItem.id.ToString();

                hDoc = LoadDocument(oRel.WebLink);

                oRel.ImageLink = sDTZprop + hDoc.DocumentNode.SelectSingleNode("//*[@id='ja-content']//table[1]//div[@class='ip_imagetab']//a//img").Attributes["src"].Value;

               // oRel.ImageLink = FileIO.DownloadFile(oRel.ImageLink);
                sPivotTableHTML = hDoc.DocumentNode.SelectSingleNode("//*[@id='ja-content']//table[2]").InnerHtml;
                oRel.BrochureLink = new String[] 
                { 
                    //FileIO.DownloadFile
                    //(
                    //Downloading Brochures, then return only file name otherwise an unneccessary loop was to be applied. 

                    sDTZprop + hDoc.DocumentNode.SelectSingleNode(@"//table[@class='summary_table']//a").Attributes["href"].Value 
                    //)
                };


                oRel.TransCategory = hDoc.DocumentNode.SelectSingleNode(@"//div[@class='ip_sidecol_subaddress']").InnerText.Split(':').Last();
                sTableData = hDoc.DocumentNode.SelectSingleNode(@"//table[@class='ip_details_table_container']").InnerText;

                sTableData = sTableData.Trim().TrimAll(@"\s\s+", "; ");
                sTableData = sTableData.Replace(":;", ":");
                String[] sCollection = sTableData.Split(';');
                 
                for (int i = 0; i < sCollection.Length; i++)
                {
                    var key = sCollection[i].Split(':').First();
                    var value = sCollection[i].Split(':').Last();

                    if (key.Contains("Dock"))
                        oRel.Dock = value;
                    if (key.Contains("Taxes"))
                        oRel.AdditionalRent = value;
                    if (key.Contains("Availability"))
                        oRel.Availability  = value;
                    if (key.Contains("Portion"))
                        oRel.BuildingSF = value;
                    if (key.Contains("Zoning"))
                        oRel.Zoning = value;


                }

                oRel.ClearHeight = String.Empty;

                hDoc.LoadHtml(hDoc.DocumentNode.SelectSingleNode(@"//table[@class='summary_table']").InnerHtml);

                oRel.ContactName = GetNodesData(hDoc, @"//div[@class='side_agent_details']//a[1]//b"); ;
                oRel.ContactDetails = GetNodesData(hDoc, @"//div[@class='ip_sidecol_cell']");
                oRel.ContactPhone = GetNodesData(hDoc, @"//div[@class='ip_sidecol_phone']"); ;
                oRel.ContactEmail = String.Empty;
                

                lstrel.Add(oRel);
            }


        }

        private static String GetNodesData(HtmlDocument hDocs, String sNodes)
        {

            var sTest = hDocs.DocumentNode.SelectNodes(sNodes);
            String sConcat = String.Empty;

            foreach (var item in sTest)
            {
                sConcat = sConcat + ", " + item.InnerText;
            }
            sConcat = sConcat.Substring(1);

            return sConcat;
        }

        private static WebRequest CreateRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000;
            request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
            return request;
        }

        private static HtmlAgilityPack.HtmlDocument LoadDocument(string url)
        {
            var document = new HtmlAgilityPack.HtmlDocument();

            
            try
            {
                document = hWeb.Load(url);
            }
            catch (HtmlWebException)
            {
            }
            catch (System.Net.WebException)
            {
                document = null;
                return document;
            }
            catch (Exception)
            {
                document = null;
                return document ;
            }
            //try
            //{
            //    using (var responseStream = CreateRequest(url).GetResponse().GetResponseStream())
            //    {
            //        document.Load(responseStream, Encoding.UTF8);
            //    }
            //}
            //catch (Exception)
            //{
            //    //just do a second try
            //    Thread.Sleep(1000);
            //    using (var responseStream = CreateRequest(url).GetResponse().GetResponseStream())
            //    {
            //        document.Load(responseStream, Encoding.UTF8);
            //    }
            //}

            return document;
        }
    }



}
