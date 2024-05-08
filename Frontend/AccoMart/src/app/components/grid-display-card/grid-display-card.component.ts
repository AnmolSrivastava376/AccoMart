import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-grid-display-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './grid-display-card.component.html',
  styleUrl: './grid-display-card.component.css'
})
export class GridDisplayCardComponent{
  @Input() products?: Product[]
}
