import { Product } from "./product";

export interface MoreProducts{
    categoryId: number,
    items: Product[],
    pageNo: 1
  }