import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductDetailCardComponent } from './product-detail-card.component';

describe('ProductDetailCardComponent', () => {
  let component: ProductDetailCardComponent;
  let fixture: ComponentFixture<ProductDetailCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductDetailCardComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ProductDetailCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});