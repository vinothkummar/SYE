using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace GDSHelpers
{
    public static class MvcHelpers
    {

        //public static IHtmlContent InsetText(
        //    this IHtmlHelper helper,
        //    string content,
        //    object htmlAttributes = null)
        //{
        //    var insetDiv = new TagBuilder("div");
        //    insetDiv.InnerHtml.AppendHtml(content);
        //    insetDiv.AddCssClass("govuk-inset-text");

        //    if (htmlAttributes != null)
        //    {
        //        var htmlAttributesDictionary = MergeClassAttributes(htmlAttributes, "form-control");
        //        insetDiv.MergeAttributes(new RouteValueDictionary(htmlAttributesDictionary));
        //    }

        //    return new HtmlString(insetDiv.ToString());
        //}

        //public static IHtmlContent InsetTextFor<TModel, TProperty>(
        //    this IHtmlHelper<TModel> helper,
        //    Expression<Func<TModel, TProperty>> expression,
        //    object htmlAttributes = null)
        //{
        //    var insetDiv = new TagBuilder("div");
        //    insetDiv.InnerHtml.AppendHtml(GetValue(helper, expression).ToString());
        //    insetDiv.AddCssClass("govuk-inset-text");

        //    if (htmlAttributes != null)
        //    {
        //        var htmlAttributesDictionary = MergeClassAttributes(htmlAttributes, "form-control");
        //        insetDiv.MergeAttributes(new RouteValueDictionary(htmlAttributesDictionary));
        //    }

        //    return new HtmlString(insetDiv.ToString());
        //}



        //private static Dictionary<string, object> MergeClassAttributes(object htmlAttributes, string cssClassName)
        //{
        //    var htmlAttributesDictionary = new Dictionary<string, object>
        //    {
        //        ["class"] = cssClassName
        //    };

        //    if (htmlAttributes != null)
        //    {
        //        foreach (var prop in htmlAttributes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        //        {
        //            if (htmlAttributesDictionary.ContainsKey(prop.Name))
        //            {
        //                htmlAttributesDictionary[prop.Name] = $"{htmlAttributesDictionary[prop.Name]} {prop.GetValue(htmlAttributes, null)}";
        //            }
        //            else
        //            {
        //                htmlAttributesDictionary.Add(prop.Name, prop.GetValue(htmlAttributes, null));
        //            }
        //        }
        //    }
        //    return htmlAttributesDictionary;
        //}


        //private static object GetValue<TModel, TProperty>(IHtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        //{
        //    var valueGetter = expression.Compile();
        //    var value = valueGetter(helper.ViewData.Model);
        //    if (value == null)
        //    {
        //        return "";
        //    }
        //    return value;
        //}


    }
}
