using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10274614
// Student Name : Cai Renjie
// Partner Name : Jackie Ang
//==========================================================

namespace PRG2_ASG
{
    class SpecialOffer
    {
        public string OfferCode { get; set; }
        public string Description { get; set; }
        public double DiscountPercent { get; set; }

        public SpecialOffer(string offerCode, string description, double discountPercent)
        {
            OfferCode = offerCode;
            Description = description;
            DiscountPercent = discountPercent;
        }

        public override string ToString()
        {
            return $"{OfferCode}: {Description} - {DiscountPercent}% off";
        }
    }
}
