import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CreateProduct } from '../../interfaces/createProduct';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { productService } from '../../services/product.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { HttpClient } from '@angular/common/http';
import { Category } from '../../interfaces/category';
import { CategoryService } from '../../services/category.services';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-product-popup',
  templateUrl: './add-product.component.html',
  imports: [CommonModule, FormsModule, NavbarComponent, SidebarComponent],
  providers: [productService, CategoryService, ToastrService],
  standalone: true,
  styleUrls: ['./add-product.component.css'],
})
export class AddProductComponent implements OnInit {
  file: File | null = null;
  uploading = false;
  uploadComplete = false;
  cldResponse: any;
  productImageUrl: string;
  categories: Category[];
  product: CreateProduct = {
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId:0,
    stock: 0,
  };
  @Output() close = new EventEmitter<void>();
  constructor(
    private productService: productService,
    private categoryService: CategoryService,
    private http: HttpClient,
    private router: Router,
    private toastr: ToastrService
  ) {}

  handleFileChange(event: any): void {
    this.file = event.target.files[0];
  }

  async uploadFile(): Promise<void> {
    if (!this.file) {
      this.toastr.error('Please select a file', undefined, { timeOut: 2000 });
      return;
    }
    const uniqueUploadId = this.generateUniqueUploadId();
    const chunkSize = 5 * 1024 * 1024;
    const totalChunks = Math.ceil(this.file.size / chunkSize);
    let currentChunk = 0;
    this.uploading = true;
    const uploadChunk = async (start: number, end: number): Promise<void> => {
      const formData = new FormData();
      formData.append('file', this.file!.slice(start, end));
      formData.append('cloud_name', 'diiyfgi9r');
      formData.append('upload_preset', 'pqfmff0z');
      const contentRange = `bytes ${start}-${end - 1}/${this.file!.size}`;
      console.log(
        `Uploading chunk for uniqueUploadId: ${uniqueUploadId}; start: ${start}, end: ${
          end - 1
        }`
      );
      try {
        const response = await this.http
          .post(
            `https://api.cloudinary.com/v1_1/diiyfgi9r/auto/upload`,
            formData,
            {
              headers: {
                'X-Unique-Upload-Id': uniqueUploadId,
                'Content-Range': contentRange,
              },
            }
          )
          .toPromise();
        currentChunk++;
        if (currentChunk < totalChunks) {
          const nextStart = currentChunk * chunkSize;
          const nextEnd = Math.min(nextStart + chunkSize, this.file!.size);
          uploadChunk(nextStart, nextEnd);
        } else {
          this.uploadComplete = true;
          this.uploading = false;
          this.cldResponse = response;
          this.toastr.success('File uploaded', undefined, { timeOut: 5000 });
          this.product.productImageUrl = this.cldResponse.url;
        }
      } catch (error) {
        this.toastr.error('Error uploading', undefined, { timeOut: 5000 });
        this.uploading = false;
      }
    };
    const start = 0;
    const end = Math.min(chunkSize, this.file.size);
    uploadChunk(start, end);
  }

  generateUniqueUploadId(): string {
    return `uqid-${Date.now()}`;
  }

  ngOnInit(): void {
    this.fetchCategories();
  }

  fetchCategories() {
    this.categoryService.fetchCategories().subscribe({
      next: (response) => {
        this.categories = response;
      },
      error: () => {
        this.toastr.error('Error', undefined, { timeOut: 5000 });
      },
    });
  }

  Cancel(): void {
    window.location.href = '/admin/products'
  }

  AddProduct() {
    if(this.product.productPrice<=0)
      {
        this.toastr.error('Please enter valid price', undefined, { timeOut: 1000 })
        return;
      }

      if(this.product.stock<=0)
      {
        this.toastr.error('Please enter valid stock',undefined, { timeOut: 1000 })
        return ;
      }

    if (
      !this.product.productName ||
      !this.product.productDesc ||
      !this.product.productImageUrl ||
      !this.product.categoryId 
    ) {
      this.toastr.error('Please fill all the fields', undefined, {
        timeOut: 2000,
      });
      return; 
    }

    this.productService.addProduct(this.product).subscribe({
      next: () => {
        this.toastr.success('Product added successfully', undefined, {
          timeOut: 2000,
        });
        window.location.href = '/admin/products'
      },
      error: () => {
        this.toastr.error('Error adding product', undefined, {
          timeOut: 2000,
        });
      }
    });
  }
}
