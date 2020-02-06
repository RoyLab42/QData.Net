using System;

namespace RoyLab.QData
{
    public class User
    {
        public int Age { get; set; }
        public DateTime BirthDay { get; set; }
        public Guid ID { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public DateTime? ParentalDay { get; set; }
        public Guid UserID { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
    }
}
