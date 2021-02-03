using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeavyTelerikChat.Models {
    
    public class Conversation {

        public IEnumerable<ConversationMember> Members { get; }

        public Message LastMessage { get; }

        public bool IsRead { get; }

        public DateTime? ReadAt { get; }

        public string Avatar { get; set; }

        public bool IsRoom { get; }

        public string Name { get; set; }

        public int Id { get; }
    }

    public class ConversationMember {

        public int Id { get; }

        public DateTime? DeliveredAt { get; }

        public DateTime? ReadAt { get; }

        public string Avatar { get; set; }

        public string Name { get; }

        public string Email { get; set; }

        public string Username { get; set; }       
    }

    public class Message {

        public int Id { get; }

        public int CreatedById { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string Html { get; set; }
        
        public string Text { get; set; }
        
    }
}