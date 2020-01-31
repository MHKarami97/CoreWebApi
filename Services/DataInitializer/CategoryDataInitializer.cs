using System.Linq;
using Data.Contracts;
using Entities.Post;

namespace Services.DataInitializer
{
    public class CategoryDataInitializer : IDataInitializer
    {
        private readonly IRepository<Category> _repository;

        public CategoryDataInitializer(IRepository<Category> repository)
        {
            _repository = repository;
        }

        public void InitializeData()
        {
            if (!_repository.TableNoTracking.Any(p => p.Name == "دسته بندی اصلی"))
            {
                _repository.Add(new Category
                {
                    Name = "دسته بندی اصلی"
                });
            }
        }
    }
}
