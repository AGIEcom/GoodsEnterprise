using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoodsEnterprise.Web.Customer.Pages
{
    public class ContactModel : PageModel
    {
        public IActionResult OnGetContactUS()
        {
            return Page();
        }
    }
}
