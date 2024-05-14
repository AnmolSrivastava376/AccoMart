// edit-product-popup.component.ts
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Product } from '../../interfaces/product'; // adjust the path as needed
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../navbar/navbar.component';
// import { AngularFireStorage } from '@angular/fire/storage';


@Component({
  selector: 'app-edit-product-popup',
  templateUrl: './edit-product-popup.component.html',
  imports:[CommonModule,FormsModule,NavbarComponent],
  standalone:true,
  styleUrls: ['./edit-product-popup.component.css']
})

export class EditProductPopupComponent implements OnInit {
  productImageUrl: string; // This will hold the URL of the uploaded image
  uploading: boolean = false;
  constructor() { }
  ngOnInit(): void {
  }

  @Input() product: Product;
  @Output() close = new EventEmitter<void>();


  closePopup(): void {
    this.close.emit();
  }
  submitForm(){
    console.log("clicked");
  }
}
