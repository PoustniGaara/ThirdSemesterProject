﻿using WebApiClient.DTOs;

namespace WebApi.DTOs
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public IEnumerable<OrderDto> Orders { get; set; }
    }
}
