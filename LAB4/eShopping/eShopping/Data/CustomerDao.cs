using System;
using System.Data;
using eShopping.Models;

namespace eShopping.Data
{
    public class CustomerDao
    {
        public bool UsernameExists(string username)
            => (int)Db.Scalar("SELECT COUNT(*) FROM dbo.Customer WHERE Username=@u", Db.P("@u", username)) > 0;

        public bool IdNumberExists(string idNumber)
            => (int)Db.Scalar("SELECT COUNT(*) FROM dbo.Customer WHERE IdNumber=@i", Db.P("@i", idNumber)) > 0;

        public int Insert(Customer c)
        {
            const string sql =
                @"INSERT dbo.Customer(FullName,BirthDate,IdNumber,Address,Phone,Username,PasswordHash,Email)
                  VALUES(@fn,@bd,@id,@ad,@ph,@us,@pw,@em);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return (int)Db.Scalar(sql,
                Db.P("@fn", c.FullName), Db.P("@bd", c.BirthDate.Date), Db.P("@id", c.IdNumber),
                Db.P("@ad", c.Address), Db.P("@ph", c.Phone), Db.P("@us", c.Username),
                Db.P("@pw", c.PasswordHash),
                Db.P("@em", string.IsNullOrWhiteSpace(c.Email) ? null : c.Email.Trim()));
        }

        public Customer GetByUsername(string username)
        {
            var dt = Db.Query("SELECT * FROM dbo.Customer WHERE Username=@u", Db.P("@u", username));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new Customer
            {
                Id = (int)r["CustomerId"],
                FullName = (string)r["FullName"],
                BirthDate = (DateTime)r["BirthDate"],
                IdNumber = (string)r["IdNumber"],
                Address = (string)r["Address"],
                Phone = (string)r["Phone"],
                Username = (string)r["Username"],
                PasswordHash = (string)r["PasswordHash"],
                Email = r["Email"] as string
            };
        }
    }
}