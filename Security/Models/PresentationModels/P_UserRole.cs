using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TsrmWebApi.Security.Models.PresentationModels
{
    public class P_UserRole
    {

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string RoleCat { get; set; }
        public string RoleCatDescription { get; set; }
    }

    public class P_UserRoleNew
    {

        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string RoleCat { get; set; }

    }

    public class P_ApplicationMenu
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuComponent { get; set; }
        public string MenuPath { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; }
        public string DisplayName { get; set; }
        public string YsnChils { get; set; }
        public string SecurityRole { get; set; }
        public string MenuIcon { get; set; }
        public string ApplicationType { get; set; }
        public string YsnPublished { get; set; }
    }

    public class P_ApplicationMenuNewMaster
    {

        public string MenuName { get; set; }


        //public int ParentId { get; set; }
        public string DisplayName { get; set; }
        //public string YsnChils { get; set; }

        public string ApplicationType { get; set; }
        //public string YsnPublished { get; set; }


    }

    public class P_ApplicationMenuNewMasterForAll
    {

        public int MenuId { get; set; }
        public string MenuName { get; set; }

        public int ParentId { get; set; }
        public string DisplayName { get; set; }
        public string YsnChils { get; set; }

        public string ApplicationType { get; set; }
        public string YsnPublished { get; set; }


    }

    public class P_ApplicationMenuForSub
    {

        public string MenuName { get; set; }
        public string MenuComponent { get; set; }
        public string MenuPath { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; }
        public string DisplayName { get; set; }
        //public string YsnChils { get; set; }
        public string SecurityRole { get; set; }
        public string MenuIcon { get; set; }
        public string ApplicationType { get; set; }
        //public string YsnPublished { get; set; }
    }


    public class P_SalesPercentage
    {

        public string MonthId { get; set; }

        public int YearId { get; set; }

        public decimal TotalSales { get; set; }
    }


    public class P_StockReportNew
    {

        public decimal ZoneId { get; set; }

        public string ZoneName { get; set; }

        public decimal DivId { get; set; }

        public string DivName { get; set; }

        public decimal AreaId { get; set; }

        public string AreaName { get; set; }

        public decimal DispointId { get; set; }

        public string DispointName { get; set; }

        public decimal ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductCategory { get; set; }

        public decimal StockQty { get; set; }

        public decimal? StockValue { get; set; }

    }
}
