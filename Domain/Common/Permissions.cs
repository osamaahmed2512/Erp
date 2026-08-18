using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public static class Permissions
    {
        public const string Users_View = "Users.View";
        public const string Users_Create = "Users.Create";
        public const string Users_Edit = "Users.Edit";
        public const string Users_Delete = "Users.Delete";


        public const string Companies_View = "Companies.View";
        public const string Companies_Add = "Companies.Add";
        public const string Companies_Edit = "Companies.Edit";
        public const string Companies_Delete = "Companies.Delete";

        public const string Departement_View = "Departement.View";
        public const string Departement_Add = "Departement.Add";
        public const string Departement_Edit = "Departement.Edit";
        public const string Departement_Delete = "Departement.Delete";
        public const string Departement_ViewAll = "Departement.ViewAll";
        
        public const string Nationaality_ViewAll = "Nationaality.ViewAll";

        public const string WorkingSchedules_View = "WorkingSchedules.View";
        public const string WorkingSchedules_ViewAll = "WorkingSchedules.ViewAll";
        public const string WorkingSchedules_Add = "WorkingSchedules.Add";
        public const string WorkingSchedules_Edit = "WorkingSchedules.Edit";
        public const string WorkingSchedules_Delete = "WorkingSchedules.Delete";
        public static List<string> GetAll()
        {
            return typeof(Permissions)
                .GetFields(System.Reflection.BindingFlags.Public |
                           System.Reflection.BindingFlags.Static)
                .Select(x => x.GetValue(null)?.ToString()!)
                .ToList();
        }
    }
}
