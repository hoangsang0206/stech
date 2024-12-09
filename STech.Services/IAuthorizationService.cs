using STech.Data.Models;

namespace STech.Services;

public interface IAuthorizationService
{
    Task<IEnumerable<FunctionCategory> > GetFunctionCategories();
    Task<Function?> GetFunction(string funcId);
    Task<IEnumerable<UserGroup>> GetUserGroups();
    Task<UserGroup?> GetUserGroup(int? groupId);
    Task<IEnumerable<Function>> GetAuthorizedFunctions(int? groupId);
    Task<bool> IsAuthorized(int? groupId, string functionCode);
    Task<bool> GrandAccess(int? groupId, string functionCode);
}