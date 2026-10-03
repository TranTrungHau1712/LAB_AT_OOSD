using System;
using System.Data.SqlClient;
using eShopping.Models;

namespace eShopping.Data
{
    public class OrderDao
    {
        /// <summary>Ghi đơn hàng + chi tiết trong 1 transaction, trả về OrderId.</summary>
        public int Insert(Order o)
        {
            using (var c = Db.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    const string sqlOrder =
                        @"INSERT dbo.Orders(CustomerId,OrderDate,OrderType,ReceiverName,ReceiverAddress,ReceiverPhone,
                                            ZoneId,CardType,CardLast4,CardHolder,AuthCode,
                                            ItemsTotal,ShippingFee,CardFee,TotalAmount,Status)
                          VALUES(@cu,@dt,@ty,@rn,@ra,@rp,@z,@ct,@l4,@ch,@au,@it,@sf,@cf,@tt,@st);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    int id;
                    using (var cmd = new SqlCommand(sqlOrder, c, tx))
                    {
                        cmd.Parameters.Add(Db.P("@cu", o.CustomerId));
                        cmd.Parameters.Add(Db.P("@dt", o.OrderDate));
                        cmd.Parameters.Add(Db.P("@ty", (byte)o.OrderType));
                        cmd.Parameters.Add(Db.P("@rn", o.Receiver.FullName));
                        cmd.Parameters.Add(Db.P("@ra", o.Receiver.Address));
                        cmd.Parameters.Add(Db.P("@rp", o.Receiver.Phone));
                        cmd.Parameters.Add(Db.P("@z", o.ZoneId));
                        cmd.Parameters.Add(Db.P("@ct", (byte)o.CardType));
                        cmd.Parameters.Add(Db.P("@l4", o.CardLast4));
                        cmd.Parameters.Add(Db.P("@ch", o.CardHolder));
                        cmd.Parameters.Add(Db.P("@au", o.AuthCode));
                        cmd.Parameters.Add(Db.P("@it", o.ItemsTotal));
                        cmd.Parameters.Add(Db.P("@sf", o.ShippingFee));
                        cmd.Parameters.Add(Db.P("@cf", o.CardFee));
                        cmd.Parameters.Add(Db.P("@tt", o.Total));
                        cmd.Parameters.Add(Db.P("@st", (byte)o.Status));
                        id = (int)cmd.ExecuteScalar();
                    }

                    foreach (var it in o.Items)
                    {
                        using (var cmd = new SqlCommand(
                            @"INSERT dbo.OrderItem(OrderId,ProductId,ProductName,Quantity,UnitPrice)
                              VALUES(@o,@p,@n,@q,@u)", c, tx))
                        {
                            cmd.Parameters.Add(Db.P("@o", id));
                            cmd.Parameters.Add(Db.P("@p", it.ProductId));
                            cmd.Parameters.Add(Db.P("@n", it.ProductName));
                            cmd.Parameters.Add(Db.P("@q", it.Quantity));
                            cmd.Parameters.Add(Db.P("@u", it.UnitPrice));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return id;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}