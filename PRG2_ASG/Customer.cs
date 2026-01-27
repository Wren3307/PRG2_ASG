using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASG
{
    class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Order> OrderList { get; set; }

        public Customer(string name, string email)
        {
            Name = name;
            Email = email;
            OrderList = new List<Order>();
        }

        public void AddOrder(Order order)
        {
            OrderList.Add(order);
        }

        public List<Order> GetPendingOrders()
        {
            return OrderList.Where(o => o.Status == "Pending").ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return OrderList.FirstOrDefault(o => o.OrderId == orderId);
        }

        public override string ToString()
        {
            return $"{Name},{Email}";
        }
    }
}
