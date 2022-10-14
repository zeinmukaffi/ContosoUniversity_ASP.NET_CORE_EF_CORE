using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ContosoUniversity.Models.SchoolViewModels
{

    public class CourseVM : IValidatableObject
    {
        public List<Course> CourseList { get; set; }

        [Display(Name = "Title :")] // attribute
        public string Title { get; set; }

        [Display(Name = "Range Credit From :")] // attribute
        public int? CreditFrom { get; set; } 
        
        [Display(Name = "Until :")] // attribute
        public int? CreditUntil { get; set; } 

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Title == null && CreditFrom == null && CreditUntil == null)
            {
                yield return new ValidationResult("search minimal 1 data!");
                //Memberi validation jika sebuah kolom pencarian tidak diisi!
            }

            if (CreditFrom != null)
            {
                yield return new ValidationResult("input range until!", new[] { "CreditUntil" });
                //Memberi validation ke variable yang dituju!
            }
            
            if (CreditUntil != null)
            {
                yield return new ValidationResult("input range from!", new[] { "CreditFrom" });
                //Memberi validation ke variable yang dituju!
            }
        }
    }


}
