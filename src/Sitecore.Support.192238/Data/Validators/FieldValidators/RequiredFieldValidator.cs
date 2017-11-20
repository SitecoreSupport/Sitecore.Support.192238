using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Sitecore.Data.Validators;

namespace Sitecore.Support.Data.Validators.FieldValidators
{
  [Serializable]
  public class RequiredFieldValidator : Sitecore.Support.Data.Validators.StandardValidator
  {
    // Methods
    public RequiredFieldValidator()
    {
    }

    public RequiredFieldValidator(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    protected override ValidatorResult Evaluate()
    {
      if (!string.IsNullOrEmpty(base.ControlValidationValue))
      {
        return ValidatorResult.Valid;
      }
      base.Text = base.GetText("Field \"{0}\" must contain a value.", new string[] { base.GetFieldDisplayName() });
      return base.GetFailedResult(ValidatorResult.CriticalError);
    }

    protected override ValidatorResult GetMaxValidatorResult() =>
      base.GetFailedResult(ValidatorResult.CriticalError);

    // Properties
    public override string Name =>
      "Required";
  }
}