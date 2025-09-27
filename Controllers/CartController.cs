using BBEcom.Common;
using BBEcom.Models;
using Microsoft.Ajax.Utilities;
using BBEcom.Models.DAO;
using BBEcom.Models.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BBEcom.Controllers
{
    public class CartController : MemberBaseController
    {
        private const string CartSession = "CART_SESSION";
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["CART_SESSION"];
            var list = new List<Cart>();
            if (cart != null)
            {
                list = (List<Cart>)cart;
            }
            return View(list);
        }

        public ActionResult AddItem(int productID, int quantity)
        {
            var product = new ProductDAO().GetProductById(productID);
            var cart = Session["CART_SESSION"];
            if (cart != null)
            {
                var list = (List<Cart>)cart;
                if (list.Exists(x => x.product.Id == productID))
                {
                    foreach (var item in list)
                    {
                        if (item.product.Id == productID)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    var item = new Cart();
                    item.product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                Session["CART_SESSION"] = list;

            }
            else
            {
                var item = new Cart();
                item.product = product;
                item.Quantity = quantity;

                var list = new List<Cart>();
                list.Add(item);
                Session["CART_SESSION"] = list;
            }
            return RedirectToAction("Index");
        }

        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<Cart>>(cartModel);
            var sesCart = (List<Cart>)Session["CART_SESSION"];
            foreach (var item in sesCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.product.Id == item.product.Id);
                if (jsonItem != null)
                {
                    // jsonItem.Quantity: input quantity

                    bool dem = new GiftcardDAO().CheckStock(jsonItem.product.Id, jsonItem.Quantity);
                    if (dem == true)
                    {
                        item.Quantity = jsonItem.Quantity;
                    }
                    else
                    {
                        return Json(new { status = false });
                    }

                }
            }
            Session["CART_SESSION"] = sesCart;

            return Json(new { status = true });

        }

        public JsonResult DeleteAllCart()
        {
            Session["CART_SESSION"] = null;
            return Json(new { status = true });
        }

        public JsonResult Delete(long id)
        {
            var sesCart = (List<Cart>)Session["CART_SESSION"];
            sesCart.RemoveAll(x => x.product.Id == id);
            Session["CART_SESSION"] = sesCart;
            return Json(new { status = true });
        }

        [HttpGet]
        public ActionResult Payment()
        {
            var cart = Session["CART_SESSION"];
            var list = new List<Cart>();
            if (cart != null)
            {
                list = (List<Cart>)cart;
            }
            return View(list);
        }

		[HttpPost]
		public ActionResult Payment(Order _order)
		{
			int price = 0;
			var cart = Session["CART_SESSION"];
			var list = new List<Cart>();
			if (cart != null)
			{
				list = (List<Cart>)cart;

				foreach (var item in list)
				{
					int money = item.product.PriceSale > 0 ? item.product.PriceSale.GetValueOrDefault(0) : item.product.Price;
					price += money * item.Quantity;
				}

				// Tạo Order
				Random r = new Random();
				var orderCode = "DH" + r.Next(0, 99) + r.Next(0, 99) + r.Next(0, 99);

				var dao = new OrderDAO();
				var sess = (UserLogin)Session[BBEcom.Common.CommonConstants.USER_SESSION];
				var customerID = sess.UserId;

				Order order = new Order();
				order.OrderCode = orderCode;
				order.CustomerID = customerID;
				order.TotalAmount = price;
				order.Status = true; // đã thanh toán tiền mặt
				order.CreatedDate = DateTime.Now;

				foreach (var item in list)
				{
					for (int i = 0; i < item.Quantity; i++)
					{
						var newOrderDetail = new OrderDetail
						{
							ProductID = item.product.Id,
							Price = item.product.PriceSale > 0 ? item.product.PriceSale.GetValueOrDefault(0) : item.product.Price,
							Quantity = item.Quantity,
							//Code = new GiftcardDAO().TakeAndDelete(item.product.Id).Code
							Code = "as"
						};
						order.OrderDetail.Add(newOrderDetail);
					}
				}

				dao.AddOrder(order);

				Session["CART_SESSION"] = null;
			}

			return RedirectPermanent("/");
		}
	}
}