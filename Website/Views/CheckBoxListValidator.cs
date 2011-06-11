namespace Opuno.Brenn.Website.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class CheckBoxListAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return false;
        }
    }

    public class CheckBoxListValidator : DataAnnotationsModelValidator<ValidationAttribute>
    {
        public CheckBoxListValidator(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute)
            : base(metadata, context, attribute)
        {  
        }
    }
}
