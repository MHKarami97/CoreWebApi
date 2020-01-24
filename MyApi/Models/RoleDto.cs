using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MyApi.Models
{
    public class RoleDto : IValidatableObject
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Regex.IsMatch(Name, "^[a-zA-Z0-9]*$"))
                yield return new ValidationResult("نقش نمی تواند شامل حروف فارسی باشد", new[] { nameof(Name) });
        }
    }
}
