using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace EffectiveMobileTestTask.BLL
{
    public class RegionPathTreeController
    {
        private readonly CompanyRegionTree _tree;
        private readonly MemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        private ConcurrentDictionary<string, List<Company>> regionCompanyDict;


        private object _lock = new object();

        public RegionPathTreeController(CompanyRegionTree tree, bool useCache = true)
        {
            _tree = tree;

            if (useCache)
            {
                _cache = new MemoryCache(new MemoryCacheOptions());
                _cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // кешируем на 10 минут
                };
            }
        }

        public async Task ReplaceAllCompaniesAsync(List<Company> companies)
        {
            _tree.Clear();

            foreach (var company in companies)
            {
                _tree.AddCompany(company);
            }

            BuildIndex();
        }

        public ConcurrentDictionary<string, List<Company>> BuildIndex()
        {
            _cache?.Clear();

            regionCompanyDict = new ConcurrentDictionary<string, List<Company>>();
            var root = _tree.GetRootNode();

            BuildIndexRecursive(root, "");
            return regionCompanyDict;
        }

        private void BuildIndexRecursive(RegionPathNode node, string currentPath)
        {
            var path = currentPath;

            if (!string.IsNullOrEmpty(node.Name))
            {
                path = string.IsNullOrEmpty(currentPath) ? $"/{node.Name}" : $"{currentPath}/{node.Name}";
            }

            // Если у узла есть компании, добавляем их в индекс
            if (node.Companies.Any())
            {
                if (!regionCompanyDict.ContainsKey(path))
                {
                    regionCompanyDict[path] = new List<Company>();
                }
                regionCompanyDict[path].AddRange(node.Companies);
            }

            // Обходим дочерние узлы
            foreach (var child in node.Children.Values)
            {
                BuildIndexRecursive(child, path);
            }
        }

        [Obsolete]
        public async Task<IReadOnlyCollection<Company>> ParallelFindCompaniesByPath(string path)
        {
            var cacheKey = path;
            if (_cache is not null && _cache.TryGetValue(cacheKey, out IReadOnlyCollection<Company> cachedResult))
            {
                return cachedResult;
            }

            var regions = GetNestedPaths(path);
            var result = new ConcurrentBag<Company>();

            if (regions.Any())
            {
                Parallel.ForEach(regions, region =>
                {
                    if (regionCompanyDict.TryGetValue(region, out var companies))
                    {
                        foreach (var company in companies)
                        {
                                result.Add(company);
                        }
                    }
                });
            }

            var finalResult = result.Distinct().ToList();
            _cache?.Set(cacheKey, finalResult, _cacheOptions);

            return result.Distinct().ToList();
        }


        /// <summary>
        /// Поиск компаний по пути
        /// </summary>
        /// <param name="path">Путь вида "/ru/msk"</param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Company>> FindCompaniesByPath(string path)
        {
            var cacheKey = path;
            if (_cache is not null && _cache.TryGetValue(cacheKey, out IReadOnlyCollection<Company> cachedResult))
            {
                return cachedResult;
            }

            var regions = GetNestedPaths(path);
            var result = new List<Company>();

            foreach (var region in regions)
            {
                if (regionCompanyDict.TryGetValue(region, out var companies))
                {
                    result.AddRange(companies);
                }
            }

            var finalResult = result.Distinct().ToList();
            _cache?.Set(cacheKey, finalResult, _cacheOptions);

            return result.Distinct().ToList();
        }

        private static List<string> GetNestedPaths(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
                return new List<string>();

            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();
            var currentPath = "";

            for (int i = 0; i < parts.Length; i++)
            {
                currentPath += "/" + parts[i];
                result.Add(currentPath);
            }

            return result;
        }

    }
}
