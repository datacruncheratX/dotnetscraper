using System;
using System.Collections.Generic;

namespace CrawlerForDIGitEx
{
    class Program
    {
        static void Main(string[] args)
        {
            List<RealEstateListings> lstRealEstates = new List<RealEstateListings>();
             
            try
            {
                TaskScheduling.ScheduleTask();
                CushManWakefield.CushWakefield(lstRealEstates);
                DTZVancouver.DtzVancouverData(lstRealEstates);
                FileIO.WriteToFile(lstRealEstates);
            }
            catch (Exception)
            {
                
                throw;
            }



        }
    }
}
