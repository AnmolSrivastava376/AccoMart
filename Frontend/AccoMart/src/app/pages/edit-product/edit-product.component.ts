import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Product } from '../../interfaces/product'; 
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ActivatedRoute, Router } from '@angular/router';
import { productService } from '../../services/product.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';

@Component({
  selector: 'app-edit-product-popup',
  templateUrl: './edit-product.component.html',
  imports:[CommonModule,FormsModule,NavbarComponent,SidebarComponent,HttpClientModule],
  providers:[productService,CategoryService],
  standalone:true,
  styleUrls: ['./edit-product.component.css']
})

export class EditProductComponent implements OnInit {

  @Output() close = new EventEmitter<void>();
  constructor(private route: ActivatedRoute , private router:Router, private productService:productService,private categoryService:CategoryService,    private http:HttpClient
  ) { }
  productImageUrl: string; 
  uploading: boolean = false;

  file: File | null = null;
  uploadComplete = false;
  cldResponse: any;


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

  product: Product = {
    productId: 0,
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: 0,
    stock:0
  };
  categories:Category[];



  ngOnInit(): void {
    this.fetchProduct();
    this.fetchCategories();
  }

  fetchProduct()
  {
    this.route.params.subscribe(params => {
      const productId = +params['productId'];
      this.productService.fetchProductById(productId)
        .subscribe(
          response => {
            this.product = response;
            console.log(this.product);
          }
        );
    });
  }

  fetchCategories(){
    this.categoryService.fetchCategories().subscribe(response=>{
     this.categories = response;
     console.log(this.categories);
    },err=>{
     console.log(err);
    })
   }

  CancelEdit(): void {
    window.location.href = '/admin/products';
  }

  showProducts() {
    window.location.href = '/admin/products';
  }

  showCategories() {
    window.location.href = '/admin/categories';
  }


  submitForm(): void {
    this.productService.editProductById(this.product.productId, this.product)
      .subscribe(
        (response: any) => {
          // Handle success, e.g., show a success message
          console.log('Product updated successfully:', response);
        },
        (error: any) => {
          // Handle error, e.g., show an error message
          console.error('Error updating product:', error);
        }
      );
  }


}
