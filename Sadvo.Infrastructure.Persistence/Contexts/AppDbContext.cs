
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Entities.Elector;
using Microsoft.EntityFrameworkCore;

namespace Sadvo.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Ciudadano> Ciudadanos { get; set; }
        public DbSet<Eleccion> Elecciones { get; set; }
        public DbSet<PartidoPolitico> PartidosPoliticos { get; set; }
        public DbSet<PuestoElectivo> PuestosElectivos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<AsignacionDirigente> AsignacionesDirigentes { get; set; }
        public DbSet<AlianzaPolitica> AlianzasPoliticas { get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<AsignacionCandidatoPuesto> AsignacionesCandidatosPuestos { get; set; }
        public DbSet<SolicitudAlianza> SolicitudAlianzas { get; set; }
        public DbSet<Voto> Votos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                modelBuilder.Entity<Usuario>(entity =>
                {
                    // Configuración de la tabla
                    entity.ToTable("Usuarios");

                    // Clave primaria
                    entity.HasKey(u => u.Id);

                    // Configuración de propiedades
                    entity.Property(u => u.Id)
                        .ValueGeneratedOnAdd();

                    entity.Property(u => u.Nombre)
                        .IsRequired()
                        .HasMaxLength(100);

                    entity.Property(u => u.Apellido)
                        .IsRequired()
                        .HasMaxLength(100);

                    entity.Property(u => u.Email)
                        .IsRequired()
                        .HasMaxLength(256);

                    entity.Property(u => u.NombreUsuario)
                        .IsRequired()
                        .HasMaxLength(50);

                    entity.Property(u => u.Contrasena)
                        .IsRequired()
                        .HasMaxLength(280);

                    // Configuración de enums
                    entity.Property(u => u.Estado)
                        .HasConversion<int>()
                        .IsRequired();

                    entity.Property(u => u.Rol)
                        .HasConversion<int>()
                        .IsRequired();

                    // Relación uno a uno con AsignacionDirigente
                    entity.HasOne(u => u.AsignacionDirigente)
                        .WithOne(ad => ad.Usuario)
                        .HasForeignKey<AsignacionDirigente>(ad => ad.UsuarioId)
                        .OnDelete(DeleteBehavior.Cascade);
                });
            });

            modelBuilder.Entity<PuestoElectivo>(entity =>
            {

                // Configuración de la tabla
                entity.ToTable("PuestosElectivos");

                // Clave primaria
                entity.HasKey(pe => pe.Id);

                // Configuración de propiedades
                entity.Property(pe => pe.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(pe => pe.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(pe => pe.Descripcion)
                    .IsRequired()
                    .HasMaxLength(500);

                // Configuración de enum
                entity.Property(pe => pe.Estado)
                    .HasConversion<int>()
                    .IsRequired();

                // Relación con AsignacionCandidatoPuesto
                entity.HasMany(pe => pe.CandidatosAsignados)
                    .WithOne(acp => acp.PuestoElectivo)
                    .HasForeignKey(acp => acp.PuestoElectivoId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<PartidoPolitico>(entity =>
            {
                // Configuración de la tabla
                entity.ToTable("PartidosPoliticos");

                // Clave primaria
                entity.HasKey(pp => pp.Id);

                // Configuración de propiedades
                entity.Property(pp => pp.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(pp => pp.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(pp => pp.Descripcion)
                    .HasMaxLength(100);

                entity.Property(pp => pp.Siglas)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(pp => pp.Logo)
                    .HasMaxLength(600); // Para URL o path del logo

                // Configuración de enum
                entity.Property(pp => pp.Estado)
                    .HasConversion<int>()
                    .IsRequired();

                // Relación con AsignacionesDirigente
                entity.HasMany(pp => pp.AsignacionesDirigente)
                    .WithOne(ad => ad.PartidoPolitico)
                    .HasForeignKey(ad => ad.PartidoPoliticoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Candidatos
                entity.HasMany(pp => pp.Candidatos)
                    .WithOne(c => c.PartidoPolitico)
                    .HasForeignKey(c => c.PartidoPoliticoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con AsignacionesCandidatos
                entity.HasMany(pp => pp.AsignacionesCandidatos)
                    .WithOne(acp => acp.PartidoPolitico)
                    .HasForeignKey(acp => acp.PartidoPoliticoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Alianzas como PartidoB
                entity.HasMany(pp => pp.Alianzas)
                    .WithOne(ap => ap.PartidoB)
                    .HasForeignKey(ap => ap.PartidoBId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Solicitudes como Solicitante
                entity.HasMany(pp => pp.Solicitudes)
                    .WithOne(sa => sa.PartidoSolicitante)
                    .HasForeignKey(sa => sa.PartidoSolicitanteId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Eleccion>(entity =>
            {

                // Configuración de la tabla
                entity.ToTable("Elecciones");

                // Clave primaria
                entity.HasKey(e => e.Id);

                // Configuración de propiedades
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FechaRealizacion)
                    .IsRequired()
                    .HasColumnType("date");

                // Configuración de enum
                entity.Property(e => e.Estado)
                    .HasConversion<int>()
                    .IsRequired();

                // Relación con Votos
                entity.HasMany(e => e.Votos)
                    .WithOne(v => v.Eleccion)
                    .HasForeignKey(v => v.EleccionId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Ciudadano>(entity =>
            {

                // Configuración de la tabla
                entity.ToTable("Ciudadanos");

                // Clave primaria
                entity.HasKey(c => c.Id);

                // Configuración de propiedades
                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Apellido)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(c => c.Identificacion)
                    .IsRequired()
                    .HasMaxLength(20);

                // Configuración de enum
                entity.Property(c => c.Estado)
                    .HasConversion<int>()
                    .IsRequired();

                // Relación con Votos
                entity.HasMany(c => c.Votos)
                    .WithOne(v => v.Ciudadano)
                    .HasForeignKey(v => v.CiudadanoId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<AsignacionDirigente>(entity =>
            {

                // Configuración de la tabla
                entity.ToTable("AsignacionesDirigentes");

                // Clave primaria
                entity.HasKey(ad => ad.Id);

                // Configuración de propiedades
                entity.Property(ad => ad.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(ad => ad.UsuarioId)
                    .IsRequired();

                entity.Property(ad => ad.PartidoPoliticoId)
                    .IsRequired();

                // Relación con Usuario (ya configurada arriba)
                entity.HasOne(a => a.Usuario)
                    .WithOne(u => u.AsignacionDirigente)
                    .HasForeignKey<AsignacionDirigente>(a => a.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con PartidoPolitico (asumiendo que existe)
                entity.HasOne(ad => ad.PartidoPolitico)
                    .WithMany(pp => pp.AsignacionesDirigente) // O WithMany(pp => pp.AsignacionesDirigentes) si PartidoPolitico tiene esa propiedad
                    .HasForeignKey(ad => ad.PartidoPoliticoId)
                    .OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<AlianzaPolitica>(entity =>
            {

                // Configuración de la tabla
                entity.ToTable("AlianzasPoliticas");

                // Clave primaria
                entity.HasKey(ap => ap.Id);

                // Configuración de propiedades
                entity.Property(ap => ap.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(ap => ap.PartidoAId)
                    .IsRequired();

                entity.Property(ap => ap.PartidoBId)
                    .IsRequired();

                entity.Property(ap => ap.FechaAlianza)
                    .IsRequired()
                    .HasColumnType("date");

                // Configuración de enum
                entity.Property(ap => ap.Estado)
                    .HasConversion<int>()
                    .IsRequired();

                // Relación con PartidoA
                entity.HasOne(ap => ap.PartidoA)
                    .WithMany() // Sin navegación inversa específica para PartidoA
                    .HasForeignKey(ap => ap.PartidoAId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con PartidoB
                entity.HasOne(ap => ap.PartidoB)
                    .WithMany(pp => pp.Alianzas) // Navegación inversa específica para PartidoB
                    .HasForeignKey(ap => ap.PartidoBId)
                    .OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<AsignacionCandidatoPuesto>(entity =>
            {
                // Configuración de la tabla
                entity.ToTable("AsignacionesCandidatosPuestos");

                // Clave primaria
                entity.HasKey(acp => acp.Id);

                // Configuración de propiedades
                entity.Property(acp => acp.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(acp => acp.CandidatoId)
                    .IsRequired();

                entity.Property(acp => acp.PuestoElectivoId)
                    .IsRequired();

                entity.Property(acp => acp.PartidoPoliticoId)
                    .IsRequired();

                // Relación con Candidato
                entity.HasOne(acp => acp.Candidato)
                    .WithMany(c => c.PuestosAsignados) // Asumiendo que Candidato tiene esta propiedad
                    .HasForeignKey(acp => acp.CandidatoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con PuestoElectivo
                entity.HasOne(acp => acp.PuestoElectivo)
                    .WithMany(pe => pe.CandidatosAsignados)
                    .HasForeignKey(acp => acp.PuestoElectivoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con PartidoPolitico
                entity.HasOne(acp => acp.PartidoPolitico)
                    .WithMany(pp => pp.AsignacionesCandidatos)
                    .HasForeignKey(acp => acp.PartidoPoliticoId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Candidato>(entity =>
            {

                // Configuración de la tabla
                entity.ToTable("Candidatos");

                // Clave primaria
                entity.HasKey(c => c.Id);

                // Configuración de propiedades
                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Apellido)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Foto)
                    .HasMaxLength(500); // Para URL o path de la foto

                entity.Property(c => c.Estado)
                    .IsRequired();

                entity.Property(c => c.PartidoPoliticoId)
                    .IsRequired();

                // Relación con PartidoPolitico
                entity.HasOne(c => c.PartidoPolitico)
                    .WithMany(pp => pp.Candidatos)
                    .HasForeignKey(c => c.PartidoPoliticoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con PuestosAsignados (AsignacionCandidatoPuesto)
                entity.HasMany(c => c.PuestosAsignados)
                    .WithOne(acp => acp.Candidato)
                    .HasForeignKey(acp => acp.CandidatoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Votos
                entity.HasMany(c => c.Votos)
                    .WithOne(v => v.Candidato)
                    .HasForeignKey(v => v.CandidatoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SolicitudAlianza>(entity =>
            {
                // Configuración de la tabla
                entity.ToTable("SolicitudesAlianzas");

                // Clave primaria
                entity.HasKey(sa => sa.Id);

                // Configuración de propiedades
                entity.Property(sa => sa.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(sa => sa.PartidoSolicitanteId)
                    .IsRequired();

                entity.Property(sa => sa.PartidoReceptorId)
                    .IsRequired();

                entity.Property(sa => sa.FechaSolicitud)
                    .IsRequired()
                    .HasColumnType("date");

                // Configuración de enum
                entity.Property(sa => sa.Estado)
                    .HasConversion<int>()
                    .IsRequired();

                // Relación con PartidoSolicitante
                entity.HasOne(sa => sa.PartidoSolicitante)
                    .WithMany(pp => pp.Solicitudes)
                    .HasForeignKey(sa => sa.PartidoSolicitanteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con PartidoReceptor
                entity.HasOne(sa => sa.PartidoReceptor)
                    .WithMany() // Sin navegación inversa específica para PartidoReceptor
                    .HasForeignKey(sa => sa.PartidoReceptorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Voto>(entity =>
            {

                // Configuración de la tabla
                entity.ToTable("Votos");

                // Clave primaria
                entity.HasKey(v => v.Id);

                // Configuración de propiedades
                entity.Property(v => v.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(v => v.CiudadanoId)
                    .IsRequired();

                entity.Property(v => v.EleccionId)
                    .IsRequired();

                entity.Property(v => v.PuestoElectivoId)
                    .IsRequired();

                entity.Property(v => v.CandidatoId)
                    .IsRequired(false); // Puede ser null en caso de voto en blanco

                entity.Property(v => v.PartidoPoliticoId)
                    .IsRequired();

                entity.Property(v => v.FechaVoto)
                    .IsRequired()
                    .HasColumnType("datetime2");

                // Relación con Ciudadano (ya configurada arriba)
                entity.HasOne(v => v.Ciudadano)
                    .WithMany(c => c.Votos)
                    .HasForeignKey(v => v.CiudadanoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Eleccion
                entity.HasOne(v => v.Eleccion)
                    .WithMany(e => e.Votos) // O WithMany(e => e.Votos) si Eleccion tiene esa propiedad
                    .HasForeignKey(v => v.EleccionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con PuestoElectivo
                entity.HasOne(v => v.PuestoElectivo)
                    .WithMany() // O WithMany(pe => pe.Votos) si PuestoElectivo tiene esa propiedad
                    .HasForeignKey(v => v.PuestoElectivoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Candidato (opcional)
                entity.HasOne(v => v.Candidato)
                    .WithMany(pp => pp.Votos) // O WithMany(c => c.Votos) si Candidato tiene esa propiedad
                    .HasForeignKey(v => v.CandidatoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con PartidoPolitico
                entity.HasOne(v => v.PartidoPolitico)
                    .WithMany() // O WithMany(pp => pp.Votos) si PartidoPolitico tiene esa propiedad
                    .HasForeignKey(v => v.PartidoPoliticoId)
                    .OnDelete(DeleteBehavior.Restrict);

            });
        }       
    }
}
