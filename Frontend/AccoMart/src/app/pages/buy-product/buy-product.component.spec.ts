import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyProductComponent } from './buy-product.component';

describe('BuyProductComponent', () => {
  let component: BuyProductComponent;
  let fixture: ComponentFixture<BuyProductComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BuyProductComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BuyProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
