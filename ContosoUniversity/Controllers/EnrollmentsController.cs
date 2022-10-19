using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;

namespace ContosoUniversity.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly SchoolContext _context;

        public EnrollmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<ActionResult> Index(int? pageNumber)
        {
            var enroll = from s in _context.Enrollments.Include(e => e.Course).Include(e => e.Student)
                         select s;
            EnrollmentVM model = new EnrollmentVM();
            int pageSize = 5;
            var paged = await PaginatedList<Enrollment>.CreateAsync(enroll.AsNoTracking(), pageNumber ?? 1, pageSize);
            ViewBag.enroll = paged;

            ViewData["CourseID"] = new SelectList(_context.Courses, "Title", "Title");
            return View(model);
        }
        public async Task<IActionResult> IndexProses(
            string sortOrder,
            EnrollmentVM model,
            bool currentFilterA,
            bool currentFilterB,
            bool currentFilterC,
            bool currentFilterD,
            bool currentFilterF,
            string currentFilter2,
            string currentFilter3,
            int? pageNumber
            )
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentSortParm"] = String.IsNullOrEmpty(sortOrder) ? "student_desc" : "";
            ViewData["CourseSortParm"] = sortOrder == "course" ? "course_desc" : "course";
            ViewData["GradeSortParm"] = sortOrder == "grade" ? "grade_desc" : "grade";

            if (model.A != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.A = currentFilterA;
            }
            if (model.B != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.B = currentFilterB;
            }
            if (model.C != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.C = currentFilterC;
            }
            if (model.D != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.D = currentFilterD;
            }
            if (model.F != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.F = currentFilterF;
            }

            if (model.Course != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.Course = currentFilter2;
            }

            if (model.Student != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.Student = currentFilter3;
            }

            ViewData["CurrentFilterA"] = model.A;
            ViewData["CurrentFilterB"] = model.B;
            ViewData["CurrentFilterC"] = model.C;
            ViewData["CurrentFilterD"] = model.D;
            ViewData["CurrentFilterF"] = model.F;
            ViewData["CurrentFilter2"] = model.Course;
            ViewData["CurrentFilter3"] = model.Student;

            ViewData["CourseID"] = new SelectList(_context.Courses, "Title", "Title");

            var enroll = from s in _context.Enrollments.Include(e => e.Course).Include(e => e.Student)
                         select s;

            if (model.A == true)
            {
                enroll = enroll.Where(s => s.Grade.Equals(Grade.A));
            }
            if (model.B == true)
            {
                enroll = enroll.Where(s => s.Grade.Equals(Grade.B));
            }
            if (model.C == true)
            {
                enroll = enroll.Where(s => s.Grade.Equals(Grade.C));
            }
            if (model.D == true)
            {
                enroll = enroll.Where(s => s.Grade.Equals(Grade.D));
            }
            if (model.F == true)
            {
                enroll = enroll.Where(s => s.Grade.Equals(Grade.F));
            }
            if (!String.IsNullOrEmpty(model.Course))
            {
                enroll = enroll.Where(s => s.Course.Title.Contains(model.Course));
            }
            if (!String.IsNullOrEmpty(model.Student))
            {
                enroll = enroll.Where(s => s.Student.FirstMidName.Contains(model.Student) 
                                            || s.Student.LastName.Contains(model.Student));
            }

            enroll = sortOrder switch
            {
                "course" => enroll.OrderBy(s => s.Course.Title),// asc title
                "course_desc" => enroll.OrderByDescending(s => s.Course.Title),//desc last name
                "grade_desc" => enroll.OrderByDescending(s => s.Grade),// desc grade
                "grade" => enroll.OrderBy(s => s.Grade),// asc grade
                "student_desc" => enroll.OrderByDescending(s => s.Student.FirstMidName),// desc first name
                _ => enroll.OrderBy(s => s.Student.FirstMidName),// asc first name
            };

            model.EnrollmentList = new List<Enrollment>();
            foreach (var enr in enroll)
            {
                model.EnrollmentList.Add(enr);
            }
            int pageSize = 5;
            var paged = await PaginatedList<Enrollment>.CreateAsync(enroll.AsNoTracking(), pageNumber ?? 1, pageSize);
            ViewBag.enroll = paged;
            return View("Index", model);
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstMidName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(enrollment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                if (ModelState.IsValid)
                {
                    _context.Add(enrollment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
                ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstMidName", enrollment.StudentID);
            }
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstMidName", enrollment.StudentID);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollToUpdate = await _context.Enrollments.FirstOrDefaultAsync(s => s.EnrollmentID == id);
            if (await TryUpdateModelAsync<Enrollment>(
            enrollToUpdate,
            "",
            s => s.Grade, s => s.CourseID, s => s.StudentID))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollToUpdate.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstMidName", enrollToUpdate.StudentID);
            return View(enrollToUpdate);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enrollments == null)
            {
                return Problem("Entity set 'SchoolContext.Enrollments'  is null.");
            }
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
          return _context.Enrollments.Any(e => e.EnrollmentID == id);
        }
    }
}
