using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Pages
{
    /// <summary>
    /// CustomerModel
    /// </summary>
    public class CustomerModel : PageModel
    {
        /// <summary>
        /// CustomerModel
        /// </summary>
        /// <param name="customer"></param>
        public CustomerModel(IGeneralRepository<Customer> customer, IGeneralRepository<Role> role)
        {
            _customer = customer;
            _role = role;
        }

        private readonly IGeneralRepository<Customer> _customer;
        private readonly IGeneralRepository<Role> _role;

        [BindProperty()]
        public Customer objCustomer { get; set; }

        public List<Customer> lstcustomer = new List<Customer>();

        public Pagination PaginationModel { get; set; } = new Pagination();
        public SelectList Roles { get; set; } = new SelectList("");

        /// <summary>
        /// OnGetAsync
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            try
            {
                ViewData["PageType"] = "List";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Constants.StatusMessage)))
                {
                    ViewData["SuccessMsg"] = HttpContext.Session.GetString(Constants.StatusMessage);
                    HttpContext.Session.SetString(Constants.StatusMessage, "");
                }
                ViewData["PagePrimaryID"] = 0;
                lstcustomer = await _customer.GetAllAsync(filter: x => x.IsDelete != true, orderBy: mt => mt.OrderByDescending(m => m.ModifiedDate == null ? m.CreatedDate : m.ModifiedDate));
                if (lstcustomer == null || lstcustomer?.Count == 0)
                {
                    ViewData["SuccessMsg"] = $"{Constants.NoRecordsFoundMessage}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetAsync(), Customer");
                throw;
            }
        }

        /// <summary>
        /// OnGetCreateAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetCreateAsync()
        {
            try
            {
                await LoadRole();
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetCreateAsync(), Customer");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetEditAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetEditAsync(int customerId)
        {
            try
            {
                objCustomer = await _customer.GetAsync(filter: x => x.Id == customerId && x.IsDelete != true);

                if (objCustomer == null)
                {
                    return Redirect("~/all-customer");
                }
                await LoadRole();
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objCustomer.Id;
                objCustomer.Password = objCustomer.Password.Decrypt(Constants.EncryptDecryptSecurity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetEditAsync(), Customer, CustomerId: { customerId }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetClear
        /// </summary>
        /// <returns></returns>
        public IActionResult OnGetClear()
        {
            try
            {
                objCustomer = new Customer();
                objCustomer.IsActive = false;
                ViewData["PageType"] = "Edit";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetClear(), Customer");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnResetClear
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetReset(int customerId)
        {
            try
            {
                await LoadRole();
                objCustomer = await _customer.GetAsync(filter: x => x.Id == customerId && x.IsDelete != true);
                ViewData["PageType"] = "Edit";
                ViewData["PagePrimaryID"] = objCustomer.Id;
                objCustomer.Password = objCustomer.Password.Decrypt(Constants.EncryptDecryptSecurity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnResetClear(), Customer, CustomerId: { objCustomer.Id }");
                throw;
            }
            return Page();
        }

        /// <summary>
        /// OnGetDeleteCustomerAsync
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDeleteCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _customer.GetAsync(filter: x => x.Id == customerId);
                if (customer != null)
                {
                    await _customer.LogicalDeleteAsync(customer);
                    ViewData["SuccessMsg"] = $"Customer: {customer.Email} {Constants.DeletedMessage}";
                    HttpContext.Session.SetString(Constants.StatusMessage, $"Customer: {customer.Email} {Constants.DeletedMessage}");
                }

                ViewData["PageType"] = "List";
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnGetDeleteCustomerAsync(), Customer, CustomerId: { customerId }");
                throw;
            }
            return Redirect("~/all-customer");
        }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                Customer existingCustomer = await _customer.GetAsync(filter: x => x.Email == objCustomer.Email && x.IsDelete != true);
                if (existingCustomer != null)
                {
                    if ((objCustomer.Id == 0) || (objCustomer.Id != 0 && objCustomer.Id != existingCustomer.Id))
                    {
                        ViewData["PageType"] = "Edit";
                        if (objCustomer.Id != 0)
                        {
                            ViewData["PagePrimaryID"] = objCustomer.Id;
                        }
                        ViewData["SuccessMsg"] = $"Customer: {objCustomer.Email} {Constants.AlreadyExistMessage}";
                        return Page();
                    }
                }

                objCustomer.Password = objCustomer.Password.Encrypt(Constants.EncryptDecryptSecurity);

                if (ModelState.IsValid)
                {
                    if (objCustomer.Id == 0)
                    {
                        objCustomer.PasswordExpiryDate = DateTime.UtcNow.AddDays(90);
                        await _customer.InsertAsync(objCustomer);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.SaveMessage);
                    }
                    else
                    {
                        await _customer.UpdateAsync(objCustomer);
                        HttpContext.Session.SetString(Constants.StatusMessage, Constants.UpdateMessage);
                    }
                    return Redirect("all-customer");
                }
                else
                {
                    await LoadRole();
                    ViewData["PageType"] = "Edit";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitAsync(), Customer, CustomerId: { objCustomer?.Id }");
                throw;
            }
        }

        /// <summary>
        /// LoadBrand
        /// </summary>
        /// <returns></returns>
        private async Task LoadRole()
        {
            Roles = new SelectList(await _role.GetAllAsync(filter: x => x.IsDelete != true),
                           "Id", "Name", null);
        }
    }
}
