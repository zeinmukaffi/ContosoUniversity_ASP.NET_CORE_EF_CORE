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
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // action to View => Students => Index
        public async Task<IActionResult> Index( 
            StudentVM model,
            string sortOrder, 
            string currentFilter,
            string currentFilter2,
            DateTime? currentFilter3,
            DateTime? currentFilter4, 
            int? pageNumber
            )
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["FirstSortParm"] = String.IsNullOrEmpty(sortOrder) ? "first_desc" : "";

            // paging //
            if (model.FirstMidName != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.FirstMidName = currentFilter;
            }

            if (model.LastName != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.LastName = currentFilter2;
            }

            if (model.EnrollmentDateFrom != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.EnrollmentDateFrom = currentFilter3;
            }

            if (model.EnrollmentDateUntil != null)
            {
                pageNumber = 1;
            }
            else
            {
                model.EnrollmentDateUntil = currentFilter4;
            }

            ViewData["CurrentFilter"] = model.FirstMidName;
            ViewData["CurrentFilter2"] = model.LastName;
            ViewData["CurrentFilter3"] = model.EnrollmentDateFrom;
            ViewData["CurrentFilter4"] = model.EnrollmentDateUntil;
            var students = from s in _context.Students
                           select s;

            // filtering //
            if (model.EnrollmentDateFrom != null && model.EnrollmentDateUntil != null)
            {
                students = students.Where(s => s.EnrollmentDate >= model.EnrollmentDateFrom && s.EnrollmentDate <= model.EnrollmentDateUntil);
            }
            if (!String.IsNullOrEmpty(model.LastName))
            {
                students = students.Where(s => s.LastName.Contains(model.LastName));
            }
            if (!String.IsNullOrEmpty(model.FirstMidName))
            {
                students = students.Where(s => s.FirstMidName.Contains(model.FirstMidName));
            }

            // sorting // 
            students = sortOrder switch
            {
                "name" => students.OrderBy(s => s.LastName),// asc last name
                "name_desc" => students.OrderByDescending(s => s.LastName),//desc last name
                "first_desc" => students.OrderByDescending(s => s.FirstMidName),// desc first name
                "Date" => students.OrderBy(s => s.EnrollmentDate),// asc date
                "date_desc" => students.OrderByDescending(s => s.EnrollmentDate),// desc date
                _ => students.OrderBy(s => s.FirstMidName),// asc first name
            };

            model.StundentList = new List<Student>();
            foreach (var student in students)
            {
                model.StundentList.Add(student);
            }
            int pageSize = 5;
            var paged = await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize);
            ViewBag.students = paged;
            return View(model);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("EnrollmentDate,FirstMidName,LastName")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
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
            return View(studentToUpdate);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }

    }
}
