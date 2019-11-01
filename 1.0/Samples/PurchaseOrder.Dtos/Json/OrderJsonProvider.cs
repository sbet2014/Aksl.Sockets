using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PurchaseOrder.Dtos
{
    public static class OrderJsonProvider
    {
        private static Random _random;
        private static List<int> _orderiIds;
        private static List<int> _productIds;

        private static string[] ProductePrix = { "Blue", "Red", "Green", "White", "Orange" };

        private static readonly string[] OrderStates = { "Pending", "Processed", "Shipped" };

        private static readonly string[] Customers = { "Alice", "Bill", "Cortana", "Smith", "Jack" };

        static OrderJsonProvider()
        {
            // var seed = DateTime.UtcNow.Millisecond;
            var seed = 0;

            //1.2亿
            _orderiIds = (from i in Enumerable.Range(seed + 1, seed +  2 * 50000000+ 2 * 50000000 + 2 * 50000000 )
                          select i).ToList();

            _productIds = (from i in Enumerable.Range(seed + 1000, seed + 2000)
                           select i).ToList();

            _random = new Random(seed);
        }

        public static Task InitializeTask()
        {
            return Task.CompletedTask;
        }

        public static byte[] CreateOrder() => OnCreateOrder();

        public static byte[] CreateOrder(int index) => OnCreateOrder(index);

        public static IEnumerable<byte[]> CreateOrders(int start = 0, int count = 10) => OnCreateOrders(start, count);

        private static byte[] OnCreateOrder()
        {
            var po = new PurchaseOrderDto()
            {
                PoNumber = _orderiIds[_random.Next(0, _orderiIds.Count - 1)],
              // CreatedDate = DateTime.Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Status = OrderStates[_random.Next(0, 2)],
                CustomerId = Customers[_random.Next(0, 4)],
                OrderLineItems = new List<PurchaseOrderLineItemDto>()
                {
                    //new PurchaseOrderLineItem() { ProductId= "Blue Widget", Quantity=54* _random.Next(1,100), UnitCost=new decimal(29.99F)},
                    //new PurchaseOrderLineItem() { ProductId= "Red Widget", Quantity=980* _random.Next(1,1000), UnitCost= new decimal(45.89F) }
                    new PurchaseOrderLineItemDto() { ProductId= $"Blue Widget {_productIds[_random.Next(0,_productIds.Count - 1)]}", Quantity=54* _random.Next(1,100), UnitCost=(decimal)(29.99F *_random.Next(1000,5000)/100)},
                    new PurchaseOrderLineItemDto() { ProductId= $"Red Widget {_productIds[_random.Next(0,_productIds.Count - 1)]}", Quantity=980* _random.Next(1,1000), UnitCost= (decimal)(45.89F *_random.Next(1000,2000)/100) }
                }.ToArray()
            };

            var msg = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(po));

            return msg;
        }

        private static byte[] OnCreateOrder(int index)
        {
            int maxPoNumber = Math.Max(index, _orderiIds.Count - 1);
            var po = new PurchaseOrderDto()
            {
                PoNumber = _orderiIds[_random.Next(0, maxPoNumber)],
                // CreatedDate = DateTime.Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Status = OrderStates[_random.Next(0, 2)],
                CustomerId = Customers[_random.Next(0, 4)],
                OrderLineItems = new List<PurchaseOrderLineItemDto>()
                {
                     new PurchaseOrderLineItemDto() { ProductId= $"Blue Widget {_productIds[_random.Next(0,_productIds.Count - 1)]}", Quantity=54* _random.Next(1,100), UnitCost=(decimal)(29.99F *_random.Next(1000,5000)/100)},
                    new PurchaseOrderLineItemDto() { ProductId= $"Red Widget {_productIds[_random.Next(0,_productIds.Count - 1)]}", Quantity=980* _random.Next(1,1000), UnitCost= (decimal)(45.89F *_random.Next(1000,2000)/100) }
                }.ToArray()
            };
            var msg = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(po));
            return msg;
        }

        private static IEnumerable<byte[]> OnCreateOrders(int start, int count)
        {
            var messages = new List<byte[]>();
            for (int i = start; i < start + count; i++)
            {
                var po = new PurchaseOrderDto()
                {
                    // PoNumber = _orderiIds[_random.Next(0, _orderiIds.Count - 1)],
                    PoNumber = _orderiIds[i],
                    Status = OrderStates[_random.Next(0, 2)],
                   // CreatedDate = DateTime.Now,
                    CustomerId = Customers[_random.Next(0, 4)],
                    ThreadId = Thread.CurrentThread.ManagedThreadId,
                    OrderLineItems = new List<PurchaseOrderLineItemDto>()
                    {
                        new PurchaseOrderLineItemDto() { ProductId= $"{ProductePrix[_random.Next(0, 4)]} Widget {_productIds[_random.Next(0,_productIds.Count - 1)]}", Quantity=54*_random.Next(1,10), UnitCost=10 *_random.Next(10,500)/100},
                        new PurchaseOrderLineItemDto() { ProductId= $"{ProductePrix[_random.Next(0, 4)]} Widget {_productIds[_random.Next(0,_productIds.Count - 1)]}", Quantity=90*_random.Next(1,100), UnitCost= 20 *_random.Next(100,2000)/100 }
                    }.ToArray()
                };

                var orderString = JsonConvert.SerializeObject(po);
                var orderBytes = Encoding.UTF8.GetBytes(orderString);

                var length = Encoding.UTF8.GetByteCount(orderString);

                var orderUTF8Bytes = new byte[length + 1];
                Encoding.UTF8.GetBytes(orderString, 0, orderString.Length, orderUTF8Bytes, 0);
                orderUTF8Bytes[orderUTF8Bytes.Length - 1] = (byte)'\n';

                //Array.Copy(orderByte, destOrderByte, orderByte.Length);
                //destOrderByte[orderByte.Length] = (byte)'\n';

                messages.Add(orderUTF8Bytes);
            }
            return messages;
        }
    }
}
