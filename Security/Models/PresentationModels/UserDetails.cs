namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class UserDetails
    {
    }

    public class UserDetailsFinal
    {
        public int Employee_ID { get; set; }
        public string Employee_Code { get; set; }
        public string Employee_Name { get; set; }
        public int Unit_ID { get; set; }
        public List<ProjectRoleFinal>? Users_Role { get; set; }
    }

    public class ProjectRoleFinal
    {
        public int Project_Id { get; set; }
        public List<int>? Roles { get; set; }
    }
}
