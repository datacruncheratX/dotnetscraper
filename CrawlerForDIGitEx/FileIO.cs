using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace CrawlerForDIGitEx
{
    static class FileIO
    {
        public static String GetDefaultDirPath()
        {
            String sPath = String.Empty;
            try
            {
                sPath = Assembly.GetExecutingAssembly().Location;
                sPath = System.IO.Path.GetDirectoryName(sPath);
                
            }
            catch (Exception)
            {
                
            
            }

            //sPath = @"E:\Work\DIGit Excavating\FinalTesting";
            return sPath;

        }

        public static void WriteToFile(List<RealEstateListings> lstRel)
        {
            //String sFilePath = GetDefaultDirPath()+  @"\CushmanDtz" + DateTime.Now.ToString("MM-dd-yyyy") + ".xlsx";
            String sFilePath = GetDefaultDirPath() + @"\CushmanDtz.xlsx";

            FileInfo fiTemplateFile = new FileInfo(GetDefaultDirPath() +  @"\TemplateOutput.xlsx");
            String sReturnedImageLink = String.Empty;

            FileInfo newFile = new FileInfo(sFilePath);
            Int32 j = 0;

            if (!File.Exists(sFilePath))
            {
                using (ExcelPackage epWorkbook = new ExcelPackage(newFile, fiTemplateFile))
                {
                    ExcelWorksheet ewsOnlyWorksheet = epWorkbook.Workbook.Worksheets[1];
                    j = 2;
                    ewsOnlyWorksheet.InsertRow(2, lstRel.Count);

                    UpdateWorkSheet(lstRel, ref sReturnedImageLink, ewsOnlyWorksheet, ref j);
                    epWorkbook.Save();

                }
            }
            else if (File.Exists(sFilePath))
            {
                List<MinValues> lstNewMin = new List<MinValues>();
                MinValues ONewMin = new MinValues();



                foreach (var item in lstRel)
                {
                    ONewMin.ListingCompany = item.ListingCompany.Trim();
                    ONewMin.WebRecordId = item.WebSiteRecordId.Trim();
                    lstNewMin.Add(ONewMin);
                }


                List<MinValues> lstOldMin = new List<MinValues>();
                MinValues OOldMin = new MinValues();

                
                using(ExcelPackage ePackage = new ExcelPackage(newFile))
                {
                    ExcelWorksheet eOldWorkSheet = ePackage.Workbook.Worksheets["OutputColumns"];
                    j = eOldWorkSheet.Dimension.Rows;

                    for (int i = 2; i < j; i++)
                    {
                        OOldMin.WebRecordId = eOldWorkSheet.Cells[String.Format("AA{0}", i)].Value.ToString().Trim();
                        OOldMin.ListingCompany = eOldWorkSheet.Cells[String.Format("R{0}", i)].Value.ToString().Trim();
                        lstOldMin.Add(OOldMin);
                    }

                    lstNewMin = lstNewMin.OrderBy(o => o.WebRecordId).ToList();
                    lstOldMin = lstOldMin.OrderBy(o => o.WebRecordId).ToList();

                    IEnumerable<MinValues> differenceObject = lstNewMin.Except(lstOldMin);

                    List<RealEstateListings> lstNRel = new List<RealEstateListings>();

                    foreach (MinValues dObj in differenceObject)
                    {
                        lstNRel.Add(
                            lstRel.Find(
                            r => r.WebSiteRecordId.Equals(dObj.WebRecordId)
                                )
                            );
                    }

                    j++ ;
                    eOldWorkSheet.InsertRow(j, lstNRel.Count);

                    UpdateWorkSheet(lstNRel, ref sReturnedImageLink, eOldWorkSheet, ref j);
                    ePackage.Save();
 
                }


            }

        }

        private static void UpdateWorkSheet(List<RealEstateListings> lstRel, ref String sReturnedImageLink, ExcelWorksheet ewsOnlyWorksheet, ref Int32 j)
        {
            for (int x = 0; x < lstRel.Count(); x++)
            {
                ewsOnlyWorksheet.Cells[String.Format("A{0}", j)].Value = (j - 1).ToString();
                ewsOnlyWorksheet.Cells[String.Format("B{0}", j)].Value = lstRel[x].City;
                ewsOnlyWorksheet.Cells[String.Format("E{0}", j)].Value = lstRel[x].Street;
                ewsOnlyWorksheet.Cells[String.Format("F{0}", j)].Value = lstRel[x].TransCategory;
                ewsOnlyWorksheet.Cells[String.Format("H{0}", j)].Value = lstRel[x].TotalSF;
                ewsOnlyWorksheet.Cells[String.Format("J{0}", j)].Value = lstRel[x].BuildingSF;
                ewsOnlyWorksheet.Cells[String.Format("K{0}", j)].Value = lstRel[x].NetRent;
                ewsOnlyWorksheet.Cells[String.Format("L{0}", j)].Value = lstRel[x].AdditionalRent;
                ewsOnlyWorksheet.Cells[String.Format("M{0}", j)].Value = lstRel[x].Dock;
                ewsOnlyWorksheet.Cells[String.Format("N{0}", j)].Value = String.Empty; //lstRel[x].Grade;
                ewsOnlyWorksheet.Cells[String.Format("O{0}", j)].Value = lstRel[x].ClearHeight;
                ewsOnlyWorksheet.Cells[String.Format("P{0}", j)].Value = lstRel[x].Zoning;
                ewsOnlyWorksheet.Cells[String.Format("Q{0}", j)].Value = lstRel[x].Availability;
                ewsOnlyWorksheet.Cells[String.Format("R{0}", j)].Value = lstRel[x].ListingCompany;
                 

                sReturnedImageLink = DownloadFile(lstRel[x].ImageLink);

                ewsOnlyWorksheet.Cells[String.Format("T{0}", j)].Hyperlink = new Uri(sReturnedImageLink);
                ewsOnlyWorksheet.Cells[String.Format("U{0}", j)].Value = lstRel[x].WebLink;

                //Downloading Brochures
                for (int i = 0; i < lstRel[x].BrochureLink.Length; i++)
                {
                    lstRel[x].BrochureLink[i] = DownloadFile(lstRel[x].BrochureLink[i].ToString());
                }

                ewsOnlyWorksheet.Cells[String.Format("V{0}", j)].Hyperlink = new Uri(String.Join(Environment.NewLine.ToString() + ",", lstRel[x].BrochureLink));
                ewsOnlyWorksheet.Cells[String.Format("W{0}", j)].Value = lstRel[x].ContactName;
                ewsOnlyWorksheet.Cells[String.Format("X{0}", j)].Value = lstRel[x].ContactPhone;
                ewsOnlyWorksheet.Cells[String.Format("Y{0}", j)].Value = lstRel[x].ContactEmail;
                ewsOnlyWorksheet.Cells[String.Format("Z{0}", j)].Value = lstRel[x].ContactDetails;
                ewsOnlyWorksheet.Cells[String.Format("AA{0}", j)].Value = lstRel[x].WebSiteRecordId;

                j++;
            }
        }

        public static String DownloadFile(String FileToDownload)
        {
            String sFileNameOnly = String.Empty;

            try
            {
                using (WebClient wClient = new WebClient())
                {
                    sFileNameOnly = FileToDownload.Split('/').ToArray().Last();
                    wClient.DownloadFile(FileToDownload, GetDefaultDirPath() + @"\" + sFileNameOnly);
                    sFileNameOnly = GetDefaultDirPath() + @"\" + sFileNameOnly;
                }
            }
            catch (WebException)
            {
                sFileNameOnly = FileToDownload; // String.Format("An error occured while downloading File at this address:{0} {1} and the error message was:{2}"
                    //,FileToDownload,Environment.NewLine, wex.Message);
            }
            catch (Exception)
            {
                sFileNameOnly = FileToDownload;  //String.Format("An error occured while downloading File at this address:{0} {1} and the error message was:{2}"
                                    //, FileToDownload, Environment.NewLine, ex.Message);
            }
            return sFileNameOnly;
        }
    }
}
