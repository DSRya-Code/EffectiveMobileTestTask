using EffectiveMobileTestTask.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectiveMobileTestTask.Tests
{
    public class CompanyRegionTreeTestContext
    {
        public CompanyRegionTree Tree { get; }

        public CompanyRegionTreeTestContext()
        {
            var companies = CompanySetUp1();
            Tree = new CompanyRegionTree();

            foreach (var company in companies)
            {
                Tree.AddCompany(company);
            }
        }

        public static List<Company> CompanySetUp1()
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

            var result = new List<Company>();
            result.Add(company1);
            result.Add(company2);
            result.Add(company3);
            result.Add(company4);

            return result;
        }
    }
}
