using System;

namespace lektion.Models
{
    public class User
    {
        public int user_id { get; set; }
        public string username { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool is_active { get; set; } = true;
        public DateTime created_at { get; set; } = DateTime.Now;
        public string department { get; set; } = string.Empty;
    }
}