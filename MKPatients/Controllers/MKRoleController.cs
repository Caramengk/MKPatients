using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MKPatients.Controllers
{
    public class MKRoleController : Controller
    {  
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public MKRoleController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {   
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        //list all current roles, links to manage roles
        public IActionResult Index()
        {
            var roles = roleManager.Roles.OrderBy(a => a.Name);
            return View(roles);
        }


        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    TempData["message"] = "Role name cannot be empty or blank.";
                    return View("Index");
                }
                   

                roleName = roleName.Trim();

                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (roleExists)
                {
                    TempData["message"] = $"role '{roleName}' already exists";
                    return View("index");
                }

                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    TempData["message"] = $"role '{roleName}' created";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = $"problem creating role '{roleName}': " + result.Errors.FirstOrDefault().Description;
                    return View("Index", result.Errors.FirstOrDefault().Description);
                } 
               
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Exception creating role '{roleName}': {ex.GetBaseException().Message}";
            }
            return View("Index");    
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleName)
        {
            try
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (roleExists)
                {
                    IdentityRole roleToDelete = await roleManager.FindByNameAsync(roleName); 
                    await roleManager.DeleteAsync(roleToDelete);
                    TempData["message"] = $"role '{roleName}' deleted";
                } 
               
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Exception creating role '{roleName}': {ex.GetBaseException().Message}";
            } 
           
            return View("Index");
        }

        public async Task<IActionResult> ManageUsersInRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
               
                return NotFound();
            }

            ViewBag.RoleId = roleId;
            ViewBag.RoleName = role.Name;

            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            return View(usersInRole.OrderBy(u => u.UserName).ToList());
        }

       
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string roleId, string userId)
        {     
            var user = await userManager.FindByIdAsync(userId);
            var role = await roleManager.FindByIdAsync(roleId);

         

            try
            {
               

                if (user == null || role == null)
                    return RedirectToAction("ManageUsers", new { id = roleId });

                var result = await userManager.AddToRoleAsync(user, role.Name);

                if (result.Succeeded)
                    return RedirectToAction("ManageUsers", new { id = roleId });
            }
            catch(Exception ex)
            {
                TempData["message"] = $"Exception adding {user.UserName} to '{role.Name}': {ex.GetBaseException().Message}";
            }

            var allUsers = await userManager.Users.ToListAsync();
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            var usersNotInRole = allUsers.Except(usersInRole).OrderBy(u => u.UserName);

            ViewData["UsersNotInRole"] = new SelectList(usersNotInRole, "Id", "UserName");


            return View("Index");
          

            
        }

        public async Task<IActionResult> RemoveUserFromRole(string roleId, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var role = await roleManager.FindByIdAsync(roleId);
            try
            {
                if (user == null || role == null)
                    return RedirectToAction("ManageUsers", new { id = roleId });

                if (role.Name == "administrators" && User.Identity.Name == user.UserName)
                    return RedirectToAction("ManageUsers", new { id = roleId }); 

                var result = await userManager.RemoveFromRoleAsync(user, role.Name);

                if (result.Succeeded)
                    return RedirectToAction("ManageUsers", new { id = roleId });
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Exception removing {user.UserName} to '{role.Name}': {ex.GetBaseException().Message}";
            }
            return View("Index");
            
        }
    }
}
