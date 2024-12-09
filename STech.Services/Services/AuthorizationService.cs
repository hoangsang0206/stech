using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.Services.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly StechDbContext _context;
    
    public AuthorizationService(StechDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<FunctionCategory>> GetFunctionCategories()
    {
        return await _context.FunctionCategories
            .Include(f => f.Functions)
            .ToListAsync();
    }

    public async Task<Function?> GetFunction(string funcId)
    {
        return await _context.Functions
            .FirstOrDefaultAsync(f => f.FuncId == funcId);
    }

    public async Task<IEnumerable<UserGroup>> GetUserGroups()
    {
        return await _context.UserGroups
            .ToListAsync();
    }

    public async Task<UserGroup?> GetUserGroup(int? groupId)
    {
        return await _context.UserGroups
            .FirstOrDefaultAsync(g => g.GroupId == groupId);
    }

    public async Task<IEnumerable<Function>> GetAuthorizedFunctions(int? groupId)
    {
        return await _context.FunctionAuthorizations
            .Where(g => g.GroupId == groupId && g.IsAuthorized == true)
            .Select(g => g.Func)
            .ToListAsync();
    }

    public async Task<bool> IsAuthorized(int? groupId, string functionCode)
    {
        return await _context.FunctionAuthorizations
            .AnyAsync(f => f.GroupId == groupId && f.Func.FuncId == functionCode 
                        && f.IsAuthorized == true);
    }

    public async Task<bool> GrandAccess(int? groupId, string functionCode)
    {
        UserGroup? group = await _context.UserGroups
            .FirstOrDefaultAsync(g => g.GroupId == groupId);
        Function? func = await _context.Functions
            .FirstOrDefaultAsync(f => f.FuncId == functionCode);

        if (group == null || func == null)
        {
            return false;
        }
        
        FunctionAuthorization? auth = await _context.FunctionAuthorizations
            .FirstOrDefaultAsync(f => f.GroupId == groupId && f.FuncId == functionCode);

        if (auth != null)
        {
            auth.IsAuthorized = true;
        }
        else
        {
            FunctionAuthorization newAuth = new FunctionAuthorization
            {
                GroupId = group.GroupId,
                FuncId = func.FuncId,
                IsAuthorized = true
            };

            await _context.FunctionAuthorizations.AddAsync(newAuth);
        }

        return await _context.SaveChangesAsync() > 0;
    }
}