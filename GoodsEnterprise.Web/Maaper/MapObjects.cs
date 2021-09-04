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
        //public int Id { get; set; }
        //public string Code { get; set; }
        //public int? BrandId { get; set; }
        //public int? CategoryId { get; set; }
        //public int? SubCategoryId { get; set; }
        //public string InnerEan { get; set; }
        //public string OuterEan { get; set; }
        //public string PackSize { get; set; }
        //public int? Upc { get; set; }
        //public int? LayerQuantity { get; set; }
        //public int? PalletQuantity { get; set; }
        //public decimal? Height { get; set; }
        //public decimal? Weight { get; set; }
        //public decimal? Width { get; set; }
        //public decimal? NetWeight { get; set; }
        //public decimal? Depth { get; set; }
        //public string Image { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public int? Createdby { get; set; }
        //public DateTime? ModifiedDate { get; set; }
        //public int? Modifiedby { get; set; }
        //public bool IsActive { get; set; }
        //public bool IsDelete { get; set; }
        public MapObjects()
        {
            CreateMap<DataRow, Product>()              
            .ForMember(d => d.Code, o => o.MapFrom(s => s["Product Code"]))
            .ForMember(d => d.InnerEan, o => o.MapFrom(s => s["Inner EAN"]))
            .ForMember(d => d.OuterEan, o => o.MapFrom(s => s["Outer EAN"]))
            //.ForMember(d => d., o => o.MapFrom(s => s["Product Description"]))
            .ForMember(d => d.Upc, o => o.MapFrom(s => Convert.ToInt32(s["UPC"])))
            .ForMember(d => d.PackSize, o => o.MapFrom(s => s["Unit Size"]))
            .ForMember(d => d.LayerQuantity, o => o.MapFrom(s => Convert.ToInt32(s["Lyr Qty"])))
            .ForMember(d => d.PalletQuantity, o => o.MapFrom(s => Convert.ToInt32(s["Plt Qty"])))
            //.ForMember(d => d., o => o.MapFrom(s => s["Case Price"]))
            //.ForMember(d => d., o => o.MapFrom(s => s["Shelf Life"]))
            .ForMember(d => d.CreatedDate, o => o.MapFrom(s => DateTime.UtcNow))
            //.ForMember(d => d.Createdby, o => o.MapFrom(s => DateTime.UtcNow))
            .ForMember(d => d.IsActive, o => o.MapFrom(s => true))
            .ForMember(d => d.IsDelete, o => o.MapFrom(s => false))
            .AfterMap((src, dest) => dest.BrandId = Common.UploadBrands.Where(x => x.Name == Convert.ToString(src["Brand"])).Select(x => x.Id).FirstOrDefault())
            .AfterMap((src, dest) => dest.CategoryId = Common.UploadCategories.Where(x => x.Name == Convert.ToString(src["Category"])).Select(x => x.Id).FirstOrDefault())
            //.IgnoreAllNonExisting()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
       
    }
}
