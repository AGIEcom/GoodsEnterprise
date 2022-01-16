using EFCore.BulkExtensions;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.CustomerModel;
using GoodsEnterprise.Model.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GoodsEnterprise.DataAccess.Implementation
{
    /// <summary>
    /// AdoDA
    /// </summary>
    public class AdoDA : IAdoDA
    {
        public AdoDA()
        {

        }
      
        public async Task<List<HomePageBrand>> GetHomePageBrandAsync()
        {
            List<HomePageBrand> lstHomePageBrand = new List<HomePageBrand>();
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-FVJMKLK;Initial Catalog=GoodsEnterpriseNew;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand("SPUI_GetHomePageBrands", con);
                // Configure command and add parameters.
                cmd.CommandType = CommandType.StoredProcedure;              
                // Execute the command.
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            HomePageBrand homePageBrand = new HomePageBrand();
                            homePageBrand.BrandId = Convert.ToInt32(reader["BrandId"]);
                            homePageBrand.BrandName = Convert.ToString(reader["BrandName"]);
                            homePageBrand.ProductCount = Convert.ToInt32(reader["ProductCount"]);
                            lstHomePageBrand.Add(homePageBrand);
                        }
                    }
                    reader.Close();
                }
                con.Close();
            }
            return lstHomePageBrand;
        }

        public async Task<List<HomePageCategory>> GetHomePageCategoryAsync()
        {
            List<HomePageCategory> lstHomePageCategory = new List<HomePageCategory>();
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-FVJMKLK;Initial Catalog=GoodsEnterpriseNew;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand("SPUI_GetHomePageCategories", con);
                // Configure command and add parameters.
                cmd.CommandType = CommandType.StoredProcedure;
                // Execute the command.
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            HomePageCategory homePageCategory = new HomePageCategory();
                            homePageCategory.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                            homePageCategory.CategoryName = Convert.ToString(reader["CategoryName"]);
                            homePageCategory.ProductCount = Convert.ToInt32(reader["ProductCount"]);
                            lstHomePageCategory.Add(homePageCategory);
                        }
                    }
                    reader.Close();
                }
                con.Close();
            }
            return lstHomePageCategory;
        }
    }
}
