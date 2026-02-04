using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10270210
// Student Name : Jackie Ang
// Partner Name : Cai Renjie
//==========================================================

namespace PRG2_ASG
{
    class FoodItem
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public FoodItem(string itemName, string description, double price)
        {
            ItemName = itemName;
            Description = description;
            Price = price;
        }

        public override string ToString()
        {
            return $"{ItemName}: {Description} - ${Price:F2}";
        }
    }
}
