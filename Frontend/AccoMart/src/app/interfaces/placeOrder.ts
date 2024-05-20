export interface CartOrder{
  userId : string
  cartId : number
  addressId: number
  deliveryId : number
}

export interface ProductOrder{
  userId : string
  addressId: number
  deliveryId : number
  productId : number
}

