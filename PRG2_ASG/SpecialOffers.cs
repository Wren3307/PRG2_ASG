using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
