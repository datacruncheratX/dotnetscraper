using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrawlerForDIGitEx
{ 
    static class CushManWakefield
    {
        static HtmlWeb hWeb = new HtmlWeb();
        public static void CushWakefield(List<RealEstateListings> lstrel)
        {
            String sbCushmanLink = @"http://calistings.cushmanwakefield.com/desktop_searchresults.aspx?Page=1&Sale=True&Lease=True&SaleLease=True&Sublease=True&PropertyType=Industrial&LandAcresRangeFrom=0&LandAcresRangeTo=100&AvailableSFRangeFrom=0&AvailableSFRangeTo=1000000&SalePriceFrom=0&SalePriceTo=50000000&Province=BC&Market=Metro+Vancouver&Submarket=All+Submarkets&Address=&Keyword=&AgentName=";
            HtmlDocument hDoc;
            try
            {
                hDoc = hWeb.Load(sbCushmanLink);
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

            HtmlDocument tableHDoc = new HtmlDocument();
            String sCushman = @"http://calistings.cushmanwakefield.com/";
            //String sPathForDownloadedData = @"E:\Work\DIGit Excavating\DownloadedData";


            String[] sSearchTerms = new String[] { "Vacancy Type", "Available SF", "Clear Height", "Loading Doors", "Asking Rent", "Additional Rent", "Available Date", "Zoning" };

            Char cSpace = (Char)32;

            String sDetailLink = String.Empty,
                   sStreetAddress = String.Empty,
                   sCity = String.Empty,
                   sTotalSF = String.Empty,
                   sNetRent = String.Empty,
                   sImageLink = String.Empty;

            //String sTestID = "427875";  //"427485"; 
            

            String sTAnother = String.Empty;
            String[] sLstArray;
            String[] sInnArray;

            HtmlNodeCollection hTableNodes = hDoc.DocumentNode.SelectNodes("//table[@id='tblSearchResults']//tr");

            Int32 iRowCount = hTableNodes.Count;

            RealEstateListings oRel = new RealEstateListings();
            //Console.WriteLine("The total value of RowCount is {0}", iRowCount);
            for (int i = 1; i < iRowCount; i++)
            {
                Console.WriteLine("Processing Cushman & Wakefield website record number:{0}", i);
                
                tableHDoc.LoadHtml(hTableNodes[i].InnerHtml);

                oRel.ListingCompany = "Cushman & Wakefield Ltd";
                oRel.WebLink = tableHDoc.DocumentNode.SelectSingleNode(@"//td[1]//a").Attributes["href"].Value.ToString() ?? String.Empty;

                oRel.ImageLink = tableHDoc.DocumentNode.SelectSingleNode(@"//td[1]//a//img").Attributes["src"].Value.ToString() ?? String.Empty;

                //oRel.ImageLink = FileIO.DownloadFile(oRel.ImageLink);

                oRel.Street = tableHDoc.DocumentNode.SelectSingleNode(@"//td[2]//a").InnerText ?? String.Empty;

                oRel.City = tableHDoc.DocumentNode.SelectSingleNode(@"//td[3]").InnerText ?? String.Empty;
                oRel.City = oRel.City.TrimAll(@"&nbsp;");

                oRel.TotalSF = tableHDoc.DocumentNode.SelectSingleNode(@"//td[4]").InnerText ?? String.Empty;

                oRel.NetRent = tableHDoc.DocumentNode.SelectSingleNode(@"//td[5]").InnerText ?? String.Empty;
                oRel.NetRent = oRel.NetRent.TrimAll(@"&nbsp;");

                sDetailLink = sCushman + oRel.WebLink;
                oRel.WebLink = sDetailLink ?? String.Empty;

                oRel.WebSiteRecordId = Regex.Match(sDetailLink, @"(TransID=\d+)").Value;
                oRel.WebSiteRecordId = oRel.WebSiteRecordId.Substring(oRel.WebSiteRecordId.IndexOfAny("0123456789".ToCharArray()));

                //var Matches = Regex.Match(sDetailLink, @"(TransID=\d+)");

                //if (Matches.Success)
                //{
                //    sTAnother = Matches.Value;
                //    sTAnother = sTAnother.Substring(sTAnother.IndexOfAny("0123456789".ToCharArray()));
                //}

                //This is temporary code remove it. 
                //if (!sTestID.Equals(sTAnother)) continue;

                hDoc = hWeb.Load(sDetailLink);

                oRel.Zoning  = hDoc.DocumentNode.SelectSingleNode(@"//table[@id='tblPropertyDetailsPropertySummary']").InnerText.Trim();
                oRel.Zoning = Regex.Match(oRel.Zoning, @"(Zoning:)\w+").Value;
                //if (sTemp.Equals("Zoning"))
                //    oRel.Zoning = tableHDoc.DocumentNode.SelectSingleNode(@"//table[@id='tblPropertyDetailsPropertySummary']//tbody//tr[4]//td[2]").InnerText;

                String sListingSummary = hDoc.DocumentNode.SelectSingleNode("//table[@id='tblPropertyDetailsListingSummarySection']").InnerText;

                sListingSummary = sListingSummary.Trim();

                sListingSummary = sListingSummary.Replace("\r\n", cSpace.ToString());

                sListingSummary = Regex.Replace(sListingSummary, @"\s\s+", ";");


                sLstArray = sListingSummary.Split(';').ToArray<String>();
                //// String[] sSearchTerms = new String[] 
                // { "Vacancy Type", "Available SF", "Clear Height", "Loading Doors", "Asking Rent", "Additional Rent", "Available Date", "Zoning" };
                foreach (String lstItem in sLstArray)
                {
                    if (lstItem.Contains("Vacancy Type"))
                        oRel.TransCategory = lstItem.Substring(Regex.Match(lstItem, ":").Index + 1) ?? String.Empty;
                    if (lstItem.Contains("Available SF"))
                        oRel.TotalSF = lstItem.Substring(Regex.Match(lstItem, ":").Index + 1) ?? String.Empty;
                    if (lstItem.Contains("Clear Height"))
                        oRel.ClearHeight = lstItem.Substring(Regex.Match(lstItem, ":").Index + 1) ?? String.Empty;
                    if (lstItem.Contains("Loading Doors"))
                        oRel.Dock = lstItem.Substring(Regex.Match(lstItem, ":").Index + 1) ?? String.Empty;
                    if (lstItem.Contains("Additional Rent"))
                        oRel.AdditionalRent = lstItem.Substring(Regex.Match(lstItem, ":").Index + 1) ?? String.Empty;
                    if (lstItem.Contains("Available Date"))
                        oRel.Availability = lstItem.Substring(Regex.Match(lstItem, ":").Index + 1) ?? String.Empty;
                }
                oRel.BuildingSF = Regex.Match(oRel.TotalSF, @"[0-9]{3,8}\s(SF)").Value.ToString() ?? String.Empty;
                oRel.ContactName = hDoc.DocumentNode.SelectSingleNode("//div[@id='propertyDetailsRightTop']//strong").InnerText ?? String.Empty;
                oRel.ContactDetails = hDoc.DocumentNode.SelectSingleNode("//*[@id='propertyDetailsRightTop']").InnerText ?? String.Empty;
                oRel.ContactPhone = Regex.Match(oRel.ContactDetails, @"\([0-9]{3}\)([0-9]{3}-[0-9]{4})").Value.ToString() ?? String.Empty;
                oRel.ContactEmail = hDoc.DocumentNode.SelectSingleNode("//*[@id='propertyDetailsRightTop']//a").InnerText ?? String.Empty;

                HtmlNodeCollection aNodes = hDoc.DocumentNode.SelectNodes("//*[@id='propertyDetailsRightBottom']//a");

                if (aNodes != null)
                {
                    if (aNodes.Count > 0)
                    {
                        sInnArray = new String[aNodes.Count];

                        for (int j = 0; j < aNodes.Count; j++)
                        {
                            sInnArray[j] = aNodes[j].Attributes["href"].Value.ToString();
                            //sInnArray[j] = FileIO.DownloadFile(aNodes[j].Attributes["href"].Value.ToString());
                        }



                        oRel.BrochureLink = sInnArray;
                    }
                }

                
                oRel.ContactDetails = oRel.ContactDetails.Substring(1, oRel.ContactDetails.IndexOf("Cushman & Wakefield") - 1);
                oRel.ContactDetails = oRel.ContactDetails.Trim();
                oRel.ContactDetails = oRel.ContactDetails.Replace("\r\n", cSpace.ToString());
                oRel.ContactDetails = Regex.Replace(oRel.ContactDetails, @"(\s\s+)", " ");

                //Console.WriteLine("Adding the item into the list");
                lstrel.Add(oRel);
            }
        }
    }
}
