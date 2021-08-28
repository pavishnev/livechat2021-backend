using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Models.RESTRequests
{
    public class CompleteRegisterAgent
    {
        [Required]
        public Guid invitationCode { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
