using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GoodsEnterprise.Model.Models
{
    public class ProductList
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string OuterEan { get; set; }
        public int FilterTotalCount { get; set; }
        public string Status { get; set; }
        //public DateTime? ModifiedDate { get; set; }
    }

    public class PromotionCostList
    {
        [Key]
        public int PromotionCostID { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public decimal PromotionCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int FilterTotalCount { get; set; }
        public string Status { get; set; }
    }

    public class BaseCostList
    {
        [Key]
        public int BaseCostID { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public decimal BaseCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int FilterTotalCount { get; set; }
        public string Status { get; set; }
        //public DateTime? ModifiedDate { get; set; }
    }

    public class SupplierList
    {
        [Key]
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string SKUCode { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int FilterTotalCount { get; set; }
        //public DateTime? ModifiedDate { get; set; }
    }

    // Enhanced SupplierImport class for Excel import functionality
    // Note: Validation attributes are now dynamically applied based on JSON configuration
    // Use SupplierImportValidationService for runtime validation
    public class SupplierImport
    {
        [Key]
        public int Id { get; set; }
        
        // All fields are now dynamically validated based on JSON configuration
        // Required/Optional status and validation rules are determined at runtime
        
        public string SupplierName { get; set; }
        public string SKUCode { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool? IsPreferred { get; set; } = false;
        public int? LeadTimeDays { get; set; }
        public string MoqCase { get; set; }
        public decimal? LastCost { get; set; }
        public string Incoterm { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        
        // Import metadata
        public int RowNumber { get; set; }
        public bool HasErrors { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public List<string> ValidationWarnings { get; set; } = new List<string>();
    }
}
