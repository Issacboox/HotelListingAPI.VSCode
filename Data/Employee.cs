using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListingAPI.VSCode.Data
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
     
        [ForeignKey("DepartmentId")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}