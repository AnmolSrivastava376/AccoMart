// edit-product-popup.component.ts
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

@Component({
  selector: 'app-add-product-popup',
  templateUrl: './add-product.component.html',
  imports: [
    CommonModule,
    FormsModule,
    NavbarComponent,
    SidebarComponent
    ],
  providers: [productService, CategoryService],
  standalone: true,
  styleUrls: ['./add-product.component.css'],
})
export class AddProductComponent implements OnInit {

  file: File | null = null;
  uploading = false;
  uploadComplete = false;
  cldResponse: any;

  @Output() close = new EventEmitter<void>();
  constructor(
    private productService: productService,
    private categoryService: CategoryService,
    private http:HttpClient
  ) {}
  handleFileChange(event: any): void {
    this.file = event.target.files[0];
  }

  async uploadFile(): Promise<void> {
    if (!this.file) {
      console.error('Please select a file.');
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
        const response = await this.http.post(
          `https://api.cloudinary.com/v1_1/diiyfgi9r/auto/upload`,
          formData,
          {
            headers: {
              'X-Unique-Upload-Id': uniqueUploadId,
              'Content-Range': contentRange,
            },
          }
        ).toPromise();

        currentChunk++;

        if (currentChunk < totalChunks) {
          const nextStart = currentChunk * chunkSize;
          const nextEnd = Math.min(nextStart + chunkSize, this.file!.size);
          uploadChunk(nextStart, nextEnd);
        } else {
          this.uploadComplete = true;
          this.uploading = false;
          this.cldResponse = response;
          console.info('File upload complete.');
          this.product.productImageUrl = this.cldResponse.url;
        }
      } catch (error) {
        console.error('Error uploading chunk:', error);
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

  productImageUrl: string;
  categories: Category[];


  product: CreateProduct = {
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: -1,
  };



  ngOnInit(): void {
    this.fetchCategories();
  }

  fetchCategories() {
    this.categoryService.fetchCategories().subscribe(
      (response) => {
        this.categories = response;
        console.log(this.categories);
      },
      (err) => {
        console.log(err);
      }
    );
  }

  Cancel(): void {
    window.location.href = '/admin/products';
  }

  showProducts() {
    window.location.href = '/admin/products';
  }

  showCategories() {
    window.location.href = '/admin/categories';
  }

  AddProduct() {
    this.productService.addProduct(this.product).subscribe((response: any) => {
      console.log('Product added successfully:', response);
    });
  }
}
