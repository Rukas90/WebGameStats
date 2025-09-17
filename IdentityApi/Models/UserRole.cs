using Microsoft.AspNetCore.Identity;

namespace IdentityApi.Models;

public class UserRole(string name) : IdentityRole<Guid>(name);