import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyNowProductCardComponent } from './buy-now-product-card.component';

describe('BuyNowProductCardComponent', () => {
  let component: BuyNowProductCardComponent;
  let fixture: ComponentFixture<BuyNowProductCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BuyNowProductCardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BuyNowProductCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
