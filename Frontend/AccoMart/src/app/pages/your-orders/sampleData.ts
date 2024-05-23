import { Order } from "../../interfaces/order";
import { Product } from "../../interfaces/product";

const products: Product[] = [
    {
        productId: 1,
        productName: "Samsung Galaxy S420 Max",
        productDesc: "Description for Product 1",
        productPrice: 10,
        productImageUrl: "https://static.remove.bg/sample-gallery/graphics/bird-thumbnail.jpg",
        categoryId: 1,
        stock:1
    },
    {
        productId: 2,
        productName: "Doraemon",
        productDesc: "Description for Product 2",
        productPrice: 20,
        productImageUrl: "https://static.remove.bg/sample-gallery/graphics/bird-thumbnail.jpg",
        categoryId: 2,
        stock:3
    },
];

const orders: Order[] = [
    {
        orderId: 1173570,
        orderDate: new Date(),
        userId: 1,
        address: 'Roop Mahal, Prem Gali, House No 420, Excuse me please',
        orderAmount: 50,
        orderTime: { hours: 10, minutes: 30 },
        itemArray: [
            { product: products[0], quantity: 2 },
            { product: products[1], quantity: 1 },
            { product: products[0], quantity: 2 },
            { product: products[1], quantity: 1 },
        ],
        deliveryServiceId: 1,
        isDelivered: false,
        expectedDate: new Date('2024-05-15'),
        isCancelled: false
    },
    {
        orderId: 2141410,
        orderDate: new Date(),
        userId: 2,
        address: 'Roop Mahal, Prem Gali, House No 420, Excuse me please',
        orderAmount: 30,
        orderTime: { hours: 12, minutes: 45 },
        itemArray: [
            { product: products[0], quantity: 2 },
            { product: products[1], quantity: 3 },
        ],
        deliveryServiceId: 1,
        isDelivered: false,
        expectedDate: new Date('2024-05-18'),
        isCancelled: false
    },
    {
        orderId: 311244,
        orderDate: new Date(),
        userId: 3,
        address: 'Roop Mahal, Prem Gali, House No 420, Excuse me please',
        orderAmount: 25,
        orderTime: { hours: 14, minutes: 15 },
        itemArray: [
            { product: products[0], quantity: 1 }
        ],
        deliveryServiceId: 1,
        isDelivered: false,
        expectedDate: new Date('2024-05-20'),
        isCancelled: false
    },
    {
        orderId: 21409951,
        orderDate: new Date('2024-05-05'),
        userId: 4,
        address: 'Roop Mahal, Prem Gali, House No 420, Excuse me please',
        orderAmount: 40,
        orderTime: { hours: 9, minutes: 0 },
        itemArray: [
            { product: products[0], quantity: 1 },
            { product: products[1], quantity: 2 }
        ],
        deliveryServiceId: 1,
        isDelivered: true,
        expectedDate: new Date('2024-05-10'),
        isCancelled: false
    },
    {
        orderId: 413159,
        orderDate: new Date('2024-04-25'),
        userId: 5,
        address: 'Roop Mahal, Prem Gali, House No 420, Excuse me please',
        orderAmount: 15,
        orderTime: { hours: 11, minutes: 30 },
        itemArray: [
            { product: products[0], quantity: 1 }
        ],
        deliveryServiceId: 1,
        isDelivered: false,
        expectedDate: new Date('2024-05-01'),
        isCancelled: true
    }
];

export default orders;
