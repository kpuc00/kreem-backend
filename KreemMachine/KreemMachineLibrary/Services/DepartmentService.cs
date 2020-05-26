using KreemMachineLibrary.Exceptions;
using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class DepartmentService
    {
        public DepartmentService() { }

        public void SaveToDatabase(Department department) {
            using (var db = new DataBaseContext()) {
                var temp = from d in db.Departments select d.Name;
                if (temp.ToList().Contains(department.Name)) {
                    throw new DepartmentExistsException("Department already exists");
                }

                db.Departments.Add(department);
                db.SaveChanges();
            }
        }

        public void Update(Department department) {
            using (var db = new DataBaseContext()) {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Delete(Department department) {
            using (var db = new DataBaseContext()) {
                db.Departments.Remove(department);
                db.SaveChanges();
            }
        }

        public List<Department> GetAll() {
            using (var db = new DataBaseContext()) {
                return db.Departments.ToList();
            }
        }
    }
}
