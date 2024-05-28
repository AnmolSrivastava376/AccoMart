import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductScrollDisplayCardComponent } from './product-scroll-display-card.component';

describe('ProductScrollDisplayCardComponent', () => {
  let component: ProductScrollDisplayCardComponent;
  let fixture: ComponentFixture<ProductScrollDisplayCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductScrollDisplayCardComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ProductScrollDisplayCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});