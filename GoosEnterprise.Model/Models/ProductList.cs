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
    public class SupplierImport : IImportModel
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

    // Product Import class for Excel import functionality
    public class ProductImport : IImportModel
    {
        [Key]
        public int Id { get; set; }
        
        // Product fields for import
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; } // For lookup during import
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } // For lookup during import
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } // For lookup during import
        public string InnerEan { get; set; }
        public string OuterEan { get; set; }
        public string UnitSize { get; set; }
        public int? Upc { get; set; }
        public int? LayerQuantity { get; set; }
        public int? PalletQuantity { get; set; }
        public decimal? CasePrice { get; set; }
        public int? ShelfLifeInWeeks { get; set; }
        public decimal? PackHeight { get; set; }
        public decimal? PackDepth { get; set; }
        public decimal? PackWidth { get; set; }
        public decimal? NetCaseWeightKg { get; set; }
        public decimal? GrossCaseWeightKg { get; set; }
        public decimal? CaseWidthMm { get; set; }
        public decimal? CaseHeightMm { get; set; }
        public decimal? CaseDepthMm { get; set; }
        public decimal? PalletWeightKg { get; set; }
        public decimal? PalletWidthMeter { get; set; }
        public decimal? PalletHeightMeter { get; set; }
        public decimal? PalletDepthMeter { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; } = true;
        public bool isTaxable { get; set; } = false;
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; } // For lookup during import
        public DateTime? ExpriyDate { get; set; }
        public int? TaxslabId { get; set; }
        public string TaxslabName { get; set; } // For lookup during import
        
        // Import metadata
        public int RowNumber { get; set; }
        public bool HasErrors { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public List<string> ValidationWarnings { get; set; } = new List<string>();
    }

    // BaseCost Import class for Excel import functionality
    public class BaseCostImport : IImportModel
    {
        [Key]
        public int Id { get; set; }
        
        // BaseCost fields for import
        public int? ProductId { get; set; }
        public string ProductCode { get; set; } // For lookup during import
        public string ProductName { get; set; } // For lookup during import
        public decimal? BaseCost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remark { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; } // For lookup during import
        public bool IsActive { get; set; } = true;
        
        // Import metadata
        public int RowNumber { get; set; }
        public bool HasErrors { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public List<string> ValidationWarnings { get; set; } = new List<string>();
    }

    // PromotionCost Import class for Excel import functionality
    public class PromotionCostImport : IImportModel
    {
        [Key]
        public int Id { get; set; }
        
        // PromotionCost fields for import
        public int? ProductId { get; set; }
        public string ProductCode { get; set; } // For lookup during import
        public string ProductName { get; set; } // For lookup during import
        public decimal? PromotionCost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remark { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; } // For lookup during import
        public bool IsActive { get; set; } = true;
        
        // Import metadata
        public int RowNumber { get; set; }
        public bool HasErrors { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public List<string> ValidationWarnings { get; set; } = new List<string>();
    }

    // Base interface for all import models
    public interface IImportModel
    {
        int RowNumber { get; set; }
        bool HasErrors { get; set; }
        List<string> ValidationErrors { get; set; }
        List<string> ValidationWarnings { get; set; }
    }
}
