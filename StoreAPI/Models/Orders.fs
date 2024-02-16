namespace StoreAPI.Models


type OrderProduct =
    {
        id : int
        count : int
    }

type OrderDetail =
    {
        mutable id : int
        location : string
        orders : OrderProduct array
    }

type OjoinOP =
    {
        id : int
        time : System.DateTime
        location : string
        productid : int
        count : int
    }

    
type PjoinOjoinOP =
    {
        id : int
        time : System.DateTime
        location : string
        productid : int
        name : string
        img : string
        count : int
    }

type ProductView = 
    {
        id : int
        name : string
        img : string
        count : int
    }

type OrderView =
    {
        id : int
        time : System.DateTime
        location : string
        products : System.Collections.Generic.List<ProductView>
    }


