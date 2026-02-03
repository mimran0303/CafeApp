using CafeApp.Data;
using CafeApp.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductRepository(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<Product> CreateAsync(Product obj)
        {
            await _db.Product.AddAsync(obj);
            await _db.SaveChangesAsync();
            return obj;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var obj = await _db.Product.FirstOrDefaultAsync(u => u.Id == id);
            if (obj == null) return false;

            if (!string.IsNullOrEmpty(obj.ImageUrl))
            {
                // Remove leading slashes/backslashes safely
                var trimmed = obj.ImageUrl.TrimStart('/', '\\');
                var resolvedImagePath = Path.Combine(_webHostEnvironment.WebRootPath, trimmed);

                // Extra safety: ensure resolved path is under web root
                var fullPath = Path.GetFullPath(resolvedImagePath);
                var webRootFull = Path.GetFullPath(_webHostEnvironment.WebRootPath);
                if (fullPath.StartsWith(webRootFull, StringComparison.OrdinalIgnoreCase) && File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            _db.Product.Remove(obj);
            return (await _db.SaveChangesAsync()) > 0;
        }

        public async Task<Product> GetAsync(int id)
        {
            var obj = await _db.Product.FirstOrDefaultAsync(u => u.Id == id);
            if (obj == null)
            {
                return new Product();
            }
            return obj;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _db.Product.Include(u => u.Category).ToListAsync();
        }

        public async Task<Product> UpdateAsync(Product obj)
        {
            var objFromDb = await _db.Product.FirstOrDefaultAsync(u => u.Id == obj.Id);
            if (objFromDb is not null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Description = obj.Description;
                objFromDb.ImageUrl = obj.ImageUrl;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Price = obj.Price;
                _db.Product.Update(objFromDb);
                await _db.SaveChangesAsync();
                return objFromDb;
            }
            return obj;
        }
    }
}
