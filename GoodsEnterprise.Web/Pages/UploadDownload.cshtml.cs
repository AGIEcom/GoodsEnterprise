using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExcelDataReader;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Maaper;
using GoodsEnterprise.Web.Utilities;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace GoodsEnterprise.Web.Pages
{
    public class UploadDownloadModel : PageModel
    {
        public UploadDownloadModel(IGeneralRepository<Brand> brand, IGeneralRepository<Category> category,
            IGeneralRepository<SubCategory> subCategory, IGeneralRepository<Product> product, IUploadDownloadDA uploadDownloadDA, IMapper mapper)
        {
            _brand = brand;
            _category = category;
            _subCategory = subCategory;
            _product = product;
            _uploadDownloadDA = uploadDownloadDA;
            _mapper = mapper;
        }
        [BindProperty]
        public IFormFile Upload { get; set; }

        Dictionary<string, bool> uploadFileFields = new Dictionary<string, bool>();
        private readonly IGeneralRepository<Brand> _brand;
        private readonly IGeneralRepository<Category> _category;
        private readonly IGeneralRepository<SubCategory> _subCategory;
        private readonly IGeneralRepository<Product> _product;
        private readonly IUploadDownloadDA _uploadDownloadDA;
        private readonly IMapper _mapper;
        public List<Product> products { get; set; }

        /// <summary>
        /// OnPostSubmitAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            DataTable productUpload;
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new MemoryStream())
                {
                    Upload.CopyTo(stream);
                    stream.Position = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        productUpload = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                            
                        }).Tables[0];
                    }
                }
                LoadFields();
                foreach (DataColumn column in productUpload.Columns)
                {
                    if (uploadFileFields.ContainsKey(column.Caption.Trim()))
                    {
                        uploadFileFields[column.Caption.Trim()] = true;
                    }
                }

                string[] missingColumns = uploadFileFields.Where(x => x.Value == false).Select(x => x.Key).ToArray() ;

                if (missingColumns.Count() > 1)
                {
                    ViewData["SuccessMsg"] = "Invalid file format (Product file format is not valid)";
                    return Page();
                }

                //if (uploadFileFields.Any(x => x.Value == false))
                //{
                //    ViewData["SuccessMsg"] = "Invalid file format (Product file format is not valid)";
                //    return Page();
                //}

                string[] duplicates = productUpload.AsEnumerable()
                   .Select(dr => Convert.ToString(dr["Outer EAN"]))
                   .GroupBy(x => x)
                   .Where(g => g.Count() > 1)
                   .Select(g => g.Key)
                   .ToArray();

                if (duplicates!= null && duplicates.Count() > 1)
                {
                    ViewData["ValidationMsg"] = "Invalid file format (Product file format is not valid)";
                    return Page();
                }

               

                var predicate = PredicateBuilder.New<DataRow>();

                foreach (string field in Constants.ProductMandatoryFields)
                {
                    predicate = predicate.Or(a => string.IsNullOrEmpty(Convert.ToString(a[field]).Trim()));
                }

                int[] missingMandatoryFields = productUpload.AsEnumerable().Where(predicate).Select(r => productUpload.Rows.IndexOf(r)+2).ToArray();


                if (missingMandatoryFields != null && missingMandatoryFields.Count() > 1)
                {
                    ViewData["SuccessMsg"] = "Invalid file format (Product file format is not valid)";
                    return Page();
                }


                await LoadProducts();

                if (products != null && products.Count > 1)
                {
                    string[] existingProducts = productUpload.AsEnumerable().Where(a => products.Select(b => b.Code).Contains(a.Field<string>("Outer EAN")))
                                              .GroupBy(product => product.Field<string>("Outer EAN").Trim())
                                              .Select(group => group.First().Field<string>("Outer EAN")).ToArray();

                    if (existingProducts.Count() > 1)
                    {
                        ViewData["SuccessMsg"] = "Invalid file format (Product file format is not valid)";
                        return Page();
                    }
                }


                await LoadBrands();
                await LoadCategories();
                await LoadSubCategories();

                await InsertNewBrands(productUpload);
                await InsertNewCategories(productUpload);
                await InsertNewSubCategories(productUpload);

                List<Product> bulkInsertProducts = _mapper.Map<List<DataRow>, List<Product>>(new List<DataRow>(productUpload.Rows.OfType<DataRow>()));
                
                await _uploadDownloadDA.BulkInsertProductAsync(bulkInsertProducts.ToList());

                ViewData["SuccessMsg"] = $"{Constants.SuccessUpload}";
                return Page();
            }
            catch (Exception ex)
            {
                ViewData["SuccessMsg"] = $"{Constants.FailureUpload}";
                Log.Error(ex, $"Error in OnPostSubmitAsync(), UploadDownload");
                throw;
            }
            finally 
            {
                Common.UploadBrands = null;
                Common.UploadCategories = null;
                Common.UploadSubCategories = null;
            }
        }

        /// <summary>
        /// LoadFields
        /// </summary>
        private void LoadFields()
        {
            try
            {
                foreach (string field in Constants.ProductMandatoryFields)
                {
                    uploadFileFields.Add(field, false);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in LoadFields(), UploadDownload");
                throw;
            }
        }

        /// <summary>
        /// LoadBrands
        /// </summary>
        /// <returns></returns>
        public async Task LoadBrands()
        {
            try
            {
                Common.UploadBrands = await _brand.GetAllAsync(filter: x => x.IsDelete != true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadBrands(), UploadDownload");
                throw;
            }         
        }

        /// <summary>
        /// LoadCategories
        /// </summary>
        /// <returns></returns>
        public async Task LoadCategories()
        {
            try
            {
                Common.UploadCategories = await _category.GetAllAsync(filter: x => x.IsDelete != true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadCategories(), UploadDownload");
                throw;
            }      
        }

        /// <summary>
        /// LoadSubCategories
        /// </summary>
        /// <returns></returns>
        public async Task LoadSubCategories()
        {
            try
            {
                Common.UploadSubCategories = await _subCategory.GetAllAsync(filter: x => x.IsDelete != true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadSubCategories(), UploadDownload");
                throw;
            }            
        }

        /// <summary>
        /// LoadProducts
        /// </summary>
        /// <returns></returns>
        public async Task LoadProducts()
        {
            try
            {
                products = await _product.GetAllAsync(filter: x => x.IsDelete != true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadProducts(), UploadDownload");
                throw;
            }
        }

        /// <summary>
        /// InsertNewBrands
        /// </summary>
        /// <param name="productUploadForBrands"></param>
        /// <returns></returns>
        private async Task InsertNewBrands(DataTable productUploadForBrands)
        {
            try
            {
                var newBrands = productUploadForBrands.AsEnumerable().Where(a => !Common.UploadBrands.Select(b => b.Name).Contains(a.Field<string>("Brand")))
                                              .GroupBy(brand => brand.Field<string>("Brand").Trim())
                                              .Select(group => group.First()).ToList();

                List<Brand> bulkInsertBrands = new List<Brand>();
                foreach (var brand in newBrands)
                {
                    var bulkInsertBrand = new Brand()
                    {
                        Name = brand.Field<string>("Brand"),
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDelete = false
                    };
                    bulkInsertBrands.Add(bulkInsertBrand);
                }

                await _uploadDownloadDA.BulkInsertBrandAsync(bulkInsertBrands);

                Common.UploadBrands.AddRange(bulkInsertBrands);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in InsertNewBrands(), UploadDownload");
                throw;
            }
        }

        /// <summary>
        /// InsertNewCategories
        /// </summary>
        /// <param name="productUploadForCategories"></param>
        /// <returns></returns>
        private async Task InsertNewCategories(DataTable productUploadForCategories)
        {
            try
            {
                var newCategories = productUploadForCategories.AsEnumerable().Where(a => !Common.UploadCategories.Select(c => c.Name).Contains(a.Field<string>("Category")))
                              .GroupBy(category => category.Field<string>("Category"))
                              .Select(group => group.First()).ToList();

                List<Category> bulkInsertCategories = new List<Category>();
                foreach (var category in newCategories)
                {
                    var bulkInsertCategory = new Category()
                    {
                        Name = category.Field<string>("Category"),
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDelete = false
                    };
                    bulkInsertCategories.Add(bulkInsertCategory);
                }

                await _uploadDownloadDA.BulkInsertCategoryAsync(bulkInsertCategories);

                Common.UploadCategories.AddRange(bulkInsertCategories);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in InsertNewCategories(), UploadDownload");
                throw;
            }
        }

        /// <summary>
        /// InsertNewSubCategories
        /// </summary>
        /// <param name="productUploadForCategories"></param>
        /// <returns></returns>
        private async Task InsertNewSubCategories(DataTable productUploadForSubCategories)
        {
            try
            {
                var newSubCategories = productUploadForSubCategories.AsEnumerable().Where(a => !Common.UploadCategories.Select(c => c.Name).Contains(a.Field<string>("SubCategory")))
                              .GroupBy(subCategory => subCategory.Field<string>("SubCategory"))
                              .Select(group => group.First()).ToList();

                List<SubCategory> bulkInsertSubCategories = new List<SubCategory>();
                foreach (var subCategory in newSubCategories)
                {
                    var bulkInsertSubCategory = new SubCategory()
                    {
                        Name = subCategory.Field<string>("SubCategory"),
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDelete = false
                    };
                    bulkInsertSubCategories.Add(bulkInsertSubCategory);
                }

                await _uploadDownloadDA.BulkInsertSubCategoryAsync(bulkInsertSubCategories);

                Common.UploadSubCategories.AddRange(bulkInsertSubCategories);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in InsertNewSubCategories(), UploadDownload");
                throw;
            }
        }
    }
}
