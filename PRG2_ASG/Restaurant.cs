using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASG
{
    class Restaurant
    {
        public string RestaurantId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<FoodItem> MenuItems { get; set; }
        public Queue<Order> OrderQueue { get; set; }
        public List<SpecialOffer> SpecialOffers { get; set; }

        public Restaurant(string restaurantId, string name, string email)
        {
            RestaurantId = restaurantId;
            Name = name;
            Email = email;
            MenuItems = new List<FoodItem>();
            OrderQueue = new Queue<Order>();
            SpecialOffers = new List<SpecialOffer>();
        }

        public void AddFoodItem(FoodItem item)
        {
            MenuItems.Add(item);
        }

        public void AddOrder(Order order)
        {
            OrderQueue.Enqueue(order);
        }

        public void DisplayMenu()
        {
            Console.WriteLine($"Restaurant: {Name} ({RestaurantId})");
            foreach (var item in MenuItems)
            {
                Console.WriteLine($" - {item.ItemName}: {item.Description} - ${item.Price:F2}");
            }
        }

        public FoodItem GetFoodItem(string itemName)
        {
            return MenuItems.FirstOrDefault(item => item.ItemName.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }

        public override string ToString()
        {
            return $"{RestaurantId},{Name},{Email}";
        }
    }
}
