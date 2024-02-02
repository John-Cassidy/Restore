export interface IShippingAddress {
  fullName: string;
  address1: string;
  address2: string;
  city: string;
  state: string;
  zip: string;
  country: string;
}

export interface IOrderItem {
  productId: number;
  name: string;
  pictureUrl: string;
  price: number;
  quantity: number;
}

export interface IOrder {
  id: number;
  buyerId: string;
  orderDate: string;
  shippingAddress: IShippingAddress;
  deliveryFee: number;
  orderItems: IOrderItem[];
  subtotal: number;
  orderStatus: string;
  total: number;
}
