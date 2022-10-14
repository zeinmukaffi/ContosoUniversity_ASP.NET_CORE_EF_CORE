using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ContosoUniversity.Models.SchoolViewModels
{

    public class StudentVM : IValidatableObject
    {
        public List<Student> StundentList { get; set; }

        [Display(Name = "Last Name :")] // attribute
        public string LastName { get; set; } 
         
        [Display(Name = "First Name :")] // attribute
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)] // attribute
        [Display(Name = "Date From :")]
        public DateTime? EnrollmentDateFrom { get; set; } // ? (NULLABLE)

        [DataType(DataType.Date)] // attribute
        [Display(Name = "Until :")]
        public DateTime? EnrollmentDateUntil { get; set; } // ? (NULLABLE)

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FirstMidName == null && LastName == null && EnrollmentDateFrom == null && EnrollmentDateUntil == null)
            {
                yield return new ValidationResult("search minimal 1 data!");
                //Memberi validation jika sebuah kolom pencarian tidak diisi!
            }

            if (EnrollmentDateFrom != null)
            {
                if(EnrollmentDateUntil == null)
                yield return new ValidationResult("input date until!", new[] { "EnrollmentDateUntil" });
                //Memberi validation ke variable yang dituju!
            }
            
            if (EnrollmentDateUntil != null)
            {
                if(EnrollmentDateFrom == null)
                yield return new ValidationResult("input date from!", new[] { "EnrollmentDateFrom" });
                //Memberi validation ke variable yang dituju!
            }
   
        }
    }


}
