using System;

namespace eShopping.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string IdNumber { get; set; }      // CMND / Passport
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }          // có thể rỗng
    }

    /// <summary>Người nhận hàng - có thể khác người mua.</summary>
    public class Receiver
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}