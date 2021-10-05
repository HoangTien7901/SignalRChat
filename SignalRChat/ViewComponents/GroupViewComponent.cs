using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Db;
using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalRChat.ViewComponents
{
    public class GroupViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupViewComponent(DataContext dataContext, UserManager<ApplicationUser> userManager){
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            var users = await _dataContext.Users.Where(x => x.Id != HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
            return View(users);
        }


    }
}
