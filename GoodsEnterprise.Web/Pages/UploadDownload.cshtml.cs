using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace GoodsEnterprise.Web.Pages
{
    public class UploadDownloadModel : PageModel
    {
        [BindProperty]
        public IFormFile Upload { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            try
            {
                ////List<UserModel> users = new List<UserModel>();
                //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                //using (var stream = new MemoryStream())
                //{
                //    Upload.CopyTo(stream);
                //    stream.Position = 0;
                //    using (var reader = ExcelReaderFactory.CreateReader(stream))
                //    {
                //        while (reader.Read()) //Each row of the file
                //        {
                //            users.Add(new UserModel { Name = reader.GetValue(0).ToString(), City = reader.GetValue(1).ToString() });
                //        }
                //    }
                //}



                return Page();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in OnPostSubmitAsync(), UploadDownload");
                throw;
            }
        }
    }
}
