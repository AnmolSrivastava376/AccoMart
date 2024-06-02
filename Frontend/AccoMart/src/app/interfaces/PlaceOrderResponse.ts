export interface PlaceOrderResponse {
    statusCode: number;
    message: string;
    stripeModel?: {
        stripeUrl: string;
    };
}