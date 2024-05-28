import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ScrollDisplayCardComponent } from './scroll-display-card.component';

describe('ScrollDisplayCardComponent', () => {
  let component: ScrollDisplayCardComponent;
  let fixture: ComponentFixture<ScrollDisplayCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScrollDisplayCardComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ScrollDisplayCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});