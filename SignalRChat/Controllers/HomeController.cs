using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SignalRChat.Db;
using SignalRChat.Hubs;
using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalRChat.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;
        private IHubContext<ChatHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext, IHubContext<ChatHub> hubContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _dataContext = dataContext;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateGroup()
        {
            var users = _dataContext.Users.Where(x => x.Id != HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(string Name, List<string> UsersIDs)
        {
            var chat = new Chat
            {
                Name = Name,
                Type = Helper.ChatType.Room
            };

            await _dataContext.AddAsync(chat);
            await _dataContext.SaveChangesAsync();
            foreach(var UserId in UsersIDs)
            {
                _dataContext.ChatUsers.Add(new ChatUsers { UserID = UserId, ChatID = chat.ID });
            }
            _dataContext.ChatUsers.Add(new ChatUsers { UserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, ChatID = chat.ID });
            await _dataContext.SaveChangesAsync();
            await _hubContext.Clients.Users(UsersIDs).SendAsync("ReceiveMessage", Name, this.User.Identity.Name + "Added you to group " + Name, "AddGroup", chat.ID);

            return RedirectToAction(nameof(Chat), new { ChatID = chat.ID });
        }

        [HttpGet]
        public ActionResult CreatePrivate()
        {
            var users = _dataContext.Users.Where(x => x.Id != HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrivate(string UserID)
        {
            var chat = new Chat
            {
                Type = Helper.ChatType.Private
            };

            await _dataContext.AddAsync(chat);
            await _dataContext.SaveChangesAsync();
            _dataContext.ChatUsers.Add(new ChatUsers { UserID = UserID, ChatID = chat.ID });
            _dataContext.ChatUsers.Add(new ChatUsers { UserID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, ChatID = chat.ID });
            await _dataContext.SaveChangesAsync();
            await _hubContext.Clients.User(UserID).SendAsync(
                "ReceiveMessage", 
                this.User.Identity.Name, 
                this.User.Identity.Name + "Added you to Private Chat", 
                "AddPrivate", 
                chat.ID);

            return RedirectToAction(nameof(Chat), new { ChatID = chat.ID });
        }

        public IActionResult Chat(int ChatID)
        {
            var chat = _dataContext.Chats
                .Include(x => x.Messages)
                .Include(x => x.ChatUsers).FirstOrDefault(x => x.ID == ChatID);
            if (chat.ChatUsers.Any(x => x.UserID == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                ViewBag.userName = _userManager.GetUserName(HttpContext.User).ToString();
                return View(chat);
            }            
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SendMessageAsync(string Text, int ChatID)
        {
            var message = new Message
            {
                SenderName = User.Identity.Name,
                DateTime = DateTime.Now,
                Text = Text,
                ChatID = ChatID
            };
            if(_dataContext.ChatUsers.Any(x => x.ChatID == ChatID && x.UserID == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                await _hubContext.Clients.Group(ChatID.ToString()).SendAsync("ReceiveMessage", message.SenderName, message.Text, "Message", null);
                await _dataContext.Messages.AddAsync(message);
                await _dataContext.SaveChangesAsync();
                return Json(true);
            }
            return Json(true);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
