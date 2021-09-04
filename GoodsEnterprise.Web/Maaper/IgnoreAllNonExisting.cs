﻿using AutoMapper;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Web.Utilities;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace GoodsEnterprise.Web.Maaper
{
    public static class AutoMapperExtrension
    {
       
            public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>
(this IMappingExpression<TSource, TDestination> expression)
            {
                var flags = BindingFlags.Public | BindingFlags.Instance;
                var sourceType = typeof(TSource);
                var destinationProperties = typeof(TDestination).GetProperties(flags);

                foreach (var property in destinationProperties)
                {
                    if (sourceType.GetProperty(property.Name, flags) == null)
                    {
                        expression.ForMember(property.Name, opt => opt.Ignore());
                    }
                }
                return expression;
            }
        
    }
}
