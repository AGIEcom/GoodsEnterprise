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

        /// <summary>
        /// UploadProductImportImage - Upload image from file path with optional resizing
        /// </summary>
        /// <param name="imagePath">Full path to the source image file</param>
        /// <param name="folderName">Folder name where images should be saved</param>
        /// <param name="configuration">Application configuration to get upload path</param>
        /// <param name="isImage500">Whether to create 500px version (250x250)</param>
        /// <param name="isImage200">Whether to create 200px thumbnail version</param>
        /// <param name="noResize">If true, saves original image without resizing</param>
        /// <returns>Tuple containing paths to saved images (500px version, 200px version)</returns>
        public static async Task<Tuple<string, string>> UploadProductImportImage(string imagePath, string folderName, IConfiguration configuration, bool isImage500 = true, bool isImage200 = true, bool noResize = false)
        {
            string saveImage500 = string.Empty;
            string saveImage200 = string.Empty;

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                string uploadPath = configuration["Application:UploadPath"];
                string uploadImageFolderPath = Path.Combine($"{uploadPath}{folderName}");

                if (!Directory.Exists(uploadImageFolderPath))
                {
                    Directory.CreateDirectory(uploadImageFolderPath);
                }

                // Extract original filename without extension
                string originalFileName = Path.GetFileNameWithoutExtension(imagePath);

                if (noResize)
                {
                    // Save original image without resizing
                    string fileName = $"{originalFileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(imagePath)}";
                    string destinationPath = Path.Combine(uploadImageFolderPath, fileName);

                    // Copy the original file
                    File.Copy(imagePath, destinationPath);

                    // Set both return values to the same path since no resizing
                    saveImage500 = destinationPath;
                    saveImage200 = destinationPath;
                }
                else
                {
                    // Process with resizing based on flags
                    string fileName500 = $"{originalFileName}_500_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(imagePath)}";
                    string fileName200 = $"{originalFileName}_200_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(imagePath)}";
                    string uploadImage500 = Path.Combine(uploadImageFolderPath, fileName500);
                    string uploadImage200 = Path.Combine(uploadImageFolderPath, fileName200);

                    using (var image = Image.FromFile(imagePath))
                    {
                        if (isImage500)
                        {
                            Image imageResized500 = image.ResizeImage(250, 250);
                            imageResized500.Save(uploadImage500);
                            saveImage500 = uploadImage500;
                        }

                        if (isImage200)
                        {
                            Image imageResized200 = image.ResizeToThumbnail();
                            imageResized200.Save(uploadImage200);
                            saveImage200 = uploadImage200;
                        }
                    }
                }
            }

            return Tuple.Create(saveImage500, saveImage200);
        }

    }
}
