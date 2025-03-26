using Api.Modelos;
using Microsoft.EntityFrameworkCore;


namespace Api
{
    public partial  class ModelOracleContext : DbContext
    {
        
  public virtual DbSet<EMP> EMP_GENERAL { get; set; }
        public virtual DbSet<detalle_grupocentrocosto> detalle { get; set; }
        public virtual DbSet<EMP_FICHA_SOCIAL> ficha { get; set; }
        public virtual DbSet<grupo_centrocosto> grupo_centrocosto { get; set; }


        public ModelOracleContext()
        {
        }

        public ModelOracleContext(DbContextOptions<ModelOracleContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//            optionsBuilder.UseOracle(@"User Id=ACADEMICO1;Password=ACADEMICO1;Data Source=192.168.0.9:1521/PRDDELTA");
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EMP>(entity =>
            {
                entity.Property(e => e.CODEMP).ValueGeneratedOnAdd();

               // entity.Property(e => e.TIPO).IsUnicode(false);

          
            });
            OnModelCreatingGeneratedProcedures(modelBuilder);
        }
        }
}
