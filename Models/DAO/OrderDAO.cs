using BBEcom.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace BBEcom.Models.DAO
{
    public class OrderDAO
    {
        private KeyDbContext _context;

        public OrderDAO()
        {
            _context = new KeyDbContext();
        }

        public Order GetOrder(string orderCode)
        {
            return _context.Orders.FirstOrDefault(x => x.OrderCode == orderCode);
        }

        public int AddOrder(Order order)
        {
            try
            {
                var od = _context.Orders.Add(order);
                _context.SaveChanges();
                return od.Id;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                    foreach (var ve in eve.ValidationErrors)
                        Console.WriteLine($"{ve.PropertyName}: {ve.ErrorMessage}");
                throw;
            }
        }

        public int AddOrderDetail(OrderDetail orderDetail)
        {
            var odd = _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
            return odd.Id;
        }

        public IEnumerable<Order> GetListOrderByCustomerID(int cusID)
        {
            return _context.Orders.Where(x => x.CustomerID == cusID).OrderByDescending(x => x.CreatedDate).ToList();
        }
    }
}
