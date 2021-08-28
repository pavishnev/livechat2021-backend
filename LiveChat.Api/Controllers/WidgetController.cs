using LiveChat.Business.SignalR;
using LiveChat.Constants.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace LiveChat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetController : ControllerBase
    {
        
        public WidgetController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        [HttpGet("chat-script")]
        public IActionResult GetChatScript()
        { 
            return Content(Properties.Resources.Chat_Script, "text/javascript");
        }

        [HttpGet("chat-styles")]
        public IActionResult GetChatStyles()
        {
            return Content(Properties.Resources.Chat_Styles, "text/css");
        }
        [HttpGet("chat-widget")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetChatWidget()
        {
            var websiteId = User.FindAll("websiteId").Select(x => x.Value).FirstOrDefault();
          
            string hostUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
            
            string cssCode = $"<link rel=\"stylesheet\" href=\"{hostUrl}/api/widget/chat-styles\">";
          
            StringBuilder jsCode = new ();
            jsCode.Append($"<livechat2021-chat website-id=\"{websiteId}\"></livechat2021-chat>");
            jsCode.Append($"<script type = \"text/javascript\" src = \"{hostUrl}/api/widget/chat-script\"></script>");
            return Ok(new
            {
                cssCode = cssCode,
                jsCode = jsCode.ToString()
            });

        }
    }
}
