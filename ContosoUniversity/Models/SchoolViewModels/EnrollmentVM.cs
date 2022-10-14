using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ContosoUniversity.Models.SchoolViewModels
{
    //public enum Grades
    //{
    //    A, B, C, D, F
    //}
    public class EnrollmentVM : IValidatableObject
    {
        public List<Enrollment> EnrollmentList { get; set; }

        [Display(Name = "Grade :")] // attribute
        public string Grade { get; set; }

        [Display(Name = "Course :")] // attribute
        public string Course { get; set; }        
        
        [Display(Name = "Student :")] // attribute
        public string Student { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Grade == null)
            {
                yield return new ValidationResult("input grade!", new[] { "Grade" });
                //Memberi validation jika sebuah kolom pencarian tidak diisi!
            }

            if (Course == null)
            {
                yield return new ValidationResult("input course's name!", new[] { "Course" });
                //Memberi validation ke variable yang dituju!
            }
            
            if (Student == null)
            {
                yield return new ValidationResult("input student's name!", new[] { "Student" });
                //Memberi validation ke variable yang dituju!
            }
        }
    }
}
