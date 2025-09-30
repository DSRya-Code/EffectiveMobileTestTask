using EffectiveMobileTestTask.BLL;
using Scalar.AspNetCore;

namespace EffectiveMobileTestTask.Extensions
{
    public static partial class Extensions
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddOpenApi();

            // Регистрация зависимостей
            builder.Services.AddSingleton<CompanyRegionTree>();
            builder.Services.AddSingleton<RegionPathTreeController>();
        }

        /// <summary>
        /// конфигурирование эндпоинтов
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication MapApplicationEndpoints(this WebApplication app)
        {            
            app.MapPost("/replace-companies", async (
                List<Company> companies,
                RegionPathTreeController controller) =>
            {
                if (companies == null || !companies.Any())
                    return Results.BadRequest("Список компаний пуст. Изменения не будут применены");

                await controller.ReplaceAllCompaniesAsync(companies);

                return Results.Ok("Изменения применены");
            })
            .WithName("ReplaceCompanies")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Замена списка компаний и их путей";
                operation.Description = "Замена списка компаний и их путей";
                return operation;
            });


            // Эндпоинт для поиска компаний по пути
            app.MapGet("/find-companies", async (string path, RegionPathTreeController controller) =>
            {
                if (string.IsNullOrEmpty(path))
                    return Results.BadRequest("Необходимо указать путь");

                var result = await controller.FindCompaniesByPath(path);
                return Results.Ok(result);
            })
            .WithName("FindCompaniesByPath")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Поиск компаний по пути";
                operation.Description = "Поиск компаний по пути";
                return operation;
            });

            return app;
        }

        /// <summary>
        /// Сидирование на старте
        /// </summary>
        /// <param name="app"></param>
        public static void SeedNodeTree(this WebApplication app)
        {
            // Сидирование на  старте
            // Загрузка данных при старте (например, тестовые данные)
            var tree = app.Services.GetRequiredService<CompanyRegionTree>();
            var controller = app.Services.GetRequiredService<RegionPathTreeController>();

            // Загружаем тестовые данные
            var companies = GetTestCompanies();
            foreach (var company in companies)
            {
                tree.AddCompany(company);
            }

            // Строим индекс
            controller.BuildIndex();
        }
        static List<Company> GetTestCompanies()
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
