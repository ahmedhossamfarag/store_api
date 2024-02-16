namespace StoreAPI.Controllers

open Microsoft.AspNetCore.Mvc
open StoreAPI.Models
open Microsoft.EntityFrameworkCore
open Microsoft.Data.SqlClient
open System.Linq

[<ApiController>]
[<Route("[controller]")>]
type OrdersController (_context : StoreDBContext) =
    inherit ControllerBase()


    [<HttpGet>]
    member this.Get() = 
        let l1 = query {
            for op in _context.OrderProducts do
            join o in _context.OrderP
                on (op.orderid = o.id)
            select {
                id = o.id
                time = o.requesttime
                location = o.location
                productid = op.productid
                count = op.count
            }
        }
        let l2 = query {
            for p in _context.Product do
            join o in l1.ToList()
                on (p.id = o.productid)
            select {
                id = o.id
                time = o.time
                location = o.location
                productid = p.id
                name = p.name
                img = p.img
                count = o.count
            }
        }
        let l3 = query {
            for o in l2.ToList() do
            groupBy o.id into g
            select {
                id = g.Key
                time = g.First().time
                location = g.First().location
                products = g.ToList().ConvertAll(fun it -> {
                                            id = it.productid
                                            name = it.name
                                            img = it.img
                                            count = it.count
                })
            }
        }
        l3.AsEnumerable()

    
    member this.exists id = async {
        if id = 0 then
            return false
        else
            return! _context.OrderP.AnyAsync(fun p -> p.id = id) |> Async.AwaitTask
    }

    [<HttpPost>]
    member this.Post(order : OrderDetail) = async {
        let! flag = this.exists order.id
        if flag  then
            _context.OrderP.Update({   
                    id = order.id
                    requesttime = System.DateTime.Now
                    location = order.location
                }) |> ignore
            let rels = query {
                for o in _context.OrderProducts do
                where (o.orderid = order.id)
                select o
            }
            _context.OrderProducts.RemoveRange(rels)
        else
            try
                let! max = _context.OrderP.MaxAsync(fun p -> p.id) |> Async.AwaitTask
                order.id <- max + 1
                with 
                    | :? System.AggregateException ->  order.id <- 1
            _context.OrderP.Add({   
                    id = order.id
                    requesttime = System.DateTime.Now
                    location = order.location
                }) |> ignore
        for p in order.orders do
            _context.OrderProducts.AddAsync(
                {
                    orderid = order.id
                    productid = p.id
                    count = p.count
                }
            ) |> ignore
        _context.SaveChangesAsync() |> ignore
        return this.Ok() :> IActionResult
        }
        
    [<HttpDelete>]
    [<Route("{id}")>]
    member this.Delete(id : int) = async {
        let order = _context.OrderP.FindAsync(id).Result
        if  box order = null then
            return this.NotFound() :> IActionResult
        else
            _context.OrderP.Remove(order) |> ignore
            let rels = query {
                for o in _context.OrderProducts do
                where (o.orderid = id)
                select o
            }
            _context.OrderProducts.RemoveRange(rels)
            _context.SaveChangesAsync() |> ignore
            return this.Ok() :> IActionResult 
    }
            