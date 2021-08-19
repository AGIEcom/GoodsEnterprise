using System;
using System.Collections.Generic;
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
        public const string UploadPath = @"\wwwroot\UploadedImages\";
        public const string SavePath = @"\UploadedImages\";
        public const string Brand = "Brand";
        public const string SaveMessage = "Saved successfully";
        public const string UpdateMessage = "Updated successfully";
        public const string DeletedMessage = "Deleted successfully";
        public const string AlreadyExistMessage = "Name already exist";
        public const string NoRecordsFoundMessage = "No Records Found";
    }
}
