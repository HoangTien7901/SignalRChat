using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static SignalRChat.Models.Helper;

namespace SignalRChat.Models
{
    public class Chat
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        [Required]
        public ChatType Type { get; set; }

        public ICollection<ChatUsers> ChatUsers { get; set; }
        public ICollection<Message> Messages { get; set; }

    }
}
