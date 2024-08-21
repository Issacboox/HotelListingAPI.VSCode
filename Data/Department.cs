namespace HotelListingAPI.VSCode.Data
{
    public class Department
    {
        public int Id { get; set; }
        public string DeptName { get; set; }
        public virtual IList<Employee> Employees { get; set; }
    }
}