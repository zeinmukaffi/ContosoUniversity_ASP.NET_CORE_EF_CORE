using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ContosoUniversity.Models.SchoolViewModels
{

    public class EnrollmentVM : IValidatableObject
    {
        public List<Enrollment> EnrollmentList { get; set; }

        [Display(Name = "Grade :")] // attribute
        public Grade? Grade { get; set; }

        [Display(Name = "Course :")] // attribute
        public string Course { get; set; }        
        
        [Display(Name = "Student :")] // attribute
        public string Student { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Grade == null && Course == null && Student == null)
            {
                yield return new ValidationResult("search minimal 1 data!");
                //Memberi validation jika sebuah kolom pencarian tidak diisi!
            }
        }
    }
}
