using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        public readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public virtual async Task<Entity?> AddAsync(Entity entity)
        {

            Console.WriteLine($"Entidad recibida: {entity.GetType().Name}");

            try
            {
                Console.WriteLine($"Añadiendo entidad al contexto...");
                await _context.Set<Entity>().AddAsync(entity);

                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChangesAsync resultado: {result} filas afectadas");

                // Si es Ciudadano, mostrar detalles
                if (entity is Ciudadano ciudadano)
                {
                    Console.WriteLine($"Ciudadano guardado:");
                }

                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR EN REPOSITORY AddAsync ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public virtual async Task<List<Entity>?> AddRangeAsync(List<Entity> entities)
        {
            await _context.Set<Entity>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }
        public virtual async Task<List<Entity>> GetAllListWithInclude(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.ToListAsync();
        }


        public virtual IQueryable<Entity> GetAllQueryWithInclude(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return query;
        }


        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<Entity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Entity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<List<Entity>> GetAllList()
        {
            var lista = await _context.Set<Entity>().ToListAsync();
            return lista;

        }

        public virtual IQueryable<Entity> GetAllQueryable()
        {
            return _context.Set<Entity>().AsQueryable();
        }

        public virtual async Task<Entity?> GetByIdAsync(int id)
        {
            var consulta = await _context.Set<Entity>().FindAsync(id);
            return consulta;
        }

        public virtual async Task<Entity?> UpdateAsync(int id, Entity entity)
        {
            var entry = await _context.Set<Entity>().FindAsync(id);
            if (entry != null)
            {
                _context.Entry(entry).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }
    }
}
