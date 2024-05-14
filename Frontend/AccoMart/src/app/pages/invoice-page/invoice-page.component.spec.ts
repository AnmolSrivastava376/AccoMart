import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicePageComponent } from './invoice-page.component';

describe('InvoicePageComponent', () => {
  let component: InvoicePageComponent;
  let fixture: ComponentFixture<InvoicePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoicePageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(InvoicePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
