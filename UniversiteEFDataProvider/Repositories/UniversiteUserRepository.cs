using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Repositories;

public class UniversiteUserRepository : Repository<IUniversiteUser>, IUniversiteUserRepository
{
    private readonly UniversiteDbContext _context;
    private readonly UserManager<UniversiteUser> _userManager;
    private readonly RoleManager<UniversiteRole> _roleManager;

    public UniversiteUserRepository(
        UniversiteDbContext context, 
        UserManager<UniversiteUser> userManager, 
        RoleManager<UniversiteRole> roleManager) 
        : base(context)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IUniversiteUser?> AddUserAsync(string login, string email, string password, string role, Etudiant? etudiant)
    {
        UniversiteUser user = new UniversiteUser { UserName = login, Email = email, Etudiant = etudiant };
        IdentityResult result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
            await _context.SaveChangesAsync();
            return user;
        }
        
        return null;
    }

    public async Task<IUniversiteUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
    
    public async Task UpdateAsync(IUniversiteUser entity, string userName, string email)
    {
        UniversiteUser user = (UniversiteUser)entity;
        user.UserName = userName;
        user.Email = email;
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> GetRolesAsync(IUniversiteUser user)
    {
        UniversiteUser u = (UniversiteUser)user;
        var roles = await _userManager.GetRolesAsync(u);
        return roles.ToList();
    }

    public async Task<int> DeleteAsync(long id)
    {
        var etud = await _context.Etudiants.FindAsync(id);
        if (etud == null) return 0;
        
        UniversiteUser? user = await _userManager.FindByEmailAsync(etud.Email);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            return 1;
        }
        return 0;
    }

    public async Task<bool> IsInRoleAsync(string email, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;
        return await _userManager.IsInRoleAsync(user, role);
    }
    
    public async Task<bool> CheckPasswordAsync(IUniversiteUser user, string password)
    {
        UniversiteUser u = (UniversiteUser)user;
        return await _userManager.CheckPasswordAsync(u, password);
    }
    
    public async Task<IUniversiteUser?> FindByEtudiantIdAsync(long etudiantId)
    {
        return await _context.UniversiteUsers.FirstOrDefaultAsync(u => u.Etudiant.Id == etudiantId);
    }
}