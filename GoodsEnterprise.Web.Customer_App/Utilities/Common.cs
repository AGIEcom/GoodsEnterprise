using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Model.Models.CustomerModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Utilities
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class Common
    {
        public static List<Brand> UploadBrands { get; set; }    
        public static List<Category> UploadCategories { get; set; }
        public static List<SubCategory> UploadSubCategories { get; set; }
        public static List<Supplier> UploadSuppliers { get; set; }
        /// <summary>
        /// UploadImages
        /// </summary>
        /// <param name="Upload"></param>
        /// <param name="name"></param>
        /// <param name="folderName"></param>
        /// <param name="isImage500"></param>
        /// <param name="isImage200"></param>
        /// <returns></returns>
        public static async Task<Tuple<string, string>> UploadImages(IFormFile Upload, string name, string folderName, bool isImage500 = true, bool isImage200 = true)
        {
            string saveImage500 = string.Empty;
            string saveImage200 = string.Empty;

            if (Upload != null)
            {
                string uploadImageFolderPath = Path.Combine($"{Directory.GetCurrentDirectory()}{Constants.UploadPath + folderName}");
                if (!Directory.Exists(uploadImageFolderPath))
                {
                    Directory.CreateDirectory(uploadImageFolderPath);
                }
                string fileName500 = $"{name}_500_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(Upload.FileName)}";
                string fileName200 = $"{name}_200_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(Upload.FileName)}";
                string uploadImage500 = Path.Combine(uploadImageFolderPath, fileName500);
                string uploadImage200 = Path.Combine(uploadImageFolderPath, fileName200);

                using (var memoryStream = new MemoryStream())
                {
                    await Upload.CopyToAsync(memoryStream);
                    using (var image = Image.FromStream(memoryStream))
                    {
                        if (isImage500)
                        {
                            Image imageResized500 = image.ResizeImage(250, 250);
                            imageResized500.Save(uploadImage500);
                            saveImage500 = Path.Combine(Constants.SavePath, folderName, fileName500);
                        }
                        //image.CompressImage(500, uploadImage);
                        if (isImage200)
                        {
                            Image imageResized200 = image.ResizeToThumbnail();
                            imageResized200.Save(uploadImage200);
                            saveImage200 = Path.Combine(Constants.SavePath, folderName, fileName200);
                        }
                    }
                }
            }
            return Tuple.Create(saveImage500, saveImage200);
        }


        public static void SendEmail(EmailSettings emailSettings,string ToAddress,string subject,string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailSettings.emailFromAddress);
                mail.To.Add(ToAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(emailSettings.smtpAddress, emailSettings.portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailSettings.emailFromAddress, emailSettings.password);
                    smtp.EnableSsl = emailSettings.enableSSL;
                    smtp.Send(mail);
                }
            }
        }
    }
}
