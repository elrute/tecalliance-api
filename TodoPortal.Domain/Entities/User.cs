using System;
using System.Collections.Generic;
using System.Text;
using TodoPortal.Domain.ValueObjects;

namespace TodoPortal.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Username { get; set; } = "";
        public Email Email { get; set; }
        public Address Address { get; set; } = new();
        public string Phone { get; set; } = "";
        public string Website { get; set; } = "";
        public Company Company { get; set; } = new();
    }

}
