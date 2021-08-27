using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;

namespace GoodsEnterprise.Web.Utilities
{
    public static class Extension
    {
        /// <summary>
        /// ResizeImage - Resize the image to the specific width and height
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image ResizeImage(this Image image, int width, int height)
        {
            var res = new Bitmap(width, height);
            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, width, height);
            }
            return res;
        }

        /// <summary>
        /// ResizeToThumbnail - Resize to thumbnail with GetThumbnailImage method
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Image ResizeToThumbnail(this Image image)
        {         
            return image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
        }

        /// <summary>
        /// CompressImage - compress the image with specified percentage
        /// </summary>
        /// <param name="img"></param>
        /// <param name="quality"></param>
        /// <param name="OutputFilePath"></param>
        public static void CompressImage(this Image img, int quality, string OutputFilePath)
        {
            //if (quality < 0 || quality > 100)
            //    throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality
            EncoderParameter qualityParam =
                new EncoderParameter(Encoder.Quality, quality);

            var jpegEncoder = ImageCodecInfo.GetImageDecoders()
                 .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            img.Save(OutputFilePath, jpegEncoder, encoderParameters);
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="security"></param>
        /// <returns></returns>
        public static string Encrypt(this string toEncrypt, string security)
        {
            byte[] inputArray = System.Text.UTF8Encoding.UTF8.GetBytes(toEncrypt);
            TripleDESCryptoServiceProvider tdspMyPro = new TripleDESCryptoServiceProvider();
            tdspMyPro.Key = System.Text.UTF8Encoding.UTF8.GetBytes(security);
            tdspMyPro.Mode = CipherMode.ECB;
            tdspMyPro.Padding = PaddingMode.PKCS7;
            ICryptoTransform ictMyPro = tdspMyPro.CreateEncryptor();
            byte[] resultArray = ictMyPro.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tdspMyPro.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <param name="security"></param>
        /// <returns></returns>
        public static string Decrypt(this string toDecrypt, string security)
        {
            byte[] inputArray = Convert.FromBase64String(toDecrypt);
            TripleDESCryptoServiceProvider tdspMyPro = new TripleDESCryptoServiceProvider();
            tdspMyPro.Key = System.Text.UTF8Encoding.UTF8.GetBytes(security);
            tdspMyPro.Mode = CipherMode.ECB;
            tdspMyPro.Padding = PaddingMode.PKCS7;
            ICryptoTransform ictMyPro = tdspMyPro.CreateDecryptor();
            byte[] resultArray = ictMyPro.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tdspMyPro.Clear();
            return System.Text.UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
