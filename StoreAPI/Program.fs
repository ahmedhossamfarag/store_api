namespace StoreAPI
#nowarn "20"
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.EntityFrameworkCore
open StoreAPI.Controllers

module Program =
    let exitCode = 0
    let connection_str = "name=ConnectionStrings:DatabaseContext"
    
    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        
        builder.Services.AddControllers()
        builder.Services.AddDbContext<StoreAPI.Models.StoreDBContext>(fun options ->
                options.UseSqlServer(connection_str) |> ignore
            )

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.Run()

        exitCode
