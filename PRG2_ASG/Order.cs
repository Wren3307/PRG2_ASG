using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASG
{
    class Order
    {
        public int OrderId { get; set; }

        public Customer Customer { get; set; }

        public Restaurant Restaurant { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public string DeliveryAddress { get; set; }

        public Dictionary<FoodItem, int> OrderedItems { get; set; }

        public string SpecialRequest { get; set; }

        public double TotalAmount { get; set; }

        public string PaymentMethod { get; set; }

        public string Status { get; set; }

        public DateTime CreatedDateTime { get; set; }


        public Order(int orderId, Customer customer, Restaurant restaurant, DateTime deliveryDateTime, string deliveryAddress)
        {
            OrderId = orderId;
            Customer = customer;
            Restaurant = restaurant;
            DeliveryDateTime = deliveryDateTime;
            DeliveryAddress = deliveryAddress;
            OrderedItems = new Dictionary<FoodItem, int>();
            SpecialRequest = "";
            TotalAmount = 0;
            PaymentMethod = "";
            Status = "Pending";
            CreatedDateTime = DateTime.Now;
        }

        public void AddItem(FoodItem item, int quantity)
        {
            if (OrderedItems.ContainsKey(item))
            {
                OrderedItems[item] += quantity;
            }
            else
            {
                OrderedItems.Add(item, quantity);
            }
            CalculateTotal();
        }

        public void CalculateTotal()
        {
            double itemsTotal = 0;
            foreach (var item in OrderedItems)
            {
                itemsTotal += item.Key.Price * item.Value;
            }
            TotalAmount = itemsTotal + 5.00;
        }

        public void DisplayOrderDetails()
        {
            Console.WriteLine($"Customer: {Customer.Name}");
            Console.WriteLine("Ordered Items:");
            int index = 1;
            foreach (var item in OrderedItems)
            {
                Console.WriteLine($"{index}. {item.Key.ItemName} - {item.Value}");
                index++;
            }
            if (!string.IsNullOrEmpty(SpecialRequest))
            {
                Console.WriteLine($"Special Request: {SpecialRequest}");
            }
            Console.WriteLine($"Delivery date/time: {DeliveryDateTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Total Amount: ${TotalAmount:F2}");
            Console.WriteLine($"Order Status: {Status}");
        }

        public string ToCSV()
        {
            string itemsStr = string.Join(";", OrderedItems.Select(kv =>$"{kv.Key.ItemName}:{kv.Value}"));
            return $"{OrderId},{Customer.Email},{Restaurant.RestaurantId}," +
                   $"{DeliveryDateTime:dd/MM/yyyy HH:mm},{DeliveryAddress}," +
                   $"{itemsStr},{SpecialRequest},{TotalAmount:F2},{PaymentMethod},{Status}";
        }
    }
}
