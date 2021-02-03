using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeavyTelerikChat.Models {
    
    public class SignInDemo {

        [Required]
        public string Username { get; set; }

        public string Path { get; set; }
    }
}