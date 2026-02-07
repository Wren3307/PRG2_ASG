using PRG2_ASG;
using System.Globalization;

//==========================================================
// Student Number : S10274614
// Student Name : Cai Renjie
// Partner Name : Jackie Ang
//==========================================================
class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static List<Customer> customers = new List<Customer>();
    static Stack<Order> refundStack = new Stack<Order>();
    static int nextOrderId = 1001;

    static void Main(string[] args)
    {
        //file load
        Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
        LoadRestaurants();
        LoadFoodItems();
        LoadCustomers();
        LoadOrders();

        //menu
        bool running = true;
        while (running)
        {
            DisplayMainMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ListAllRestaurantsAndMenuItems();
                    break;
                case "2":
                    ListAllOrders();
                    break;
                case "3":
                    CreateNewOrder();
                    break;
                case "4":
                    ProcessOrder();
                    break;
                case "5":
                    ModifyOrder();
                    break;
                case "6":
                    DeleteOrder();
                    break;
                case "0":
                    SaveQueueAndStack();
                    running = false;
                    Console.WriteLine("Exiting system. Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again."); //incase random entry
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    static void DisplayMainMenu()
    {
        Console.WriteLine("\n===== Gruberoo Food Delivery System =====");
        Console.WriteLine("1. List all restaurants and menu items");
        Console.WriteLine("2. List all orders");
        Console.WriteLine("3. Create a new order");
        Console.WriteLine("4. Process an order");
        Console.WriteLine("5. Modify an existing order");
        Console.WriteLine("6. Delete an existing order");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }

    // Feature 1: load restaurants and food items - By Cai Renjie
    static void LoadRestaurants()
    {
        if (!File.Exists("restaurants.csv"))
        {
            Console.WriteLine("0 restaurants loaded!");
            return;
        }

        string[] lines = File.ReadAllLines("restaurants.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(',');
            if (data.Length >= 3)
            {
                Restaurant restaurant = new Restaurant(data[0].Trim(), data[1].Trim(), data[2].Trim());
                restaurants.Add(restaurant);
                count++;
            }
        }

        Console.WriteLine($"{count} restaurants loaded!");
    }

    static void LoadFoodItems()
    {
        if (!File.Exists("fooditems.csv"))
        {
            Console.WriteLine("0 food items loaded!");
            return;
        }

        string[] lines = File.ReadAllLines("fooditems.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(',');
            if (data.Length >= 4)
            {
                string restaurantId = data[0].Trim();
                Restaurant restaurant = restaurants.FirstOrDefault(r => r.RestaurantId == restaurantId);

                if (restaurant != null)
                {
                    FoodItem item = new FoodItem(data[1].Trim(),
                                                 data[2].Trim(),
                                                 double.Parse(data[3].Trim()));
                    restaurant.AddFoodItem(item);
                    count++;
                }
            }
        }

        Console.WriteLine($"{count} food items loaded!");
    }

    // Feature 2: load the customers and orders - By Jackie
    static void LoadCustomers()
    {
        if (!File.Exists("customers.csv"))
        {
            Console.WriteLine("0 customers loaded!");
            return;
        }

        string[] lines = File.ReadAllLines("customers.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(',');
            if (data.Length >= 2)
            {
                Customer customer = new Customer(data[0].Trim(), data[1].Trim());
                customers.Add(customer);
                count++;
            }
        }

        Console.WriteLine($"{count} customers loaded!");
    }

    static void LoadOrders()
    {
        if (!File.Exists("orders.csv"))
        {
            Console.WriteLine("0 orders loaded!");
            return;
        }

        string[] lines = File.ReadAllLines("orders.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            //split by comma
            List<string> fields = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            for (int j = 0; j < lines[i].Length; j++)
            {
                char c = lines[i][j];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }
            fields.Add(currentField);

            if (fields.Count >= 10)
            {
                int orderId = int.Parse(fields[0].Trim());
                string customerEmail = fields[1].Trim();
                string restaurantId = fields[2].Trim();

                Customer customer = customers.FirstOrDefault(c => c.Email.Equals(customerEmail, StringComparison.OrdinalIgnoreCase));
                Restaurant restaurant = restaurants.FirstOrDefault(r => r.RestaurantId == restaurantId);

                if (customer != null && restaurant != null)
                {
                    //change delivery date and time separately to datetime data type
                    string deliveryDate = fields[3].Trim();
                    string deliveryTime = fields[4].Trim();
                    DateTime deliveryDateTime = DateTime.Parse($"{deliveryDate} {deliveryTime}");

                    string deliveryAddress = fields[5].Trim();

                    //create date/time
                    string createdDateTimeStr = fields[6].Trim();

                    Order order = new Order(orderId, customer, restaurant, deliveryDateTime, deliveryAddress);

                    order.TotalAmount = double.Parse(fields[7].Trim());

                    order.Status = fields[8].Trim();

                    // get the items entered (format: "Item1, Qty|Item2, Qty")
                    string itemsStr = fields[9].Trim();
                    string[] items = itemsStr.Split('|');

                    foreach (string item in items)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] itemData = item.Split(',');
                            if (itemData.Length == 2)
                            {
                                string itemName = itemData[0].Trim();
                                int quantity = int.Parse(itemData[1].Trim());
                                FoodItem foodItem = restaurant.GetFoodItem(itemName);
                                if (foodItem != null)
                                {
                                    //adding item without re-counting total
                                    if (order.OrderedItems.ContainsKey(foodItem))
                                    {
                                        order.OrderedItems[foodItem] += quantity;
                                    }
                                    else
                                    {
                                        order.OrderedItems.Add(foodItem, quantity);
                                    }
                                }
                            }
                        }
                    }

                    customer.AddOrder(order);
                    restaurant.AddOrder(order);

                    if (orderId >= nextOrderId)
                    {
                        nextOrderId = orderId + 1;
                    }

                    count++;
                }
            }
        }

        Console.WriteLine($"{count} orders loaded!");
    }

    // Feature 3: list all restaurants and menu items - By Jackie
    static void ListAllRestaurantsAndMenuItems()
    {
        Console.WriteLine("\nAll Restaurants and Menu Items");
        Console.WriteLine("==============================");

        foreach (Restaurant restaurant in restaurants)
        {
            restaurant.DisplayMenu();
            Console.WriteLine();
        }
    }

    // Feature 4: list all orders - By Renjie
    static void ListAllOrders()
    {
        Console.WriteLine("\nAll Orders");
        Console.WriteLine("==========");
        Console.WriteLine($"{"Order ID",-10} {"Customer",-15} {"Restaurant",-20} " +
                        $"{"Delivery Date/Time",-20} {"Amount",-10} {"Status",-12}");
        Console.WriteLine(new string('-', 95));

        foreach (Customer customer in customers)
        {
            foreach (Order order in customer.OrderList)
            {
                Console.WriteLine($"{order.OrderId,-10} {customer.Name,-15} " +
                                $"{order.Restaurant.Name,-20} " +
                                $"{order.DeliveryDateTime:dd/MM/yyyy HH:mm}     " +
                                $"${order.TotalAmount,-9:F2} {order.Status,-12}");
            }
        }
    }

    // Feature 5: create a new order - By Jackie
    static void CreateNewOrder()
    {
        Console.WriteLine("\nCreate New Order");
        Console.WriteLine("================");

        // ask for email
        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine().Trim();
        Customer customer = customers.FirstOrDefault(c => c.Email.Equals(customerEmail, StringComparison.OrdinalIgnoreCase));

        if (customer == null)
        {
            Console.WriteLine("Error: Customer not found.");
            return;
        }

        // get id
        Console.Write("Enter Restaurant ID: ");
        string restaurantId = Console.ReadLine().Trim();
        Restaurant restaurant = restaurants.FirstOrDefault(r => r.RestaurantId.Equals(restaurantId, StringComparison.OrdinalIgnoreCase));

        if (restaurant == null)
        {
            Console.WriteLine("Error: Restaurant not found.");
            return;
        }

        if (restaurant.MenuItems.Count == 0)
        {
            Console.WriteLine("Error: Restaurant has no menu items.");
            return;
        }

        // get date time then parse and store
        DateTime deliveryDateTime;
        while (true)
        {
            Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
            string dateStr = Console.ReadLine().Trim();
            Console.Write("Enter Delivery Time (hh:mm): ");
            string timeStr = Console.ReadLine().Trim();

            if (DateTime.TryParse($"{dateStr} {timeStr}", out deliveryDateTime))
            {
                if (deliveryDateTime < DateTime.Now)
                {
                    Console.WriteLine("Error: Delivery date/time must be in the future.");
                    continue;
                }
                break;
            }
            else
            {
                Console.WriteLine("Error: Invalid date/time format. Please try again.");
            }
        }

        // addr
        Console.Write("Enter Delivery Address: ");
        string deliveryAddress = Console.ReadLine().Trim();

        if (string.IsNullOrEmpty(deliveryAddress))
        {
            Console.WriteLine("Error: Delivery address cannot be empty.");
            return;
        }

        Order order = new Order(nextOrderId, customer, restaurant, deliveryDateTime, deliveryAddress);

        // displaying
        Console.WriteLine("\nAvailable Food Items:");
        for (int i = 0; i < restaurant.MenuItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {restaurant.MenuItems[i].ItemName} - " +
                            $"${restaurant.MenuItems[i].Price:F2}");
        }

        // selecting
        while (true)
        {
            Console.Write("Enter item number (0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int itemNum))
            {
                Console.WriteLine("Error: Invalid input.");
                continue;
            }

            if (itemNum == 0)
                break;

            if (itemNum < 1 || itemNum > restaurant.MenuItems.Count)
            {
                Console.WriteLine("Error: Invalid item number.");
                continue;
            }

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 1)
            {
                Console.WriteLine("Error: Invalid quantity.");
                continue;
            }

            order.AddItem(restaurant.MenuItems[itemNum - 1], quantity);
        }

        if (order.OrderedItems.Count == 0)
        {
            Console.WriteLine("Error: Order must contain at least one item.");
            return;
        }

        // special req
        Console.Write("Add special request? [Y/N]: ");
        string response = Console.ReadLine().Trim().ToUpper();
        if (response == "Y")
        {
            Console.Write("Enter special request: ");
            order.SpecialRequest = Console.ReadLine().Trim();
        }

        double itemsTotal = order.TotalAmount - 5.00;
        Console.WriteLine($"\nOrder Total: ${itemsTotal:F2} + $5.00 (delivery) = " +
                        $"${order.TotalAmount:F2}");

        // payment
        Console.Write("Proceed to payment? [Y/N]: ");
        response = Console.ReadLine().Trim().ToUpper();
        if (response != "Y")
        {
            Console.WriteLine("Order cancelled.");
            return;
        }

        while (true)
        {
            Console.Write("Payment method:\n[CC] Credit Card / [PP] PayPal / " +
                        "[CD] Cash on Delivery: ");
            string payment = Console.ReadLine().Trim().ToUpper();

            if (payment == "CC" || payment == "PP" || payment == "CD")
            {
                order.PaymentMethod = payment;
                break;
            }
            Console.WriteLine("Error: Invalid payment method.");
        }

        // add orders
        customer.AddOrder(order);
        restaurant.AddOrder(order);

        // save to file
        using (StreamWriter sw = File.AppendText("orders.csv"))
        {
            sw.WriteLine(order.ToCSV());
        }

        Console.WriteLine($"\nOrder {order.OrderId} created successfully! Status: Pending");
        nextOrderId++;
    }

    // Feature 6: process an order - By Renjie
    static void ProcessOrder()
    {
        Console.WriteLine("\nProcess Order");
        Console.WriteLine("=============");

        Console.Write("Enter Restaurant ID: ");
        string restaurantId = Console.ReadLine().Trim();
        Restaurant restaurant = restaurants.FirstOrDefault(r => r.RestaurantId.Equals(restaurantId, StringComparison.OrdinalIgnoreCase));

        if (restaurant == null)
        {
            Console.WriteLine("Error: Restaurant not found.");
            return;
        }

        if (restaurant.OrderQueue.Count == 0)
        {
            Console.WriteLine("No orders to process.");
            return;
        }

        Queue<Order> tempQueue = new Queue<Order>();

        while (restaurant.OrderQueue.Count > 0)
        {
            Order order = restaurant.OrderQueue.Dequeue();

            Console.WriteLine($"\nOrder {order.OrderId}:");
            order.DisplayOrderDetails();

            Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
            string action = Console.ReadLine().Trim().ToUpper();

            switch (action)
            {
                case "C":
                    if (order.Status == "Pending")
                    {
                        order.Status = "Preparing";
                        Console.WriteLine($"Order {order.OrderId} confirmed. " +
                                        $"Status: Preparing");
                    }
                    else
                    {
                        Console.WriteLine($"Error: Can only confirm pending orders. " +
                                        $"Current status: {order.Status}");
                    }
                    tempQueue.Enqueue(order);
                    break;

                case "R":
                    if (order.Status == "Pending")
                    {
                        order.Status = "Rejected";
                        refundStack.Push(order);
                        Console.WriteLine($"Order {order.OrderId} rejected. " +
                                        $"Refund of ${order.TotalAmount:F2} processed.");
                    }
                    else
                    {
                        Console.WriteLine($"Error: Can only reject pending orders. " +
                                        $"Current status: {order.Status}");
                        tempQueue.Enqueue(order);
                    }
                    break;

                case "S":
                    if (order.Status == "Cancelled")
                    {
                        Console.WriteLine($"Order {order.OrderId} skipped.");
                    }
                    else
                    {
                        Console.WriteLine($"Error: Can only skip cancelled orders. " +
                                        $"Current status: {order.Status}");
                    }
                    tempQueue.Enqueue(order);
                    break;

                case "D":
                    if (order.Status == "Preparing")
                    {
                        order.Status = "Delivered";
                        Console.WriteLine($"Order {order.OrderId} delivered. " +
                                        $"Status: Delivered");
                    }
                    else
                    {
                        Console.WriteLine($"Error: Can only deliver preparing orders. " +
                                        $"Current status: {order.Status}");
                        tempQueue.Enqueue(order);
                    }
                    break;

                default:
                    Console.WriteLine("Invalid action. Order returned to queue.");
                    tempQueue.Enqueue(order);
                    break;
            }
        }

        while (tempQueue.Count > 0)
        {
            restaurant.OrderQueue.Enqueue(tempQueue.Dequeue());
        }
    }

    // Feature 7: modifying an existing order - By Jackie
    static void ModifyOrder()
    {
        Console.WriteLine("\nModify Order");
        Console.WriteLine("============");

        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine().Trim();
        Customer customer = customers.FirstOrDefault(c => c.Email.Equals(customerEmail, StringComparison.OrdinalIgnoreCase));

        if (customer == null)
        {
            Console.WriteLine("Error: Customer not found.");
            return;
        }

        List<Order> pendingOrders = customer.GetPendingOrders();
        if (pendingOrders.Count == 0)
        {
            Console.WriteLine("No pending orders found.");
            return;
        }

        Console.WriteLine("Pending Orders:");
        foreach (Order order in pendingOrders)
        {
            Console.WriteLine(order.OrderId);
        }

        Console.Write("Enter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Error: Invalid Order ID.");
            return;
        }

        Order selectedOrder = customer.GetOrderById(orderId);
        if (selectedOrder == null || selectedOrder.Status != "Pending")
        {
            Console.WriteLine("Error: Order not found or not pending.");
            return;
        }

        // display order details
        Console.WriteLine("\nOrder Items:");
        int index = 1;
        foreach (var item in selectedOrder.OrderedItems)
        {
            Console.WriteLine($"{index}. {item.Key.ItemName} - {item.Value}");
            index++;
        }
        Console.WriteLine($"\nAddress:\n{selectedOrder.DeliveryAddress}");
        Console.WriteLine($"\nDelivery Date/Time:\n" +
                        $"{selectedOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");

        Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time: ");
        string choice = Console.ReadLine().Trim();

        switch (choice)
        {
            case "1":
                // code below will modify the items
                Console.WriteLine("\nCurrent items will be replaced with new selection.");
                selectedOrder.OrderedItems.Clear();

                // display
                Console.WriteLine("\nAvailable Food Items:");
                for (int i = 0; i < selectedOrder.Restaurant.MenuItems.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {selectedOrder.Restaurant.MenuItems[i].ItemName} - " +
                                    $"${selectedOrder.Restaurant.MenuItems[i].Price:F2}");
                }

                // select
                while (true)
                {
                    Console.Write("Enter item number (0 to finish): ");
                    if (!int.TryParse(Console.ReadLine(), out int itemNum))
                    {
                        Console.WriteLine("Error: Invalid input.");
                        continue;
                    }

                    if (itemNum == 0)
                        break;

                    if (itemNum < 1 || itemNum > selectedOrder.Restaurant.MenuItems.Count)
                    {
                        Console.WriteLine("Error: Invalid item number.");
                        continue;
                    }

                    Console.Write("Enter quantity: ");
                    if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 1)
                    {
                        Console.WriteLine("Error: Invalid quantity.");
                        continue;
                    }

                    FoodItem item = selectedOrder.Restaurant.MenuItems[itemNum - 1];
                    if (selectedOrder.OrderedItems.ContainsKey(item))
                    {
                        selectedOrder.OrderedItems[item] += quantity;
                    }
                    else
                    {
                        selectedOrder.OrderedItems.Add(item, quantity);
                    }
                }

                if (selectedOrder.OrderedItems.Count == 0)
                {
                    Console.WriteLine("Error: Order must contain at least one item.");
                    break;
                }

                double oldTotal = selectedOrder.TotalAmount;
                selectedOrder.CalculateTotal();
                double newTotal = selectedOrder.TotalAmount;

                Console.WriteLine($"Order {orderId} updated.");
                Console.WriteLine($"Old Total: ${oldTotal:F2}");
                Console.WriteLine($"New Total: ${newTotal:F2}");

                // check if total increased to see if payment needed
                if (newTotal > oldTotal)
                {
                    Console.WriteLine($"Additional payment required: ${(newTotal - oldTotal):F2}");
                    Console.Write("Proceed to payment? [Y/N]: ");
                    string response = Console.ReadLine().Trim().ToUpper();
                    if (response != "Y")
                    {
                        Console.WriteLine("Order modification cancelled.");
                    }
                    else
                    {
                        Console.WriteLine("Payment processed successfully.");
                    }
                }
                else if (newTotal < oldTotal)
                {
                    Console.WriteLine($"Refund amount: ${(oldTotal - newTotal):F2}");
                }
                break;

            case "2":
                Console.Write("Enter new Delivery Address: ");
                string newAddress = Console.ReadLine().Trim();
                if (!string.IsNullOrEmpty(newAddress))
                {
                    selectedOrder.DeliveryAddress = newAddress;
                    Console.WriteLine($"Order {orderId} updated. " + $"New Address: {newAddress}");
                }
                break;

            case "3":
                Console.Write("Enter new Delivery Time (hh:mm): ");
                string timeStr = Console.ReadLine().Trim();
                string dateStr = selectedOrder.DeliveryDateTime.ToString("dd/MM/yyyy");
                if (DateTime.TryParse($"{dateStr} {timeStr}", out DateTime newTime))
                {
                    selectedOrder.DeliveryDateTime = newTime;
                    Console.WriteLine($"Order {orderId} updated. " + $"New Delivery Time: {timeStr}");
                }
                else
                {
                    Console.WriteLine("Error: Invalid time format.");
                }
                break;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    // Feature 8: delete an existing order - By Renjie
    static void DeleteOrder()
    {
        Console.WriteLine("\nDelete Order");
        Console.WriteLine("============");

        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine().Trim();
        Customer customer = customers.FirstOrDefault(c => c.Email.Equals(customerEmail, StringComparison.OrdinalIgnoreCase));

        if (customer == null)
        {
            Console.WriteLine("Error: Customer not found.");
            return;
        }

        List<Order> pendingOrders = customer.GetPendingOrders();
        if (pendingOrders.Count == 0)
        {
            Console.WriteLine("No pending orders found.");
            return;
        }

        Console.WriteLine("Pending Orders:");
        foreach (Order order in pendingOrders)
        {
            Console.WriteLine(order.OrderId);
        }

        Console.Write("Enter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Error: Invalid Order ID.");
            return;
        }

        Order selectedOrder = customer.GetOrderById(orderId);
        if (selectedOrder == null || selectedOrder.Status != "Pending")
        {
            Console.WriteLine("Error: Order not found or not pending.");
            return;
        }

        // display order details
        selectedOrder.DisplayOrderDetails();

        Console.Write("\nConfirm deletion? [Y/N]: ");
        string response = Console.ReadLine().Trim().ToUpper();

        if (response == "Y")
        {
            selectedOrder.Status = "Cancelled";
            refundStack.Push(selectedOrder);
            Console.WriteLine($"Order {orderId} cancelled. " + $"Refund of ${selectedOrder.TotalAmount:F2} processed.");
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }
    }

    static void SaveQueueAndStack()
    {
        // saves queue
        using (StreamWriter sw = new StreamWriter("queue.csv"))
        {
            sw.WriteLine("OrderId,CustomerEmail,RestaurantId,Status");
            foreach (Restaurant restaurant in restaurants)
            {
                foreach (Order order in restaurant.OrderQueue)
                {
                    sw.WriteLine($"{order.OrderId},{order.Customer.Email}," +
                               $"{order.Restaurant.RestaurantId},{order.Status}");
                }
            }
        }

        using (StreamWriter sw = new StreamWriter("stack.csv"))
        {
            sw.WriteLine("OrderId,CustomerEmail,RestaurantId,Status,RefundAmount");
            foreach (Order order in refundStack)
            {
                sw.WriteLine($"{order.OrderId},{order.Customer.Email}," +
                           $"{order.Restaurant.RestaurantId},{order.Status}," +
                           $"{order.TotalAmount:F2}");
            }
        }

        Console.WriteLine("Queue and stack data saved successfully.");
    }
}