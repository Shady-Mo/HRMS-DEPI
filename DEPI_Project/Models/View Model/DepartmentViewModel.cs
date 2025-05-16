using System.ComponentModel.DataAnnotations;

namespace DEPI_Project.Models.View_Model {
    public class DepartmentViewModel {
        public string Name { get; set; }
        public int EmployeeCapacity { get; set; }
        public int EstablishedYear { get; set; }
        public string Location { get; set; }
    }
}
