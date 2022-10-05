using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

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
        public async Task<IActionResult> Index(
            string sortOrder,
            Grade? currentFilter,
            string currentFilter2,
            string currentFilter3,
            Grade? searchGrade,
            string searchCourse,
            string searchStudent,
            int? pageNumber
            )
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentSortParm"] = String.IsNullOrEmpty(sortOrder) ? "student_desc" : "";
            ViewData["CourseSortParm"] = sortOrder == "course" ? "course_desc" : "course";
            ViewData["GradeSortParm"] = sortOrder == "grade" ? "grade_desc" : "grade";

            if (searchGrade != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchGrade = currentFilter;
            }

            if (searchCourse != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchCourse = currentFilter2;
            }

            if (searchStudent != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchStudent = currentFilter3;
            }

            ViewData["CurrentFilter"] = searchGrade;
            ViewData["CurrentFilter2"] = searchCourse;
            ViewData["CurrentFilter3"] = searchStudent;

            var enroll = from s in _context.Enrollments.Include(e => e.Course).Include(e => e.Student) 
                         select s;

            if (searchGrade != null)
            {
                enroll = enroll.Where(s => s.Grade == searchGrade);
            }
            if (!String.IsNullOrEmpty(searchCourse))
            {
                enroll = enroll.Where(s => s.Course.Title.Contains(searchCourse));
            }
            if (!String.IsNullOrEmpty(searchStudent))
            {
                enroll = enroll.Where(s => s.Student.FirstMidName.Contains(searchStudent) 
                                            || s.Student.LastName.Contains(searchStudent));
            }

            switch (sortOrder)
            {
                case "course":
                    enroll = enroll.OrderBy(s => s.Course.Title); // asc title
                    break;
                case "course_desc":
                    enroll = enroll.OrderByDescending(s => s.Course.Title); //desc last name
                    break;
                case "grade_desc":
                    enroll = enroll.OrderByDescending(s => s.Grade); // desc grade
                    break;
                case "grade":
                    enroll = enroll.OrderBy(s => s.Grade); // asc grade
                    break;
                case "student_desc":
                    enroll = enroll.OrderByDescending(s => s.Student.FirstMidName); // desc first name
                    break;
                default:
                    enroll = enroll.OrderBy(s => s.Student.FirstMidName); // asc first name
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Enrollment>.CreateAsync(enroll.AsNoTracking(), pageNumber ?? 1, pageSize));
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
