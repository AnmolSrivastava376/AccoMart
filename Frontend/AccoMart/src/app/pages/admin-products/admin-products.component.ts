import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { HttpClientModule } from '@angular/common/http';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';
import { forkJoin } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LoaderComponent } from '../../components/loader/loader.component';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [
    NavbarComponent,
    CommonModule,
    SidebarComponent,
    HttpClientModule,
    LoaderComponent,
  ],
  providers: [productService, CategoryService],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.css',
})
export class AdminProductsComponent implements OnInit {
  loadMore: boolean = true;
  disable: boolean = false;
  pageNo: number = 1;
  products: Product[] = [];
  product: Product;
  categories: Category[];
  selectedProduct: Product;
  isLoading = true;
  constructor(
    private productService: productService,
    private categoryService: CategoryService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.fetchCategories();
  }

  getCategoryName(categoryId: number): string {
    const category = this.categories.find(
      (cat) => cat.categoryId === categoryId
    );
    return category ? category.categoryName : '';
  }

  fetchCategories() {
    this.categoryService.fetchCategories().subscribe({
      next: (response) => {
        this.categories = response;
        this.fetchProductsByPageNo();
      },
      error: () => {
        this.isLoading = false;
        this.toastr.error('Error fetching products', undefined, {
          timeOut: 5000,
        });
      },
    });
  }

  fetchProductsByPageNo() {
    this.isLoading = true;
    this.productService.fetchAllProductsPagewise(this.pageNo).subscribe({
      next: (response) => {
        if (
          response === null ||
          response === undefined ||
          response.length === 0
        ) {
          this.disable = true;
        }
        response.forEach((product) => {
          this.products.push(product);
        });
        this.isLoading = false;
      },
      complete: () => {
        this.pageNo++;
      },
      error: () => {
        this.isLoading = false;
        this.toastr.error('Error fetching products', undefined, {
          timeOut: 5000,
        });
      },
    });
  }

  openAddProductPage() {
    this.router.navigate(['/admin/product/add']);
  }

  openEditPage(product: Product): void {
    this.selectedProduct = product;
    this.router.navigate(['/admin/product/edit', product.productId]);
  }

  deleteProduct(productId: number) {
    this.productService.deleteProductById(productId).subscribe({
      next: () => {
        this.toastr.success('Product Deleted', undefined, { timeOut: 5000 });
        this.products = this.products.filter(
          (product) => product.productId !== productId
        );
      },
      error: () => {
        this.toastr.error('Error deleting products', undefined, {
          timeOut: 5000,
        });
      },
    });
  }

  openDeletePopup(product: Product) {
    const confirmDelete = window.confirm(
      `Are you sure you want to delete ${product.productName}?`
    );
    if (confirmDelete) {
      this.deleteProduct(product.productId);
    }
  }

  mergeResults(searchValue: string) {
    const searchNumber = parseInt(searchValue);
    if (!isNaN(searchNumber)) {
      this.productService
        .fetchProductById(searchNumber)
        .subscribe((response) => {
          this.products = [];
          if (response.categoryId) {
            this.products.push(response);
          }
          this.isLoading = false;
        });
    } else {
      forkJoin([
        this.productService.fetchProductByName(searchValue),
        this.productService.fetchProductByCategoryName(searchValue),
      ]).subscribe({
        next: ([productsByName, productsByCategory]) => {
          this.products = [];
          this.products = [...productsByName, ...productsByCategory];
          this.isLoading = false;
        },
        error: () => {
          this.toastr.error('Error searching products', undefined, {
            timeOut: 5000,
          });
        },
      });
    }
  }

  searchFunction(event: any) {
    this.isLoading = true;
    const searchValue = event.target.value;
    if(searchValue=='')
      {
        this.pageNo=1;
        this.fetchProductsByPageNo();

      }else{
        this.mergeResults(searchValue);

      }
  }
}
