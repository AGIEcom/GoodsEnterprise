using AutoMapper;
using ExcelDataReader;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Pages
{
    public class UploadDownloadModel : PageModel
    {
        public UploadDownloadModel(IGeneralRepository<Brand> brand, IGeneralRepository<Category> category,
            IGeneralRepository<SubCategory> subCategory, IGeneralRepository<Product> product, IGeneralRepository<Supplier> supplier, IUploadDownloadDA uploadDownloadDA, IMapper mapper)
        {
            _brand = brand;
            _category = category;
            _subCategory = subCategory;
            _product = product;
            _supplier = supplier;
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
        private readonly IGeneralRepository<Supplier> _supplier;
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

                productUpload = productUpload.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull ||
                                     string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();

                LoadFields();
                foreach (DataColumn column in productUpload.Columns)
                {
                    if (uploadFileFields.ContainsKey(column.Caption.Trim()))
                    {
                        uploadFileFields[column.Caption.Trim()] = true;
                    }
                }

                string[] missingColumns = uploadFileFields.Where(x => x.Value == false).Select(x => x.Key).ToArray() ;

                if (missingColumns.Count() > 0)
                {
                    string messages = string.Empty;
                    foreach (string msg in missingColumns)
                    {
                        messages += msg + "<br />";
                    }
                    ViewData["ValidationMsg"] = $"Below Mandatory columns are missing<br /><div style='max-height: 200px; overflow-y: auto; border: 1px solid #ccc; padding: 10px; background-color: #f9f9f9;'>{messages}</div>";
                    return Page();
                }

                string[] duplicates = productUpload.AsEnumerable()
                   .Select(dr => Convert.ToString(dr["Outer EAN"]))
                   .GroupBy(x => x)
                   .Where(g => g.Count() > 1)
                   .Select(g => g.Key)
                   .ToArray();

                if (duplicates!= null && duplicates.Count() > 0)
                {
                    string messages = string.Empty;
                    foreach (string msg in duplicates)
                    {
                        messages += msg + "<br />";
                    }
                    ViewData["ValidationMsg"] = $"Below Outer EAN has duplicate values, please correct it and retry<br /><div style='max-height: 200px; overflow-y: auto; border: 1px solid #ccc; padding: 10px; background-color: #f9f9f9;'>{messages}</div>";
                    return Page();
                }               

                var predicate = PredicateBuilder.New<DataRow>();

                foreach (string field in Constants.ProductMandatoryFields)
                {
                    predicate = predicate.Or(a => string.IsNullOrEmpty(Convert.ToString(a[field]).Trim()));
                }

                int[] missingMandatoryFields = productUpload.AsEnumerable().Where(predicate).Select(r => productUpload.Rows.IndexOf(r)+2).ToArray();


                if (missingMandatoryFields != null && missingMandatoryFields.Count() > 0)
                {
                    string messages = string.Empty;
                    foreach (int msg in missingMandatoryFields)
                    {
                        messages += msg + "<br />";
                    }
                    ViewData["ValidationMsg"] = $"Mandatory fields are missing in the below Row number, please correct it and retry<br /><div style='max-height: 200px; overflow-y: auto; border: 1px solid #ccc; padding: 10px; background-color: #f9f9f9;'>{messages}</div>";
                    return Page();
                }

                // Validate Expiry Date format
                if (productUpload.Columns.Contains("Expriy Date"))
                {
                    List<int> invalidDateRows = new List<int>();
                    
                    for (int i = 0; i < productUpload.Rows.Count; i++)
                    {
                        var expiryDateValue = productUpload.Rows[i]["Expriy Date"];
                        
                        if (expiryDateValue != null && !string.IsNullOrWhiteSpace(expiryDateValue.ToString()))
                        {
                            if (!DateTime.TryParse(expiryDateValue.ToString(), out DateTime parsedDate))
                            {
                                invalidDateRows.Add(i + 2); // +2 because Excel rows start from 1 and we have header row
                            }
                        }
                    }
                    
                    if (invalidDateRows.Count > 0)
                    {
                        string messages = string.Empty;
                        foreach (int rowNum in invalidDateRows)
                        {
                            messages += rowNum + "<br />";
                        }
                        ViewData["ValidationMsg"] = $"Invalid date format found in Expiry Date column at the below Row numbers, please correct it and retry<br /><div style='max-height: 200px; overflow-y: auto; border: 1px solid #ccc; padding: 10px; background-color: #f9f9f9;'>{messages}</div>";
                        return Page();
                    }
                }

                await LoadProducts();

                if (products != null && products.Count > 0)
                {
                    string[] existingProducts = productUpload.AsEnumerable().Where(a => products.Select(b => b.OuterEan).Contains(a.Field<object>("Outer EAN")?.ToString()))
                                              .GroupBy(product => product.Field<object>("Outer EAN")?.ToString().Trim())
                                              .Select(group => group.First().Field<object>("Outer EAN")?.ToString()).ToArray();

                    if (existingProducts.Count() > 1)
                    {
                        string messages = string.Empty;
                        foreach (string msg in existingProducts)
                        {
                            messages += msg + "<br />";
                        }
                        ViewData["ValidationMsg"] = $"Below Outer EAN are already exists, please correct it and retry<br /><div style='max-height: 200px; overflow-y: auto; border: 1px solid #ccc; padding: 10px; background-color: #f9f9f9;'>{messages}</div>";
                        return Page();
                    }
                }

                await LoadBrands();
                await LoadCategories();
                await LoadSubCategories();

                await InsertNewBrands(productUpload);
                await InsertNewCategories(productUpload);
                await InsertNewSubCategories(productUpload);

                await LoadSuppliers();

                await InsertNewSuppliers(productUpload);

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
                Common.UploadSuppliers = null;
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
        /// LoadSuppliers
        /// </summary>
        /// <returns></returns>
        public async Task LoadSuppliers()
        {
            try
            {
                Common.UploadSuppliers = await _supplier.GetAllAsync(filter: x => x.IsDelete != true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"LoadSuppliers(), UploadDownload");
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
                if (!productUploadForBrands.Columns.Contains("Brand"))
                {
                    return; 
                }
                var newBrands = productUploadForBrands.AsEnumerable().Where(a => !Common.UploadBrands.Select(b => b.Name).Contains(a.Field<object>("Brand")?.ToString()))
                                              .GroupBy(brand => brand.Field<object>("Brand")?.ToString().Trim())
                                              .Select(group => group.First()).ToList();

                List<Brand> bulkInsertBrands = new List<Brand>();
                foreach (var brand in newBrands)
                {
                    var bulkInsertBrand = new Brand()
                    {
                        Name = brand.Field<object>("Brand")?.ToString(),
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
                if (!productUploadForCategories.Columns.Contains("Category"))
                {
                    return;
                }
                var newCategories = productUploadForCategories.AsEnumerable().Where(a => !Common.UploadCategories.Select(c => c.Name).Contains(a.Field<object>("Category")?.ToString()))
                              .GroupBy(category => category.Field<object>("Category")?.ToString())
                              .Select(group => group.First()).ToList();

                List<Category> bulkInsertCategories = new List<Category>();
                foreach (var category in newCategories)
                {
                    var bulkInsertCategory = new Category()
                    {
                        Name = category.Field<object>("Category")?.ToString(),
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
                if (!productUploadForSubCategories.Columns.Contains("SubCategory"))
                {
                    return;
                }
                var newSubCategories = productUploadForSubCategories.AsEnumerable().Where(a => !Common.UploadSubCategories.Select(c => c.Name).Contains(a.Field<object>("SubCategory")?.ToString()))
                              .GroupBy(subCategory => subCategory.Field<object>("SubCategory")?.ToString())
                              .Select(group => group.First()).ToList();

                List<SubCategory> bulkInsertSubCategories = new List<SubCategory>();
                foreach (var subCategory in newSubCategories)
                {
                    var bulkInsertSubCategory = new SubCategory()
                    {
                        Name = subCategory.Field<object>("SubCategory")?.ToString(),
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

        /// <summary>
        /// InsertNewSuppliers
        /// </summary>
        /// <param name="productUploadForCategories"></param>
        /// <returns></returns>
        private async Task InsertNewSuppliers(DataTable productUploadForSuppliers)
        {
            try
            {
                if (!productUploadForSuppliers.Columns.Contains("Supplier"))
                {
                    return;
                }
                var newSuppliers = productUploadForSuppliers.AsEnumerable().Where(a => !Common.UploadSuppliers.Select(c => c.Name).Contains(a.Field<object>("Supplier")?.ToString()))
                              .GroupBy(supplier => supplier.Field<object>("Supplier")?.ToString())
                              .Select(group => group.First()).ToList();

                List<Supplier> bulkInsertSuppliers = new List<Supplier>();
                foreach (var supplier in newSuppliers)
                {
                    var bulkInsertSupplier = new Supplier()
                    {
                        Name = supplier.Field<object>("Supplier")?.ToString(),
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDelete = false
                    };
                    bulkInsertSuppliers.Add(bulkInsertSupplier);
                }

                await _uploadDownloadDA.BulkInsertSupplierAsync(bulkInsertSuppliers);

                Common.UploadSuppliers.AddRange(bulkInsertSuppliers);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in InsertNewSuppliers(), UploadDownload");
                throw;
            }
        }
    }
}
