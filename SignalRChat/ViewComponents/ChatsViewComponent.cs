using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Db;
using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.ViewComponents
{
    public class ChatsViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatsViewComponent(DataContext dataContext, UserManager<ApplicationUser> userManager){
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            var chats = await _dataContext.Chats.Include("ChatUsers.User")
                               .Where(x => x.ChatUsers.Any(a => a.UserID == userId)).OrderByDescending(x => x.ID).ToListAsync();
            return View(chats);
        }

    }
}
