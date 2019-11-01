using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace PurchaseOrder.Dtos
{
    public static class OrderJsonSerializer
    {
        public static PurchaseOrderDto DeserializeOrder(byte[] orderBytes)
                                                  => OnDeserializeOrder(orderBytes);

        public static byte[] SerializeOrder(PurchaseOrderDto purchaseOrder)
                                                   => OnSerializeOrder(purchaseOrder);

        private static PurchaseOrderDto OnDeserializeOrder(byte[] message)
        {
            var orderString = Encoding.UTF8.GetString(message);
            var order = JsonConvert.DeserializeObject<PurchaseOrderDto>(orderString);
            return order;
        }

        public static IEnumerable<PurchaseOrderDto> DeserializeOrders(IEnumerable<byte[]> messages)
        {
            var orders = new List<PurchaseOrderDto>();
            string orderString = default;
            PurchaseOrderDto order = default;
            byte[] orderByte = default;

            foreach (var ob in messages)
            {
                try
                {
                    orderByte = ob;
                    orderString = Encoding.UTF8.GetString(ob);
                    order = JsonConvert.DeserializeObject<PurchaseOrderDto>(orderString);
                    //var po = OnDeserializeOrder(ob);
                    orders.Add(order);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while process a order: {ex.Message}");
                }
            }
            return orders;
        }

        private static byte[] OnSerializeOrder(PurchaseOrderDto purchaseOrder)
        {
            var po = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(purchaseOrder));

            return po;
        }
    }
}
