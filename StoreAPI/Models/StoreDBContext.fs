namespace StoreAPI.Models

open Microsoft.EntityFrameworkCore


[<CLIMutable>]
[<PrimaryKey("id")>]
type Product = 
    {
    mutable id : int
    name : string
    price : double
    img : string
    discreption : string
    }

[<CLIMutable>]
[<PrimaryKey("id")>]
type OrderP =
    {
    mutable id : int
    requesttime : System.DateTime
    location : string
    }

[<CLIMutable>]
[<PrimaryKey("orderid","productid")>]
type OrderProducts = 
    {
    orderid : int
    productid : int
    count : int
    }

type StoreDBContext(options : DbContextOptions<StoreDBContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable products : DbSet<Product>
    [<DefaultValue>]
    val mutable orders : DbSet<OrderP>
    [<DefaultValue>]
    val mutable ordersProducts : DbSet<OrderProducts>

     member public this.Product with get() = this.products
                                    and set p = this.products <- p
     member public this.OrderP with get() = this.orders
                                    and set o = this.orders <- o
     member public this.OrderProducts with get() = this.ordersProducts
                                        and set o = this.ordersProducts <- o




