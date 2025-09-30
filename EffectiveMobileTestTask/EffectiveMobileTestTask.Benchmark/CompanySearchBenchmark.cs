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
            var companies = GenerateLargeTestData();

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

        // === Короткие пути ===
        [Benchmark]
        public List<Company> SearchViaTree_ShortPath()
        {
            return _tree.FindCompaniesByPath("/ru");
        }

        [Benchmark]
        public async Task<IReadOnlyCollection<Company>> SearchViaController_ShortPath()
        {
            return await _controller.FindCompaniesByPath("/ru");
        }

        // === Средние пути ===
        [Benchmark]
        public List<Company> SearchViaTree_MediumPath()
        {
            return _tree.FindCompaniesByPath("/ru/msk/50");
        }

        [Benchmark]
        public async Task<IReadOnlyCollection<Company>> SearchViaController_MediumPath()
        {
            return await _controller.FindCompaniesByPath("/ru/msk/50");
        }

        // === Длинные пути ===
        [Benchmark]
        public List<Company> SearchViaTree_LongPath()
        {
            return _tree.FindCompaniesByPath("/ru/msk/99/district/7/building/2");
        }

        [Benchmark]
        public async Task<IReadOnlyCollection<Company>> SearchViaController_LongPath()
        {
            return await _controller.FindCompaniesByPath("/ru/msk/99/district/7/building/2");
        }

        private static List<Company> GenerateLargeTestData()
        {
            var companies = new List<Company>();

            // 1000 компаний
            for (int i = 0; i < 1000; i++)
            {
                var company = new Company { Name = $"Company {i}" };

                // Добавляем пути разной длины: от коротких до очень длинных
                company.Paths.Add($"/ru/msk/{i % 100}/district/{i % 10}/building/{i % 5}");
                company.Paths.Add($"/ru/spb/{i % 80}/area/{i % 5}");
                company.Paths.Add($"/ru/svrd/{i % 60}/city/{i % 3}");
                company.Paths.Add($"/ru/kzn/{i % 40}/street/{i % 2}");
                company.Paths.Add($"/ru/{i % 10}"); // короткие пути

                companies.Add(company);
            }

            return companies;
        }
    }
}
