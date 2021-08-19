using GoodsEnterprise.Model.Models;
using Microsoft.AspNetCore.Http;
using System;
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
        /// <summary>
        /// UploadImages
        /// </summary>
        /// <param name="Upload"></param>
        /// <returns></returns>
        public static async Task<Tuple<string, string>> UploadImages(IFormFile Upload, Brand objBrand)
        {
            string saveImage500 = string.Empty;
            string saveImage200 = string.Empty;

            if (Upload != null)
            {
                string uploadImageFolderPath = Path.Combine($"{Directory.GetCurrentDirectory()}{Constants.UploadPath + Constants.Brand}");
                string fileName500 = $"{objBrand.Name}_500_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(Upload.FileName)}";
                string fileName200 = $"{objBrand.Name}_200_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(Upload.FileName)}";
                string uploadImage500 = Path.Combine(uploadImageFolderPath, fileName500);
                string uploadImage200 = Path.Combine(uploadImageFolderPath, fileName200);

                using (var memoryStream = new MemoryStream())
                {
                    await Upload.CopyToAsync(memoryStream);
                    using (var image = Image.FromStream(memoryStream))
                    {
                        Image imageResized500 = image.ResizeImage(250, 250);
                        imageResized500.Save(uploadImage500);
                        saveImage500 = Path.Combine(Constants.SavePath, Constants.Brand, fileName500);
                        //image.CompressImage(500, uploadImage);

                        Image imageResized200 = image.ResizeToThumbnail();
                        imageResized200.Save(uploadImage200);
                        saveImage200 = Path.Combine(Constants.SavePath, Constants.Brand, fileName200);
                    }
                }
            }
            return Tuple.Create(saveImage500, saveImage200);
        }
    }
}
