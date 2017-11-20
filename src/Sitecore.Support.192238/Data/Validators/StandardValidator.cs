using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.UI;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Validators;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Reflection;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.ContentEditor;

namespace Sitecore.Support.Data.Validators
{
  public abstract class StandardValidator : Sitecore.Data.Validators.StandardValidator
  {
    protected StandardValidator()
    {
    }

    protected StandardValidator(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
    protected override Item GetItem()
    {
      if (ItemUri == null)
      {
        return null;
      }
      Item item = Database.GetItem(ItemUri);
      // Added '&& item.Template.StandardValues.Versions.Count == 0' to fix the issue #192238
      if ((item == null) || (item.Versions.Count == 0 && item.Template.StandardValues.Versions.Count == 0))
      {
        return null;
      }
      this.UpdateItem(item);
      return item;
    }
    private void UpdateItem(Item item)
    {
      Assert.ArgumentNotNull(item, "item");
      using (new SecurityDisabler())
      {
        item.Editing.BeginEdit();
      }
      Page handler = null;
      HttpContext current = HttpContext.Current;
      if (current != null)
      {
        handler = current.Handler as Page;
      }
      foreach (Field field in item.Fields)
      {
        string controlToValidate = this.ControlToValidate;
        if (!string.IsNullOrEmpty(controlToValidate) && (field.ID == this.FieldID))
        {
          string property = (current != null) ? RuntimeValidationValues.Current[controlToValidate] : null;
          if ((handler != null) && (property != null))
          {
            Control control = handler.FindControl(controlToValidate);
            if (control != null)
            {
              IContentField field2 = control as IContentField;
              if (field2 != null)
              {
                property = field2.GetValue();
              }
              else
              {
                ValidationPropertyAttribute attribute = ReflectionUtil.GetAttribute(control, typeof(ValidationPropertyAttribute)) as ValidationPropertyAttribute;
                if (attribute != null)
                {
                  property = ReflectionUtil.GetProperty(control, attribute.Name) as string;
                }
              }
            }
          }
          if ((property == null) && (current != null))
          {
            property = current.Request.Form[controlToValidate];
          }
          if ((property != null) && (property != "__#!$No value$!#__"))
          {
            field.Value = property;
          }
        }
      }
    }
  }
}