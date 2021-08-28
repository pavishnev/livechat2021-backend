using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Models
{
   public class ClientModel
    {
        public string Name { get; set; }
        public string WebsiteId { get; set; }
        public string Id { get; set; }//зачем в этой модели айди? 
    }
}
