using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ContosoUniversity.Models.SchoolViewModels
{

    public class CourseVM : IValidatableObject
    {
        public List<Course> CourseList { get; set; }

        [Display(Name = "Title :")] // attribute
        public string Title { get; set; }

        [Display(Name = "Credit :")] // attribute
        public string Credit { get; set; } 

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Title == null)
            {
                yield return new ValidationResult("input title!", new[] { "Title" });
                //Memberi validation jika sebuah kolom pencarian tidak diisi!
            }

            if (Credit == null)
            {
                yield return new ValidationResult("input credit!", new[] { "Credit" });
                //Memberi validation ke variable yang dituju!
            }
        }
    }


}
