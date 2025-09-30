using BenchmarkDotNet.Attributes;
using EffectiveMobileTestTask.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectiveMobileTestTask.Benchmark
{

    [MemoryDiagnoser]
    public class CompanySearchBenchmark
    {
        private CompanyRegionTree _tree;
        private RegionPathTreeController _controller;

        [GlobalSetup]
        public void Setup()
        {
            // Создаём тестовые данные
            var companies = GetTestCompanies();

            // Инициализируем дерево
            _tree = new CompanyRegionTree();
            foreach (var company in companies)
            {
                _tree.AddCompany(company);
            }

            // Инициализируем контроллер
            _controller = new RegionPathTreeController(_tree, false);
            _controller.BuildIndex(); // строим индекс
        }

        [Benchmark]
        public List<Company> SearchViaTree()
        {
            return _tree.FindCompaniesByPath("/ru");
        }

        [Benchmark]
        public async Task<IReadOnlyCollection<Company>> SearchViaController()
        {
            return await _controller.FindCompaniesByPath("/ru");
        }

        private static List<Company> GetTestCompanies()
        {
            var companies = new List<Company>();

            // Добавим больше данных для реалистичного теста
            for (int i = 0; i < 1000; i++)
            {
                var company = new Company { Name = $"Company {i}" };
                company.Paths.Add($"/ru/msk/{i}");
                company.Paths.Add($"/ru/spb/{i}");
                company.Paths.Add($"/ru/svrd/{i}");
                companies.Add(company);
            }

            return companies;
        }
    }
}
