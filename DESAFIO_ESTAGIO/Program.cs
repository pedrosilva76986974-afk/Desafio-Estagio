
namespace DESAFIO_ESTAGIO
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();
            builder.Services.AddEndpointsApiExplorer();
            var app = builder.Build();      
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.Run();
        }
    }
}