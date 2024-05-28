import { HttpClientModule } from '@angular/common/http';
import { Component, EventEmitter, Output } from '@angular/core';
import { Product } from '../../interfaces/product';
import { ProductCardComponent } from '../product-card/product-card.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { searchService } from '../../services/search.service';

@Component({
  selector: 'app-searchbar',
  standalone: true,
  imports: [HttpClientModule, CommonModule, ProductCardComponent, FormsModule],
  providers: [searchService],
  templateUrl: './searchbar.component.html',
  styleUrl: './searchbar.component.css',
})
export class SearchbarComponent {
  prefix: string = '';
  searchResult: Product[] = [];
  @Output() searchCompleted: EventEmitter<Product[]> = new EventEmitter<Product[]>();
  constructor(private searchservice: searchService) {}

  onSearch(event: Event) {
    event.preventDefault();
    if (this.prefix.trim().length > 0) {
      this.searchservice.searchProductByprefix(this.prefix.trim()).subscribe(
        (products) => {
          this.searchResult = products;
          this.searchCompleted.emit(products);
        },
        (error) => {
          console.error('Error fetching search result:', error);
        }
      );
    } else {
      alert('Please type something in the searchbox');
    }
  }
}
