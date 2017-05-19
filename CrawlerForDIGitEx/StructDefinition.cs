using System;

namespace CrawlerForDIGitEx
{
    struct RealEstateListings
    {
        public String City { get; set; }
        public String Street { get; set; }
        public String TransCategory { get; set; }
        public String TotalSF { get; set; }
        public String BuildingSF { get; set; }
        public String NetRent { get; set; }
        public String AdditionalRent { get; set; } //
        public String Dock { get; set; } //loading doors
        // public String Grade{get;set;} //drive in
        public String ClearHeight { get; set; }
        public String Zoning { get; set; }
        public String Availability { get; set; }
        public String ListingCompany { get; set; }
        public String WebLink { get; set; }
        public String ImageLink { get; set; }
        public String[] BrochureLink { get; set; }
        public String ContactName { get; set; }
        public String ContactPhone { get; set; }
        public String ContactEmail { get; set; }
        public String ContactDetails { get; set; }
        public String WebSiteRecordId { get; set; }
    }


    struct MinValues : IEquatable<MinValues>
    {
        public String ListingCompany { get; set; }
        public String WebRecordId { get; set; }

        public bool Equals(MinValues other)
        {
            if (Object.ReferenceEquals(other, null)) return false;

            if (Object.ReferenceEquals(this, other)) return true;
             
            return
                ListingCompany.Equals(other.ListingCompany) &&
                WebRecordId.Equals(other.WebRecordId);
        }

        public override int GetHashCode()
        {
            int hashListingCompany = ListingCompany == null ? 0 : ListingCompany.GetHashCode();

            int hasWebRecordId = WebRecordId == null ? 0 : WebRecordId.GetHashCode();

            return
                hashListingCompany ^ hasWebRecordId;
        }
    }
}
