using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(userManager.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName = "Basma Mohsen",
                    Email = "basmamohsen53@gmail.com",
                    UserName = "basmamohsen",
                    PhoneNumber = "01014654026"
                };
                await userManager.CreateAsync(user, "P@$$w0rd");
            }
        }
    }
}
