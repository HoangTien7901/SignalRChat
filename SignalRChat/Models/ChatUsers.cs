using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Models
{
    public class ChatUsers
    {
        [Key]
        public int Id { get; set; }
        public string UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public ApplicationUser User { get; set; }
        public int ChatID { get; set; }
        [ForeignKey(nameof(ChatID))]
        public Chat Chat { get; set; }
    }
}
