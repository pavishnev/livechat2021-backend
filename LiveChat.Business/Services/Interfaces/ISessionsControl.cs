using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Services.Interfaces
{
   public interface ISessionsControl
    {
      Task<bool> Run();
    }
}
