using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Utilities
{
    /// <summary>
    /// Constants
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Constants
        /// </summary>
        private Constants()
        {

        }
        public const string StatusMessage = "StatusMessage";
        public const string SavePath = @"\UploadedImages\";
        public const string Brand = "Brand";
        public const string Category = "Category";
        public const string Product = "Product";
        public const string SaveMessage = "Saved successfully";
        public const string UpdateMessage = "Updated successfully";
        public const string DeletedMessage = "Deleted successfully";
        public const string AlreadyExistMessage = "Name already exist";
        public const string NoRecordsFoundMessage = "No Records Found";
        public const string LoginSession = "LoginSession";
        public const string UserNamePasswordincorrect = "Username and password incorrect";
        public const string UserNameNotavailable = "Username not available";
        public const string EncryptDecryptSecurity = "mysmallkey123456";
        public const string SuccessUpload = "Prodcuts uploaded sucessfully";
        public const string FailureUpload = "Some problem occured while uploading the file, please contact Administrator";

        public static readonly string[] ProductMandatoryFields = { "Product Code" , "Category", "Brand", "Supplier", "Inner EAN", "Outer EAN", "Product Description", "UPC", "Unit Size", "Lyr Qty", "Plt Qty", "Case Price", "IsActive", "Expriy Date", "Image" };

        public static readonly string[] ProductFieldsDownload = { "Code", "Brand", "Category", "SubCategory", "Supplier", "InnerEan", "OuterEan", "PackSize", "Upc", "LayerQuantity", "PalletQuantity", "Height", "Weight",
        "Width", "NetWeight", "Depth", "IsActive", "ExpiryDate"};

        public static readonly string[] PromotionCostFields = { "Start", "End", "Sellout Start", "Sellout End", "Supplier:", "Product", "Outer Barcode", "Inner Barcode", "W/sale Nett Cost", "Bonus Description", "Sell Out Description" };
    }
}
