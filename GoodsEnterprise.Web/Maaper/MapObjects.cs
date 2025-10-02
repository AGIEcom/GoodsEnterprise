using AutoMapper;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace GoodsEnterprise.Web.Maaper
{
    public class MapObjects:AutoMapper.Profile
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
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
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? SupplierId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Seebelow { get; set; }
        public string Seebelow1 { get; set; }
        public MapObjects()
        {
            CreateMap<DataRow, Product>()              
            .ForMember(d => d.Code, o => o.MapFrom(s => s["Product Code"]))
            .ForMember(d => d.InnerEan, o => o.MapFrom(s => s["Inner EAN"]))
            .ForMember(d => d.OuterEan, o => o.MapFrom(s => s["Outer EAN"]))
            .ForMember(d => d.Upc, o => o.MapFrom(s => Convert.ToInt32(string.IsNullOrEmpty(Convert.ToString(s["UPC"])) ? "0" : s["UPC"])))
            .ForMember(d => d.UnitSize, o => o.MapFrom(s => s["Unit Size"]))
            .ForMember(d => d.LayerQuantity, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Lyr Qty"])) ? "0" : s["Lyr Qty"])))
            .ForMember(d => d.PalletQuantity, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Plt Qty"])) ? "0" : s["Plt Qty"])))
            .ForMember(d => d.CasePrice, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Case Price"])) ? "0" : s["Case Price"])))
            .ForMember(d => d.ShelfLifeInWeeks, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Shelf Life(Weeks)"])) ? "0" : s["Shelf Life(Weeks)"])))
            .ForMember(d => d.PackWidth, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Pack Width"])) ? "0" : s["Pack Width"])))
            .ForMember(d => d.PackDepth, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Pack Depth"])) ? "0" : s["Pack Depth"])))
            .ForMember(d => d.PackHeight, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Pack Height"])) ? "0" : s["Pack Height"])))
            .ForMember(d => d.NetCaseWeightKg, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Net Case Weight KG"])) ? "0" : s["Net Case Weight KG"])))
            .ForMember(d => d.GrossCaseWeightKg, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Gross Case Weight KG"])) ? "0" : s["Gross Case Weight KG"])))
            .ForMember(d => d.CaseWidthMm, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Case Width (mm)"])) ? "0" : s["Case Width (mm)"])))
            .ForMember(d => d.CaseDepthMm, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Case Depth (mm)"])) ? "0" : s["Case Depth (mm)"])))
            .ForMember(d => d.CaseHeightMm, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Case Height (mm)"])) ? "0" : s["Case Height (mm)"])))
            .ForMember(d => d.PalletWeightKg, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Pallet Weight (kg)"])) ? "0" : s["Pallet Weight (kg)"])))
            .ForMember(d => d.PalletWidthMeter, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Pallet Width (m)"])) ? "0" : s["Pallet Width (m)"])))
            .ForMember(d => d.PalletDepthMeter, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Pallet Depth (m)"])) ? "0" : s["Pallet Depth (m)"])))
            .ForMember(d => d.PalletHeightMeter, o => o.MapFrom(s => Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(s["Pallet Height (m)"])) ? "0" : s["Pallet Height (m)"])))
            .ForMember(d => d.CreatedDate, o => o.MapFrom(s => DateTime.UtcNow))
            .ForMember(d => d.ExpiryDate, o => o.MapFrom(s => string.IsNullOrEmpty(Convert.ToString(s["Expiry Date"])) ? (DateTime?)null : DateTime.Parse(Convert.ToString(s["Expriy Date"]))))
            //.ForMember(d => d.Seebelow, o => o.MapFrom(s => Convert.ToString(string.IsNullOrEmpty(Convert.ToString(s["See below"])) ? "" : s["See below"])))
            .ForMember(d => d.Seebelow, o => o.MapFrom(s => Convert.ToString(s["See below"])))
            .ForMember(d => d.Seebelow1, o => o.MapFrom(s => Convert.ToString(string.IsNullOrEmpty(Convert.ToString(s["See below1"])) ? "" : s["See below1"])))
            .ForMember(d => d.Image, o => o.MapFrom(s => Convert.ToString(string.IsNullOrEmpty(Convert.ToString(s["Image"])) ? "" : s["Image"])))
            //.ForMember(d => d.Createdby, o => o.MapFrom(s => DateTime.UtcNow))
            .ForMember(d => d.IsActive, o => o.MapFrom(s => true))
            .ForMember(d => d.IsDelete, o => o.MapFrom(s => false))
            .AfterMap((src, dest) => dest.BrandId = Common.UploadBrands.Where(x => x.Name == Convert.ToString(src["Brand"])).Select(x => x.Id).FirstOrDefault())
            .AfterMap((src, dest) => dest.CategoryId = Common.UploadCategories.Where(x => x.Name == Convert.ToString(src["Category"])).Select(x => x.Id).FirstOrDefault())
            .AfterMap((src, dest) => dest.SubCategoryId = Common.UploadSubCategories.Where(x => x.Name == Convert.ToString(src["SubCategory"])).Select(x => x.Id).FirstOrDefault())
            .AfterMap((src, dest) => dest.SupplierId = Common.UploadSuppliers.Where(x => x.Name == Convert.ToString(src["Supplier"])).Select(x => x.Id).FirstOrDefault())
            //.IgnoreAllNonExisting()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
       
    }
}
