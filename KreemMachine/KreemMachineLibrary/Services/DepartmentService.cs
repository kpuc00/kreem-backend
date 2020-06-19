using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Exceptions;
using System.Data.Entity;

namespace KreemMachineLibrary.Services
{
    public class DepartmentService
    {
        public void SaveToDatabase(Department department)
        {
            using (var db = new DataBaseContext())
            {
                var temp = from d in db.Departments select d.Name;
                if (temp.ToList().Contains(department.Name))
                {
                    throw new DepartmentExistsException("Department already exists");
                }

                db.Departments.Add(department);
                db.SaveChanges();
            }
        }

        public void Update(Department department)
        {
            using (var db = new DataBaseContext())
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public int Delete(Department department)
        {
            department.Deleted = 1;

            using (var db = new DataBaseContext())
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
            }

            return 1;
        }

        public List<Department> GetAllViewable()
        {
            using (var db = new DataBaseContext())
            {
                return db.Departments.Where(d => d.Deleted == 0).ToList();
            }
        }

        public List<Department> GetAll()
        {
            using (var db = new DataBaseContext())
            {
                return db.Departments.ToList();
            }
        }

        public List<Department> FilterDepartments(string s)
        {
            using (var db = new DataBaseContext())
            {
                var departments = db.Departments.Where(d => d.Deleted == 0);

                    return departments
                        .Where(d => d.Name.ToLower().Contains(s.ToLower()))
                        .ToList();
            }
        }
    }
}

