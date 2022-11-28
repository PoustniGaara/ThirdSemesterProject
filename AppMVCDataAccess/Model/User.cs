﻿using System;
namespace DataAccessLayer.Model
{
    public class User
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }    
        public int PhoneNumber { get; set; }
        public string Address { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public IEnumerable<Order> Orders { get; set; }


        public User()
        {

        }
        public User(string email, string name, string surename, int phoneNumber, string address, string username, string password, bool isAdmin)
        {
            Email = email;
            Name = name;
            Surename = surename;
            PhoneNumber = phoneNumber;
            Address = address;
            Username = username;
            Password = password;
            IsAdmin = isAdmin;
        }   
    }
}

