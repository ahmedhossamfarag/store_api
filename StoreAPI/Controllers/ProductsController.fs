namespace StoreAPI.Controllers

open Microsoft.AspNetCore.Mvc
open StoreAPI.Models
open Microsoft.EntityFrameworkCore
open Microsoft.Data.SqlClient




[<ApiController>]
[<Route("[controller]")>]
type ProductsController (_context : StoreDBContext) =
    inherit ControllerBase()

    [<HttpGet>]
    member this.Get() = _context.Product.ToArrayAsync()


    member this.exists id = async {
        if id = 0 then
            return false
        else
            return! _context.Product.AnyAsync(fun p -> p.id = id) |> Async.AwaitTask
    } 
    

    [<HttpPost>]
    member this.Post(product : Product) = async {
       let! flag = this.exists product.id
       if   flag  then
           _context.Update(product) |> ignore
       else
            try
                let max = _context.Product.MaxAsync(fun p -> p.id).Result
                product.id <- max + 1
            with 
                | :? System.AggregateException ->  product.id <- 1
            
            _context.Product.Add(product) |> ignore
       _context.SaveChanges() |> ignore
       return this.Ok() :> IActionResult
    }

    [<HttpDelete>]
    [<Route("{id}")>]
    member this.Delete(id : int) = async {
        let product = _context.Product.FindAsync(id).Result
        if  box product = null then
            return this.NotFound() :> IActionResult
        else
            _context.Product.Remove(product) |> ignore
            let rels = query {
                for o in _context.OrderProducts do
                where (o.productid = id)
                select o
            }
            _context.OrderProducts.RemoveRange(rels)
            _context.SaveChangesAsync() |> ignore
            return this.Ok() :> IActionResult
    }
            