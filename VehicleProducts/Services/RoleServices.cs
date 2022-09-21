using Microsoft.AspNetCore.Identity;

namespace VehicleProducts.Services
{
	/// <summary>
	/// Creating Roles 
	/// 
	/// Error:  An unhandled exception has occurred while executing the request.
    /// System.InvalidOperationException: Unable to resolve service for type 'Microsoft.AspNetCore.Identity.UI.Services.IEmailSender' while attempting to activate 'VehicleProducts.Areas.Identity.Pages.Account.RegisterModel'.
	/// Reference: https://stackoverflow.com/questions/42188927/how-to-add-custom-roles-to-asp-net-core/42204984#42204984
	/// </summary>
	public class RoleServices
	{
		public static async Task CreateRoles(IServiceProvider serviceProvider)
		{
			//// Adding custom roles 
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(); 
			var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
			IdentityResult? roleResult = null; 

			string[] roleNames = { "Admin", "Customer" };

			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName); 

				if ( ! roleExist )
				{
					// Create the roles and seed them to the db 
					roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

				}// end if 
			}// end foreach 

			// Preparing to create an admin user 
			string adminUserEmail = "admin@youremail.com"; 
			string adminUserPassword = "$Password1234"; 
			var adminUser = new IdentityUser
			{
				UserName = adminUserEmail, 
				Email = adminUserEmail,
				EmailConfirmed = true,
			}; 

			var user = await userManager.FindByEmailAsync(adminUserEmail);

			if ( user == null )
			{
				//// Creating Admin user 
				var createAdminUser = await userManager.CreateAsync(adminUser, adminUserPassword);

				//// Checking whether if we successfully created.
				if (createAdminUser.Succeeded)
				{
					// Assing the admin role 
					await userManager.AddToRoleAsync(adminUser, roleNames[0]); 

				}// end if 
			}// end if 

		}// end CreateRoles
	}// class RoleServices
}
