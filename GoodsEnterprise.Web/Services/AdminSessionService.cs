using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace GoodsEnterprise.Web.Services
{
    public interface IAdminSessionService
    {
        Admin GetCurrentAdmin();
        void SetCurrentAdmin(Admin admin);
    }

    public class AdminSessionService : IAdminSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Admin _cachedAdmin;

        public AdminSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //public Admin GetCurrentAdmin()
        //{
        //    if (_cachedAdmin != null)
        //        return _cachedAdmin;

        //    var adminJson = _httpContextAccessor.HttpContext?.Session?.GetString(Constants.LoginSession);
        //    if (!string.IsNullOrEmpty(adminJson))
        //    {
        //        _cachedAdmin = JsonConvert.DeserializeObject<Admin>(adminJson);
        //    }
        //    return _cachedAdmin;
        //}
        public Admin GetCurrentAdmin()
        {
            if (_cachedAdmin != null)
                return _cachedAdmin;

            var adminJson = _httpContextAccessor.HttpContext?.Session?.GetString(Constants.LoginSession);
            if (string.IsNullOrEmpty(adminJson))
            {
                throw new UnauthorizedAccessException("No admin is currently logged in.");
            }

            _cachedAdmin = JsonConvert.DeserializeObject<Admin>(adminJson);
            if (_cachedAdmin == null)
            {
                throw new InvalidOperationException("Failed to deserialize admin user from session.");
            }

            return _cachedAdmin;
        }
        public void SetCurrentAdmin(Admin admin)
        {
            var adminJson = JsonConvert.SerializeObject(admin);
            _httpContextAccessor.HttpContext?.Session?.SetString(Constants.LoginSession, adminJson);
            _cachedAdmin = admin;
        }
    }
}
