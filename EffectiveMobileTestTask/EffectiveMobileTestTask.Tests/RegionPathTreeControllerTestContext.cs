using EffectiveMobileTestTask.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectiveMobileTestTask.Tests
{
    public class RegionPathTreeControllerTestContext
    {
        public CompanyRegionTree Tree { get; }
        public RegionPathTreeController Controller { get; }

        public RegionPathTreeControllerTestContext()
        {
            Tree = new CompanyRegionTree();

            var companies = GetTestCompanies();
            foreach (var company in companies)
            {
                Tree.AddCompany(company);
            }

            Controller = new RegionPathTreeController(Tree);
            Controller.BuildIndex();
        }

        private static List<Company> GetTestCompanies()
        {
            var company1 = new Company { Name = "Яндекс.Директ" };
            company1.Paths.Add("/ru");

            var company2 = new Company { Name = "Ревдинский рабочий" };
            company2.Paths.Add("/ru/svrd/revda");
            company2.Paths.Add("/ru/svrd/pervik");

            var company3 = new Company { Name = "Газета уральских москвичей" };
            company3.Paths.Add("/ru/msk");
            company3.Paths.Add("/ru/permobl");
            company3.Paths.Add("/ru/chelobl");

            var company4 = new Company { Name = "Крутая реклама" };
            company4.Paths.Add("/ru/svrd");

            return new List<Company> { company1, company2, company3, company4 };
        }
    }
}
