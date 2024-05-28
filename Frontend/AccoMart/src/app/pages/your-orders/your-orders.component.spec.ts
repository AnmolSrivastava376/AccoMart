import { ComponentFixture, TestBed } from '@angular/core/testing';
import { YourOrdersComponent } from './your-orders.component';

describe('YourOrdersComponent', () => {
  let component: YourOrdersComponent;
  let fixture: ComponentFixture<YourOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YourOrdersComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(YourOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});