using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        public static List<PromotionCost> UploadPromotionCosts { get; set; }
        public static List<Product> Uploadproducts { get; set; }
        /// <summary>
        /// UploadImages
        /// </summary>
        /// <param name="Upload"></param>
        /// <param name="name"></param>
        /// <param name="folderName"></param>
        /// <param name="configuration"></param>
        /// <param name="isImage500"></param>
        /// <param name="isImage200"></param>
        /// <returns></returns>
        public static async Task<Tuple<string, string>> UploadImages(IFormFile Upload, string name, string folderName, IConfiguration configuration, bool isImage500 = true, bool isImage200 = true)
        {
            string saveImage500 = string.Empty;
            string saveImage200 = string.Empty;

            if (Upload != null)
            {
                string uploadPath = configuration["Application:UploadPath"];
                //string uploadImageFolderPath = Path.Combine($"{Directory.GetCurrentDirectory()}{uploadPath + folderName}");
                string uploadImageFolderPath = Path.Combine($"{uploadPath}{folderName}");
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
                            //saveImage500 = Path.Combine(Constants.SavePath, folderName, fileName500);
                            saveImage500 = uploadImage500;
                        }
                        //image.CompressImage(500, uploadImage);
                        if (isImage200)
                        {
                            Image imageResized200 = image.ResizeToThumbnail();
                            imageResized200.Save(uploadImage200);
                            //saveImage200 = Path.Combine(Constants.SavePath, folderName, fileName200);
                            saveImage200 = uploadImage200;
                        }
                    }
                }
            }
            return Tuple.Create(saveImage500, saveImage200);
        }        
    }
}
